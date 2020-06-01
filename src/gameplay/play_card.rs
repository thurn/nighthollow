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
        cards::{Card, HasCardState, Scroll, Spell},
        creatures::{Creature, CreatureData, CreatureState, HasCreatureData},
        players::{HasOwner, Player},
        primitives::{BoardPosition, CardId, CreatureId, FileValue, PlayerName, RankValue},
    },
    requests,
    rules::engine::{EntityId, Rule, RulesEngine, Trigger},
};

enum CardWithTarget {
    Creature(CardId, RankValue, FileValue),
    Spell(CardId, CreatureId),
    Scroll(CardId),
}

pub fn on_play_card_request(
    engine: &mut RulesEngine,
    message: &api::PlayCardRequest,
) -> Result<api::CommandList> {
    let mut result = vec![];
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
    engine.invoke_as_group(&mut result, Trigger::CardPlayed(player_name, card_id))?;

    let player = engine.game.player_mut(player_name);
    let mut card = player.remove_from_hand(card_id)?;
    card.card_state_mut().revealed_to_opponent = true;
    let card_data = commands::card_data(&card);

    use api::play_card_request::PlayCard::*;
    match (card, request) {
        (Card::Creature(creature_data), PlayCreature(play_creature)) => on_play_creature(
            engine,
            &mut result,
            creature_data,
            card_data,
            requests::convert_rank(play_creature.rank_position())?,
            requests::convert_file(play_creature.file_position())?,
        ),
        (Card::Spell(spell), PlayAttachment(play_attachment)) => on_play_attachment(
            engine,
            &mut result,
            spell,
            card_data,
            requests::convert_creature_id(
                play_attachment
                    .creature_id
                    .as_ref()
                    .ok_or_else(|| eyre!("creature_id is required"))?,
            ),
        ),
        (Card::Scroll(scroll), PlayUntargeted(_)) => {
            on_play_scroll(engine, &mut result, scroll, card_data)
        }
        _ => Err(eyre!("Card did not match targeting type")),
    }?;

    Ok(commands::groups(result))
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
    rank: RankValue,
    file: FileValue,
) -> Result<()> {
    let creature_id = creature_data.creature_id();
    let player = engine.game.player_mut(creature_data.owner());
    let player_name = player.name;
    let creature = Creature {
        data: creature_data,
        position: BoardPosition { rank, file },
        spells: vec![],
        state: CreatureState::default(),
    };
    let rules = creature.data.rules.clone();

    reveal_card(player_name, result, card_data, Some(rank), Some(file))?;

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
    let spell_id = spell.spell_id();
    let player = engine.game.player_mut(spell.owner());
    let player_name = player.name;
    let creature = player
        .creatures
        .iter_mut()
        .find(|c| c.creature_id() == target_id)
        .ok_or_else(|| eyre!("Creature not found"))?;

    reveal_card(
        player_name,
        result,
        card_data,
        Some(creature.position.rank),
        Some(creature.position.file),
    )?;

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

    reveal_card(player.name, result, card_data, None, None)?;

    player.scrolls.push(scroll);

    engine.invoke_as_group(result, Trigger::ScrollPlayed(player_name, scroll_id))?;
    Ok(())
}
