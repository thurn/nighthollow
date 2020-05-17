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

use super::rules::{Effect, Effects, Rule, RuleContext};
use crate::{
    api,
    model::{
        primitives::{CreatureId, HealthValue, ManaValue},
        types::{Creature, Damage, Game, Player},
    },
};

/// Handles running the Combat phase of the game and populating a list of
/// resulting Commands
pub fn run_combat(game: &mut Game) -> Result<api::CommandList> {
    let mut round_number = 1;
    run_all_rules(game, |rule, rc, e| rule.on_combat_start(rc, e));

    while has_living_creatures(&game.user) && has_living_creatures(&game.enemy) {
        run_all_rules(game, |rule, rc, e| rule.on_round_start(rc, e, round_number));
        for creature_id in initiative_order(game) {
            run_creature_rules(game, creature_id, |rule, rc, e| rule.on_action_start(rc, e));
        }

        run_all_rules(game, |rule, rc, e| rule.on_round_end(rc, e, round_number));
        round_number += 1;
    }

    run_all_rules(game, |rule, rc, e| rule.on_combat_end(rc, e));

    Err(eyre!("Not implemented"))
}

/// True if this player has any living creatures
fn has_living_creatures(player: &Player) -> bool {
    player.creatures.iter().any(Creature::is_alive)
}

/// Get a reference to a creature with the given ID
fn find_creature(game: &Game, creature_id: CreatureId) -> Result<&Creature> {
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
fn run_all_rules(
    game: &mut Game,
    function: impl Fn(&Box<dyn Rule>, &RuleContext, &mut Effects) -> (),
) {
    let mut effects = Effects::new();
    for creature_id in all_creature_ids(game) {
        populate_effects(game, creature_id, &mut effects, |rule, rc, e| {
            function(rule, rc, e)
        });
    }
    resolve_effects(game, effects);
}

/// Executes a given rule callback function on the rules for a specific creature
fn run_creature_rules(
    game: &mut Game,
    creature_id: CreatureId,
    function: impl Fn(&Box<dyn Rule>, &RuleContext, &mut Effects) -> (),
) {
    let mut effects = Effects::new();
    populate_effects(game, creature_id, &mut effects, function);
    resolve_effects(game, effects);
}

/// Runs a rule callback function to populate the "Effects" buffer with the
/// rule's effects
fn populate_effects(
    game: &Game,
    creature_id: CreatureId,
    effects: &mut Effects,
    function: impl Fn(&Box<dyn Rule>, &RuleContext, &mut Effects) -> (),
) {
    let creature = find_creature(game, creature_id).expect("");
    for rule in creature.archetype.rules.iter() {
        let rule_context = RuleContext {
            rule_id: 17,
            owner: creature,
            game,
        };
        function(rule, &rule_context, effects);
    }
}

/// Applies mutations to the game state based on the effects in the provided
/// Effects buffer, recursivley triggering further rules
fn resolve_effects(game: &mut Game, effects: Effects) {
    for effect in effects.into_iter() {
        match effect {
            Effect::ApplyDamage(creature_id, damage) => apply_damage(creature_id, damage),
            Effect::ApplyDamageRef(creature, damage) => apply_damage_ref(creature, damage),
            Effect::HealDamage(creature_id, healed) => heal_damage(creature_id, healed),
            Effect::GainMana(creature_id, mana) => gain_mana(creature_id, mana),
            Effect::SpendMana(creature_id, mana) => spend_mana(creature_id, mana),
            Effect::SetModifier(creature_id, stat_name, modifier) => {
                set_modifier(creature_id, stat_name, modifier)
            }
        }
    }
}

fn apply_damage(creature_id: CreatureId, damage: Damage) {
    todo!()
}

fn apply_damage_ref(creature: &Creature, damage: Damage) {
    todo!()
}

fn heal_damage(creature_id: CreatureId, healed: HealthValue) {
    todo!()
}

fn set_modifier(
    creature_id: CreatureId,
    stat_name: crate::model::stats::StatName,
    modifier: crate::model::stats::Modifier,
) {
    todo!()
}

fn spend_mana(creature_id: CreatureId, mana: ManaValue) {
    todo!()
}

fn gain_mana(creature_id: CreatureId, mana: ManaValue) {
    todo!()
}
