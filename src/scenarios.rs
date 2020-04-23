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
        Attack, Card, CardVariant, Cost, Creature, CreatureState, Crystal, Deck, Game, GameStatus,
        ManaCost, Player, PlayerStatus,
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
            cost: mana_cost(School::Flame, ManaValue::new(200), 1),
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
            cost: mana_cost(School::Flame, ManaValue::new(400), 2),
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
            cost: mana_cost(School::Flame, ManaValue::new(100), 1),
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
            cost: mana_cost(School::Flame, ManaValue::new(50), 1),
            school: School::Flame,
        },
        state,
        current_health: HealthValue::from(75),
        maximum_health: HealthValue::from(75),
        attacks: vec![Attack::BasicAttack(HealthValue::from(15))],
    }
}

fn flame_crystal() -> Crystal {
    Crystal {
        card: Card {
            id: next_identifier(),
            name: String::from("Flame Crystal"),
            cost: Cost::None,
            school: School::Flame,
        },
        mana_per_turn: ManaValue::from(100),
        influence_per_turn: Influence {
            flame: 2,
            ..Influence::default()
        },
    }
}

fn creature_card(function: &impl Fn(CreatureState) -> Creature) -> CardVariant {
    CardVariant::Creature(function(CreatureState::Default))
}

fn crystal_card(function: &impl Fn() -> Crystal) -> CardVariant {
    CardVariant::Crystal(function())
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

fn deck() -> Deck {
    Deck {
        cards: vec![
            creature_card(&demon_wolf),
            creature_card(&cyclops),
            creature_card(&metalon),
            creature_card(&treant),
            crystal_card(&flame_crystal),
        ],
        weights: vec![4000; 5],
    }
}

fn opening_hands(game: &mut Game) {
    std::mem::replace(
        game,
        Game {
            status: GameStatus {
                phase: GamePhase::Main,
            },
            user: Player {
                status: PlayerStatus {
                    mana: ManaValue::from(300),
                    influence: Influence {
                        flame: 6,
                        ..Influence::default()
                    },
                    ..PlayerStatus::default()
                },
                deck: deck(),
                hand: vec![
                    creature_card(&demon_wolf),
                    creature_card(&cyclops),
                    creature_card(&metalon),
                    crystal_card(&flame_crystal),
                ],
                crystals: vec![flame_crystal(), flame_crystal(), flame_crystal()],
                ..Player::default()
            },
            enemy: Player {
                status: PlayerStatus {
                    mana: ManaValue::from(300),
                    influence: Influence {
                        flame: 6,
                        ..Influence::default()
                    },
                    ..PlayerStatus::default()
                },
                deck: deck(),
                hand: vec![
                    creature_card(&demon_wolf),
                    creature_card(&cyclops),
                    creature_card(&metalon),
                    crystal_card(&flame_crystal),
                ],
                crystals: vec![flame_crystal(), flame_crystal(), flame_crystal()],
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
                status: PlayerStatus {
                    mana: ManaValue::from(300),
                    influence: Influence {
                        flame: 6,
                        ..Influence::default()
                    },
                    ..PlayerStatus::default()
                },
                deck: deck(),
                hand: vec![creature_card(&metalon), crystal_card(&flame_crystal)],
                creatures: vec![
                    in_play(&demon_wolf),
                    attacker(0, &cyclops),
                    defender(0, &treant),
                ],
                crystals: vec![flame_crystal(), flame_crystal(), flame_crystal()],
                ..Player::default()
            },
            enemy: Player {
                status: PlayerStatus {
                    mana: ManaValue::from(300),
                    influence: Influence {
                        flame: 6,
                        ..Influence::default()
                    },
                    ..PlayerStatus::default()
                },
                deck: deck(),
                hand: vec![
                    creature_card(&treant),
                    creature_card(&cyclops),
                    crystal_card(&flame_crystal),
                ],
                creatures: vec![attacker(0, &demon_wolf), defender(0, &metalon)],
                crystals: vec![flame_crystal(), flame_crystal(), flame_crystal()],
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
