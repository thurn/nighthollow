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

use eyre::eyre;
use eyre::Result;

use crate::{
    model::{assets::*, cards, cards::*, creatures::*, games::*, primitives::*, stats::*},
    rules::basic::BaseMeleeDamageAttack,
};

pub fn load_scenario(name: &str) -> Result<Game> {
    cards::debug_reset_id_generation();

    match name {
        "basic" => Ok(basic()),
        _ => Err(eyre!("Unrecognized scenario name: {}", name)),
    }
}

fn basic() -> Game {
    Game {
        id: 1,
        state: GameState {
            phase: GamePhase::Main,
            turn: 1,
        },
        user: new_player(PlayerName::User, basic_deck),
        enemy: new_player(PlayerName::Enemy, basic_deck),
    }
}

fn new_player(name: PlayerName, deck: impl Fn(PlayerName) -> Vec<Card>) -> Player {
    Player {
        name,
        state: PlayerState::default(),
        deck: Deck::new(deck(name), name),
        hand: vec![],
        creatures: vec![],
        scrolls: vec![],
    }
}

fn berserker(owner: PlayerName) -> CreatureData {
    CreatureData {
        card_data: CardData {
            id: 0,
            owner,
            cost: Cost::StandardCost(StandardCost {
                power: 2,
                influence: Influence::flame(1),
            }),
            name: String::from("Berserker"),
            school: School::Flame,
            text: String::from("Berserker"),
        },
        base_type: CreatureType::Berserker,
        stats: CreatureStats {
            health_total: Stat::new(100),
            base_damage: vec![DamageStat::new(25, DamageType::Physical)],
            ..CreatureStats::default()
        },
        rules: vec![BaseMeleeDamageAttack::new(SkillAnimation::Skill2)],
    }
}

fn wizard(owner: PlayerName) -> CreatureData {
    CreatureData {
        card_data: CardData {
            id: 0,
            owner,
            cost: Cost::StandardCost(StandardCost {
                power: 3,
                influence: Influence::flame(2),
            }),
            name: String::from("Wizard"),
            school: School::Flame,
            text: String::from("Wizard"),
        },
        base_type: CreatureType::Wizard,
        stats: CreatureStats {
            health_total: Stat::new(100),
            base_damage: vec![DamageStat::new(10, DamageType::Physical)],
            ..CreatureStats::default()
        },
        rules: vec![BaseMeleeDamageAttack::new(SkillAnimation::Skill3)],
    }
}

fn rage(owner: PlayerName) -> Spell {
    Spell {
        card_data: CardData {
            id: 0,
            owner,
            cost: Cost::StandardCost(StandardCost {
                power: 1,
                influence: Influence::flame(1),
            }),
            name: String::from("Rage"),
            school: School::Flame,
            text: String::from("Rage"),
        },
        base_type: SpellType::Rage,
    }
}

fn flame_scroll(owner: PlayerName) -> Scroll {
    Scroll {
        card_data: CardData {
            id: 0,
            owner,
            cost: Cost::None,
            name: String::from("Flame Scroll"),
            school: School::Flame,
            text: String::from("Flame Scroll"),
        },
        base_type: ScrollType::FlameScroll,
    }
}

fn basic_deck(owner: PlayerName) -> Vec<Card> {
    vec![
        Card::Creature(berserker(owner)),
        Card::Creature(wizard(owner)),
        Card::Spell(rage(owner)),
        Card::Scroll(flame_scroll(owner)),
    ]
}
