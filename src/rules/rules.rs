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

use std::{fmt::Debug, iter};

use crate::prelude::*;

use dyn_clone::DynClone;

use super::effects2::{self, CreatureMutationEvent, Effects, SetModifier};
use crate::{
    api, commands,
    model::{
        cards::{Card, CardWithTarget, HasCardData, Scroll, Spell},
        creatures::{Creature, Damage, DamageResult, HasCreatureData},
        games::{Game, Player},
        primitives::{
            ActionNumber, CardId, CreatureId, HealthValue, Influence, LifeValue, ManaValue,
            PlayerName, PowerValue, RoundNumber, RuleId, TurnNumber,
        },
        stats::{Modifier, Operation, StatName},
    },
};
use effects2::{CreatureMutationEventType, EffectSource, MutationEvent};

pub trait BaseRule<T> {
    /// Called a the start of a new game turn
    fn on_turn_start(&self, context: &RuleContext<T>, effects: &mut Effects) {}

    /// Called at end of turn
    fn on_turn_end(&self, context: &RuleContext<T>, effects: &mut Effects) {}

    /// Called when a new combat phase begins
    fn on_combat_start(&self, context: &RuleContext<T>, effects: &mut Effects) {}

    /// Called when a combat phase ends
    fn on_combat_end(&self, context: &RuleContext<T>, effects: &mut Effects) {}

    fn on_owner_life_changed(
        &self,
        context: &RuleContext<T>,
        effects: &mut Effects,
        amount: LifeValue,
    ) {
    }

    fn on_owner_power_changed(
        &self,
        context: &RuleContext<T>,
        effects: &mut Effects,
        amount: PowerValue,
    ) {
    }

    fn on_owner_influence_changed(
        &self,
        context: &RuleContext<T>,
        effects: &mut Effects,
        influence: &Influence,
    ) {
    }

    fn on_any_card_played(&self, context: &RuleContext<T>, effects: &mut Effects, card: &Card) {}

    fn on_any_creature_played(
        &self,
        context: &RuleContext<T>,
        effects: &mut Effects,
        creature: &Creature,
    ) {
    }

    fn on_any_scroll_played(
        &self,
        context: &RuleContext<T>,
        effects: &mut Effects,
        creature: &Scroll,
    ) {
    }

    fn on_any_spell_played(
        &self,
        context: &RuleContext<T>,
        effects: &mut Effects,
        creature: &Spell,
    ) {
    }
}

#[typetag::serde(tag = "type")]
pub trait CreatureRule: Debug + Send + DynClone + BaseRule<Creature> {
    /// Called at the start of a combat round
    fn on_round_start(
        &self,
        context: &RuleContext<Creature>,
        effects: &mut Effects,
        _round: RoundNumber,
    ) {
    }

    /// Called at the end of a combat round
    fn on_round_end(
        &self,
        context: &RuleContext<Creature>,
        effects: &mut Effects,
        _round: RoundNumber,
    ) {
    }

    /// Called when it is time for this creature to take an action during
    /// combat.
    fn on_action_start(&self, context: &RuleContext<Creature>, effects: &mut Effects) {}

    /// Called at the end of this creature's combat action
    fn on_action_end(&self, context: &RuleContext<Creature>, effects: &mut Effects) {}

    /// Called to calculate the priority of the main combat skill for this
    /// creature. If this rule contributes a skill which should be considered
    /// as this creature's action, return a priority number. The rule for this
    /// creature which returns the highest skill priority will have its
    /// on_invoke_skill() callback invoked. In the case of a tie, a random
    /// skill from among the tied skills is selected.
    fn on_calculate_skill_priority(&self, context: &RuleContext<Creature>) -> Option<u32> {
        None
    }

    /// Called during a creature's action when this rule returns the highest
    /// priority value from on_calculate_skill_priority, as discussed above
    fn on_invoke_skill(&self, context: &RuleContext<Creature>, effects: &mut Effects) {}

    /// Called when this creature applies damage to an opposing creature with
    /// the final damage value
    fn on_applied_damage(
        &self,
        context: &RuleContext<Creature>,
        effects: &mut Effects,
        damage: &Damage,
        to_target: &Creature,
    ) {
    }

    /// Called when this creature is damaged by an opposing creature with the
    /// final damage value
    fn on_took_damage(
        &self,
        context: &RuleContext<Creature>,
        effects: &mut Effects,
        damage: &Damage,
        from_source: &Creature,
    ) {
    }

    /// Called whenever any creature is damaged
    fn on_any_creature_damaged(
        &self,
        context: &RuleContext<Creature>,
        effects: &mut Effects,
        damage: &Damage,
        attacker: &Creature,
        defender: &Creature,
    ) {
    }

    /// Called when this creature kills an enemy creature
    fn on_killed_enemy(
        &self,
        context: &RuleContext<Creature>,
        effects: &mut Effects,
        enemy: &Creature,
    ) {
    }

    /// Called when this creature dies (its damage total exceeds its health value)
    fn on_death(
        &self,
        context: &RuleContext<Creature>,
        effects: &mut Effects,
        killed_by: &Creature,
    ) {
    }

    /// Called whenever any creature on either side of combat dies
    fn on_any_creature_killed(
        &self,
        context: &RuleContext<Creature>,
        effects: &mut Effects,
        attacker: &Creature,
        defender: &Creature,
    ) {
    }

    /// Called when this creature heals damage
    fn on_healed(
        &self,
        context: &RuleContext<Creature>,
        effects: &mut Effects,
        amount: HealthValue,
        healed_by: &Creature,
    ) {
    }

    /// Called when this creature applies a heal to another creature
    fn on_applied_heal(
        &self,
        context: &RuleContext<Creature>,
        effects: &mut Effects,
        amount: HealthValue,
        target: &Creature,
    ) {
    }

    /// Called whenever any creature causes damage to be healed
    fn on_any_creature_healed(
        &self,
        context: &RuleContext<Creature>,
        effects: &mut Effects,
        amount: HealthValue,
        source: &Creature,
        target: &Creature,
    ) {
    }

    /// Called when this creature's mana is increased
    fn on_mana_gained(
        &self,
        context: &RuleContext<Creature>,
        effects: &mut Effects,
        amount: ManaValue,
        source: &Creature,
    ) {
    }

    /// Called when this creature's mana is decreased
    fn on_mana_lost(
        &self,
        context: &RuleContext<Creature>,
        effects: &mut Effects,
        amount: ManaValue,
        source: &Creature,
    ) {
    }

    /// Called when a stat modifier is set on this creature
    fn on_stat_modifier_set(
        &self,
        context: &RuleContext<Creature>,
        effects: &mut Effects,
        modifier: &SetModifier,
        source: &Creature,
    ) {
    }
}

dyn_clone::clone_trait_object!(CreatureRule);

#[typetag::serde(tag = "type")]
pub trait GameRule: Debug + Send + DynClone + BaseRule<Player> {
    fn on_start_game_request(&self, context: &RuleContext<Player>, effects: &mut Effects) {}

    fn on_start_derek_request(&self, context: RuleContext<&Player>, effects: &mut Effects) {}

    fn on_play_card_request(
        &self,
        context: &RuleContext<Player>,
        effects: &mut Effects,
        card: &CardWithTarget,
    ) {
    }

    fn on_main_button_click_request(&self, context: &RuleContext<Player>, effects: &mut Effects) {}
}

dyn_clone::clone_trait_object!(GameRule);

pub struct RuleContext<'a, T> {
    pub rule_id: RuleId,
    pub creature: &'a T,
    pub game: &'a Game,
    pub source: EffectSource,
}

impl<'a> RuleContext<'a, Creature> {
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
            RuleScope::CreatureIfDead(_) => !creature.is_alive(),
            _ => creature.is_alive(),
        }
    }

    fn should_process_rule(&self, index: usize) -> bool {
        match self {
            RuleScope::SpecificRule(_, position) => index == *position,
            _ => true,
        }
    }
}

pub struct RuleArgs<'a, T> {
    pub rc: &'a RuleContext<'a, T>,
    pub effects: &'a mut Effects,
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
    function: impl Fn(&Box<dyn CreatureRule>, RuleArgs<Creature>) -> Result<()>,
) -> Result<()> {
    let mut effects = Effects::new();

    evaluate_rule_function(game, scope, &mut effects, function)?;
    if effects.len() == 0 {
        return Ok(());
    }

    loop {
        let mut events: Vec<MutationEvent> = vec![];
        for effect in effects.iter() {
            effects2::apply_effect(game, effect, commands, &mut events)?;
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
    function: impl Fn(&Box<dyn CreatureRule>, RuleArgs<Creature>) -> Result<()>,
) -> Result<()> {
    let mut commands: Vec<api::Command> = vec![];
    run_rule(game, &mut commands, scope, function)?;
    if commands.len() > 0 {
        groups.push(api::CommandGroup { commands });
    }
    Ok(())
}

pub fn run_game_rule(
    game: &mut Game,
    player_name: PlayerName,
    function: impl Fn(&Box<dyn GameRule>, &RuleContext<Player>, &mut Effects) -> Result<()>,
) -> Result<Vec<api::CommandGroup>> {
    let mut effects = Effects::new();
    function(
        &game.rule,
        &RuleContext {
            rule_id: RuleId {
                creature_id: 0,
                index: 0,
            },
            creature: game.player(player_name),
            game,
            source: EffectSource::Game,
        },
        &mut effects,
    )?;

    if effects.len() == 0 {
        return Ok(vec![commands::empty_group()]);
    }

    let mut commands = vec![];

    loop {
        let mut events: Vec<MutationEvent> = vec![];
        for effect in effects.iter() {
            effects2::apply_effect(game, effect, &mut commands, &mut events)?;
        }

        effects = Effects::new();
        for event in events.iter() {
            trigger_mutation_events(game, &mut effects, event)?;
        }

        if effects.len() == 0 {
            break;
        }
    }

    Ok(vec![commands::group(commands)])
}

pub struct RuleData<'a, RuleType, DataType> {
    rule: &'a RuleType,
    data: &'a DataType,
}

pub trait RuleProvider<RuleType, DataType> {
    fn rules<'a>(&self, game: &'a Game) -> Vec<RuleData<'a, RuleType, DataType>>;
    fn rule_id(&self, data: &DataType, index: usize) -> RuleId;
    fn source(&self, data: &DataType) -> EffectSource;
}

struct GameRuleProvider {
    player_name: PlayerName,
}

impl RuleProvider<Box<dyn GameRule>, Player> for GameRuleProvider {
    fn rules<'a>(&self, game: &'a Game) -> Vec<RuleData<'a, Box<dyn GameRule>, Player>> {
        vec![RuleData {
            rule: &game.rule,
            data: game.player(self.player_name),
        }]
    }

    fn rule_id(&self, data: &Player, index: usize) -> RuleId {
        RuleId {
            index,
            creature_id: 17,
        }
    }

    fn source(&self, data: &Player) -> EffectSource {
        EffectSource::Game
    }
}

fn test_my_rule(game: &mut Game, commands: &mut Vec<api::Command>) -> Result<()> {
    execute_rule(
        game,
        commands,
        GameRuleProvider {
            player_name: PlayerName::User,
        },
        |r, rc, e| {
            r.on_start_game_request(rc, e);
            Ok(())
        },
    )
}

pub fn execute_rule<R, D>(
    game: &mut Game,
    commands: &mut Vec<api::Command>,
    provider: impl RuleProvider<R, D>,
    function: impl Fn(&R, &RuleContext<D>, &mut Effects) -> Result<()>,
) -> Result<()> {
    let mut effects = Effects::new();

    for rule_data in provider.rules(game) {
        function(
            rule_data.rule,
            &RuleContext {
                rule_id: provider.rule_id(rule_data.data, 0),
                creature: rule_data.data,
                game,
                source: provider.source(rule_data.data),
            },
            &mut effects,
        )?;
    }

    Ok(())
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
    function: impl Fn(&Box<dyn CreatureRule>, RuleArgs<Creature>) -> Result<()>,
) -> Result<()> {
    for creature_id in scope.ids(game) {
        let creature = game.creature(creature_id)?;
        if !scope.should_process_creature(creature) {
            continue;
        }

        for (index, rule) in creature.data.rules.iter().enumerate() {
            if scope.should_process_rule(index) {
                let rule_context = RuleContext {
                    rule_id: RuleId { creature_id, index },
                    creature,
                    game,
                    source: EffectSource::Creature(creature.creature_id()),
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

struct CreatureRuleEvaluator<'a> {
    game: &'a Game,
    effects: &'a mut Effects,
    event: &'a effects2::CreatureMutationEvent,
    source_creature: CreatureId,
}

impl<'a> CreatureRuleEvaluator<'a> {
    fn on_applied(
        &mut self,
        function: impl Fn(&Box<dyn CreatureRule>, &RuleContext<Creature>, &mut Effects, &Creature),
    ) -> Result<()> {
        let target = self.game.creature(self.event.target_creature)?;
        evaluate_rule_function(
            self.game,
            RuleScope::Creature(self.source_creature),
            self.effects,
            |r, args| {
                function(r, args.rc, args.effects, target);
                Ok(())
            },
        )
    }

    fn on_received(
        &mut self,
        function: impl Fn(&Box<dyn CreatureRule>, &RuleContext<Creature>, &mut Effects, &Creature),
    ) -> Result<()> {
        let source = self.game.creature(self.source_creature)?;
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
        function: impl Fn(
            &Box<dyn CreatureRule>,
            &RuleContext<Creature>,
            &mut Effects,
            &Creature,
            &Creature,
        ),
    ) -> Result<()> {
        let source = self.game.creature(self.source_creature)?;
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
    if let EffectSource::Creature(creature_id) = event.source {
        match &event.mutation {
            effects2::Mutation::CreatureMutation(creature_mutation) => {
                trigger_creature_mutation_events(game, effects, creature_id, &creature_mutation)?;
            }
        }
    }
    Ok(())
}

fn trigger_creature_mutation_events(
    game: &Game,
    effects: &mut Effects,
    source_creature: CreatureId,
    event: &CreatureMutationEvent,
) -> Result<()> {
    let mut evaluator = CreatureRuleEvaluator {
        game,
        effects,
        event,
        source_creature,
    };

    match &event.event_type {
        CreatureMutationEventType::AppliedDamage(damage) => {
            evaluator.on_applied(|r, rc, e, to| r.on_applied_damage(rc, e, damage, to))?;
            evaluator.on_received(|r, rc, e, from| r.on_took_damage(rc, e, damage, from))?;
            evaluator.on_any(|r, rc, e, attacker, defender| {
                r.on_any_creature_damaged(rc, e, damage, attacker, defender)
            })?;
        }
        CreatureMutationEventType::Killed(_) => {
            evaluator.on_applied(|r, rc, e, to| r.on_killed_enemy(rc, e, to))?;
            evaluator.on_received(|r, rc, e, from| r.on_death(rc, e, from))?;
            evaluator.on_any(|r, rc, e, attacker, defender| {
                r.on_any_creature_killed(rc, e, attacker, defender)
            })?;
        }
        CreatureMutationEventType::Healed(amount) => {
            evaluator.on_applied(|r, rc, e, to| r.on_applied_heal(rc, e, *amount, to))?;
            evaluator.on_received(|r, rc, e, from| r.on_healed(rc, e, *amount, from))?;
            evaluator.on_any(|r, rc, e, attacker, defender| {
                r.on_any_creature_healed(rc, e, *amount, attacker, defender)
            })?;
        }
        CreatureMutationEventType::GainedMana(amount) => {
            evaluator.on_received(|r, rc, e, from| r.on_mana_gained(rc, e, *amount, from))?;
        }
        CreatureMutationEventType::LostMana(amount) => {
            evaluator.on_received(|r, rc, e, from| r.on_mana_lost(rc, e, *amount, from))?;
        }
        CreatureMutationEventType::SetModifier(modifier) => {
            evaluator
                .on_received(|r, rc, e, from| r.on_stat_modifier_set(rc, e, modifier, from))?;
        }
    }
    Ok(())
}
