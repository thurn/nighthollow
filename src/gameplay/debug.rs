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
    api::{self, MDebugRequest},
    commands,
    model::{
        cards::{HasCardData, HasCardId},
        players::Player,
        primitives::PlayerName,
    },
    requests,
    rules::engine::{EntityId, RulesEngine},
};
use api::MDebugPlayerUpdate;

pub fn handle_debug_request(
    engine: &mut RulesEngine,
    request: &MDebugRequest,
    result: &mut Vec<api::CommandGroup>,
) -> Result<()> {
    let mut group = vec![];

    request
        .player_updates
        .iter()
        .try_for_each(|u| handle_player_update(engine, u, &mut group))?;

    result.push(commands::group(group));
    Ok(())
}

fn handle_player_update(
    engine: &mut RulesEngine,
    update: &MDebugPlayerUpdate,
    commands: &mut Vec<api::Command>,
) -> Result<()> {
    update.draw_cards_at_index.iter().try_for_each(|i| {
        draw_card_at_index(
            engine,
            requests::convert_player_name(update.player_name())?,
            *i,
            commands,
            true,
        )
    })?;
    Ok(())
}

fn draw_card_at_index(
    engine: &mut RulesEngine,
    player_name: PlayerName,
    index: u32,
    commands: &mut Vec<api::Command>,
    revealed: bool,
) -> Result<()> {
    let player = engine.game.player_mut(player_name);
    let card = player.deck.draw_card_at_index(index as usize)?;
    let card_id = card.card_id();
    let rules = card.card_data().rules.clone();
    commands.push(commands::draw_or_update_card_command(&card));
    player.hand.push(card);
    engine.add_rules(EntityId::CardId(card_id), rules);
    Ok(())
}
