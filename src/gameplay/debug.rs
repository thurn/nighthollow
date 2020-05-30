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

use eyre::Result;

use crate::{
    api::{self, MDebugRequest},
    commands,
    model::games::Player,
    requests,
    rules::engine::RulesEngine,
};

pub fn draw_debug_cards(
    engine: &mut RulesEngine,
    request: &MDebugRequest,
    result: &mut Vec<api::CommandGroup>,
) -> Result<()> {
    let mut group = vec![];
    request
        .draw_user_cards
        .iter()
        .try_for_each(|i| draw_card_at_index(&mut engine.game.user, *i, &mut group, true))?;
    request
        .draw_enemy_cards
        .iter()
        .try_for_each(|i| draw_card_at_index(&mut engine.game.enemy, *i, &mut group, false))?;
    result.push(commands::group(group));

    Ok(())
}

fn draw_card_at_index(
    player: &mut Player,
    index: u32,
    commands: &mut Vec<api::Command>,
    revealed: bool,
) -> Result<()> {
    let card = player.deck.draw_card_at_index(index as usize)?;
    commands.push(commands::draw_or_update_card_command(&card));
    player.hand.push(card);
    Ok(())
}
