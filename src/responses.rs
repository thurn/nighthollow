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
    model::{
        cards::{Card, CardWithTarget},
        games::Game,
        primitives::{CardId, FileValue, PlayerName, RankValue},
    },
    rules::engine::RulesEngine,
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
        api::request::Request::StartGame(message) => start_game(message),
        api::request::Request::PlayCard(message) => with_engine(|e| play_card(e, message)),
        api::request::Request::ClickMainButton(message) => {
            with_engine(|e| click_main_button(e, message))
        }
        _ => commands::empty(),
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

fn start_game(message: &api::StartGameRequest) -> Result<api::CommandList> {
    match &mut ENGINES.lock() {
        Ok(engines) => {
            let game = scenarios::basic();
            let commands = vec![];
            engines.insert(String::from("game"), RulesEngine::new(game));
            Ok(commands::single_group(commands))
        }
        Err(_) => Err(eyre!("Could not lock mutex")),
    }
}

fn convert_player_name(player_name: api::PlayerName) -> Result<PlayerName> {
    match player_name {
        api::PlayerName::User => Ok(PlayerName::User),
        api::PlayerName::Enemy => Ok(PlayerName::Enemy),
        _ => Err(eyre!("Unrecognized player name: {:?}", player_name)),
    }
}

fn play_card(engine: &mut RulesEngine, message: &api::PlayCardRequest) -> Result<api::CommandList> {
    // Ok(commands::groups(rules::run_game_rule(
    //     game,
    //     convert_player_name(message.player())?,
    //     |r, rc, e| {
    //         let card_id = message
    //             .card_id
    //             .as_ref()
    //             .ok_or_else(|| eyre!("card_id is required"))?
    //             .value;
    //         let play = message
    //             .play_card
    //             .as_ref()
    //             .ok_or_else(|| eyre!("play_card is required"))?;
    //         r.on_play_card_request(
    //             rc,
    //             e,
    //             &card_with_target(rc.game, rc.creature.name, card_id, play)?,
    //         );
    //         Ok(())
    //     },
    // )?))
    commands::empty()
}

fn click_main_button(
    engine: &mut RulesEngine,
    message: &api::MClickMainButtonRequest,
) -> Result<api::CommandList> {
    // Ok(commands::groups(rules::run_game_rule(
    //     game,
    //     convert_player_name(message.player())?,
    //     |r, rc, e| {
    //         r.on_main_button_click_request(rc, e);
    //         Ok(())
    //     },
    // )?))
    commands::empty()
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

fn card_with_target<'a>(
    game: &'a Game,
    player_name: PlayerName,
    card_id: CardId,
    request: &api::play_card_request::PlayCard,
) -> Result<CardWithTarget<'a>> {
    let card = game.player(player_name).card(card_id)?;
    use api::play_card_request::PlayCard::*;
    match (card, request) {
        (Card::Creature(creature), PlayCreature(play_creature)) => Ok(CardWithTarget::Creature(
            creature,
            convert_rank(play_creature.rank_position())?,
            convert_file(play_creature.file_position())?,
        )),
        (Card::Spell(spell), PlayAttachment(play_attachment)) => {
            let creature_id = play_attachment
                .creature_id
                .as_ref()
                .ok_or_else(|| eyre!("creature_id is required"))?
                .value;
            Ok(CardWithTarget::Spell(spell, game.creature(creature_id)?))
        }
        (Card::Scroll(scroll), PlayUntargeted(_)) => Ok(CardWithTarget::Scroll(scroll)),
        _ => Err(eyre!("Card did not match targeting type")),
    }
}
