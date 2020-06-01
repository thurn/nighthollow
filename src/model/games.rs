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

use super::{
    cards::{Card, Cost, Deck, HasCardData, HasCardId, HasCardState, Scroll},
    creatures::{Creature, CreatureData, HasCreatureData},
    players::Player,
    stats::{Stat, StatName, Tag, TagName},
};
use crate::{
    api, commands,
    model::primitives::*,
    rules::engine::{Rule, RulesEngine},
};

#[derive(Serialize, Deserialize, Debug, Clone)]
pub enum MainButtonState {
    ToCombat,
    EndTurn,
}

#[derive(Serialize, Deserialize, Debug, Clone)]
pub struct GameState {
    pub phase: GamePhase,
    pub turn: TurnNumber,
    pub main_button: MainButtonState,
}

#[derive(Serialize, Deserialize, Debug, Clone)]
pub struct Game {
    pub id: GameId,
    pub state: GameState,
    pub user: Player,
    pub enemy: Player,
}

impl Game {
    pub fn all_creatures(&self) -> impl Iterator<Item = &Creature> {
        self.user
            .creatures
            .iter()
            .chain(self.enemy.creatures.iter())
    }

    pub fn all_creatures_mut(&mut self) -> impl Iterator<Item = &mut Creature> {
        self.user
            .creatures
            .iter_mut()
            .chain(self.enemy.creatures.iter_mut())
    }

    pub fn player(&self, player: PlayerName) -> &Player {
        match player {
            PlayerName::User => &self.user,
            PlayerName::Enemy => &self.enemy,
        }
    }

    pub fn player_mut(&mut self, player: PlayerName) -> &mut Player {
        match player {
            PlayerName::User => &mut self.user,
            PlayerName::Enemy => &mut self.enemy,
        }
    }

    pub fn creature(&self, creature_id: CreatureId) -> Result<&Creature> {
        self.all_creatures()
            .find(|c| c.creature_id() == creature_id)
            .ok_or_else(|| eyre!("Creature ID {} not found", creature_id))
    }

    pub fn creature_mut(&mut self, creature_id: CreatureId) -> Result<&mut Creature> {
        self.user
            .creatures
            .iter_mut()
            .chain(self.enemy.creatures.iter_mut())
            .find(|c| c.creature_id() == creature_id)
            .ok_or_else(|| eyre!("Creature ID {} not found", creature_id))
    }
}
