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

use serde::{Deserialize, Serialize};

use crate::primitives::HealthValue;

#[derive(Serialize, Deserialize, Debug)]
pub enum Attack {
    BasicAttack(HealthValue),
}

#[derive(Serialize, Deserialize, Debug)]
pub struct Unit {
    current_health: HealthValue,
    maximum_health: HealthValue,
    attacks: Vec<Attack>,
}

impl Unit {
    pub fn new(health: i32, attack: i32) -> Unit {
        Unit {
            current_health: HealthValue::new(health),
            maximum_health: HealthValue::new(health),
            attacks: vec![Attack::BasicAttack(HealthValue::new(attack))],
        }
    }

    pub fn display_health(&self) -> String {
        format!(
            "{}%",
            100.0 * i32::from(self.current_health) as f64 / i32::from(self.maximum_health) as f64
        )
    }
}
