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

use super::{combat, play_card};
use crate::{
    agents::agent::AgentAction,
    api::{self, MClickMainButtonRequest},
    commands,
    model::{cards::CardWithTarget, primitives::GamePhase},
    rules::engine::{RulesEngine, Trigger},
};
use play_card::PlayCardWithTarget;

pub fn on_click_main_button_request(
    engine: &mut RulesEngine,
    message: &MClickMainButtonRequest,
) -> Result<api::CommandList> {
    match engine.game.state.phase {
        GamePhase::Main => Ok(commands::groups(advance_turn(engine)?)),
        GamePhase::Preparation => Ok(commands::groups(to_main_phase(engine)?)),
    }
}

fn to_main_phase(engine: &mut RulesEngine) -> Result<Vec<api::CommandGroup>> {
    let mut result = vec![];

    apply_agent_actions(engine, &mut result)?;

    combat::run_combat(engine, &mut result)?;

    engine.game.state.phase = GamePhase::Main;
    engine.invoke_as_group(&mut result, Trigger::MainPhaseStart)?;

    result.push(commands::single(commands::update_interface_state_command(
        true, "End Turn",
    )));

    Ok(result)
}

fn advance_turn(engine: &mut RulesEngine) -> Result<Vec<api::CommandGroup>> {
    let mut result = vec![];

    apply_agent_actions(engine, &mut result)?;

    engine.invoke_as_group(&mut result, Trigger::TurnEnd)?;
    engine.game.state.phase = GamePhase::Preparation;
    engine.game.state.turn += 1;
    engine.invoke_as_group(&mut result, Trigger::TurnStart)?;

    if engine.game.all_creatures().any(|x| true) {
        result.push(commands::single(commands::update_interface_state_command(
            true, "Combat!",
        )));
    } else {
        result.extend(to_main_phase(engine)?);
    }

    Ok(result)
}

fn apply_agent_actions(
    engine: &mut RulesEngine,
    result: &mut Vec<api::CommandGroup>,
) -> Result<()> {
    loop {
        let action = engine
            .game
            .agent
            .next_action(&engine.game, &engine.game.enemy)?;

        match action {
            AgentAction::Pass => {
                println!("agent::pass");
                break;
            }
            AgentAction::PlayCard(card_with_target) => {
                let card = PlayCardWithTarget::from(card_with_target);
                println!("play card {}", card.id);
                play_card::play(engine, card, result)?;
            }
        }
    }
    Ok(())
}
