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

use std::collections::BTreeSet;

use crate::prelude::*;

use super::{
    creature_skills,
    events::{Event, Events},
};
use crate::{
    api, commands,
    model::{cards::HasCardData, games::Game, players::HasOwner},
};

pub fn generate(game: &Game, events: Events, result: &mut Vec<api::Command>) -> Result<()> {
    let mut update_players = BTreeSet::new();
    let mut update_cards = BTreeSet::new();
    let mut played_cards = BTreeSet::new();

    for event_data in &events.data {
        match &event_data.event {
            Event::CardDrawn(player_name, card_id) => {
                update_cards.insert(*card_id);
            }
            Event::CardPlayed(player_name, card_id) => {
                played_cards.insert(*card_id);
            }
            Event::PlayerAttributeModified(modified) => {
                update_players.insert(modified.player_name);
            }
            Event::CreatureSkillUsed(skill) => {
                result.push(creature_skills::command_for_skill(game, &skill)?);
            }
            Event::CanPlayCardModified(card_id) => {
                update_cards.insert(*card_id);
            }
            _ => {}
        }
    }

    for player in update_players {
        result.push(commands::update_player_command(game.player(player)));
    }

    for card_id in update_cards {
        if !played_cards.contains(&card_id) && game.has_card(card_id) {
            result.push(commands::draw_or_update_card_command(game.card(card_id)?));
        }
    }

    Ok(())
}
