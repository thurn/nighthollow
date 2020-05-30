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
    api, commands,
    gameplay::{debug, play_card},
    model::{
        games::Game,
        primitives::{CardId, FileValue, PlayerName, RankValue},
    },
    rules::engine::{RulesEngine, Trigger},
    test_data::scenarios,
};
use eyre::eyre;
use eyre::Result;
use std::{collections::HashMap, sync::Mutex};

lazy_static! {
    static ref ENGINES: Mutex<HashMap<String, RulesEngine>> = Mutex::new(HashMap::new());
}

pub fn handle_request(request_message: &api::Request) -> Result<api::CommandList> {
    let request = request_message
        .request
        .as_ref()
        .ok_or_else(|| eyre!("Request is required"))?;

    match request {
        api::request::Request::StartGame(message) => on_start_game(message),
        api::request::Request::Debug(message) => on_debug(message),
        api::request::Request::PlayCard(message) => {
            with_engine(|e| play_card::on_play_card_request(e, message))
        }
        _ => commands::empty(),
    }
}

pub fn convert_player_name(player_name: api::PlayerName) -> Result<PlayerName> {
    match player_name {
        api::PlayerName::User => Ok(PlayerName::User),
        api::PlayerName::Enemy => Ok(PlayerName::Enemy),
        _ => Err(eyre!("Unrecognized player name: {:?}", player_name)),
    }
}

pub fn convert_rank(rank: api::RankValue) -> Result<RankValue> {
    match rank {
        api::RankValue::Rank1 => Ok(RankValue::Rank1),
        api::RankValue::Rank2 => Ok(RankValue::Rank2),
        api::RankValue::Rank3 => Ok(RankValue::Rank3),
        api::RankValue::Rank4 => Ok(RankValue::Rank4),
        api::RankValue::Rank5 => Ok(RankValue::Rank5),
        _ => Err(eyre!("Invalid rank: {:?}", rank)),
    }
}

pub fn convert_file(file: api::FileValue) -> Result<FileValue> {
    match file {
        api::FileValue::File1 => Ok(FileValue::File1),
        api::FileValue::File2 => Ok(FileValue::File2),
        api::FileValue::File3 => Ok(FileValue::File3),
        api::FileValue::File4 => Ok(FileValue::File4),
        api::FileValue::File5 => Ok(FileValue::File5),
        _ => Err(eyre!("Invalid file: {:?}", file)),
    }
}

fn with_engine(
    function: impl FnOnce(&mut RulesEngine) -> Result<api::CommandList>,
) -> Result<api::CommandList> {
    match &mut ENGINES.lock() {
        Ok(engines) => {
            // Clone the engine and mutate it so that on error the previous value
            // will be preserved.
            let mut engine = engines
                .get("game")
                .ok_or_else(|| eyre!("Game not found"))?
                .clone();
            let result = function(&mut engine)?;
            engines.insert(String::from("game"), engine);
            Ok(result)
        }
        Err(_) => Err(eyre!("Could not lock mutex")),
    }
}

fn on_start_game(message: &api::StartGameRequest) -> Result<api::CommandList> {
    initialize(scenarios::basic(), |engine, result| {
        engine.invoke_as_group(result, Trigger::GameStart)
    })
}

fn on_debug(message: &api::MDebugRequest) -> Result<api::CommandList> {
    initialize(scenarios::basic(), |engine, result| {
        debug::on_debug_request(engine, message, result)
    })
}

fn initialize(
    game: Game,
    function: impl FnOnce(&mut RulesEngine, &mut Vec<api::CommandGroup>) -> Result<()>,
) -> Result<api::CommandList> {
    match &mut ENGINES.lock() {
        Ok(engines) => {
            let game = scenarios::basic();
            let mut commands = vec![commands::group(vec![
                commands::initiate_game_command(game.id),
                commands::update_player_command(&game.user),
                commands::update_player_command(&game.enemy),
            ])];
            let mut engine = RulesEngine::new(game);
            function(&mut engine, &mut commands)?;
            engines.insert(String::from("game"), engine);
            Ok(commands::groups(commands))
        }
        Err(_) => Err(eyre!("Could not lock mutex")),
    }
}
