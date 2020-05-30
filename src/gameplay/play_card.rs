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

use eyre::{eyre, Result};

use crate::{
    api, commands,
    model::{
        cards::Card,
        creatures::{Creature, CreatureData, CreatureState, HasCreatureData},
        games::Player,
        primitives::{BoardPosition, CardId, CreatureId, FileValue, PlayerName, RankValue},
    },
    requests,
    rules::engine::{RulesEngine, Trigger},
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
    let card = player.remove_from_hand(card_id)?;
    let card_data = commands::card_data(&card);

    use api::play_card_request::PlayCard::*;
    let trigger = match (card, request) {
        (Card::Creature(creature_data), PlayCreature(play_creature)) => on_play_creature(
            player,
            &mut result,
            creature_data,
            card_data,
            requests::convert_rank(play_creature.rank_position())?,
            requests::convert_file(play_creature.file_position())?,
        ),
        // (Card::Spell(spell), PlayAttachment(play_attachment)) => commands::empty(),
        // (Card::Scroll(scroll), PlayUntargeted(_)) => commands::empty(),
        _ => Err(eyre!("Card did not match targeting type")),
    }?;

    engine.invoke_as_group(&mut result, trigger)?;
    Ok(commands::groups(result))
}

fn on_play_creature(
    player: &mut Player,
    result: &mut Vec<api::CommandGroup>,
    creature_data: CreatureData,
    card_data: api::CardData,
    rank: RankValue,
    file: FileValue,
) -> Result<Trigger> {
    let creature_id = creature_data.creature_id();
    let creature = Creature {
        data: creature_data,
        position: BoardPosition { rank, file },
        spells: vec![],
        state: CreatureState::default(),
    };

    if player.name == PlayerName::User {
        result.push(commands::single(commands::destroy_card_command(
            player.name,
            card_data
                .card_id
                .ok_or_else(|| eyre!("card_id is required"))?,
            false,
        )));
    } else {
        result.push(commands::single(commands::reveal_card_command(
            card_data,
            1000,
            Some(rank),
            Some(file),
        )));
    }

    result.push(commands::single(
        commands::create_or_update_creature_command(&creature),
    ));
    player.creatures.push(creature);

    Ok(Trigger::CreaturePlayed(player.name, creature_id))
}
