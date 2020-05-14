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
    api, commands,
    model::primitives::{BoardPosition, FileValue, PlayerName, RankValue},
    model::types::{Card, Creature, Game, HasCardData},
    test_data::basic,
};
use commands::CardMetadata;
use std::{collections::HashMap, sync::Mutex};

lazy_static! {
    static ref GAMES: Mutex<HashMap<String, Game>> = Mutex::new(HashMap::new());
}

pub fn handle_request(request_message: api::Request) -> Result<api::CommandList> {
    let request = request_message.request.ok_or(eyre!("Request not found"))?;
    match request {
        api::request::Request::StartGame(_) => {
            let (game, commands) = start_game()?;
            let mut games = GAMES.lock().expect("Could not lock game");
            games.insert(String::from("game"), game);
            Ok(commands)
        }
        api::request::Request::PlayCard(message) => with_game(|game| play_card(message, game)),
        _ => Ok(commands::empty()),
    }
}

fn with_game(
    function: impl FnOnce(&mut Game) -> Result<api::CommandList>,
) -> Result<api::CommandList> {
    let mut games = GAMES.lock().expect("Could not lock game");
    let mut game = games.remove("game").expect("Game not found");
    let result = function(&mut game);
    games.insert(String::from("game"), game);
    result
}

fn metadata(card: &Card, is_user: bool) -> api::DrawCardCommand {
    commands::draw_card(
        card,
        &CardMetadata {
            owner: if is_user {
                PlayerName::User
            } else {
                PlayerName::Enemy
            },
            id: card.card_data().id,
            revealed: is_user,
            can_play: is_user,
            can_resposition_creature: is_user,
        },
    )
}

pub fn start_game() -> Result<(Game, api::CommandList)> {
    let game = basic::opening_hands();
    let user = game.user.hand.iter().map(|c| metadata(c, true));
    let enemy = game.enemy.hand.iter().map(|c| metadata(c, false));

    let list = api::CommandList {
        command_groups: vec![api::CommandGroup {
            commands: user
                .chain(enemy)
                .map(|c| api::Command {
                    command: Some(api::command::Command::DrawCard(c)),
                })
                .collect::<Vec<_>>(),
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

fn find_creature_mut(
    creature_id: api::CreatureId,
    creatures: &mut Vec<Creature>,
) -> Result<&mut Creature> {
    let result = creatures
        .iter_mut()
        .find(|c| c.card_data().id == creature_id.value)
        .ok_or(eyre!("Creature ID not found: {:?}", creature_id))?;
    Ok(result)
}

pub fn play_card(request: api::PlayCardRequest, game: &mut Game) -> Result<api::CommandList> {
    let card_id = request.card_id.ok_or(eyre!("card_id is required"))?;
    let play_card = request.play_card.ok_or(eyre!("play_card is required!"))?;
    let card = remove_card(card_id, &mut game.user.hand)?;
    match (card, play_card) {
        (Card::Creature(c), api::play_card_request::PlayCard::PlayCreature(play)) => {
            game.user.creatures.push(Creature {
                archetype: c,
                position: BoardPosition {
                    rank: convert_rank(play.rank_position())?,
                    file: convert_file(play.file_position())?,
                },
                spells: vec![],
            });
            Ok(commands::empty())
        }
        (Card::Spell(s), api::play_card_request::PlayCard::PlayAttachment(play)) => {
            let creature = find_creature_mut(
                play.creature_id.ok_or(eyre!("creature_id is required"))?,
                &mut game.user.creatures,
            )?;
            creature.spells.push(s);
            Ok(commands::empty())
        }
        (Card::Scroll(s), api::play_card_request::PlayCard::PlayUntargeted(_)) => {
            game.user.scrolls.push(s);
            Ok(commands::empty())
        }
        _ => Err(eyre!("Target type does not match card type!")),
    }
}
