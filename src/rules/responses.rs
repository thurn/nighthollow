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

use eyre::Result;

use super::events::{Event, Events};
use crate::{api, commands, model::games::Game};

pub fn generate(game: &Game, events: Events, result: &mut Vec<api::Command>) -> Result<()> {
    let mut update_players = BTreeSet::new();
    for event_data in &events.data {
        match event_data.event {
            Event::CardDrawn(player_name, card_id) => {
                result.push(commands::draw_or_update_card_command(
                    game.player(player_name).card(card_id)?,
                ));
            }
            Event::PlayerAttributeSet(player_name, _) => {
                update_players.insert(player_name);
            }
        }
    }

    for player in &update_players {
        result.push(commands::update_player_command(game.player(*player)));
    }

    Ok(())
}
