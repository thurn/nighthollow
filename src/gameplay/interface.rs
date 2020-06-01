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

use super::combat;
use crate::{
    api::{self, MClickMainButtonRequest},
    commands,
    model::primitives::GamePhase,
    rules::engine::{RulesEngine, Trigger},
};

pub fn on_click_main_button_request(
    engine: &mut RulesEngine,
    message: &MClickMainButtonRequest,
) -> Result<api::CommandList> {
    match engine.game.state.phase {
        GamePhase::Main => advance_turn(engine),
        GamePhase::Preparation => combat::run_combat(engine),
    }
}

fn advance_turn(engine: &mut RulesEngine) -> Result<api::CommandList> {
    let mut result = vec![];
    engine.invoke_trigger(&mut result, Trigger::TurnStart)?;
    engine.game.state.phase = GamePhase::Preparation;
    Ok(commands::single_group(result))
}
