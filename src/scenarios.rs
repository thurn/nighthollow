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
    model::{
        Attack, Card, CardVariant, Cost, Creature, CreatureState, Game, GameStatus, ManaCost,
        Player, PlayerStatus,
    },
    primitives::{
        CombatPosition, GamePhase, HealthValue, Influence, InterfaceError, ManaValue, Result,
        School,
    },
};
use std::sync::atomic::{AtomicI32, Ordering};

static NEXT_IDENTIFIER_INDEX: AtomicI32 = AtomicI32::new(1);

pub fn load_scenario(state: &mut Game, name: &str) -> Result<()> {
    NEXT_IDENTIFIER_INDEX.store(1, Ordering::Relaxed);
    match name {
        "empty" => Ok(()),
        "opening" => Ok(opening_hands(state)),
        "combat" => Ok(combat(state)),
        _ => InterfaceError::result(format!("Unknown scenario {}", name)),
    }
}

fn mana_cost(school: School, mana: ManaValue, influence: i32) -> Cost {
    Cost::ManaCost(ManaCost {
        mana,
        influence: Influence::new(school, influence),
    })
}

fn demon_wolf(state: CreatureState) -> Creature {
    Creature {
        card: Card {
            id: next_identifier(),
            name: String::from("Demon Wolf"),
            cost: mana_cost(School::Flame, ManaValue::new(2), 1),
            school: School::Flame,
        },
        state,
        current_health: HealthValue::from(100),
        maximum_health: HealthValue::from(100),
        attacks: vec![Attack::BasicAttack(HealthValue::from(10))],
    }
}

fn cyclops(state: CreatureState) -> Creature {
    Creature {
        card: Card {
            id: next_identifier(),
            name: String::from("Cyclops"),
            cost: mana_cost(School::Flame, ManaValue::new(4), 2),
            school: School::Flame,
        },
        state,
        current_health: HealthValue::from(250),
        maximum_health: HealthValue::from(250),
        attacks: vec![Attack::BasicAttack(HealthValue::from(10))],
    }
}

fn metalon(state: CreatureState) -> Creature {
    Creature {
        card: Card {
            id: next_identifier(),
            name: String::from("Metalon"),
            cost: mana_cost(School::Flame, ManaValue::new(3), 1),
            school: School::Flame,
        },
        state,
        current_health: HealthValue::from(200),
        maximum_health: HealthValue::from(200),
        attacks: vec![Attack::BasicAttack(HealthValue::from(10))],
    }
}

fn treant(state: CreatureState) -> Creature {
    Creature {
        card: Card {
            id: next_identifier(),
            name: String::from("Treant"),
            cost: mana_cost(School::Flame, ManaValue::new(1), 1),
            school: School::Flame,
        },
        state,
        current_health: HealthValue::from(75),
        maximum_health: HealthValue::from(75),
        attacks: vec![Attack::BasicAttack(HealthValue::from(15))],
    }
}

fn in_hand(function: &impl Fn(CreatureState) -> Creature) -> CardVariant {
    CardVariant::Creature(function(CreatureState::Default))
}

fn in_play(function: &impl Fn(CreatureState) -> Creature) -> Creature {
    function(CreatureState::Default)
}

fn attacker(position: i32, function: &impl Fn(CreatureState) -> Creature) -> Creature {
    function(CreatureState::Attacking(
        CombatPosition::from(position).expect("Invalid"),
    ))
}

fn defender(position: i32, function: &impl Fn(CreatureState) -> Creature) -> Creature {
    function(CreatureState::Defending(
        CombatPosition::from(position).expect("Invalid"),
    ))
}

fn opening_hands(game: &mut Game) {
    std::mem::replace(
        game,
        Game {
            status: GameStatus {
                phase: GamePhase::Main,
            },
            user: Player {
                hand: vec![in_hand(&demon_wolf), in_hand(&cyclops), in_hand(&metalon)],
                ..Player::default()
            },
            enemy: Player {
                hand: vec![in_hand(&demon_wolf), in_hand(&cyclops), in_hand(&metalon)],
                ..Player::default()
            },
        },
    );
}

fn combat(game: &mut Game) {
    std::mem::replace(
        game,
        Game {
            status: GameStatus {
                phase: GamePhase::PreCombat,
            },
            user: Player {
                hand: vec![in_hand(&metalon)],
                creatures: vec![
                    in_play(&demon_wolf),
                    attacker(0, &cyclops),
                    defender(0, &treant),
                ],
                ..Player::default()
            },
            enemy: Player {
                hand: vec![in_hand(&treant), in_hand(&cyclops)],
                creatures: vec![attacker(0, &demon_wolf), defender(0, &metalon)],
                ..Player::default()
            },
        },
    );
}

fn next_identifier() -> String {
    to_identifier(NEXT_IDENTIFIER_INDEX.fetch_add(1, Ordering::Relaxed))
}

fn to_identifier(index: i32) -> String {
    let mut dividend = index;
    let mut column_name = String::new();
    let mut modulo: u8;

    while dividend > 0 {
        modulo = ((dividend - 1) % 26) as u8;
        column_name.insert(0, (65 + modulo) as char);
        dividend = (dividend - modulo as i32) / 26;
    }

    return column_name;
}
