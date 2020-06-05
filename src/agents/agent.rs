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

use dyn_clone::DynClone;
use std::fmt::Debug;

use crate::{
    api,
    model::{
        cards::{Card, CardWithTarget},
        creatures::Creature,
        games::Game,
        players::Player,
        primitives::{BoardPosition, CreatureId, FileValue, GamePhase, RankValue},
    },
    rules::engine::RulesEngine,
};

#[derive(Debug, Clone)]
pub enum AgentAction<'a> {
    Pass,
    PlayCard(CardWithTarget<'a>),
}

#[typetag::serde(tag = "type")]
pub trait Agent: Debug + Send + DynClone {
    fn next_action<'a>(&self, game: &'a Game, player: &'a Player) -> Result<AgentAction<'a>>;
}

dyn_clone::clone_trait_object!(Agent);

/// Agent which always unconditionally passes
#[derive(Serialize, Deserialize, Debug, Clone)]
pub struct NullAgent {}

#[typetag::serde]
impl Agent for NullAgent {
    fn next_action<'a>(&self, game: &'a Game, player: &'a Player) -> Result<AgentAction<'a>> {
        Ok(AgentAction::Pass)
    }
}
