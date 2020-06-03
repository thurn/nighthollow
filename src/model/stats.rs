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

use std::{collections::BTreeMap, fmt::Debug};

use crate::prelude::*;

use crate::rules::engine::RuleIdentifier;

#[derive(Serialize, Deserialize, Debug, Copy, Clone)]
pub enum StatName {}

#[derive(Serialize, Deserialize, Debug, Clone)]
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

#[derive(Serialize, Deserialize, Debug, Copy, Clone)]
pub struct Modifier {
    pub value: u32,
    pub operation: Operation,
    pub source: RuleIdentifier,
}

#[derive(Serialize, Deserialize, Debug, Copy, Clone)]
pub enum Operation {
    Set,
    Add,
    IncreaseByPercent,
    MultiplyByPercent,
}

#[derive(Serialize, Deserialize, Debug, Clone)]
pub enum TagName {}

#[derive(Serialize, Deserialize, Debug, Clone)]
pub struct Tag {
    value: bool,
    modifiers: BTreeMap<RuleIdentifier, bool>,
}

impl Default for Tag {
    fn default() -> Self {
        Self {
            value: false,
            modifiers: BTreeMap::new(),
        }
    }
}

impl Tag {
    pub fn value(&self) -> bool {
        self.value
    }

    pub fn set_modifier(&mut self, modifier: TagModifier) {
        self.modifiers.insert(modifier.source, modifier.value);
        self.update();
    }

    fn update(&mut self) {
        self.value = if self.modifiers.is_empty() {
            false
        } else {
            self.modifiers.values().all(|v| *v)
        }
    }
}

#[derive(Serialize, Deserialize, Debug, Copy, Clone)]
pub struct TagModifier {
    pub value: bool,
    pub source: RuleIdentifier,
}
