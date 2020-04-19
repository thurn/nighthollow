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
    primitives::{CombatPosition, GamePhase, InterfaceError, ManaValue, Result, School},
    state::{Game, GameState, PlayerState},
};

pub fn load_scenario(state: &mut Game, name: String) -> Result<()> {
    match name.as_str() {
        "empty" => Ok(()),
        "opening" => Ok(opening_hands(state)),
        "combat" => Ok(combat(state)),
        _ => InterfaceError::result(format!("Unknown scenario {}", name)),
    }
}

fn demon_wolf(position: Option<CombatPosition>) -> Card {
    Card::new_unit(
        "Demon Wolf",
        Cost::mana_cost(School::Flame, ManaValue::new(2), 1),
        100,
        10,
        position,
    )
}

fn cyclops(position: Option<CombatPosition>) -> Card {
    Card::new_unit(
        "Cyclops",
        Cost::mana_cost(School::Flame, ManaValue::new(4), 2),
        200,
        10,
        position,
    )
}

fn metalon(position: Option<CombatPosition>) -> Card {
    Card::new_unit(
        "Metalon",
        Cost::mana_cost(School::Flame, ManaValue::new(3), 1),
        250,
        10,
        position,
    )
}

fn treant(position: Option<CombatPosition>) -> Card {
    Card::new_unit(
        "Treant",
        Cost::mana_cost(School::Flame, ManaValue::new(1), 1),
        60,
        10,
        position,
    )
}

fn opening_hands(state: &mut Game) {
    state.update(Game {
        state: GameState {
            auto_advance: true,
            phase: GamePhase::Main,
        },
        user: PlayerState {
            hand: vec![demon_wolf(None), cyclops(None), metalon(None)],
            ..PlayerState::default()
        },
        enemy: PlayerState {
            mana: 0,
            hand: vec![demon_wolf(None), cyclops(None), metalon(None)],
            ..PlayerState::default()
        },
    });
}

fn combat(state: &mut Game) {
    state.update(Game {
        state: GameState {
            auto_advance: true,
            phase: GamePhase::Attackers,
        },
        user: PlayerState {
            attackers: vec![demon_wolf(Some(CombatPosition::Position2))],
            defenders: vec![cyclops(Some(CombatPosition::Position1))],
            reserve: vec![metalon(None)],
            hand: vec![treant(None)],
            ..PlayerState::default()
        },
        enemy: PlayerState {
            mana: 0,
            attackers: vec![cyclops(Some(CombatPosition::Position0))],
            defenders: vec![metalon(Some(CombatPosition::Position2))],
            reserve: vec![treant(None)],
            hand: vec![demon_wolf(None)],
            ..PlayerState::default()
        },
    });
}
