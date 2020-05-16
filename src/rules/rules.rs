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

use crate::{
    api::CreatureId,
    model::{
        primitives::{ActionNumber, HealthValue, ManaValue, RoundNumber, RuleId, TurnNumber},
        stats::{Modifier, StatName},
        types::{Creature, Damage, Game},
    },
};
use serde::{Deserialize, Serialize};

#[typetag::serde(tag = "type")]
pub trait Rule: Debug + Send {
    /// Called a the start of a new game turn
    fn on_turn_start(&self, c: &RuleContext) {}

    /// Called at end of turn
    fn on_turn_end(&self, c: &RuleContext) {}

    /// Called when a new combat phase begins
    fn on_combat_start(&self, c: &RuleContext) {}

    /// Called when a combat phase ends
    fn on_combat_end(&self, c: &RuleContext) {}

    /// Called at the start of a combat round
    fn on_round_start(&self, c: &RuleContext, round: RoundNumber) {}

    /// Called at the end of a combat round
    fn on_round_end(&self, c: &RuleContext, round: RoundNumber) {}

    /// Called when it is time for this creature to take an action during
    /// combat. Only called if the creature is alive.
    fn on_action_start(&self, c: &RuleContext) {}

    /// Called when this creature applies damage to an opposing creature with
    /// the final damage value
    fn on_applied_damage(&self, c: &RuleContext, damage: &Damage, creature: &Creature) {}

    /// Called when this creature is damaged by an opposing creature with the
    /// final damage value
    fn on_took_damage(&self, c: &RuleContext, damage: &Damage, creature: &Creature) {}

    /// Called whenever any creature is damaged
    fn on_any_creature_damaged(
        &self,
        c: &RuleContext,
        attacker: &Creature,
        defender: &Creature,
        damage: &Damage,
    ) {
    }

    /// Called when this creature kills an enemy creature
    fn on_killed_enemy(&self, c: &RuleContext, enemy: &Creature) {}

    /// Called when this creature dies (its damage total exceeds its health value)
    fn on_death(&self, c: &RuleContext, killed_by: &Creature) {}

    /// Called whenever any creature on either side of combat dies
    fn on_any_creature_killed(&self, c: &RuleContext, attacker: &Creature, defender: &Creature) {}
}

pub enum Effect {
    ApplyDamage(CreatureId, Damage),
    HealDamage(CreatureId, HealthValue),
    GainMana(CreatureId, ManaValue),
    SpendMana(CreatureId, ManaValue),
    SetModifier(CreatureId, StatName, Modifier),
}

pub struct Effects {
    effects: Vec<Effect>,
}

impl Effects {
    pub fn new() -> Effects {
        Effects { effects: vec![] }
    }

    pub fn push_effect(&mut self, effect: Effect) {
        self.effects.push(effect);
    }
}

pub struct RuleContext<'a> {
    pub rule_id: RuleId,
    pub output: &'a mut Effects,
    pub owner: &'a Creature,
    pub game: &'a Game,
}
