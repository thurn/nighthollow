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

use crate::{
    api, interface,
    model::{
        primitives::{Influence, PlayerName, School, SCHOOLS},
        types::{Card, Cost, CreatureArchetype, HasCardData, ManaCost, Scroll, Spell},
    },
};

pub fn card_id(index: i32, owner: PlayerName) -> api::CardId {
    api::CardId {
        index,
        owner: player(owner).into(),
    }
}

pub fn creature_id(index: i32, owner: PlayerName) -> api::CreatureId {
    api::CreatureId {
        index,
        owner: player(owner).into(),
    }
}

pub fn prefab(address: &str) -> api::Asset {
    api::Asset {
        address: address.to_string(),
        asset_type: api::AssetType::Prefab.into(),
    }
}

pub fn sprite(address: &str) -> api::Asset {
    api::Asset {
        address: address.to_string(),
        asset_type: api::AssetType::Prefab.into(),
    }
}

pub fn player(name: PlayerName) -> api::PlayerName {
    match name {
        PlayerName::User => api::PlayerName::User,
        PlayerName::Enemy => api::PlayerName::Enemy,
    }
}

pub fn text(text: &str) -> api::RichText {
    api::RichText {
        text: text.to_string(),
    }
}

pub fn influence_type(school: &School) -> api::InfluenceType {
    match school {
        School::Light => api::InfluenceType::Light,
        School::Sky => api::InfluenceType::Sky,
        School::Flame => api::InfluenceType::Flame,
        School::Ice => api::InfluenceType::Ice,
        School::Earth => api::InfluenceType::Earth,
        School::Shadow => api::InfluenceType::Shadow,
    }
}

pub fn influence(influence: &Influence) -> Vec<api::Influence> {
    let mut result = Vec::new();
    for school in SCHOOLS.iter() {
        if influence.value(school) > 0 {
            result.push(api::Influence {
                influence_type: influence_type(school).into(),
                value: influence.value(school),
            })
        }
    }
    result
}

pub fn mana_cost(cost: &ManaCost) -> api::StandardCost {
    api::StandardCost {
        mana_cost: cost.mana,
        influence_cost: influence(&cost.influence),
    }
}

pub fn cost(cost: &Cost) -> api::card_data::Cost {
    match cost {
        Cost::None => api::card_data::Cost::NoCost(api::NoCost {}),
        Cost::ManaCost(c) => api::card_data::Cost::StandardCost(mana_cost(c)),
    }
}

pub fn creature_archetype(
    creature: &CreatureArchetype,
    metadata: &CardMetadata,
) -> api::CreatureData {
    api::CreatureData {
        creature_id: Some(creature_id(metadata.index, metadata.owner)),
        prefab: Some(prefab(&interface::creature_address(creature.base_type))),
        rank_position: api::RankValue::RankUnspecified.into(),
        file_position: api::FileValue::FileUnspecified.into(),
        can_be_repositioned: metadata.can_resposition_creature,
        attachments: vec![],
    }
}

pub fn spell(spell: &Spell) -> api::AttachmentData {
    api::AttachmentData {
        image: Some(sprite(&interface::spell_image_address(spell.base_type))),
    }
}

pub fn scroll(scroll: &Scroll) -> api::UntargetedData {
    api::UntargetedData {}
}

pub fn card_type(card: &Card, metadata: &CardMetadata) -> api::card_data::CardType {
    match card {
        Card::Creature(c) => {
            api::card_data::CardType::CreatureCard(creature_archetype(c, metadata))
        }
        Card::Spell(s) => api::card_data::CardType::AttachmentCard(spell(s)),
        Card::Scroll(s) => api::card_data::CardType::UntargetedCard(scroll(s)),
    }
}

pub struct CardMetadata {
    pub owner: PlayerName,
    pub index: i32,
    pub revealed: bool,
    pub can_play: bool,
    pub can_resposition_creature: bool,
}

pub fn card_data(card: &Card, metadata: &CardMetadata) -> api::CardData {
    api::CardData {
        card_id: Some(card_id(metadata.index, metadata.owner)),
        prefab: Some(prefab(&format!("Cards/{:?}Card", card.card_data().school))),
        name: card.card_data().name.clone(),
        image: Some(sprite(&interface::card_image_address(card))),
        text: Some(text(&card.card_data().text)),
        is_revealed: metadata.revealed,
        can_be_played: metadata.can_play,
        cost: Some(cost(&card.card_data().cost)),
        card_type: Some(card_type(card, metadata)),
    }
}

pub fn draw_card(card: &Card, metadata: &CardMetadata) -> api::DrawCardCommand {
    api::DrawCardCommand {
        card: Some(card_data(card, metadata)),
    }
}

pub fn empty() -> api::CommandList {
    api::CommandList {
        command_groups: vec![],
    }
}
