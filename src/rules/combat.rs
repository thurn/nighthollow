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

use color_eyre::Result;

use super::rules::{self, Rule, RuleContext, RuleScope};
use crate::{
    api, commands,
    model::{
        creatures::{Creature, Damage, DamageResult},
        games::{Game, Player},
        primitives::{CreatureId, HealthValue, ManaValue, RuleId},
        stats::{Modifier, Operation, StatName},
    },
};

/// Handles running the Combat phase of the game and populating a list of
/// resulting Commands
pub fn run_combat(game: &mut Game) -> Result<api::CommandList> {
    let mut result: Vec<api::CommandGroup> = vec![];
    rules::run_as_group(game, &mut result, RuleScope::AllCreatures, |rule, args| {
        Ok(rule.on_combat_start(args.rc, args.effects))
    })?;

    let mut round_number = 1;
    while has_living_creatures(&game.user) && has_living_creatures(&game.enemy) {
        let mut commands: Vec<api::Command> = vec![];

        rules::run_rule(
            game,
            &mut commands,
            RuleScope::AllCreatures,
            |rule, args| Ok(rule.on_round_start(args.rc, args.effects, round_number)),
        )?;

        for creature_id in initiative_order(game) {
            rules::run_rule(
                game,
                &mut commands,
                RuleScope::Creature(creature_id),
                |rule, args| Ok(rule.on_action_start(args.rc, args.effects)),
            )?;

            invoke_main_skill(game, &mut commands, creature_id)?;

            rules::run_rule(
                game,
                &mut commands,
                RuleScope::Creature(creature_id),
                |rule, args| Ok(rule.on_action_end(args.rc, args.effects)),
            )?;
        }

        rules::run_rule(
            game,
            &mut commands,
            RuleScope::AllCreatures,
            |rule, args| Ok(rule.on_round_end(args.rc, args.effects, round_number)),
        )?;

        if commands.len() > 0 {
            result.push(api::CommandGroup { commands });
        }
        round_number += 1;
    }

    rules::run_as_group(game, &mut result, RuleScope::AllCreatures, |rule, args| {
        Ok(rule.on_combat_end(args.rc, args.effects))
    })?;

    for creature in game.user.creatures.iter_mut() {
        creature.reset()
    }
    for creature in game.enemy.creatures.iter_mut() {
        creature.reset()
    }

    Ok(api::CommandList {
        command_groups: result,
    })
}

/// True if this player has any living creatures
fn has_living_creatures(player: &Player) -> bool {
    player.creatures.iter().any(|c| c.is_alive)
}

/// Returns an iterator over creature IDs in initiatve order
fn initiative_order(game: &Game) -> impl Iterator<Item = CreatureId> {
    // Assumptions:
    // 1) The set of creatures does not change during combat
    // 2) Initiative order is evaluated only once, at start of combat
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

struct RuleWithPriority {
    priority: u32,
    index: usize,
}

fn invoke_main_skill(
    game: &mut Game,
    commands: &mut Vec<api::Command>,
    creature_id: CreatureId,
) -> Result<()> {
    let creature = game.creature(creature_id)?;
    let highest_priority = creature
        .data
        .rules
        .iter()
        .enumerate()
        .map(|(index, rule)| {
            rule.on_calculate_skill_priority(&RuleContext {
                rule_id: RuleId { creature_id, index },
                creature,
                game,
            })
            .map(|priority| RuleWithPriority { priority, index })
        })
        .filter_map(|x| x)
        .max_by_key(|prioritized| prioritized.priority);

    if let Some(rule_with_priority) = highest_priority {
        rules::run_rule(
            game,
            commands,
            RuleScope::SpecificRule(creature_id, rule_with_priority.index),
            |rule, args| Ok(rule.on_invoke_skill(args.rc, args.effects)),
        )?;
    }

    Ok(())
}
