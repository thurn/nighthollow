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

use super::primitives::*;

pub struct ManaCost {
    pub mana: i32,
    pub influence: Influence,
}

pub enum Cost {
    None,
    ManaCost(ManaCost),
}

pub struct CardData {
    pub id: i32,
    pub cost: Cost,
    pub name: String,
    pub school: School,
    pub text: String,
}

pub trait HasCardData {
    fn card_data(&self) -> &CardData;
}

impl HasCardData for CardData {
    fn card_data(&self) -> &CardData {
        self
    }
}

#[derive(Debug, PartialEq, Eq, Copy, Clone)]
pub enum CreatureType {
    Berserker,
    Mage,
}

pub struct CreatureArchetype {
    pub card_data: CardData,
    pub base_type: CreatureType,
    pub health: i32,
}

impl HasCardData for CreatureArchetype {
    fn card_data(&self) -> &CardData {
        &self.card_data
    }
}

pub struct Creature {
    pub archetype: CreatureArchetype,
    pub position: BoardPosition,
    pub spells: Vec<Spell>,
}

impl HasCardData for Creature {
    fn card_data(&self) -> &CardData {
        &self.archetype.card_data()
    }
}

#[derive(Debug, PartialEq, Eq, Copy, Clone)]
pub enum SpellType {
    Rage,
}

pub struct Spell {
    pub card_data: CardData,
    pub base_type: SpellType,
}

impl HasCardData for Spell {
    fn card_data(&self) -> &CardData {
        &self.card_data
    }
}

#[derive(Debug, PartialEq, Eq, Copy, Clone)]
pub enum ScrollType {
    FlameScroll,
}

pub struct Scroll {
    pub card_data: CardData,
    pub base_type: ScrollType,
}

impl HasCardData for Scroll {
    fn card_data(&self) -> &CardData {
        &self.card_data
    }
}

pub enum Card {
    Creature(CreatureArchetype),
    Spell(Spell),
    Scroll(Scroll),
}

impl HasCardData for Card {
    fn card_data(&self) -> &CardData {
        match self {
            Card::Creature(c) => c.card_data(),
            Card::Spell(s) => s.card_data(),
            Card::Scroll(s) => s.card_data(),
        }
    }
}

pub struct PlayerState {
    pub current_life: i32,
    pub maximum_life: i32,
    pub current_mana: i32,
    pub maximum_mana: i32,
    pub current_influence: Influence,
    pub maximum_influence: Influence,
}

impl Default for PlayerState {
    fn default() -> Self {
        PlayerState {
            current_life: 25,
            maximum_life: 25,
            current_mana: 0,
            maximum_mana: 0,
            current_influence: Influence::default(),
            maximum_influence: Influence::default(),
        }
    }
}

pub struct Player {
    pub state: PlayerState,
    pub hand: Vec<Card>,
    pub creatures: Vec<Creature>,
    pub scrolls: Vec<Scroll>,
}

pub struct GameState {
    pub phase: GamePhase,
}

pub struct Game {
    pub state: GameState,
    pub user: Player,
    pub enemy: Player,
}
