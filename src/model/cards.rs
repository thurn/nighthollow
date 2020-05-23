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

use super::{
    assets::SpellType,
    creatures::CreatureData,
    games::HasOwner,
    primitives::{CardId, Influence, PlayerName, School},
};


use serde::{Deserialize, Serialize};

#[derive(Serialize, Deserialize, Debug, Clone)]
pub struct ManaCost {
    pub mana: i32,
    pub influence: Influence,
}

#[derive(Serialize, Deserialize, Debug, Clone)]
pub enum Cost {
    None,
    ManaCost(ManaCost),
}

#[derive(Serialize, Deserialize, Debug, Clone)]
pub struct CardData {
    pub id: CardId,
    pub owner: PlayerName,
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

impl<T: HasCardData> HasOwner for T {
    fn owner(&self) -> PlayerName {
        self.card_data().owner
    }
}

#[derive(Serialize, Deserialize, Debug, Clone)]
pub struct Spell {
    pub card_data: CardData,
    pub base_type: SpellType,
}

impl HasCardData for Spell {
    fn card_data(&self) -> &CardData {
        &self.card_data
    }
}

#[derive(Serialize, Deserialize, Debug, PartialEq, Eq, Copy, Clone)]
pub enum ScrollType {
    FlameScroll,
}

#[derive(Serialize, Deserialize, Debug, Clone)]
pub struct Scroll {
    pub card_data: CardData,
    pub base_type: ScrollType,
}

impl HasCardData for Scroll {
    fn card_data(&self) -> &CardData {
        &self.card_data
    }
}

#[derive(Serialize, Deserialize, Debug, Clone)]
pub enum Card {
    Creature(CreatureData),
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
