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

use color_eyre::Result;
use eyre::eyre;

use crate::{
    api, interface,
    model::{
        creatures::{Creature, CreatureData},
        primitives::{FileValue, Influence, PlayerName, RankValue, School, SCHOOLS},
        types::{Card, Cost, HasCardData, HasOwner, ManaCost, Player, Scroll, Spell},
    },
};

pub fn card_id(id: i32) -> api::CardId {
    api::CardId { value: id }
}

pub fn creature_id(id: i32) -> api::CreatureId {
    api::CreatureId { value: id }
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
        asset_type: api::AssetType::Sprite.into(),
    }
}

pub fn player_name(name: PlayerName) -> api::PlayerName {
    match name {
        PlayerName::User => api::PlayerName::User,
        PlayerName::Enemy => api::PlayerName::Enemy,
    }
}

pub fn rank_value(rank: RankValue) -> api::RankValue {
    match rank {
        RankValue::Rank0 => api::RankValue::Rank0,
        RankValue::Rank1 => api::RankValue::Rank1,
        RankValue::Rank2 => api::RankValue::Rank2,
        RankValue::Rank3 => api::RankValue::Rank3,
        RankValue::Rank4 => api::RankValue::Rank4,
        RankValue::Rank5 => api::RankValue::Rank5,
    }
}

pub fn file_value(file: FileValue) -> api::FileValue {
    match file {
        FileValue::File0 => api::FileValue::File0,
        FileValue::File1 => api::FileValue::File1,
        FileValue::File2 => api::FileValue::File2,
        FileValue::File3 => api::FileValue::File3,
        FileValue::File4 => api::FileValue::File4,
        FileValue::File5 => api::FileValue::File5,
    }
}

pub fn player_data(player: &Player) -> api::PlayerData {
    api::PlayerData {
        player_name: player_name(player.name).into(),
        current_life: player.state.current_life,
        maximum_life: player.state.maximum_life,
        current_mana: player.state.current_mana,
        maximum_mana: player.state.maximum_mana,
        current_influence: influence(&player.state.current_influence),
        maximum_influence: influence(&player.state.current_influence),
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
    creature: &CreatureData,
    metadata: &CreatureMetadata,
) -> api::CreatureData {
    api::CreatureData {
        creature_id: Some(creature_id(metadata.id)),
        prefab: Some(prefab(&interface::creature_address(creature.base_type))),
        owner: player_name(creature.owner()).into(),
        rank_position: api::RankValue::RankUnspecified.into(),
        file_position: api::FileValue::FileUnspecified.into(),
        can_be_repositioned: metadata.can_resposition_creature,
        attachments: vec![],
    }
}

pub fn creature_data(creature: &Creature, metadata: &CreatureMetadata) -> api::CreatureData {
    api::CreatureData {
        rank_position: rank_value(creature.position.rank).into(),
        file_position: file_value(creature.position.file).into(),
        attachments: creature.spells.iter().map(spell).collect(),
        ..creature_archetype(&creature.data, metadata)
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
            api::card_data::CardType::CreatureCard(creature_archetype(c, &metadata.creature))
        }
        Card::Spell(s) => api::card_data::CardType::AttachmentCard(spell(s)),
        Card::Scroll(s) => api::card_data::CardType::UntargetedCard(scroll(s)),
    }
}

pub struct CreatureMetadata {
    pub id: i32,
    pub can_resposition_creature: bool,
}

pub struct CardMetadata {
    pub id: i32,
    pub revealed: bool,
    pub can_play: bool,
    pub creature: CreatureMetadata,
}

pub fn card_data(card: &Card, metadata: &CardMetadata) -> api::CardData {
    api::CardData {
        card_id: Some(card_id(metadata.id)),
        prefab: Some(prefab(&format!("Cards/{:?}Card", card.card_data().school))),
        name: card.card_data().name.clone(),
        owner: player_name(card.owner()).into(),
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

pub fn draw_card_command(card: &Card, metadata: &CardMetadata) -> api::Command {
    api::Command {
        command: Some(api::command::Command::DrawCard(draw_card(card, metadata))),
    }
}

pub fn update_player(player: &Player) -> api::UpdatePlayerCommand {
    api::UpdatePlayerCommand {
        player: Some(player_data(player)),
    }
}

pub fn update_player_command(player: &Player) -> api::Command {
    api::Command {
        command: Some(api::command::Command::UpdatePlayer(update_player(player))),
    }
}

pub fn play_card(
    card: &Card,
    metadata: &CardMetadata,
    delay_milliseconds: i32,
    rank_position: Option<RankValue>,
    file_position: Option<FileValue>,
) -> api::PlayCardCommand {
    api::PlayCardCommand {
        card: Some(card_data(card, metadata)),
        reveal_delay_milliseconds: delay_milliseconds,
        rank_position: rank_position
            .map_or(api::RankValue::RankUnspecified, |r| rank_value(r))
            .into(),
        file_position: file_position
            .map_or(api::FileValue::FileUnspecified, |f| file_value(f))
            .into(),
        ..api::PlayCardCommand::default()
    }
}

pub fn play_card_command(
    card: &Card,
    metadata: &CardMetadata,
    delay_milliseconds: i32,
    rank_position: Option<RankValue>,
    file_position: Option<FileValue>,
) -> api::Command {
    api::Command {
        command: Some(api::command::Command::PlayCard(play_card(
            card,
            metadata,
            delay_milliseconds,
            rank_position,
            file_position,
        ))),
    }
}

pub fn create_or_update_creature(
    creature: &Creature,
    metadata: &CreatureMetadata,
) -> api::CreateOrUpdateCreatureCommand {
    api::CreateOrUpdateCreatureCommand {
        creature: Some(creature_data(creature, metadata)),
    }
}

pub fn create_or_update_creature_command(
    creature: &Creature,
    metadata: &CreatureMetadata,
) -> api::Command {
    api::Command {
        command: Some(api::command::Command::CreateOrUpdateCreature(
            create_or_update_creature(creature, metadata),
        )),
    }
}

pub fn empty() -> Result<api::CommandList> {
    Ok(api::CommandList {
        command_groups: vec![],
    })
}

pub fn group(commands: Vec<api::Command>) -> Result<api::CommandList> {
    Ok(api::CommandList {
        command_groups: vec![api::CommandGroup { commands: commands }],
    })
}
