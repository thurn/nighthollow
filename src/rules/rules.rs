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

use std::fmt::Debug;

use color_eyre::Result;
use eyre::eyre;
use serde::{Deserialize, Serialize};

use super::effects::{self, Effects, MutationEvent, MutationEventType, SetModifier};
use crate::{
    api,
    model::{
        primitives::{
            ActionNumber, CardId, CreatureId, HealthValue, ManaValue, RoundNumber, RuleId,
            TurnNumber,
        },
        stats::{Modifier, Operation, StatName},
        types::{Creature, Damage, DamageResult, Game, HasCardData, Player},
    },
};

#[typetag::serde(tag = "type")]
pub trait Rule: Debug + Send {
    /// Called a the start of a new game turn
    fn on_turn_start(&self, context: &RuleContext, effects: &mut Effects) {}

    /// Called at end of turn
    fn on_turn_end(&self, context: &RuleContext, effects: &mut Effects) {}

    /// Called when a new combat phase begins
    fn on_combat_start(&self, context: &RuleContext, effects: &mut Effects) {}

    /// Called when a combat phase ends
    fn on_combat_end(&self, context: &RuleContext, effects: &mut Effects) {}

    /// Called at the start of a combat round
    fn on_round_start(&self, context: &RuleContext, effects: &mut Effects, round: RoundNumber) {}

    /// Called at the end of a combat round
    fn on_round_end(&self, context: &RuleContext, effects: &mut Effects, round: RoundNumber) {}

    /// Called when it is time for this creature to take an action during
    /// combat.
    fn on_action_start(&self, context: &RuleContext, effects: &mut Effects) {}

    /// Called at the end of this creature's combat action
    fn on_action_end(&self, context: &RuleContext, effects: &mut Effects) {}

    /// Called to calculate the priority of the main combat skill for this
    /// creature. If this rule contributes a skill which should be considered
    /// as this creature's action, return a priority number. The rule for this
    /// creature which returns the highest skill priority will have its
    /// on_invoke_skill() callback invoked. In the case of a tie, a random
    /// skill from among the tied skills is selected.
    fn on_calculate_skill_priority(&self, context: &RuleContext) -> Option<u32> {
        None
    }

    /// Called during a creature's action when this rule returns the highest
    /// priority value from on_calculate_skill_priority, as discussed above
    fn on_invoke_skill(&self, context: &RuleContext, effects: &mut Effects) {}

    /// Called when this creature applies damage to an opposing creature with
    /// the final damage value
    fn on_applied_damage(
        &self,
        context: &RuleContext,
        effects: &mut Effects,
        damage: &Damage,
        to_target: &Creature,
    ) {
    }

    /// Called when this creature is damaged by an opposing creature with the
    /// final damage value
    fn on_took_damage(
        &self,
        context: &RuleContext,
        effects: &mut Effects,
        damage: &Damage,
        from_source: &Creature,
    ) {
    }

    /// Called whenever any creature is damaged
    fn on_any_creature_damaged(
        &self,
        context: &RuleContext,
        effects: &mut Effects,
        damage: &Damage,
        attacker: &Creature,
        defender: &Creature,
    ) {
    }

    /// Called when this creature kills an enemy creature
    fn on_killed_enemy(&self, context: &RuleContext, effects: &mut Effects, enemy: &Creature) {}

    /// Called when this creature dies (its damage total exceeds its health value)
    fn on_death(&self, context: &RuleContext, effects: &mut Effects, killed_by: &Creature) {}

    /// Called whenever any creature on either side of combat dies
    fn on_any_creature_killed(
        &self,
        context: &RuleContext,
        effects: &mut Effects,
        attacker: &Creature,
        defender: &Creature,
    ) {
    }

    /// Called when this creature heals damage
    fn on_healed(
        &self,
        context: &RuleContext,
        effects: &mut Effects,
        amount: HealthValue,
        healed_by: &Creature,
    ) {
    }

    /// Called when this creature applies a heal to another creature
    fn on_applied_heal(
        &self,
        context: &RuleContext,
        effects: &mut Effects,
        amount: HealthValue,
        target: &Creature,
    ) {
    }

    /// Called whenever any creature causes damage to be healed
    fn on_any_creature_healed(
        &self,
        context: &RuleContext,
        effects: &mut Effects,
        amount: HealthValue,
        source: &Creature,
        target: &Creature,
    ) {
    }

    /// Called when this creature's mana is increased
    fn on_mana_gained(
        &self,
        context: &RuleContext,
        effects: &mut Effects,
        amount: ManaValue,
        source: &Creature,
    ) {
    }

    /// Called when this creature's mana is decreased
    fn on_mana_lost(
        &self,
        context: &RuleContext,
        effects: &mut Effects,
        amount: ManaValue,
        source: &Creature,
    ) {
    }

    /// Called when a stat modifier is set on this creature
    fn on_stat_modifier_set(
        &self,
        context: &RuleContext,
        effects: &mut Effects,
        modifier: &SetModifier,
        source: &Creature,
    ) {
    }
}

// pub enum Effect {
//     ApplyDamage {
//         creature_id: CreatureId,
//         damage: Damage,
//     },
//     HealDamage {
//         creature_id: CreatureId,
//         amount: HealthValue,
//     },
//     GainMana {
//         creature_id: CreatureId,
//         amount: ManaValue,
//     },
//     LoseMana {
//         creature_id: CreatureId,
//         amount: ManaValue,
//     },
//     SetModifier {
//         creature_id: CreatureId,
//         stat: StatName,
//         value: u32,
//         operation: Operation,
//     },
// }

// pub struct EffectData {
//     pub effect: Effect,
//     pub rule_id: RuleId,
//     pub source_creature_id: CreatureId,
// }

// pub struct Effects {
//     effects: Vec<EffectData>,
// }

// impl Effects {
//     pub fn new() -> Effects {
//         Effects { effects: vec![] }
//     }

//     pub fn push_effect(&mut self, context: &RuleContext, effect: Effect) {
//         self.effects.push(EffectData {
//             effect,
//             rule_id: context.rule_id,
//             source_creature_id: context.creature.creature_id(),
//         });
//     }

//     pub fn len(&self) -> usize {
//         self.effects.len()
//     }

//     pub fn iter(&self) -> impl Iterator<Item = &EffectData> {
//         self.effects.iter()
//     }
// }

pub struct RuleContext<'a> {
    pub rule_id: RuleId,
    pub creature: &'a Creature,
    pub game: &'a Game,
}

impl<'a> RuleContext<'a> {
    pub fn owner(&self) -> &Player {
        self.game.player(self.creature.card_data().owner)
    }

    pub fn opponent(&self) -> &Player {
        self.game.player(self.creature.card_data().owner.opponent())
    }
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
pub enum RuleScope {
    AllCreatures,
    Creature(CreatureId),
    CreatureIfDead(CreatureId),
    SpecificRule(CreatureId, usize),
}

impl RuleScope {
    fn ids(self, game: &Game) -> Vec<CreatureId> {
        match self {
            RuleScope::AllCreatures => all_creature_ids(game).collect::<Vec<_>>(),
            RuleScope::Creature(creature_id) => vec![creature_id],
            RuleScope::CreatureIfDead(creature_id) => vec![creature_id],
            RuleScope::SpecificRule(creature_id, _) => vec![creature_id],
        }
    }

    fn should_process_creature(&self, creature: &Creature) -> bool {
        match self {
            RuleScope::CreatureIfDead(_) => !creature.is_alive,
            _ => creature.is_alive,
        }
    }

    fn should_process_rule(&self, index: usize) -> bool {
        match self {
            RuleScope::SpecificRule(_, position) => index == *position,
            _ => true,
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
pub fn run_rule(
    game: &mut Game,
    commands: &mut Vec<api::Command>,
    scope: RuleScope,
    function: impl Fn(&Box<dyn Rule>, RuleArgs) -> Result<()>,
) -> Result<()> {
    let mut effects = Effects::new();

    evaluate_rule_function(game, scope, &mut effects, function)?;
    if effects.len() == 0 {
        return Ok(());
    }

    loop {
        let mut events: Vec<MutationEvent> = vec![];
        for effect in effects.iter() {
            effects::apply_effect(game, effect, commands, &mut events)?;
        }

        effects = Effects::new();
        for event in events.iter() {
            trigger_mutation_events(game, &mut effects, event)?;
        }

        if effects.len() == 0 {
            break;
        }
    }

    Ok(())
}

/// Helper function which invokes run_rule() and aggregates the resulting
/// commands into a single CommandGroup, which is added to the 'groups' Vec.
pub fn run_as_group(
    game: &mut Game,
    groups: &mut Vec<api::CommandGroup>,
    scope: RuleScope,
    function: impl Fn(&Box<dyn Rule>, RuleArgs) -> Result<()>,
) -> Result<()> {
    let mut commands: Vec<api::Command> = vec![];
    run_rule(game, &mut commands, scope, function)?;
    if commands.len() > 0 {
        groups.push(api::CommandGroup { commands });
    }
    Ok(())
}

pub struct RuleArgs<'a> {
    pub rc: &'a RuleContext<'a>,
    pub effects: &'a mut Effects,
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
        if !scope.should_process_creature(creature) {
            continue;
        }

        for (index, rule) in creature.archetype.rules.iter().enumerate() {
            if scope.should_process_rule(index) {
                let rule_context = RuleContext {
                    rule_id: RuleId { creature_id, index },
                    creature,
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
    }

    Ok(())
}

// pub enum MutationEvent {
//     CreatureKilled {
//         attacker: CreatureId,
//         defender: CreatureId,
//     },
// }

// fn apply_mutation(
//     game: &mut Game,
//     effect_data: &EffectData,
//     commands: &mut Vec<api::Command>,
//     events: &mut Vec<MutationEvent>,
// ) -> Result<()> {
//     match &effect_data.effect {
//         Effect::ApplyDamage {
//             creature_id,
//             damage,
//         } => {
//             let creature = game.creature_mut(*creature_id)?;
//             match creature.apply_damage(damage.total()) {
//                 DamageResult::Killed => events.push(MutationEvent::CreatureKilled {
//                     attacker: effect_data.source_creature_id,
//                     defender: *creature_id,
//                 }),
//                 _ => {}
//             }
//         }

//         Effect::HealDamage {
//             creature_id,
//             amount,
//         } => game.creature_mut(*creature_id)?.heal(*amount),

//         Effect::GainMana {
//             creature_id,
//             amount,
//         } => game.creature_mut(*creature_id)?.gain_mana(*amount),

//         Effect::LoseMana {
//             creature_id,
//             amount,
//         } => game.creature_mut(*creature_id)?.lose_mana(*amount)?,

//         Effect::SetModifier {
//             creature_id,
//             stat,
//             value,
//             operation,
//         } => game
//             .creature_mut(*creature_id)?
//             .stats_mut()
//             .get_mut(*stat)
//             .set_modifier(Modifier {
//                 value: *value,
//                 operation: *operation,
//                 source: effect_data.rule_id,
//             }),
//     }
//     Ok(())
// }

// fn trigger_events(game: &Game, event: &MutationEvent, effects: &mut Effects) -> Result<()> {
//     match event {
//         MutationEvent::CreatureKilled { attacker, defender } => {
//             trigger_on_death_events(game, effects, *attacker, *defender)
//         }
//     }
// }

struct CreatureRuleEvaluator<'a> {
    game: &'a Game,
    effects: &'a mut Effects,
    event: &'a effects::MutationEvent,
}

impl<'a> CreatureRuleEvaluator<'a> {
    fn on_applied(
        &mut self,
        function: impl Fn(&Box<dyn Rule>, &RuleContext, &mut Effects, &Creature),
    ) -> Result<()> {
        let target = self.game.creature(self.event.target_creature)?;
        evaluate_rule_function(
            self.game,
            RuleScope::Creature(self.event.source_creature),
            self.effects,
            |r, args| {
                function(r, args.rc, args.effects, target);
                Ok(())
            },
        )
    }

    fn on_received(
        &mut self,
        function: impl Fn(&Box<dyn Rule>, &RuleContext, &mut Effects, &Creature),
    ) -> Result<()> {
        let source = self.game.creature(self.event.source_creature)?;
        evaluate_rule_function(
            self.game,
            RuleScope::Creature(self.event.target_creature),
            self.effects,
            |r, args| {
                function(r, args.rc, args.effects, source);
                Ok(())
            },
        )
    }

    fn on_any(
        &mut self,
        function: impl Fn(&Box<dyn Rule>, &RuleContext, &mut Effects, &Creature, &Creature),
    ) -> Result<()> {
        let source = self.game.creature(self.event.source_creature)?;
        let target = self.game.creature(self.event.target_creature)?;
        evaluate_rule_function(
            self.game,
            RuleScope::AllCreatures,
            self.effects,
            |r, args| {
                function(r, args.rc, args.effects, source, target);
                Ok(())
            },
        )
    }
}

fn trigger_mutation_events(
    game: &Game,
    effects: &mut Effects,
    event: &MutationEvent,
) -> Result<()> {
    let mut evaluator = CreatureRuleEvaluator {
        game,
        effects,
        event,
    };

    match &event.event_type {
        MutationEventType::AppliedDamage(damage) => {
            evaluator.on_applied(|r, rc, e, to| r.on_applied_damage(rc, e, damage, to))?;
            evaluator.on_received(|r, rc, e, from| r.on_took_damage(rc, e, damage, from))?;
            evaluator.on_any(|r, rc, e, attacker, defender| {
                r.on_any_creature_damaged(rc, e, damage, attacker, defender)
            })?;
        }
        MutationEventType::Killed(_) => {
            evaluator.on_applied(|r, rc, e, to| r.on_killed_enemy(rc, e, to))?;
            evaluator.on_received(|r, rc, e, from| r.on_death(rc, e, from))?;
            evaluator.on_any(|r, rc, e, attacker, defender| {
                r.on_any_creature_killed(rc, e, attacker, defender)
            })?;
        }
        MutationEventType::Healed(amount) => {
            evaluator.on_applied(|r, rc, e, to| r.on_applied_heal(rc, e, *amount, to))?;
            evaluator.on_received(|r, rc, e, from| r.on_healed(rc, e, *amount, from))?;
            evaluator.on_any(|r, rc, e, attacker, defender| {
                r.on_any_creature_healed(rc, e, *amount, attacker, defender)
            })?;
        }
        MutationEventType::GainedMana(amount) => {
            evaluator.on_received(|r, rc, e, from| r.on_mana_gained(rc, e, *amount, from))?;
        }
        MutationEventType::LostMana(amount) => {
            evaluator.on_received(|r, rc, e, from| r.on_mana_lost(rc, e, *amount, from))?;
        }
        MutationEventType::SetModifier(modifier) => {
            evaluator
                .on_received(|r, rc, e, from| r.on_stat_modifier_set(rc, e, modifier, from))?;
        }
    }
    Ok(())
}

// fn trigger_effect_events(
//     game: &Game,
//     effects: &mut Effects,
//     effect_data: &EffectData,
// ) -> Result<()> {
//     match &effect_data.effect {
//         Effect::ApplyDamage {
//             creature_id,
//             damage,
//         } => trigger_apply_damage_events(
//             game,
//             effects,
//             effect_data.source_creature_id,
//             *creature_id,
//             damage,
//         ),

//         Effect::HealDamage {
//             creature_id,
//             amount,
//         } => evaluate_rule_function(
//             game,
//             RuleScope::Creature(*creature_id),
//             effects,
//             |r, args| {
//                 r.on_healed(
//                     args.rc,
//                     args.effects,
//                     *amount,
//                     args.rc.game.creature(effect_data.source_creature_id)?,
//                 );
//                 Ok(())
//             },
//         ),

//         Effect::GainMana {
//             creature_id,
//             amount,
//         } => evaluate_rule_function(
//             game,
//             RuleScope::Creature(*creature_id),
//             effects,
//             |r, args| {
//                 r.on_mana_gained(
//                     args.rc,
//                     args.effects,
//                     *amount,
//                     args.rc.game.creature(effect_data.source_creature_id)?,
//                 );
//                 Ok(())
//             },
//         ),

//         Effect::LoseMana {
//             creature_id,
//             amount,
//         } => evaluate_rule_function(
//             game,
//             RuleScope::Creature(*creature_id),
//             effects,
//             |r, args| {
//                 r.on_mana_lost(
//                     args.rc,
//                     args.effects,
//                     *amount,
//                     args.rc.game.creature(effect_data.source_creature_id)?,
//                 );
//                 Ok(())
//             },
//         ),

//         Effect::SetModifier {
//             creature_id,
//             stat,
//             value,
//             operation,
//         } => Ok(()), // evaluate_rule_function(
//                      //     game,
//                      //     RuleScope::Creature(*creature_id),
//                      //     effects,
//                      //     |r, args| {
//                      //         r.on_stat_modifier_set(
//                      //             args.rc,
//                      //             args.effects,
//                      //             *stat,
//                      //             Modifier {
//                      //                 value: *value,
//                      //                 operation: *operation,
//                      //                 source: effect_data.rule_id,
//                      //             },
//                      //             args.rc.game.creature(effect_data.source_creature_id)?,
//                      //         );
//                      //         Ok(())
//                      //     },
//                      // ),
//     }
// }

// fn trigger_apply_damage_events(
//     game: &Game,
//     effects: &mut Effects,
//     attacker_id: CreatureId,
//     defender_id: CreatureId,
//     damage: &Damage,
// ) -> Result<()> {
//     evaluate_rule_function(
//         game,
//         RuleScope::Creature(attacker_id),
//         effects,
//         |r, args| {
//             r.on_applied_damage(
//                 args.rc,
//                 args.effects,
//                 damage,
//                 args.rc.game.creature(defender_id)?,
//             );
//             Ok(())
//         },
//     )?;

//     evaluate_rule_function(
//         game,
//         RuleScope::Creature(defender_id),
//         effects,
//         |r, args| {
//             r.on_took_damage(
//                 args.rc,
//                 args.effects,
//                 damage,
//                 args.rc.game.creature(attacker_id)?,
//             );
//             Ok(())
//         },
//     )?;

//     evaluate_rule_function(game, RuleScope::AllCreatures, effects, |r, args| {
//         r.on_any_creature_damaged(
//             args.rc,
//             args.effects,
//             damage,
//             args.rc.game.creature(attacker_id)?,
//             args.rc.game.creature(defender_id)?,
//         );
//         Ok(())
//     })?;
//     Ok(())
// }

// fn trigger_on_death_events(
//     game: &Game,
//     effects: &mut Effects,
//     attacker_id: CreatureId,
//     defender_id: CreatureId,
// ) -> Result<()> {
//     evaluate_rule_function(
//         game,
//         RuleScope::Creature(attacker_id),
//         effects,
//         |r, args| {
//             r.on_killed_enemy(args.rc, args.effects, args.rc.game.creature(defender_id)?);
//             Ok(())
//         },
//     )?;

//     evaluate_rule_function(
//         game,
//         RuleScope::CreatureIfDead(defender_id),
//         effects,
//         |r, args| {
//             r.on_death(args.rc, args.effects, args.rc.game.creature(attacker_id)?);
//             Ok(())
//         },
//     )?;

//     evaluate_rule_function(game, RuleScope::AllCreatures, effects, |r, args| {
//         r.on_any_creature_killed(
//             args.rc,
//             args.effects,
//             args.rc.game.creature(attacker_id)?,
//             args.rc.game.creature(defender_id)?,
//         );
//         Ok(())
//     })?;
//     Ok(())
// }
