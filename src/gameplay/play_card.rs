// Copyright © 2020-present Derek Thurn

// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at

//    https://www.apache.org/licenses/LICENSE-2.0

// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.

use crate::prelude::*;

use crate::{
    api, commands,
    model::{
        cards::{Card, CardWithTarget, HasCardId, HasCardState, Scroll, Spell},
        creatures::{Creature, CreatureData, CreatureState, HasCreatureData},
        games::Game,
        players::{HasOwner, Player},
        primitives::{BoardPosition, CardId, CreatureId, FileValue, PlayerName, RankValue},
    },
    requests,
    rules::engine::{EntityId, Rule, RulesEngine, Trigger},
};
use api::play_card_request::PlayCard;

pub fn on_play_card_request(
    engine: &mut RulesEngine,
    message: &api::PlayCardRequest,
) -> Result<api::CommandList> {
    let mut result = vec![];
    play(
        engine,
        to_card_with_target(&engine.game, message)?,
        &mut result,
    )?;
    Ok(commands::groups(result))
}

pub fn play(
    engine: &mut RulesEngine,
    card_with_target: PlayCardWithTarget,
    result: &mut Vec<api::CommandGroup>,
) -> Result<()> {
    let owner = engine.game.card(card_with_target.id)?.owner();

    // Must be before card is removed from hand to avoid broken update events:
    engine.invoke_as_group(result, Trigger::CardPlayed(owner, card_with_target.id))?;

    let player = engine.game.player_mut(owner);
    let mut card = player.remove_from_hand(card_with_target.id)?;
    card.card_state_mut().revealed_to_opponent = true;
    let card_data = commands::card_data(&card);

    match (card, card_with_target.target) {
        (Card::Creature(creature_data), PlayCardTarget::Creature(position)) => {
            on_play_creature(engine, result, creature_data, card_data, position)
        }
        (Card::Spell(spell), PlayCardTarget::Spell(target_id)) => {
            on_play_attachment(engine, result, spell, card_data, target_id)
        }
        (Card::Scroll(scroll), PlayCardTarget::Scroll) => {
            on_play_scroll(engine, result, scroll, card_data)
        }
        _ => Err(eyre!("Card did not match targeting type")),
    }
}

pub enum PlayCardTarget {
    Creature(BoardPosition),
    Spell(CreatureId),
    Scroll,
}

pub struct PlayCardWithTarget {
    pub id: CardId,
    pub target: PlayCardTarget,
}

impl PlayCardWithTarget {
    pub fn from(card: CardWithTarget) -> PlayCardWithTarget {
        match card {
            CardWithTarget::Creature(c, position) => PlayCardWithTarget {
                id: c.card_id(),
                target: PlayCardTarget::Creature(position),
            },
            CardWithTarget::Spell(s, target_creature) => PlayCardWithTarget {
                id: s.card_id(),
                target: PlayCardTarget::Spell(target_creature.creature_id()),
            },
            CardWithTarget::Scroll(s) => PlayCardWithTarget {
                id: s.card_id(),
                target: PlayCardTarget::Scroll,
            },
        }
    }
}

fn to_card_with_target<'a>(
    game: &'a Game,
    message: &api::PlayCardRequest,
) -> Result<PlayCardWithTarget> {
    let card_id = message
        .card_id
        .as_ref()
        .ok_or_else(|| eyre!("card_id is required"))?
        .value;
    let request = message
        .play_card
        .as_ref()
        .ok_or_else(|| eyre!("play_card is required"))?;
    let player_name = requests::convert_player_name(message.player())?;
    let card = game.player(player_name).card(card_id)?;

    let target = match (card, request) {
        (Card::Creature(creature_data), PlayCard::PlayCreature(play_creature)) => {
            Ok(PlayCardTarget::Creature(BoardPosition {
                rank: requests::convert_rank(play_creature.rank_position())?,
                file: requests::convert_file(play_creature.file_position())?,
            }))
        }
        (Card::Spell(spell), PlayCard::PlayAttachment(play_attachment)) => {
            Ok(PlayCardTarget::Spell(requests::convert_creature_id(
                play_attachment
                    .creature_id
                    .as_ref()
                    .ok_or_else(|| eyre!("creature_id is required"))?,
            )))
        }
        (Card::Scroll(scroll), PlayCard::PlayUntargeted(_)) => Ok(PlayCardTarget::Scroll),
        _ => Err(eyre!("Card did not match targeting type")),
    }?;

    Ok(PlayCardWithTarget {
        id: card_id,
        target,
    })
}

fn reveal_card(
    player_name: PlayerName,
    result: &mut Vec<api::CommandGroup>,
    card_data: api::CardData,
    rank: Option<RankValue>,
    file: Option<FileValue>,
) -> Result<()> {
    if player_name == PlayerName::User {
        result.push(commands::single(commands::destroy_card_command(
            player_name,
            card_data
                .card_id
                .ok_or_else(|| eyre!("card_id is required"))?,
            false,
        )));
    } else {
        result.push(commands::single(commands::reveal_card_command(
            card_data, 1000, rank, file,
        )));
    }

    Ok(())
}

fn on_play_creature(
    engine: &mut RulesEngine,
    result: &mut Vec<api::CommandGroup>,
    creature_data: CreatureData,
    card_data: api::CardData,
    position: BoardPosition,
) -> Result<()> {
    let player = engine.game.player_mut(creature_data.owner());
    let player_name = player.name;

    if !player.creature_position_available(position) {
        return Err(eyre!("Creature position {:?} already occupied!", position));
    }

    let creature = Creature {
        data: creature_data,
        position,
        spells: vec![],
        state: CreatureState::default(),
    };
    let rules = creature.data.rules.clone();
    let creature_id = creature.creature_id();

    reveal_card(
        player_name,
        result,
        card_data,
        Some(position.rank),
        Some(position.file),
    )?;

    result.push(commands::single(
        commands::create_or_update_creature_command(&creature),
    ));
    player.creatures.push(creature);

    engine.add_rules(EntityId::CreatureId(creature_id), rules);
    engine.invoke_as_group(result, Trigger::CreaturePlayed(player_name, creature_id))?;
    Ok(())
}

fn on_play_attachment(
    engine: &mut RulesEngine,
    result: &mut Vec<api::CommandGroup>,
    spell: Spell,
    card_data: api::CardData,
    target_id: CreatureId,
) -> Result<()> {
    let player = engine.game.player_mut(spell.owner());
    let player_name = player.name;
    let creature = engine.game.creature_mut(target_id)?;

    reveal_card(
        player_name,
        result,
        card_data,
        Some(creature.position.rank),
        Some(creature.position.file),
    )?;
    let spell_id = spell.spell_id();

    creature.spells.push(spell);
    result.push(commands::single(
        commands::create_or_update_creature_command(&creature),
    ));

    engine.invoke_as_group(
        result,
        Trigger::SpellPlayed(player_name, target_id, spell_id),
    )?;
    Ok(())
}

fn on_play_scroll(
    engine: &mut RulesEngine,
    result: &mut Vec<api::CommandGroup>,
    scroll: Scroll,
    card_data: api::CardData,
) -> Result<()> {
    let scroll_id = scroll.scroll_id();
    let player = engine.game.player_mut(scroll.owner());
    let player_name = player.name;
    let rules = scroll.rules.clone();

    reveal_card(player.name, result, card_data, None, None)?;

    player.scrolls.push(scroll);

    engine.add_rules(EntityId::ScrollId(scroll_id), rules);
    engine.invoke_as_group(result, Trigger::ScrollPlayed(player_name, scroll_id))?;
    Ok(())
}
