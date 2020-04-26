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

use crate::{attributes::*, effect::*, model::*, primitives::*};
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
        attack: Attack::BasicAttack(HealthValue::from(10)),
        abilities: Abilities::default(),
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
        attack: Attack::BasicAttack(HealthValue::from(10)),
        abilities: Abilities::default(),
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
        attack: Attack::BasicAttack(HealthValue::from(10)),
        abilities: Abilities::default(),
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
        attack: Attack::BasicAttack(HealthValue::from(15)),
        abilities: Abilities::default(),
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

fn fire_elemental(state: CreatureState) -> Creature {
    Creature {
        card: Card {
            id: next_identifier(),
            name: String::from("Fire Elemental"),
            cost: mana_cost(School::Flame, ManaValue::new(500), 4),
            school: School::Flame,
        },
        state,
        current_health: HealthValue::from(400),
        maximum_health: HealthValue::from(400),
        attack: Attack::BasicAttack(HealthValue::from(500)),
        abilities: Abilities::default(),
    }
}

fn fearless_halberdier(state: CreatureState) -> Creature {
    Creature {
        card: Card {
            id: next_identifier(),
            name: String::from("Fearless Halberdier"),
            cost: mana_cost(School::Flame, ManaValue::new(300), 2),
            school: School::Flame,
        },
        state,
        current_health: HealthValue::from(200),
        maximum_health: HealthValue::from(200),
        attack: Attack::BasicAttack(HealthValue::from(300)),
        abilities: Abilities::default(),
    }
}

fn goblin_assailant(state: CreatureState) -> Creature {
    Creature {
        card: Card {
            id: next_identifier(),
            name: String::from("Goblin Assailant"),
            cost: mana_cost(School::Flame, ManaValue::new(200), 2),
            school: School::Flame,
        },
        state,
        current_health: HealthValue::from(200),
        maximum_health: HealthValue::from(200),
        attack: Attack::BasicAttack(HealthValue::from(200)),
        abilities: Abilities::default(),
    }
}

async fn target_creature_or_player(game: &Game) -> &impl Attackable {
    &game.user.status
}

async fn target_creature(game: &Game) -> &Creature {
    &game.user.creatures[0]
}

fn apply_damage(target: &impl Attackable, damage: Damage) {}

struct AttackModifier {}

fn add_attack_modifier(target: &Creature, attack: HealthValue) -> AttackModifier {
    AttackModifier {}
}

fn remove_attack_modifier(target: &Creature, modifier: AttackModifier) {}

async fn end_of_turn(game: &Game) {}

async fn shock_test(game: &Game) {
    let target = target_creature_or_player(game).await;
    apply_damage(target, Damage::fire(2));
}

async fn infuriate_test(game: &Game) {
    let target = target_creature(game).await;
    let modifier = add_attack_modifier(target, HealthValue::from(300));
    end_of_turn(game).await;
    remove_attack_modifier(target, modifier);
}

async fn player_dealt_damage_event(game: &Game) -> bool {
    true
}

async fn alive(game: &Game) -> bool {
    true
}

async fn wildfire_test(game: &Game) {
    while alive(game).await && player_dealt_damage_event(game).await {}
}

fn shock() -> Spell {
    Spell {
        card: Card {
            id: next_identifier(),
            name: String::from("Shock"),
            cost: mana_cost(School::Flame, ManaValue::new(100), 2),
            school: School::Flame,
        },
        effects: vec![Box::new(DamageCreatureOrPlayerEffect(Damage::fire(200)))],
    }
}

fn infuriate() -> Spell {
    Spell {
        card: Card {
            id: next_identifier(),
            name: String::from("Infuriate"),
            cost: mana_cost(School::Flame, ManaValue::new(100), 2),
            school: School::Flame,
        },
        effects: vec![
            Box::new(BonusAttackThisTurnEffect(HealthValue::from(300))),
            Box::new(BonusHealthThisTurnEffect(HealthValue::from(200))),
        ],
    }
}

fn chandras_outrage() -> Spell {
    Spell {
        card: Card {
            id: next_identifier(),
            name: String::from("Chandra's Outrage"),
            cost: mana_cost(School::Flame, ManaValue::new(400), 4),
            school: School::Flame,
        },
        effects: vec![
            Box::new(DamageCreatureEffect(Damage::fire(400))),
            Box::new(DamagePlayerEffect(Damage::fire(200))),
        ],
    }
}

fn hostile_minotaur(state: CreatureState) -> Creature {
    Creature {
        card: Card {
            id: next_identifier(),
            name: String::from("Hostile Minotaur"),
            cost: mana_cost(School::Flame, ManaValue::new(400), 2),
            school: School::Flame,
        },
        state,
        current_health: HealthValue::from(300),
        maximum_health: HealthValue::from(300),
        attack: Attack::BasicAttack(HealthValue::from(300)),
        abilities: Abilities {
            attributes: vec![Attribute::Swift],
            ..Abilities::default()
        },
    }
}

fn keldon_raider(state: CreatureState) -> Creature {
    Creature {
        card: Card {
            id: next_identifier(),
            name: String::from("Keldon Raider"),
            cost: mana_cost(School::Flame, ManaValue::new(400), 4),
            school: School::Flame,
        },
        state,
        current_health: HealthValue::from(300),
        maximum_health: HealthValue::from(300),
        attack: Attack::BasicAttack(HealthValue::from(400)),
        abilities: Abilities {
            triggers: vec![Trigger(
                TriggerName::Play,
                vec![Box::new(MayDiscardToDrawEffect())],
            )],
            ..Abilities::default()
        },
    }
}

fn lavakin_brawler(state: CreatureState) -> Creature {
    Creature {
        card: Card {
            id: next_identifier(),
            name: String::from("Lavakin Brawler"),
            cost: mana_cost(School::Flame, ManaValue::new(400), 200),
            school: School::Flame,
        },
        state,
        current_health: HealthValue::from(400),
        maximum_health: HealthValue::from(400),
        attack: Attack::BasicAttack(HealthValue::from(200)),
        abilities: Abilities {
            triggers: vec![Trigger(
                TriggerName::Attack,
                vec![Box::new(BrawlerEffect {
                    tag: CreatureTag::Elemental,
                    bonus: HealthValue::from(1),
                })],
            )],
            ..Abilities::default()
        },
    }
}

fn engulfing_eruption() -> Spell {
    Spell {
        card: Card {
            id: next_identifier(),
            name: String::from("Engulfing Eruption"),
            cost: mana_cost(School::Flame, ManaValue::new(400), 4),
            school: School::Flame,
        },
        effects: vec![Box::new(DamageCreatureEffect(Damage::fire(500)))],
    }
}

fn rubblebelt_recluse(state: CreatureState) -> Creature {
    Creature {
        card: Card {
            id: next_identifier(),
            name: String::from("Rubblebelt Recluse"),
            cost: mana_cost(School::Flame, ManaValue::new(500), 2),
            school: School::Flame,
        },
        state,
        current_health: HealthValue::from(500),
        maximum_health: HealthValue::from(500),
        attack: Attack::BasicAttack(HealthValue::from(600)),
        abilities: Abilities {
            attributes: vec![Attribute::MustAttack],
            ..Abilities::default()
        },
    }
}

fn scorch_spitter(state: CreatureState) -> Creature {
    Creature {
        card: Card {
            id: next_identifier(),
            name: String::from("Scorch Spitter"),
            cost: mana_cost(School::Flame, ManaValue::new(100), 2),
            school: School::Flame,
        },
        state,
        current_health: HealthValue::from(100),
        maximum_health: HealthValue::from(100),
        attack: Attack::BasicAttack(HealthValue::from(100)),
        abilities: Abilities {
            triggers: vec![Trigger(
                TriggerName::Attack,
                vec![Box::new(DamagePlayerEffect(Damage::fire(1)))],
            )],
            ..Abilities::default()
        },
    }
}

fn maniacal_rage() -> Spell {
    Spell {
        card: Card {
            id: next_identifier(),
            name: String::from("Maniacal Rage"),
            cost: mana_cost(School::Flame, ManaValue::new(200), 2),
            school: School::Flame,
        },
        effects: vec![
            Box::new(BonusAttackEffect(HealthValue::from(200))),
            Box::new(BonusHealthEffect(HealthValue::from(200))),
            Box::new(ApplyAttributeEffect(Attribute::CantDefend)),
        ],
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
