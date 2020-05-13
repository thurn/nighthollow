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

use crate::model::types::{Card, CreatureType, ScrollType, SpellType};

pub fn creature_address(creature_type: CreatureType) -> String {
    format!("Creatures/{:?}", creature_type)
}

pub fn card_image_address(card: &Card) -> String {
    match card {
        Card::Creature(c) => creature_image_address(c.base_type),
        Card::Spell(s) => spell_image_address(s.base_type),
        Card::Scroll(s) => scroll_image_address(s.base_type),
    }
}

pub fn creature_image_address(creature_type: CreatureType) -> String {
    format!("CreatureImages/{:?}", creature_type)
}

pub fn spell_image_address(spell_type: SpellType) -> String {
    let image = match spell_type {
        SpellType::Rage => "SpellBook01_01",
    };

    format!("Spells/{}", image)
}

pub fn scroll_image_address(scroll_type: ScrollType) -> String {
    let image = match scroll_type {
        ScrollType::FlameScroll => "ScrollsAndBooks_21_t",
    };

    format!("Scrolls/{}", image)
}
