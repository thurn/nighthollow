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

use std::time::Instant;

use eyre::eyre;
use eyre::Result;

use crate::{
    api, commands,
    model::cards::{Card, HasCardData, HasCardState},
    model::creatures::{Creature, CreatureState},
    model::games::{Game, Player},
    model::primitives::{
        BoardPosition, CardId, CreatureId, FileValue, GamePhase, PlayerName, RankValue,
    },
    rules::combat,
    test_data::scenarios,
};
use api::{
    MClickMainButtonRequest, MDebugDrawCardsRequest, MDebugLoadScenarioRequest,
    MDebugRunRequestSequenceRequest,
};
use std::{collections::HashMap, sync::Mutex};

lazy_static! {
    static ref GAMES: Mutex<HashMap<String, Game>> = Mutex::new(HashMap::new());
}

pub fn handle_request(request_message: &api::Request) -> Result<api::CommandList> {
    let now = Instant::now();
    let request = request_message
        .request
        .as_ref()
        .ok_or(eyre!("Request not found"))?;
    let got_request = now.elapsed().as_secs_f64();
    let start = Instant::now();

    let result = match request {
        api::request::Request::RunRequestSequence(message) => run_request_sequence(message),
        api::request::Request::StartGame(_) => match &mut GAMES.lock() {
            Ok(games) => {
                let (game, commands) = start_game()?;
                games.insert(String::from("game"), game);
                Ok(commands)
            }
            Err(_) => Err(eyre!("Could not lock mutex")),
        },
        api::request::Request::LoadScenario(message) => load_scenario(message),
        api::request::Request::DrawCards(message) => {
            with_game(|game| debug_draw_cards(game, message))
        }
        api::request::Request::PlayCard(message) => with_game(|game| play_card(message, game)),
        api::request::Request::ClickMainButton(message) => {
            with_game(|game| click_main_button(message, game))
        }
        _ => commands::empty(),
    };

    println!(
        "Request {:?} completed in {:.3} seconds (got req in {:.3} seconds)",
        commands::request_name(request_message),
        start.elapsed().as_secs_f64(),
        got_request
    );
    result
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

fn draw_card(player: &mut Player, result: &mut Vec<api::Command>) -> Result<()> {
    let card = player.draw_card()?;
    result.push(commands::draw_or_update_card_command(&card));
    Ok(())
}

pub fn start_game() -> Result<(Game, api::CommandList)> {
    let mut game = scenarios::basic();
    let mut list = vec![next_turn_button(true)];

    for i in 0..6 {
        draw_card(&mut game.user, &mut list)?;
        draw_card(&mut game.enemy, &mut list)?;
    }

    let list = api::CommandList {
        command_groups: vec![
            commands::single(commands::initiate_game_command(&game)),
            commands::group(list),
        ],
    };

    Ok((game, list))
}

fn convert_rank(rank: api::RankValue) -> Result<RankValue> {
    match rank {
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
        api::FileValue::File1 => Ok(FileValue::File1),
        api::FileValue::File2 => Ok(FileValue::File2),
        api::FileValue::File3 => Ok(FileValue::File3),
        api::FileValue::File4 => Ok(FileValue::File4),
        api::FileValue::File5 => Ok(FileValue::File5),
        _ => Err(eyre!("Invalid file: {:?}", file)),
    }
}

fn convert_player_name(player_name: api::PlayerName) -> Result<PlayerName> {
    match player_name {
        api::PlayerName::User => Ok(PlayerName::User),
        api::PlayerName::Enemy => Ok(PlayerName::Enemy),
        _ => Err(eyre!("Unrecognized player name: {:?}", player_name)),
    }
}

fn find_card(game: &Game, player: PlayerName, card_id: CardId) -> Result<usize> {
    game.player(player)
        .hand
        .iter()
        .position(|c| c.card_data().id == card_id)
        .ok_or(eyre!("Card ID not found: {:?}", card_id))
}

fn remove_card(game: &mut Game, player: PlayerName, card_id: CardId) -> Result<Card> {
    let position = find_card(game, player, card_id)?;
    Ok(game.player_mut(player).hand.remove(position))
}

pub fn find_creature(creature_id: api::CreatureId, creatures: &Vec<Creature>) -> Result<&Creature> {
    let result = creatures
        .iter()
        .find(|c| c.card_data().id == creature_id.value)
        .ok_or(eyre!("Creature ID not found: {:?}", creature_id))?;
    Ok(result)
}

pub fn find_creature_mut(
    creature_id: CreatureId,
    creatures: &mut Vec<Creature>,
) -> Result<&mut Creature> {
    let result = creatures
        .iter_mut()
        .find(|c| c.card_data().id == creature_id)
        .ok_or(eyre!("Creature ID not found: {:?}", creature_id))?;
    Ok(result)
}

pub fn play_card(request: &api::PlayCardRequest, game: &mut Game) -> Result<api::CommandList> {
    let mut result = vec![];
    let player_name = convert_player_name(request.player())?;

    let card_id = request
        .card_id
        .as_ref()
        .ok_or(eyre!("card_id is required"))?
        .value;
    let play_card = request
        .play_card
        .as_ref()
        .ok_or(eyre!("play_card is required!"))?;
    let mut card = remove_card(game, player_name, card_id)?;
    card.card_state_mut().revealed_to_opponent = true;

    let card_data = commands::card_data(&card);
    let player = game.player_mut(player_name);
    let cost = card.card_data().cost.clone();

    match (card, play_card) {
        (Card::Creature(c), api::play_card_request::PlayCard::PlayCreature(play)) => {
            let rank = convert_rank(play.rank_position())?;
            let file = convert_file(play.file_position())?;
            let creature = Creature {
                data: c,
                position: BoardPosition { rank, file },
                spells: vec![],
                state: CreatureState::default(),
            };

            if player_name == PlayerName::User {
                result.push(commands::single(commands::destroy_card_command(
                    player_name,
                    card_id,
                    false,
                )));
            } else {
                result.push(commands::single(commands::reveal_card_command(
                    card_data,
                    1000,
                    Some(rank),
                    Some(file),
                )));
            }

            result.push(commands::single(update_creature(&creature)));
            player.creatures.push(creature);
            player.pay_cost(&cost, &mut result)?;
        }
        (Card::Spell(s), api::play_card_request::PlayCard::PlayAttachment(play)) => {
            let creature = find_creature_mut(
                play.creature_id
                    .as_ref()
                    .ok_or(eyre!("creature_id is required"))?
                    .value,
                &mut player.creatures,
            )?;
            creature.spells.push(s);

            if player_name == PlayerName::User {
                result.push(commands::single(commands::destroy_card_command(
                    player_name,
                    card_id,
                    false,
                )));
            } else {
                result.push(commands::single(commands::reveal_card_command(
                    card_data,
                    1000,
                    Some(creature.position.rank),
                    Some(creature.position.file),
                )));
            }

            result.push(commands::single(update_creature(&creature)));
            player.pay_cost(&cost, &mut result)?;
        }
        (Card::Scroll(s), api::play_card_request::PlayCard::PlayUntargeted(_)) => {
            if player_name == PlayerName::User {
                result.push(commands::single(commands::destroy_card_command(
                    player_name,
                    card_id,
                    false,
                )));
            } else {
                result.push(commands::single(commands::reveal_card_command(
                    card_data, 500, None, None,
                )));
            }
            player.add_scroll(s, &mut result);
        }
        _ => return Err(eyre!("Target type does not match card type!")),
    }

    Ok(api::CommandList {
        command_groups: result,
    })
}

pub fn update_creature(creature: &Creature) -> api::Command {
    commands::create_or_update_creature_command(creature)
}

pub fn click_main_button(
    message: &MClickMainButtonRequest,
    game: &mut Game,
) -> Result<api::CommandList> {
    match game.state.phase {
        GamePhase::Preparation => to_main_phase(game),
        GamePhase::Main => to_preparation_phase(game),
    }
}

fn to_main_phase(game: &mut Game) -> Result<api::CommandList> {
    game.state.phase = GamePhase::Main;
    let mut combat = combat::run_combat(game)?;
    combat.insert(0, commands::single(next_turn_button(false)));
    combat.push(commands::single(next_turn_button(true)));
    Ok(commands::groups(combat))
}

fn to_preparation_phase(game: &mut Game) -> Result<api::CommandList> {
    game.state.phase = GamePhase::Preparation;
    let mut result = vec![commands::single(combat_button())];

    game.user.upkeep(&mut result)?;
    game.enemy.upkeep(&mut result)?;

    Ok(commands::groups(result))
}

pub fn load_scenario(request: &MDebugLoadScenarioRequest) -> Result<api::CommandList> {
    let game = scenarios::load_scenario(&request.scenario_name)?;

    match &mut GAMES.lock() {
        Ok(games) => {
            let command = commands::initiate_game_command(&game);
            games.insert(String::from("game"), game);
            Ok(commands::single_group(vec![
                command,
                next_turn_button(true),
            ]))
        }
        Err(_) => Err(eyre!("Could not lock mutex")),
    }
}

pub fn debug_draw_cards(
    game: &mut Game,
    request: &MDebugDrawCardsRequest,
) -> Result<api::CommandList> {
    let mut commands = vec![];

    request
        .draw_user_cards
        .iter()
        .try_for_each(|i| draw_card_at_index(&mut game.user, *i, &mut commands, true))?;
    request
        .draw_enemy_cards
        .iter()
        .try_for_each(|i| draw_card_at_index(&mut game.enemy, *i, &mut commands, false))?;

    Ok(commands::single_group(commands))
}

fn draw_card_at_index(
    player: &mut Player,
    index: u32,
    commands: &mut Vec<api::Command>,
    revealed: bool,
) -> Result<()> {
    let card = player.draw_card_at_index(index as usize)?;
    commands.push(commands::draw_or_update_card_command(&card));
    Ok(())
}

pub fn run_request_sequence(message: &MDebugRunRequestSequenceRequest) -> Result<api::CommandList> {
    let start = Instant::now();
    let mut result = vec![];

    for request in message.requests.iter() {
        result.extend(handle_request(request)?.command_groups);
    }

    let result = api::CommandList {
        command_groups: result,
    };

    println!(
        "All requests completed in {:.3} seconds",
        start.elapsed().as_secs_f64()
    );

    Ok(result)
}

fn next_turn_button(enabled: bool) -> api::Command {
    commands::update_interface_state_command(enabled, String::from("Next Turn"), 1)
}

fn combat_button() -> api::Command {
    commands::update_interface_state_command(true, String::from("Combat"), 1)
}
