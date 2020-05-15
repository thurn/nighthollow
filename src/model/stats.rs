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
use serde::{Deserialize, Serialize};

use super::{
    primitives::PlayerName,
    types::{Creature, Game, HasCardData, Player},
};

#[derive(Serialize, Deserialize, Debug)]
pub struct Stat {
    value: StatValue,
}

#[derive(Serialize, Deserialize, Debug)]
enum StatValue {
    Unmodified(i32),
    Modified(i32, Box<StatModifiers>),
}

#[derive(Serialize, Deserialize, Debug)]
struct StatModifiers {
    set_base_modifiers: Vec<Modifier>,
    add_modifiers: Vec<Modifier>,
    increase_modifiers: Vec<Modifier>,
    multiply_modifiers: Vec<Modifier>,
}

impl StatModifiers {
    fn new() -> StatModifiers {
        StatModifiers {
            set_base_modifiers: vec![],
            add_modifiers: vec![],
            increase_modifiers: vec![],
            multiply_modifiers: vec![],
        }
    }

    fn add(&mut self, modifier: Modifier) {
        match modifier.effect.operation {
            Operation::SetBase => self.set_base_modifiers.push(modifier),
            Operation::Add => self.add_modifiers.push(modifier),
            Operation::IncreaseByPercent => self.increase_modifiers.push(modifier),
            Operation::MultiplyByPercent => self.multiply_modifiers.push(modifier),
        }
    }
}

impl Stat {
    pub fn new(value: i32) -> Stat {
        Stat {
            value: StatValue::Unmodified(value),
        }
    }

    pub fn add_modifier(&mut self, modifier: Modifier) {
        todo!("Implement this")
    }

    pub fn get(&self, owner: &Creature, game: &Game) -> Result<i32> {
        match &self.value {
            StatValue::Unmodified(v) => Ok(*v),
            StatValue::Modified(v, m) => todo!("Implement this"),
        }
    }
}

#[derive(Serialize, Deserialize, Debug)]
pub struct Modifier {
    // Applies a computation to the base stat value of this modifier
    effect: Effect,

    // Dicates how long this modifier should stay in effect for
    lifetime: Lifetime,

    // Optionally, the ID of the creature and player that added this modifier.
    // If "None", the modifier is assumed to have been added by the owning
    // creature and player.
    added_by_creature_id: Option<i32>,
    added_by_player: Option<PlayerName>,
}

impl Modifier {
    pub fn self_modifier(effect: Effect, lifetime: Lifetime) -> Modifier {
        Modifier {
            effect,
            lifetime,
            added_by_creature_id: None,
            added_by_player: None,
        }
    }

    pub fn external_modifier(effect: Effect, lifetime: Lifetime, added_by: &Creature) -> Modifier {
        Modifier {
            effect,
            lifetime,
            added_by_creature_id: Some(added_by.card_data().id),
            added_by_player: Some(added_by.card_data().owner),
        }
    }
}

#[derive(Serialize, Deserialize, Debug, Copy, Clone)]
pub enum Operation {
    SetBase,
    Add,
    IncreaseByPercent,
    MultiplyByPercent,
}

#[derive(Serialize, Deserialize, Debug)]
pub struct Effect {
    operation: Operation,
    operand: Operand,
}

#[derive(Serialize, Deserialize, Debug)]
pub enum Operand {
    Constant(i32),
}

pub struct EvaluationContext<'a> {
    game: &'a Game,
    owning_player: &'a Player,
    owning_creature: &'a Creature,
    added_by_player: &'a Player,
    added_by_creature: &'a Creature,
}

#[typetag::serde(tag = "type")]
pub trait CustomEffect: Debug {
    fn evaluate(&self, value: i32, context: &EvaluationContext) -> i32;
}

#[derive(Serialize, Deserialize, Debug)]
pub enum Lifetime {
    Permanent,
    EndOfTurn { turn_number: i32 },
    EndOfCombat { turn_number: i32 },
    EndOfRound { round_number: i32 },
}

#[typetag::serde(tag = "type")]
pub trait CustomLifetime: Debug {
    fn is_alive(&self, context: &EvaluationContext) -> bool;
}
