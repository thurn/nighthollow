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
    model::primitives::PlayerName,
    model::types::{Card, Game, HasCardData},
    test_data::basic,
};
use commands::CardMetadata;
use std::{collections::HashMap, sync::Mutex};

lazy_static! {
    static ref GAMES: Mutex<HashMap<String, Game>> = Mutex::new(HashMap::new());
}

fn commands(groups: Vec<api::CommandGroup>) -> api::CommandList {
    api::CommandList {
        command_groups: groups,
    }
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
        api::request::Request::PlayCard(message) => {
            let mut games = GAMES.lock().expect("Could not lock game");
            let mut game = games.remove("game").expect("Game not found");
            let result = play_card(message, &mut game);
            games.insert(String::from("game"), game);
            result
        }
        _ => Ok(commands(vec![])),
    }
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

pub fn play_card(request: api::PlayCardRequest, game: &mut Game) -> Result<api::CommandList> {
    Ok(commands(vec![]))
}
