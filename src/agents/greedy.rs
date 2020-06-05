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

use crate::prelude::*;

use super::agent::{Agent, AgentAction};
use crate::{
    api,
    model::{
        cards::{Card, CardWithTarget, Cost, HasCardData},
        creatures::Creature,
        games::Game,
        players::Player,
        primitives::{BoardPosition, FileValue, GamePhase, RankValue, RANKS},
    },
    rules::engine::RulesEngine,
};
use std::cmp::Ordering;

/// Agent which plays as many cards as possible each turn, starting with the
/// most expensive
#[derive(Serialize, Deserialize, Debug, Clone)]
pub struct GreedyAgent {}

#[typetag::serde]
impl Agent for GreedyAgent {
    fn next_action<'a>(&self, game: &'a Game, player: &'a Player) -> Result<AgentAction<'a>> {
        match game.state.phase {
            GamePhase::Preparation => Ok(AgentAction::Pass),
            GamePhase::Main => {
                if let Some(card) = select_card(game, player) {
                    Ok(AgentAction::PlayCard(select_target(game, player, card)?))
                } else {
                    Ok(AgentAction::Pass)
                }
            }
        }
    }
}

fn select_card<'a>(game: &'a Game, player: &'a Player) -> Option<&'a Card> {
    player
        .hand
        .iter()
        .filter(|c| c.can_play())
        .max_by_key(|c| scalar_cost(&c.card_data().cost))
}

fn scalar_cost(cost: &Cost) -> u32 {
    match cost {
        Cost::ScrollPlay => 100,
        Cost::StandardCost(s) => s.power + s.influence.total(),
    }
}

fn select_target<'a>(
    game: &'a Game,
    player: &'a Player,
    card: &'a Card,
) -> Result<CardWithTarget<'a>> {
    Ok(match card {
        Card::Creature(c) => CardWithTarget::Creature(c, select_creature_position(player)?),
        Card::Spell(s) => CardWithTarget::Spell(s, select_spell_target(player)?),
        Card::Scroll(s) => CardWithTarget::Scroll(s),
    })
}

fn select_creature_position(player: &Player) -> Result<BoardPosition> {
    for rank in RANKS.iter() {
        for file in &[
            FileValue::File3,
            FileValue::File2,
            FileValue::File4,
            FileValue::File1,
            FileValue::File5,
        ] {
            let position = BoardPosition {
                rank: *rank,
                file: *file,
            };
            if player.creature_position_available(position) {
                return Ok(position);
            }
        }
    }

    Err(eyre!("Board is full"))
}

fn select_spell_target(player: &Player) -> Result<&Creature> {
    player
        .creatures
        .iter()
        .min_by_key(|c| c.spells.len())
        .ok_or_else(|| eyre!("Player has no creatures!"))
}
