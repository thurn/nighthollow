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

use crate::model::{
    primitives::{
        ActionNumber, CardId, CreatureId, HealthValue, ManaValue, RoundNumber, RuleId, TurnNumber,
    },
    stats::{Modifier, Operation, StatName},
    types::{Creature, Damage, Game},
};
use serde::{Deserialize, Serialize};

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
    /// combat. Only called if the creature is alive.
    fn on_action_start(&self, context: &RuleContext, effects: &mut Effects) {}

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
        stat: StatName,
        modifier: Modifier,
        source: &Creature,
    ) {
    }
}

#[derive(Serialize, Deserialize, Debug)]
struct TestRule {
    state: i32,
}

#[typetag::serde]
impl Rule for TestRule {
    fn on_action_start(&self, context: &RuleContext, effects: &mut Effects) {
        effects.push_effect(
            context,
            Effect::ApplyDamage {
                creature_id: 23,
                damage: Damage { values: vec![] },
            },
        )
    }
}

pub enum Effect {
    ApplyDamage {
        creature_id: CreatureId,
        damage: Damage,
    },
    HealDamage {
        creature_id: CreatureId,
        amount: HealthValue,
    },
    GainMana {
        creature_id: CreatureId,
        amount: ManaValue,
    },
    LoseMana {
        creature_id: CreatureId,
        amount: ManaValue,
    },
    SetModifier {
        creature_id: CreatureId,
        stat: StatName,
        value: u32,
        operation: Operation,
    },
}

pub struct EffectData {
    pub effect: Effect,
    pub rule_id: RuleId,
    pub source_creature_id: CreatureId,
}

pub struct Effects {
    effects: Vec<EffectData>,
}

impl Effects {
    pub fn new() -> Effects {
        Effects { effects: vec![] }
    }

    pub fn push_effect(&mut self, context: &RuleContext, effect: Effect) {
        self.effects.push(EffectData {
            effect,
            rule_id: context.rule_id,
            source_creature_id: context.owner.dead_or_alive().creature_id(),
        });
    }

    pub fn len(&self) -> usize {
        self.effects.len()
    }

    pub fn iter(&self) -> impl Iterator<Item = &EffectData> {
        self.effects.iter()
    }
}

pub enum CreatureState<'a> {
    Living(&'a Creature),
    Dead(&'a Creature),
}

impl<'a> CreatureState<'a> {
    pub fn new(creature: &'a Creature) -> CreatureState<'a> {
        if creature.is_alive() {
            CreatureState::Living(creature)
        } else {
            CreatureState::Dead(creature)
        }
    }

    /// I'm a cowboy,
    /// on a steel horse I ride.
    /// And I'm wanted,
    /// dead or alive.
    pub fn dead_or_alive(&self) -> &Creature {
        match self {
            CreatureState::Living(c) => c,
            CreatureState::Dead(c) => c,
        }
    }
}

pub struct RuleContext<'a> {
    pub rule_id: RuleId,
    pub owner: CreatureState<'a>,
    pub game: &'a Game,
}
