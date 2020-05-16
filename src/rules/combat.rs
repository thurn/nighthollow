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
use eyre::eyre;

use super::rules::{Effects, Rule, RuleContext};
use crate::{
    api,
    model::{
        primitives::CreatureId,
        types::{Creature, Game, Player},
    },
};

/// Handles running the Combat phase of the game and populating a list of
/// resulting Commands
pub fn run_combat(game: &mut Game) -> Result<api::CommandList> {
    let mut round_number = 1;
    run_all_rules(game, |rule, rc| rule.on_combat_start(rc));

    while has_living_creatures(&game.user) && has_living_creatures(&game.enemy) {
        run_all_rules(game, |rule, rc| rule.on_round_start(rc, round_number));

        for creature_id in initiative_order(game) {
            run_creature_rules(game, creature_id, |rule, rc| rule.on_action_start(rc));
        }

        run_all_rules(game, |rule, rc| rule.on_round_end(rc, round_number));
        round_number += 1;
    }

    run_all_rules(game, |rule, rc| rule.on_combat_end(rc));

    Err(eyre!("Not implemented"))
}

/// True if this player has any living creatures
fn has_living_creatures(player: &Player) -> bool {
    player.creatures.iter().any(Creature::is_alive)
}

/// Get a refernece to a creature with the given ID
fn get_creature(game: &Game, creature_id: CreatureId) -> Result<&Creature> {
    game.user
        .creatures
        .iter()
        .chain(game.enemy.creatures.iter())
        .find(|c| c.creature_id() == creature_id)
        .ok_or(eyre!("Creature {} not found", creature_id))
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

/// Returns an iterator over all creature IDs in the game in their insertion order
fn all_creature_ids(game: &Game) -> impl Iterator<Item = CreatureId> {
    game.user
        .creatures
        .iter()
        .chain(game.enemy.creatures.iter())
        .map(|c| c.creature_id())
        .collect::<Vec<_>>()
        .into_iter()
}

/// Executes a given rule callback function on every rule in the game
fn run_all_rules(game: &mut Game, function: impl Fn(&Box<dyn Rule>, &RuleContext) -> ()) {
    let mut effects = Effects::new();
    for creature_id in all_creature_ids(game) {
        populate_effects(game, creature_id, &mut effects, |rule, rc| {
            function(rule, rc)
        });
    }
    resolve_effects(game, &effects);
}

/// Executes a given rule callback function on the rules for a specific creature
fn run_creature_rules(
    game: &mut Game,
    creature_id: CreatureId,
    function: impl Fn(&Box<dyn Rule>, &RuleContext) -> (),
) {
    let mut effects = Effects::new();
    populate_effects(game, creature_id, &mut effects, function);
    resolve_effects(game, &effects);
}

/// Runs a rule callback function to populate the "Effects" buffer with the
/// rule's effects
fn populate_effects(
    game: &Game,
    creature_id: CreatureId,
    effects: &mut Effects,
    function: impl Fn(&Box<dyn Rule>, &RuleContext) -> (),
) {
    let creature = get_creature(game, creature_id).expect("");
    for rule in creature.archetype.rules.iter() {
        let rule_context = RuleContext {
            rule_id: 17,
            output: effects,
            owner: creature,
            game,
        };
        function(rule, &rule_context);
    }
}

/// Applies mutations to the game state based on the effects in the provided
/// Effects buffer, recursivley triggering further rules
fn resolve_effects(game: &mut Game, effects: &Effects) {}
