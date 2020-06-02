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
        creatures::HasCreatureData,
        games::Game,
        players::Player,
        primitives::{CreatureId, GamePhase},
    },
    rules::engine::{RulesEngine, Trigger},
};

pub fn run_combat(engine: &mut RulesEngine) -> Result<api::CommandList> {
    let mut result = vec![];

    engine.invoke_as_group(&mut result, Trigger::CombatStart)?;
    let mut round_number = 1;

    while has_living_creatures(&engine.game.user) && has_living_creatures(&engine.game.enemy) {
        let mut commands = vec![];
        engine.invoke_trigger(&mut commands, Trigger::RoundStart(round_number))?;

        for creature_id in initiative_order(&engine.game) {
            if engine.game.creature(creature_id)?.is_alive() {
                engine.invoke_trigger(&mut commands, Trigger::ActionStart(creature_id))?;

                if let Some(skill_id) = engine.game.creature(creature_id)?.highest_priority_skill()
                {
                    engine.invoke_rule(
                        skill_id,
                        &mut commands,
                        Trigger::InvokeSkill(creature_id),
                    )?;
                }

                engine.invoke_trigger(&mut commands, Trigger::ActionEnd(creature_id))?;
            }
        }

        engine.invoke_trigger(&mut commands, Trigger::RoundEnd(round_number))?;

        if !commands.is_empty() {
            result.push(commands::group(commands));
        }
        round_number += 1;
    }

    engine.invoke_as_group(&mut result, Trigger::CombatEnd)?;

    run_end_of_combat(&mut engine.game, &mut result);

    engine.game.state.phase = GamePhase::Main;
    Ok(commands::groups(result))
}

/// True if this player has any living creatures
fn has_living_creatures(player: &Player) -> bool {
    player.creatures.iter().any(|c| c.is_alive())
}

/// Returns an iterator over creature IDs in initiatve order
fn initiative_order(game: &Game) -> impl Iterator<Item = CreatureId> {
    let mut pairs = game
        .user
        .creatures
        .iter()
        .chain(game.enemy.creatures.iter())
        .map(|c| (c.stats().initiative.value(), c.creature_id()))
        .collect::<Vec<_>>();
    pairs.sort();
    pairs.into_iter().map(|pair| pair.1)
}

fn run_end_of_combat(game: &mut Game, commands: &mut Vec<api::CommandGroup>) {
    let mut group1 = vec![];
    let mut group2 = vec![];

    for creature in game.all_creatures_mut() {
        if creature.is_alive() {
            group1.push(commands::remove_creature_command(creature.creature_id()));
        }

        creature.reset();
        group2.push(commands::create_or_update_creature_command(&creature));
    }

    commands.push(commands::single(commands::wait_command(1000)));
    commands.push(commands::group(group1));
    commands.push(commands::group(group2));
}
