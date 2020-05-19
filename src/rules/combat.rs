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

use super::rules::{CreatureState, Effect, EffectData, Effects, Rule, RuleContext};
use crate::{
    api,
    model::{
        primitives::{CreatureId, HealthValue, ManaValue},
        stats::Modifier,
        types::{Creature, Damage, Game, Player},
    },
};
use std::iter;

/// Handles running the Combat phase of the game and populating a list of
/// resulting Commands
// pub fn run_combat(game: &mut Game) -> Result<api::CommandList> {
//     let mut round_number = 1;
//     run_all_rules(game, |rule, rc, e| rule.on_combat_start(rc, e))?;

//     while has_living_creatures(&game.user) && has_living_creatures(&game.enemy) {
//         run_all_rules(game, |rule, rc, e| rule.on_round_start(rc, e, round_number))?;
//         for creature_id in initiative_order(game) {
//             run_creature_rules(game, creature_id, |rule, rc, e| rule.on_action_start(rc, e))?;
//         }

//         run_all_rules(game, |rule, rc, e| rule.on_round_end(rc, e, round_number))?;
//         round_number += 1;
//     }

//     run_all_rules(game, |rule, rc, e| rule.on_combat_end(rc, e))?;

//     Err(eyre!("Not implemented"))
// }

/// True if this player has any living creatures
fn has_living_creatures(player: &Player) -> bool {
    player.creatures.iter().any(Creature::is_alive)
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

#[derive(Debug, Copy, Clone)]
enum RuleScope {
    AllCreatures,
    Creature(CreatureId),
}

impl RuleScope {
    fn ids(self, game: &Game) -> Vec<CreatureId> {
        match self {
            RuleScope::AllCreatures => all_creature_ids(game).collect::<Vec<_>>(),
            RuleScope::Creature(creature_id) => vec![creature_id],
        }
    }
}

/// Top-level entry point for executing a Rule and applying all resulting
/// game mutations.
///
/// Rule execution involves the following steps:
/// 1) Allocating a new Effects buffer to store generated effects
/// 2) Evaluating the provided 'function' one time for each Rule matching the
/// scope specified in RuleScope, pouplating the Effects buffer with the
/// effects of those rules.
/// 3) Actually mutating the provided Game to resolve those effects
/// 4) Triggering events caused by those mutations. Each mutation has a
/// corresponding event, e.g. if you apply damage via the ApplyDamage effect,
/// various events like "on_applied_damage" and "on_took_damage" trigger. This
/// results in a new batch of Effects being generated.
/// 5) If any new effects were generated, *those* effects are recursively
/// applied and we return to step #3
///
/// Thus, rule execution proceeds in batches of 'evaluation' and 'execution'
/// steps. This separation is intended to ensure that each Rule event callback
/// sees a consistent, event-order-independent snapshot of the game state.
fn run_rule(
    game: &mut Game,
    scope: RuleScope,
    function: impl Fn(&Box<dyn Rule>, RuleArgs) -> Result<()>,
) -> Result<()> {
    let mut effects = Effects::new();

    evaluate_rule_function(game, scope, &mut effects, function)?;

    loop {
        for effect in effects.iter() {
            apply_mutation(game, effect)?;
        }

        let mut triggered_effects = Effects::new();
        for effect in effects.iter() {
            trigger_effect_events(game, &mut triggered_effects, effect)?;
        }

        if triggered_effects.len() == 0 {
            break;
        }

        effects = triggered_effects;
    }

    Ok(())
}

struct RuleArgs<'a> {
    rc: &'a RuleContext<'a>,
    effects: &'a mut Effects,
}

/// Helper to build the arguments for a Rule callback function. Takes a
/// game state and effects buffer and packages them up into the correct
/// RuleContext to use as a callback argument, and then invokes the provided
/// function once for each Rule matching the provided RuleScope, which should
/// result in 'effects' being populated with all of the Effects generated by
/// those rules
fn evaluate_rule_function(
    game: &Game,
    scope: RuleScope,
    effects: &mut Effects,
    function: impl Fn(&Box<dyn Rule>, RuleArgs) -> Result<()>,
) -> Result<()> {
    for creature_id in scope.ids(game) {
        let creature = game.creature(creature_id)?;
        for rule in creature.archetype.rules.iter() {
            let rule_context = RuleContext {
                rule_id: 17,
                owner: CreatureState::new(creature),
                game,
            };

            function(
                rule,
                RuleArgs {
                    rc: &rule_context,
                    effects,
                },
            )?;
        }
    }

    Ok(())
}

fn apply_mutation(game: &mut Game, effect_data: &EffectData) -> Result<()> {
    Ok(())
}

fn trigger_effect_events(
    game: &Game,
    effects: &mut Effects,
    effect_data: &EffectData,
) -> Result<()> {
    match &effect_data.effect {
        Effect::ApplyDamage {
            creature_id,
            damage,
        } => trigger_apply_damage_events(
            game,
            effects,
            effect_data.source_creature_id,
            *creature_id,
            damage,
        ),
        _ => Ok(()),
    }
}

fn trigger_apply_damage_events(
    game: &Game,
    effects: &mut Effects,
    attacker_id: CreatureId,
    defender_id: CreatureId,
    damage: &Damage,
) -> Result<()> {
    evaluate_rule_function(
        game,
        RuleScope::Creature(attacker_id),
        effects,
        |r, args| {
            r.on_applied_damage(
                args.rc,
                args.effects,
                damage,
                args.rc.game.creature(defender_id)?,
            );
            Ok(())
        },
    )?;

    evaluate_rule_function(
        game,
        RuleScope::Creature(defender_id),
        effects,
        |r, args| {
            r.on_took_damage(
                args.rc,
                args.effects,
                damage,
                args.rc.game.creature(attacker_id)?,
            );
            Ok(())
        },
    )?;

    evaluate_rule_function(game, RuleScope::AllCreatures, effects, |r, args| {
        r.on_any_creature_damaged(
            args.rc,
            args.effects,
            damage,
            args.rc.game.creature(attacker_id)?,
            args.rc.game.creature(defender_id)?,
        );
        Ok(())
    })?;
    Ok(())
}

// Executes a given rule callback function on every rule in the game
// fn run_all_rules(
//     game: &mut Game,
//     function: impl Fn(&Box<dyn Rule>, &RuleContext, &mut Effects) -> (),
// ) -> Result<()> {
//     let mut effects = Effects::new();
//     for creature_id in all_creature_ids(game) {
//         populate_effects(game, creature_id, &mut effects, |rule, rc, e| {
//             function(rule, rc, e)
//         })?;
//     }
//     resolve_effects(game, effects)
// }

// Executes a given rule callback function on the rules for a specific creature
// fn run_creature_rules(
//     game: &mut Game,
//     creature_id: CreatureId,
//     function: impl Fn(&Box<dyn Rule>, &RuleContext, &mut Effects),
// ) -> Result<()> {
//     let mut effects = Effects::new();
//     populate_effects(game, creature_id, &mut effects, function)?;
//     resolve_effects(game, effects)?;
//     Ok(())
// }

// Runs a rule callback function to populate the "Effects" buffer with the
// rule's effects
// fn populate_effects(
//     game: &Game,
//     creature_id: CreatureId,
//     effects: &mut Effects,
//     function: impl Fn(&Box<dyn Rule>, &RuleContext, &mut Effects) -> (),
// ) -> Result<()> {
//     let creature = game.creature(creature_id)?;
//     for rule in creature.archetype.rules.iter() {
//         let rule_context = RuleContext {
//             rule_id: 17,
//             owner: CreatureState::new(creature),
//             game,
//             effects,
//         };
//         function(rule, &rule_context, effects);
//     }
//     Ok(())
// }

// Applies mutations to the game state based on the effects in the provided
// Effects buffer, recursivley triggering further rules
// fn resolve_effects(game: &mut Game, effects: Effects) -> Result<()> {
//     for effect_data in effects.iter() {
//         let effect = &effect_data.effect;
//         match effect {
//             Effect::ApplyDamage {
//                 creature_id,
//                 damage,
//             } => game.creature_mut(*creature_id)?.stats_mut().damage += damage.total(),
//             Effect::HealDamage {
//                 creature_id,
//                 amount,
//             } => game.creature_mut(*creature_id)?.stats_mut().damage -= amount,
//             Effect::GainMana {
//                 creature_id,
//                 amount,
//             } => game.creature_mut(*creature_id)?.stats_mut().mana += amount,
//             Effect::SpendMana {
//                 creature_id,
//                 amount,
//             } => game.creature_mut(*creature_id)?.stats_mut().mana -= amount,
//             Effect::SetModifier {
//                 creature_id,
//                 stat,
//                 value,
//                 operation,
//             } => game
//                 .creature_mut(*creature_id)?
//                 .stats_mut()
//                 .get_mut(*stat)
//                 .add_modifier(Modifier {
//                     value: *value,
//                     operation: *operation,
//                     source: effect_data.rule_id,
//                 }),
//         }
//     }
//     Ok(())
// }

// fn trigger_effect_rules(game: &mut Game, effects: Effects) -> Result<()> {
//     let mut new_effects = Effects::new();
//     for effect_data in effects.iter() {
//         let effect = &effect_data.effect;
//         match effect {
//             Effect::ApplyDamage {
//                 creature_id,
//                 damage,
//             } => {
//                 populate_effects(
//                     game,
//                     effect_data.source_creature_id,
//                     &mut new_effects,
//                     |rule, rc, e| {
//                         let target = rc.game.creature(*creature_id).expect("");
//                         rule.on_applied_damage(rc, e, damage, target)
//                     },
//                 )?;
//                 populate_effects(game, *creature_id, &mut new_effects, |rule, rc, e| {
//                     let from = rc.game.creature(effect_data.source_creature_id).expect("");
//                     rule.on_took_damage(rc, e, damage, from)
//                 })?;
//             }
//             Effect::HealDamage {
//                 creature_id,
//                 amount,
//             } => game.creature_mut(*creature_id)?.stats_mut().damage -= amount,
//             Effect::GainMana {
//                 creature_id,
//                 amount,
//             } => game.creature_mut(*creature_id)?.stats_mut().mana += amount,
//             Effect::SpendMana {
//                 creature_id,
//                 amount,
//             } => game.creature_mut(*creature_id)?.stats_mut().mana -= amount,
//             Effect::SetModifier {
//                 creature_id,
//                 stat,
//                 value,
//                 operation,
//             } => game
//                 .creature_mut(*creature_id)?
//                 .stats_mut()
//                 .get_mut(*stat)
//                 .add_modifier(Modifier {
//                     value: *value,
//                     operation: *operation,
//                     source: effect_data.rule_id,
//                 }),
//         }
//     }
//     Ok(())
// }
