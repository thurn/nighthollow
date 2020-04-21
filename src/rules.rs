// Copyright The Magewatch Project

// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at

//    https://www.apache.org/licenses/LICENSE-2.0

// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.

use std::collections::BTreeMap;

use serde::{Deserialize, Serialize};

use crate::{model::Card, unit::Unit};

#[derive(Serialize, Deserialize, Debug, PartialEq, Eq, Copy, Clone, PartialOrd, Ord)]
pub enum Trigger {
    CardIsDrawn,
    CardIsPayed,
    UnitIsPlaced,
    UnitIsKilled,
}

#[derive(Serialize, Deserialize, Debug)]
pub struct RulesEngine {
    pub rules: BTreeMap<Trigger, Vec<Rule>>,
}

#[derive(Serialize, Deserialize, Debug)]
pub struct Rule {
    pub name: String,
    pub trigger: Trigger,
    pub conditions: Vec<Condition>,
    pub effects: Vec<Effect>,
}

#[derive(Serialize, Deserialize, Debug)]
pub struct Condition {
    query: Query,
}

#[derive(Serialize, Deserialize, Debug)]
pub struct Effect {
    query: Query,
}

#[derive(Serialize, Deserialize, Debug)]
pub enum EffectType {
    DrawCard { card: Card },
    ApplyAttacks { attacker: Unit, defender: Unit },
}

pub fn excute_effect(effect: EffectType) {
    match effect {
        EffectType::DrawCard { card } => println!("Got card: {}", card.name),
        EffectType::ApplyAttacks { attacker, defender } => {
            println!("Attack from {:?} on {:?}", attacker, defender)
        }
    }
}

#[derive(Serialize, Deserialize, Debug)]
pub struct Query {}

pub fn make_rule() -> Rule {
    Rule {
        name: String::from("Derek"),
        trigger: Trigger::CardIsDrawn,
        conditions: vec![],
        effects: vec![],
    }
}
