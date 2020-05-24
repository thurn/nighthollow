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
    model::cards::{Card, HasCardData},
    model::creatures::Creature,
    model::games::{Game, Player},
    model::primitives::{BoardPosition, CardId, FileValue, GamePhase, PlayerName, RankValue},
    rules::combat,
    test_data::{scenarios, standard},
};
use api::{
    CreatureId, MDebugDrawCardsRequest, MDebugLoadScenarioRequest, MDebugRunRequestSequenceRequest,
};
use commands::{CardMetadata, CreatureMetadata};
use std::{collections::HashMap, sync::Mutex};

lazy_static! {
    static ref GAMES: Mutex<HashMap<String, Game>> = Mutex::new(HashMap::new());
}

pub fn handle_request(request_message: api::Request) -> Result<api::CommandList> {
    let request = request_message.request.ok_or(eyre!("Request not found"))?;
    match request {
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
        revealed,
        can_play: is_user,
        creature: CreatureMetadata {
            can_resposition_creature: is_user,
        },
    }
}

pub fn start_game() -> Result<(Game, api::CommandList)> {
    let game = standard::opening_hands();
    let user = game
        .user
        .hand
        .iter()
        .map(|c| commands::draw_card_command(c, metadata(c, true)));
    let enemy = game
        .enemy
        .hand
        .iter()
        .map(|c| commands::draw_card_command(c, metadata(c, false)));

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
    let mut result = vec![];
    let player_name = convert_player_name(request.player())?;

    let card_id = request
        .card_id
        .as_ref()
        .ok_or(eyre!("card_id is required"))?
        .value;
    let play_card = request.play_card.ok_or(eyre!("play_card is required!"))?;
    let card = remove_card(game, player_name, card_id)?;
    let card_data = commands::card_data(&card, metadata(&card, true));
    let player = game.player_mut(player_name);
    let mut push_group = |command| result.push(commands::single(command));

    match (card, play_card) {
        (Card::Creature(c), api::play_card_request::PlayCard::PlayCreature(play)) => {
            let rank = convert_rank(play.rank_position())?;
            let file = convert_file(play.file_position())?;
            let creature = Creature {
                data: c,
                position: BoardPosition { rank, file },
                is_alive: true,
                spells: vec![],
            };

            if player_name == PlayerName::User {
                push_group(commands::destroy_card_command(player_name, card_id, false));
            } else {
                push_group(commands::reveal_card_command(
                    card_data,
                    1000,
                    Some(rank),
                    Some(file),
                ));
            }

            push_group(update_creature(&creature));
            player.creatures.push(creature);
        }
        (Card::Spell(s), api::play_card_request::PlayCard::PlayAttachment(play)) => {
            let creature = find_creature_mut(
                play.creature_id.ok_or(eyre!("creature_id is required"))?,
                &mut player.creatures,
            )?;
            creature.spells.push(s);

            if player_name == PlayerName::User {
                push_group(commands::destroy_card_command(player_name, card_id, false));
            } else {
                push_group(commands::reveal_card_command(
                    card_data,
                    1000,
                    Some(creature.position.rank),
                    Some(creature.position.file),
                ));
            }

            push_group(update_creature(&creature));
        }
        (Card::Scroll(s), api::play_card_request::PlayCard::PlayUntargeted(_)) => {
            player.scrolls.push(s);
            if player_name == PlayerName::User {
                push_group(commands::destroy_card_command(player_name, card_id, false));
            } else {
                push_group(commands::reveal_card_command(card_data, 500, None, None));
            }
            push_group(commands::update_player_command(player));
        }
        _ => return Err(eyre!("Target type does not match card type!")),
    }

    Ok(api::CommandList {
        command_groups: result,
    })
}

fn update_creature(creature: &Creature) -> api::Command {
    commands::create_or_update_creature_command(
        creature,
        CreatureMetadata {
            can_resposition_creature: creature.card_data().owner == PlayerName::User,
        },
    )
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

    Ok(commands::group(vec![
        upkeep(PlayerName::User, &mut game.user),
        upkeep(PlayerName::Enemy, &mut game.enemy),
    ]))
}

pub fn load_scenario(request: MDebugLoadScenarioRequest) -> Result<api::CommandList> {
    let game = scenarios::load_scenario(&request.scenario_name)?;

    match &mut GAMES.lock() {
        Ok(games) => {
            games.insert(String::from("game"), game);
            commands::empty()
        }
        Err(_) => Err(eyre!("Could not lock mutex")),
    }
}

pub fn debug_draw_cards(
    game: &mut Game,
    request: MDebugDrawCardsRequest,
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

    Ok(commands::group(commands))
}

fn draw_card_at_index(
    player: &mut Player,
    index: u32,
    commands: &mut Vec<api::Command>,
    revealed: bool,
) -> Result<()> {
    let card = player.deck.draw_card_at_index(index as usize)?;
    commands.push(commands::draw_card_command(
        &card,
        metadata(&card, revealed),
    ));
    player.add_to_hand(card);
    Ok(())
}

pub fn run_request_sequence(message: MDebugRunRequestSequenceRequest) -> Result<api::CommandList> {
    let mut result = vec![];
    for request in message.requests {
        result.extend(handle_request(request)?.command_groups);
    }

    Ok(api::CommandList {
        command_groups: result,
    })
}
