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
use eyre::{eyre};

use crate::{
    api, commands, console,
    model::cards::{Card, HasCardData},
    model::creatures::Creature,
    model::games::{Game, Player},
    model::primitives::{BoardPosition, FileValue, GamePhase, PlayerName, RankValue},
    rules::combat,
    test_data::scenarios,
};
use commands::{CardMetadata, CreatureMetadata};
use std::{collections::HashMap, sync::Mutex};

lazy_static! {
    static ref GAMES: Mutex<HashMap<String, Game>> = Mutex::new(HashMap::new());
}

pub fn handle_request(request_message: api::Request) -> Result<api::CommandList> {
    let request = request_message.request.ok_or(eyre!("Request not found"))?;
    match request {
        api::request::Request::RunConsoleCommand(message) => {
            with_game(|game| console::run_console_command(message, game))
        }
        api::request::Request::StartGame(_) => match &mut GAMES.lock() {
            Ok(games) => {
                let (game, commands) = start_game()?;
                games.insert(String::from("game"), game);
                Ok(commands)
            }
            Err(_) => Err(eyre!("Could not lock mutex")),
        },
        api::request::Request::PlayCard(message) => {
            with_game(|game| play_card(message, &mut game.user))
        }
        api::request::Request::AdvancePhase(_) => with_game(|game| advance_game_phase(game)),
        _ => commands::empty(),
    }
}

fn with_game(
    function: impl FnOnce(&mut Game) -> Result<api::CommandList>,
) -> Result<api::CommandList> {
    match &mut GAMES.lock() {
        Ok(games) => {
            let mut game = games.remove("game").ok_or(eyre!("Game not found"))?;
            let result = function(&mut game);
            games.insert(String::from("game"), game);
            result
        }
        Err(_) => Err(eyre!("Could not lock mutex")),
    }
}

pub fn metadata(card: &Card, revealed: bool) -> CardMetadata {
    let is_user = card.card_data().owner == PlayerName::User;
    CardMetadata {
        id: card.card_data().id,
        revealed,
        can_play: is_user,
        creature: CreatureMetadata {
            id: card.card_data().id,
            can_resposition_creature: is_user,
        },
    }
}

pub fn start_game() -> Result<(Game, api::CommandList)> {
    let game = scenarios::opening_hands();
    let user = game
        .user
        .hand
        .iter()
        .map(|c| commands::draw_card_command(c, &metadata(c, true)));
    let enemy = game
        .enemy
        .hand
        .iter()
        .map(|c| commands::draw_card_command(c, &metadata(c, false)));

    let list = api::CommandList {
        command_groups: vec![api::CommandGroup {
            commands: user.chain(enemy).collect::<Vec<_>>(),
        }],
    };

    Ok((game, list))
}

fn convert_rank(rank: api::RankValue) -> Result<RankValue> {
    match rank {
        api::RankValue::Rank0 => Ok(RankValue::Rank0),
        api::RankValue::Rank1 => Ok(RankValue::Rank1),
        api::RankValue::Rank2 => Ok(RankValue::Rank2),
        api::RankValue::Rank3 => Ok(RankValue::Rank3),
        api::RankValue::Rank4 => Ok(RankValue::Rank4),
        api::RankValue::Rank5 => Ok(RankValue::Rank5),
        _ => Err(eyre!("Invalid rank: {:?}", rank)),
    }
}

fn convert_file(file: api::FileValue) -> Result<FileValue> {
    match file {
        api::FileValue::File0 => Ok(FileValue::File0),
        api::FileValue::File1 => Ok(FileValue::File1),
        api::FileValue::File2 => Ok(FileValue::File2),
        api::FileValue::File3 => Ok(FileValue::File3),
        api::FileValue::File4 => Ok(FileValue::File4),
        api::FileValue::File5 => Ok(FileValue::File5),
        _ => Err(eyre!("Invalid file: {:?}", file)),
    }
}

fn find_card(card_id: api::CardId, hand: &Vec<Card>) -> Result<usize> {
    hand.iter()
        .position(|c| c.card_data().id == card_id.value)
        .ok_or(eyre!("Card ID not found: {:?}", card_id))
}

fn remove_card(card_id: api::CardId, hand: &mut Vec<Card>) -> Result<Card> {
    let position = find_card(card_id, hand)?;
    Ok(hand.remove(position))
}

pub fn find_creature(creature_id: api::CreatureId, creatures: &Vec<Creature>) -> Result<&Creature> {
    let result = creatures
        .iter()
        .find(|c| c.card_data().id == creature_id.value)
        .ok_or(eyre!("Creature ID not found: {:?}", creature_id))?;
    Ok(result)
}

pub fn find_creature_mut(
    creature_id: api::CreatureId,
    creatures: &mut Vec<Creature>,
) -> Result<&mut Creature> {
    let result = creatures
        .iter_mut()
        .find(|c| c.card_data().id == creature_id.value)
        .ok_or(eyre!("Creature ID not found: {:?}", creature_id))?;
    Ok(result)
}

pub fn play_card(request: api::PlayCardRequest, player: &mut Player) -> Result<api::CommandList> {
    let card_id = request.card_id.ok_or(eyre!("card_id is required"))?;
    let play_card = request.play_card.ok_or(eyre!("play_card is required!"))?;
    let card = remove_card(card_id, &mut player.hand)?;
    match (card, play_card) {
        (Card::Creature(c), api::play_card_request::PlayCard::PlayCreature(play)) => {
            player.creatures.push(Creature {
                data: c,
                position: BoardPosition {
                    rank: convert_rank(play.rank_position())?,
                    file: convert_file(play.file_position())?,
                },
                is_alive: true,
                spells: vec![],
            });
            commands::empty()
        }
        (Card::Spell(s), api::play_card_request::PlayCard::PlayAttachment(play)) => {
            let creature = find_creature_mut(
                play.creature_id.ok_or(eyre!("creature_id is required"))?,
                &mut player.creatures,
            )?;
            creature.spells.push(s);
            commands::empty()
        }
        (Card::Scroll(s), api::play_card_request::PlayCard::PlayUntargeted(_)) => {
            player.scrolls.push(s);
            commands::empty()
        }
        _ => Err(eyre!("Target type does not match card type!")),
    }
}

pub fn advance_game_phase(game: &mut Game) -> Result<api::CommandList> {
    match game.state.phase {
        GamePhase::Preparation => to_main_phase(game),
        GamePhase::Main => to_preparation_phase(game),
    }
}

fn to_main_phase(game: &mut Game) -> Result<api::CommandList> {
    game.state.phase = GamePhase::Main;
    combat::run_combat(game)
}

fn upkeep(_player_name: PlayerName, player: &mut Player) -> api::Command {
    player.state.current_mana = player.state.maximum_mana;
    player
        .state
        .current_influence
        .set_to(&player.state.maximum_influence);
    commands::update_player_command(player)
}

fn to_preparation_phase(game: &mut Game) -> Result<api::CommandList> {
    game.state.phase = GamePhase::Preparation;

    commands::group(vec![
        upkeep(PlayerName::User, &mut game.user),
        upkeep(PlayerName::Enemy, &mut game.enemy),
    ])
}
