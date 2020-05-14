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
    api::{request, CommandGroup, CommandList, PlayCardRequest, Request, StartGameRequest},
    model::types::Game,
    test_data::basic,
};
use std::{collections::HashMap, sync::Mutex};

lazy_static! {
    static ref GAMES: Mutex<HashMap<String, Game>> = Mutex::new(HashMap::new());
}

fn commands(groups: Vec<CommandGroup>) -> CommandList {
    CommandList {
        command_groups: groups,
    }
}

pub fn handle_request(request_message: Request) -> Result<CommandList> {
    let request = request_message.request.ok_or(eyre!("Request not found"))?;
    match request {
        request::Request::StartGame(_) => {
            let (game, commands) = start_game()?;
            let mut games = GAMES.lock().expect("Could not lock game");
            games.insert(String::from("game"), game);
            Ok(commands)
        }
        request::Request::PlayCard(message) => {
            let mut games = GAMES.lock().expect("Could not lock game");
            let mut game = games.remove("game").expect("Game not found");
            let result = play_card(message, &mut game);
            games.insert(String::from("game"), game);
            result
        }
        _ => Ok(commands(vec![])),
    }
}

pub fn start_game() -> Result<(Game, CommandList)> {
    Ok((basic::opening_hands(), commands(vec![])))
}

pub fn play_card(request: PlayCardRequest, game: &mut Game) -> Result<CommandList> {
    Ok(commands(vec![]))
}
