// Copyright The Magewatch Project

// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at

//    https://www.apache.org/licenses/LICENSE-2.0

// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.

use serde::{Deserialize, Serialize};

use crate::{
    card::Card,
    primitives::{GamePhase, Influence, InterfaceError, PlayerName, Result},
};

pub type Zone = Vec<Card>;

#[derive(Serialize, Deserialize, Debug)]
pub struct PlayerState {
    pub mana: i32,
    pub influence: Influence,
    pub hero: Card,
    pub hand: Zone,
    pub reserve: Zone,
    pub defenders: Zone,
    pub attackers: Zone,
}

impl PlayerState {
    pub fn find_card<'a>(identifier: &str, zone: &'a mut Zone) -> &'a mut Card {
        zone.into_iter()
            .find(|x| x.identifier == identifier.to_uppercase())
            .expect("msg")
    }

    fn remove_card(identifier: &str, zone: &mut Zone) -> Result<Card> {
        if let Some(position) = zone
            .iter()
            .position(|x| x.identifier == identifier.to_uppercase())
        {
            Ok(zone.remove(position))
        } else {
            InterfaceError::result(format!(
                "Identifier not found {:?} for zone {:?}",
                identifier, zone
            ))
        }
    }

    pub fn move_card(identifier: &str, from: &mut Zone, to: &mut Zone) -> Result<()> {
        let removed = PlayerState::remove_card(identifier, from)?;
        to.push(removed);
        Ok(())
    }
}

impl Default for PlayerState {
    fn default() -> Self {
        PlayerState {
            mana: 0,
            influence: Influence::default(),
            hero: Card::default_hero(),
            hand: Vec::new(),
            reserve: Vec::new(),
            defenders: Vec::new(),
            attackers: Vec::new(),
        }
    }
}

#[derive(Serialize, Deserialize, Debug)]
pub struct GameState {
    pub auto_advance: bool,
    pub phase: GamePhase,
}

impl GameState {
    fn reset(&mut self) {
        self.auto_advance = false;
        self.phase = GamePhase::Main;
    }
}

#[derive(Serialize, Deserialize, Debug)]
pub struct Game {
    pub state: GameState,
    pub user: PlayerState,
    pub enemy: PlayerState,
}

impl Default for Game {
    fn default() -> Self {
        Game {
            state: GameState {
                auto_advance: false,
                phase: GamePhase::Main,
            },
            user: PlayerState::default(),
            enemy: PlayerState::default(),
        }
    }
}

impl Game {
    pub fn update(&mut self, other: Game) {
        self.state = other.state;
        self.user = other.user;
        self.enemy = other.enemy;
    }

    pub fn player(&self, name: PlayerName) -> &PlayerState {
        match name {
            PlayerName::User => &self.user,
            PlayerName::Enemy => &self.enemy,
        }
    }

    pub fn player_mut(&mut self, name: PlayerName) -> &mut PlayerState {
        match name {
            PlayerName::User => &mut self.user,
            PlayerName::Enemy => &mut self.enemy,
        }
    }
}
