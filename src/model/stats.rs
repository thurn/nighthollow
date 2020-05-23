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

use serde::{Deserialize, Serialize};

use super::primitives::{PlayerName, RuleId};

#[derive(Serialize, Deserialize, Debug, Copy, Clone)]
pub enum StatName {}

#[derive(Serialize, Deserialize, Debug)]
pub struct Stat {
    value: u32,
    set_base_modifiers: Vec<Modifier>,
    add_modifiers: Vec<Modifier>,
    increase_modifiers: Vec<Modifier>,
    multiply_modifiers: Vec<Modifier>,
}

impl Stat {
    pub fn new(value: u32) -> Stat {
        Stat {
            value,
            set_base_modifiers: vec![],
            add_modifiers: vec![],
            increase_modifiers: vec![],
            multiply_modifiers: vec![],
        }
    }

    pub fn value(&self) -> u32 {
        self.value
    }

    pub fn set_modifier(&mut self, modifier: Modifier) {
        todo!()
    }
}

#[derive(Serialize, Deserialize, Debug)]
pub struct Modifier {
    pub value: u32,
    pub operation: Operation,
    pub source: RuleId,
}

#[derive(Serialize, Deserialize, Debug, Copy, Clone)]
pub enum Operation {
    SetBase,
    Add,
    IncreaseByPercent,
    MultiplyByPercent,
}

#[derive(Serialize, Deserialize, Debug)]
pub enum TagName {}

#[derive(Serialize, Deserialize, Debug)]
pub struct Tag {
    value: bool,
    modifiers: Vec<TagModifier>,
}

#[derive(Serialize, Deserialize, Debug)]
pub struct TagModifier {
    value: bool,
    source: RuleId,
}
