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

use crate::prelude::*;

use crate::{
    gameplay::{
        basic_attacks::BasicMeleeAttack,
        core::{
            card_rules::CoreCardRules, creature_rules::CoreCreatureRules,
            player_rules::CorePlayerRules, scroll_rules::CoreScrollRules,
        },
    },
    model::{
        assets::*,
        cards,
        cards::*,
        creatures::*,
        games::*,
        players::{Player, PlayerState},
        primitives::*,
        stats::*,
    },
};
use std::collections::BTreeMap;

pub fn load_scenario(name: &str) -> Result<Game> {
    cards::debug_reset_id_generation();

    match name {
        "standard" => Ok(standard()),
        "basic_turn" => Ok(basic_turn()),
        _ => Err(eyre!("Unrecognized scenario name: {}", name)),
    }
}

fn standard() -> Game {
    Game {
        id: 1,
        state: GameState::default(),
        user: new_player(PlayerName::User, basic_deck),
        enemy: new_player(PlayerName::Enemy, basic_deck),
    }
}

fn basic_turn() -> Game {
    Game {
        id: 1,
        state: GameState::default(),
        user: basic_turn_player(new_player(PlayerName::User, basic_deck)),
        enemy: basic_turn_player(new_player(PlayerName::Enemy, basic_deck)),
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
        rules: vec![CorePlayerRules::new()],
    }
}

fn basic_turn_player(mut player: Player) -> Player {
    player.state.current_power = 8;
    player.state.maximum_power = 8;
    player.state.current_influence = Influence::single(6, School::Flame);
    player.state.maximum_influence = Influence::single(6, School::Flame);
    player
}

fn berserker(owner: PlayerName) -> CreatureData {
    CreatureData {
        card_data: CardData {
            id: 0,
            rules: vec![CoreCardRules::new()],
            owner,
            state: CardState::default(),
            cost: Cost::StandardCost(StandardCost {
                power: 2,
                influence: Influence::single(1, School::Flame),
            }),
            name: String::from("Berserker"),
            school: School::Flame,
            text: String::from("Berserker"),
        },
        base_type: CreatureType::Berserker,
        stats: CreatureStats {
            health_total: Stat::new(100),
            base_damage: DamageStats {
                physical: Stat::new(25),
                ..DamageStats::default()
            },
            ..CreatureStats::default()
        },
        rules: vec![
            CoreCreatureRules::new(),
            BasicMeleeAttack::new(SkillAnimation::Skill2),
        ],
    }
}

fn wizard(owner: PlayerName) -> CreatureData {
    CreatureData {
        card_data: CardData {
            id: 0,
            rules: vec![CoreCardRules::new()],
            owner,
            state: CardState::default(),
            cost: Cost::StandardCost(StandardCost {
                power: 3,
                influence: Influence::single(2, School::Flame),
            }),
            name: String::from("Wizard"),
            school: School::Flame,
            text: String::from("Wizard"),
        },
        base_type: CreatureType::Wizard,
        stats: CreatureStats {
            health_total: Stat::new(100),
            base_damage: DamageStats {
                physical: Stat::new(10),
                ..DamageStats::default()
            },
            ..CreatureStats::default()
        },
        rules: vec![
            CoreCreatureRules::new(),
            BasicMeleeAttack::new(SkillAnimation::Skill3),
        ],
    }
}

fn rage(owner: PlayerName) -> Spell {
    Spell {
        card_data: CardData {
            id: 0,
            rules: vec![CoreCardRules::new()],
            owner,
            state: CardState::default(),
            cost: Cost::StandardCost(StandardCost {
                power: 1,
                influence: Influence::single(1, School::Flame),
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
            rules: vec![CoreCardRules::new()],
            owner,
            state: CardState::default(),
            cost: Cost::ScrollPlay,
            name: String::from("Flame Scroll"),
            school: School::Flame,
            text: String::from("Flame Scroll"),
        },
        base_type: ScrollType::FlameScroll,
        stats: ScrollStats {
            added_current_power: 1,
            added_maximum_power: 1,
            added_current_influence: Influence::single(1, School::Flame),
            added_maximum_influence: Influence::single(1, School::Flame),
        },
        rules: vec![CoreScrollRules::new()],
    }
}

fn basic_deck(owner: PlayerName) -> Vec<Card> {
    vec![
        Card::Creature(berserker(owner)),
        Card::Creature(wizard(owner)),
        Card::Spell(rage(owner)),
        Card::Scroll(flame_scroll(owner)),
        Card::Creature(berserker(owner)),
        Card::Creature(wizard(owner)),
        Card::Creature(berserker(owner)),
        Card::Creature(berserker(owner)),
        Card::Creature(wizard(owner)),
        Card::Spell(rage(owner)),
        Card::Scroll(flame_scroll(owner)),
        Card::Scroll(flame_scroll(owner)),
        Card::Scroll(flame_scroll(owner)),
        Card::Scroll(flame_scroll(owner)),
        Card::Scroll(flame_scroll(owner)),
    ]
}
