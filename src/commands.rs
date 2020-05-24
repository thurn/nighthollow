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

use crate::{
    api, interface,
    model::{
        assets::CreatureType,
        cards::{Card, Cost, HasCardData, ManaCost, Scroll, Spell},
        creatures::{Creature, CreatureData},
        games::{Game, HasOwner, Player},
        primitives::{
            CardId, CreatureId, FileValue, GameId, Influence, PlayerName, RankValue, School,
            SkillAnimation, SCHOOLS,
        },
    },
};
use api::{MOnImpactNumber, MSkillAnimation, MSkillAnimationNumber, MUseCreatureSkillCommand};

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
        RankValue::Rank1 => api::RankValue::Rank1,
        RankValue::Rank2 => api::RankValue::Rank2,
        RankValue::Rank3 => api::RankValue::Rank3,
        RankValue::Rank4 => api::RankValue::Rank4,
        RankValue::Rank5 => api::RankValue::Rank5,
    }
}

pub fn file_value(file: FileValue) -> api::FileValue {
    match file {
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

pub fn adapt_skill_animation(
    animation: SkillAnimation,
    creature_type: CreatureType,
) -> MSkillAnimation {
    MSkillAnimation {
        skill: match animation {
            SkillAnimation::Skill1 => MSkillAnimationNumber::Skill1,
            SkillAnimation::Skill2 => MSkillAnimationNumber::Skill2,
            SkillAnimation::Skill3 => MSkillAnimationNumber::Skill3,
            SkillAnimation::Skill4 => MSkillAnimationNumber::Skill4,
            SkillAnimation::Skill5 => MSkillAnimationNumber::Skill5,
        }
        .into(),
        impact_count: 1,
    }
}

pub fn creature_archetype(
    creature: &CreatureData,
    metadata: CreatureMetadata,
) -> api::CreatureData {
    api::CreatureData {
        creature_id: Some(creature_id(creature.card_data().id)),
        prefab: Some(prefab(&interface::creature_address(creature.base_type))),
        owner: player_name(creature.owner()).into(),
        rank_position: api::RankValue::RankUnspecified.into(),
        file_position: api::FileValue::FileUnspecified.into(),
        can_be_repositioned: metadata.can_resposition_creature,
        attachments: vec![],
    }
}

pub fn creature_data(creature: &Creature, metadata: CreatureMetadata) -> api::CreatureData {
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

pub fn scroll(_scroll: &Scroll) -> api::UntargetedData {
    api::UntargetedData {}
}

pub fn card_type(card: &Card, metadata: CardMetadata) -> api::card_data::CardType {
    match card {
        Card::Creature(c) => {
            api::card_data::CardType::CreatureCard(creature_archetype(c, metadata.creature))
        }
        Card::Spell(s) => api::card_data::CardType::AttachmentCard(spell(s)),
        Card::Scroll(s) => api::card_data::CardType::UntargetedCard(scroll(s)),
    }
}

pub struct CreatureMetadata {
    pub can_resposition_creature: bool,
}

pub struct CardMetadata {
    pub revealed: bool,
    pub can_play: bool,
    pub creature: CreatureMetadata,
}

pub fn card_data(card: &Card, metadata: CardMetadata) -> api::CardData {
    api::CardData {
        card_id: Some(card_id(card.card_data().id)),
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

pub fn draw_card(card: &Card, metadata: CardMetadata) -> api::DrawCardCommand {
    api::DrawCardCommand {
        card: Some(card_data(card, metadata)),
    }
}

pub fn draw_card_command(card: &Card, metadata: CardMetadata) -> api::Command {
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

pub fn reveal_card(
    card: api::CardData,
    delay_milliseconds: i32,
    rank_position: Option<RankValue>,
    file_position: Option<FileValue>,
) -> api::RevealCardCommand {
    api::RevealCardCommand {
        card: Some(card),
        reveal_delay_milliseconds: delay_milliseconds,
        rank_position: rank_position
            .map_or(api::RankValue::RankUnspecified, |r| rank_value(r))
            .into(),
        file_position: file_position
            .map_or(api::FileValue::FileUnspecified, |f| file_value(f))
            .into(),
        ..api::RevealCardCommand::default()
    }
}

pub fn reveal_card_command(
    card: api::CardData,
    delay_milliseconds: i32,
    rank_position: Option<RankValue>,
    file_position: Option<FileValue>,
) -> api::Command {
    api::Command {
        command: Some(api::command::Command::RevealCard(reveal_card(
            card,
            delay_milliseconds,
            rank_position,
            file_position,
        ))),
    }
}

pub fn create_or_update_creature(
    creature: &Creature,
    metadata: CreatureMetadata,
) -> api::CreateOrUpdateCreatureCommand {
    api::CreateOrUpdateCreatureCommand {
        creature: Some(creature_data(creature, metadata)),
    }
}

pub fn create_or_update_creature_command(
    creature: &Creature,
    metadata: CreatureMetadata,
) -> api::Command {
    api::Command {
        command: Some(api::command::Command::CreateOrUpdateCreature(
            create_or_update_creature(creature, metadata),
        )),
    }
}

pub fn destroy_card_command(player: PlayerName, id: CardId, must_exist: bool) -> api::Command {
    api::Command {
        command: Some(api::command::Command::DestroyCard(
            api::MDestroyCardCommand {
                card_id: Some(card_id(id)),
                player: player_name(player).into(),
                must_exist,
            },
        )),
    }
}

pub fn initiate_game_command(game: &Game) -> api::Command {
    api::Command {
        command: Some(api::command::Command::InitiateGame(
            api::MInitiateGameCommand {
                new_game_id: Some(api::GameId { value: game.id }),
                initial_user_state: Some(player_data(&game.user)),
                initial_enemy_state: Some(player_data(&game.enemy)),
            },
        )),
    }
}

pub fn use_creature_skill_command(
    source_creature: CreatureId,
    creature_type: CreatureType,
    animation: SkillAnimation,
    on_impact: Vec<MOnImpactNumber>,
    melee_target: Option<CreatureId>,
) -> api::Command {
    api::Command {
        command: Some(api::command::Command::UseCreatureSkill(
            MUseCreatureSkillCommand {
                source_creature: Some(creature_id(source_creature)),
                animation: Some(adapt_skill_animation(animation, creature_type)),
                on_impact,
                melee_target: melee_target.map(creature_id),
            },
        )),
    }
}

pub fn remove_creature_command(creature: CreatureId) -> api::Command {
    api::Command {
        command: Some(api::command::Command::RemoveCreature(
            api::RemoveCreatureCommand {
                creature_id: Some(creature_id(creature)),
            },
        )),
    }
}

pub fn wait_command(milliseconds: i32) -> api::Command {
    api::Command {
        command: Some(api::command::Command::Wait(api::WaitCommand {
            wait_time_milliseconds: milliseconds,
        })),
    }
}

pub fn empty() -> Result<api::CommandList> {
    Ok(api::CommandList {
        command_groups: vec![],
    })
}

pub fn single(command: api::Command) -> api::CommandGroup {
    api::CommandGroup {
        commands: vec![command],
    }
}

pub fn group(commands: Vec<api::Command>) -> api::CommandGroup {
    api::CommandGroup { commands }
}

pub fn single_group(commands: Vec<api::Command>) -> api::CommandList {
    api::CommandList {
        command_groups: vec![group(commands)],
    }
}

pub fn groups(command_groups: Vec<api::CommandGroup>) -> api::CommandList {
    api::CommandList { command_groups }
}
