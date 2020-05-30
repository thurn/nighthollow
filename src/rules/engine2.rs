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

use dyn_clone::DynClone;
use eyre::Result;

use super::{
    command_generation,
    effects::{self, Effect, EffectSource, Effects},
    events::{self, Event, Events},
    scopes::RuleScope,
};
use crate::{
    api,
    model::{
        cards::{Card, Scroll, Spell},
        creatures::{Creature, Damage},
        games::{Game, Player},
        primitives::{
            CreatureId, HealthValue, Influence, LifeValue, ManaValue, PlayerName, PowerValue,
            RoundNumber,
        },
    },
};

#[derive(Debug, Clone)]
pub struct RuleIdentifier {
    pub index: usize,
    pub source: EffectSource,
}

pub struct RuleContext<'a, T> {
    pub identifier: RuleIdentifier,
    pub data: &'a T,
    pub game: &'a Game,
    pub effects: &'a mut Effects,
}

impl<'a, T> RuleContext<'a, T> {
    pub fn identifier(&self) -> RuleIdentifier {
        self.identifier.clone()
    }

    pub fn push_effect(&mut self, effect: Effect) {
        // self.effects.push_effect(self.identifier(), effect);
    }
}

pub trait Rule<T> {
    /// Called at the start of a new game
    fn on_game_start(&self, context: RuleContext<T>) {}

    /// Called at the end of the game
    fn on_game_end(&self, context: RuleContext<T>) {}

    /// Called at the start of a new game turn
    fn on_turn_start(&self, context: RuleContext<T>) {}

    /// Called at end of turn
    fn on_turn_end(&self, context: RuleContext<T>) {}

    /// Called when a new combat phase begins
    fn on_combat_start(&self, context: RuleContext<T>) {}

    /// Called when a combat phase ends
    fn on_combat_end(&self, context: RuleContext<T>) {}

    /// Called at the start of a combat round
    fn on_round_start(&self, context: RuleContext<T>, round: RoundNumber) {}

    /// Called at the end of a combat round
    fn on_round_end(&self, context: &RuleContext<T>, round: RoundNumber) {}

    /// Called whenever any creature is damaged
    fn on_any_creature_damaged(
        &self,
        context: RuleContext<T>,
        damage: &Damage,
        attacker: &Creature,
        defender: &Creature,
    ) {
    }

    /// Called whenever any creature on either side of combat dies
    fn on_any_creature_killed(
        &self,
        context: RuleContext<Creature>,
        attacker: &Creature,
        defender: &Creature,
    ) {
    }

    /// Called whenever any creature causes damage to be healed
    fn on_any_creature_healed(
        &self,
        context: RuleContext<T>,
        amount: HealthValue,
        source: &Creature,
        target: &Creature,
    ) {
    }
}

#[typetag::serde(tag = "type")]
pub trait PlayerRule: Debug + Send + DynClone + Rule<Player> {
    fn on_card_drawn(&self, context: RuleContext<Player>, card: &Card) {}

    fn on_opponent_card_drawn(&self, context: RuleContext<Player>, card: &Card) {}

    fn on_creature_played(&self, context: RuleContext<Player>, creature: &Creature) {}

    fn on_opponent_creature_played(&self, context: RuleContext<Player>, creature: &Creature) {}

    fn on_scroll_played(&self, context: RuleContext<Player>, creature: &Scroll) {}

    fn on_opponent_scroll_played(&self, context: RuleContext<Player>, creature: &Scroll) {}

    fn on_spell_played(&self, context: RuleContext<Player>, creature: &Spell) {}

    fn on_opponent_spell_played(&self, context: RuleContext<Player>, creature: &Spell) {}
}

dyn_clone::clone_trait_object!(PlayerRule);

#[typetag::serde(tag = "type")]
pub trait CreatureRule: Debug + Send + DynClone + Rule<Creature> {
    /// Called when it is time for this creature to take an action during
    /// combat.
    fn on_action_start(&self, context: RuleContext<Creature>) {}

    /// Called at the end of this creature's combat action
    fn on_action_end(&self, context: RuleContext<Creature>) {}

    /// Called during a creature's action when this rule returns the highest
    /// priority value from on_calculate_skill_priority, as discussed above
    fn on_invoke_skill(&self, context: RuleContext<Creature>) {}

    /// Called when this creature applies damage to an opposing creature with
    /// the final damage value
    fn on_applied_damage(
        &self,
        context: RuleContext<Creature>,
        damage: &Damage,
        to_target: &Creature,
    ) {
    }

    /// Called when this creature is damaged by an opposing creature with the
    /// final damage value
    fn on_took_damage(
        &self,
        context: RuleContext<Creature>,
        damage: &Damage,
        from_source: &Creature,
    ) {
    }

    /// Called when this creature kills an enemy creature
    fn on_killed_enemy(&self, context: RuleContext<Creature>, enemy: &Creature) {}

    /// Called when this creature dies (its damage total exceeds its health value)
    fn on_death(&self, context: RuleContext<Creature>, killed_by: &Creature) {}

    /// Called when this creature heals damage
    fn on_healed(&self, context: RuleContext<Creature>, amount: HealthValue, healed_by: &Creature) {
    }

    /// Called when this creature applies a heal to another creature
    fn on_applied_heal(
        &self,
        context: RuleContext<Creature>,
        amount: HealthValue,
        target: &Creature,
    ) {
    }

    /// Called when this creature's mana is increased
    fn on_mana_gained(&self, context: RuleContext<Creature>, amount: ManaValue, source: &Creature) {
    }

    /// Called when this creature's mana is decreased
    fn on_mana_lost(&self, context: RuleContext<Creature>, amount: ManaValue, source: &Creature) {}

    // Called when a stat modifier is set on this creature
    // fn on_stat_modifier_set(
    //     &self,
    //     context: RuleContext<Creature>,
    //     modifier: &SetModifier,
    //     source: &Creature,
    // ) {
    // }
}

dyn_clone::clone_trait_object!(CreatureRule);

pub fn execute_rule<R, D>(
    game: &mut Game,
    commands: &mut Vec<api::Command>,
    scope: impl RuleScope<R, D>,
    function: impl Fn(&R, RuleContext<D>) -> Result<()>,
) -> Result<()> {
    let mut effects = Effects::new();
    populate_rule_effects(game, scope, &mut effects, function)?;

    let mut event_index = 0;
    let mut events = Events::new();

    while effects.len() > 0 {
        for effect in effects.iter() {
            effects::apply_effect(game, &mut events, effect)?;
        }

        effects = Effects::new();

        for event in &events.data[event_index..] {
            // events::execute_event_rules(game, &mut effects, event)?;
        }
        event_index = events.data.len();
    }

    command_generation::generate(game, events, commands)?;

    Ok(())
}

pub fn populate_rule_effects<R, D>(
    game: &Game,
    scope: impl RuleScope<R, D>,
    effects: &mut Effects,
    function: impl Fn(&R, RuleContext<D>) -> Result<()>,
) -> Result<()> {
    for rule_data in scope.rules(game).iter() {
        function(
            rule_data.rule,
            RuleContext {
                identifier: RuleIdentifier {
                    index: rule_data.index,
                    source: rule_data.source.clone(),
                },
                data: rule_data.data,
                game,
                effects,
            },
        )?;
    }
    Ok(())
}
