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
    api,
    commands::{self, CreatureMetadata},
    model::{
        creatures::Creature,
        primitives::{BoardPosition, FileValue, PlayerName, RankValue},
        types::{Card, Game, HasCardData, Player},
    },
    requests,
};

pub fn run_console_command(
    request: api::RunConsoleCommandRequest,
    game: &mut Game,
) -> Result<api::CommandList> {
    let mut result: Vec<api::CommandGroup> = Vec::new();
    result.extend(play_first_card(
        &mut game.enemy,
        &CardTarget::Position(BoardPosition::new(RankValue::Rank3, FileValue::File2)),
    )?);
    result.extend(play_first_card(
        &mut game.enemy,
        &CardTarget::Position(BoardPosition::new(RankValue::Rank4, FileValue::File2)),
    )?);
    result.extend(play_first_card(
        &mut game.enemy,
        &CardTarget::Position(BoardPosition::new(RankValue::Rank5, FileValue::File2)),
    )?);
    Ok(api::CommandList {
        command_groups: result,
    })
}

enum CardTarget {
    Position(BoardPosition),
    None,
}

impl CardTarget {
    fn position(&self) -> Result<&BoardPosition> {
        match self {
            CardTarget::Position(p) => Ok(p),
            _ => Err(eyre!("Expected position target type")),
        }
    }
}

fn play_first_card(player: &mut Player, target: &CardTarget) -> Result<Vec<api::CommandGroup>> {
    let mut result: Vec<api::CommandGroup> = Vec::new();
    let card_id = player
        .hand
        .get(0)
        .ok_or(eyre!("Card not found"))?
        .card_data()
        .id;
    {
        let card = &player.hand.get(0).ok_or(eyre!("Card not found"))?;
        if let Card::Creature(c) = card {
            let position = target.position()?;

            result.push(api::CommandGroup {
                commands: vec![commands::play_card_command(
                    card,
                    &requests::metadata(card, true),
                    2000,
                    Some(position.rank),
                    Some(position.file),
                )],
            });

            let request = api::PlayCardRequest {
                game_id: None,
                card_id: Some(commands::card_id(card.card_data().id)),
                play_card: Some(api::play_card_request::PlayCard::PlayCreature(
                    api::PlayCreatureCard {
                        rank_position: commands::rank_value(position.rank).into(),
                        file_position: commands::file_value(position.file).into(),
                    },
                )),
            };

            result.extend(requests::play_card(request, player)?.command_groups);
        }
    }

    if let Ok(creature) = requests::find_creature(commands::creature_id(card_id), &player.creatures)
    {
        result.push(api::CommandGroup {
            commands: vec![commands::create_or_update_creature_command(
                creature,
                &CreatureMetadata {
                    id: card_id,
                    can_resposition_creature: false,
                },
            )],
        });
    }

    Ok(result)
}
