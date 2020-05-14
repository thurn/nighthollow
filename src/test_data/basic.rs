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

use std::sync::atomic::{AtomicI32, Ordering};

use crate::model::primitives::*;
use crate::model::types::*;

static NEXT_IDENTIFIER_INDEX: AtomicI32 = AtomicI32::new(1);

fn next_id() -> i32 {
    NEXT_IDENTIFIER_INDEX.fetch_add(1, Ordering::Relaxed)
}

pub fn berserker(owner: PlayerName) -> CreatureArchetype {
    CreatureArchetype {
        card_data: CardData {
            id: next_id(),
            owner,
            cost: Cost::ManaCost(ManaCost {
                mana: 2,
                influence: Influence::flame(1),
            }),
            name: String::from("Berserker"),
            school: School::Flame,
            text: "Anger & Axes".to_string(),
        },
        base_type: CreatureType::Berserker,
        health: 200,
    }
}

pub fn mage(owner: PlayerName) -> CreatureArchetype {
    CreatureArchetype {
        card_data: CardData {
            id: next_id(),
            owner,
            cost: Cost::ManaCost(ManaCost {
                mana: 3,
                influence: Influence::flame(2),
            }),
            name: String::from("Mage"),
            school: School::Flame,
            text: "Whiz! Zoom!".to_string(),
        },
        base_type: CreatureType::Mage,
        health: 100,
    }
}

pub fn rage(owner: PlayerName) -> Spell {
    Spell {
        card_data: CardData {
            id: next_id(),
            owner,
            cost: Cost::ManaCost(ManaCost {
                mana: 1,
                influence: Influence::flame(1),
            }),
            name: String::from("Rage"),
            school: School::Flame,
            text: "Adds Bonus Damage on Hits".to_string(),
        },
        base_type: SpellType::Rage,
    }
}

pub fn flame_scroll(owner: PlayerName) -> Scroll {
    Scroll {
        card_data: CardData {
            id: next_id(),
            owner,
            cost: Cost::None,
            name: String::from("Flame Scroll"),
            school: School::Flame,
            text: "Adds 1 mana and 1 flame influence".to_string(),
        },
        base_type: ScrollType::FlameScroll,
    }
}

pub fn new_player(name: PlayerName) -> Player {
    Player {
        name,
        state: PlayerState::default(),
        hand: vec![
            Card::Creature(berserker(name)),
            Card::Creature(berserker(name)),
            Card::Creature(mage(name)),
            Card::Spell(rage(name)),
            Card::Scroll(flame_scroll(name)),
            Card::Scroll(flame_scroll(name)),
        ],
        creatures: vec![],
        scrolls: vec![],
    }
}

pub fn opening_hands() -> Game {
    Game {
        state: GameState {
            phase: GamePhase::Main,
        },
        user: new_player(PlayerName::User),
        enemy: new_player(PlayerName::Enemy),
    }
}
