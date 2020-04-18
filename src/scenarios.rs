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

use crate::{
    card::{Card, Cost},
    primitives::{GamePhase, InterfaceError, ManaValue, Result, School},
    state::{InterfaceOptions, InterfaceState, PlayerState},
};

pub fn load_scenario(state: &mut InterfaceState, name: String) -> Result<()> {
    state.reset();
    match name.as_str() {
        "empty" => Ok(()),
        "opening" => Ok(opening_hands(state)),
        _ => InterfaceError::result(format!("Unknown scenario {}", name)),
    }
}

fn opening_hands(state: &mut InterfaceState) {
    state.update(InterfaceState {
        options: InterfaceOptions { auto_advance: true },
        phase: GamePhase::Main,
        player: PlayerState {
            mana: 0,
            hand: vec![
                Card::new_unit(
                    "Demon Wolf",
                    Cost::mana_cost(School::Flame, ManaValue::new(2), 1),
                    100,
                    10,
                ),
                Card::new_unit(
                    "Cyclops",
                    Cost::mana_cost(School::Flame, ManaValue::new(4), 2),
                    200,
                    10,
                ),
                Card::new_unit(
                    "Metalon",
                    Cost::mana_cost(School::Flame, ManaValue::new(3), 1),
                    250,
                    10,
                ),
            ],
            reserve: vec![],
            defenders: vec![],
            attackers: vec![],
        },
        enemy: PlayerState {
            mana: 0,
            hand: vec![
                Card::new_unit(
                    "Demon Wolf",
                    Cost::mana_cost(School::Flame, ManaValue::new(1), 1),
                    100,
                    10,
                ),
                Card::new_unit(
                    "Cyclops",
                    Cost::mana_cost(School::Flame, ManaValue::new(3), 1),
                    200,
                    10,
                ),
                Card::new_unit(
                    "Metalon",
                    Cost::mana_cost(School::Flame, ManaValue::new(2), 1),
                    250,
                    10,
                ),
            ],
            reserve: vec![],
            defenders: vec![],
            attackers: vec![],
        },
    });
}
