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
    primitives::{GamePhase, InterfaceError, PlayerName, Result, ZoneName},
};

#[derive(Serialize, Deserialize, Debug)]
pub struct PlayerState {
    pub mana: i32,
    pub hand: Vec<Card>,
    pub reserve: Vec<Card>,
    pub defenders: Vec<Card>,
    pub attackers: Vec<Card>,
}

impl PlayerState {
    fn reset(&mut self) {
        self.mana = 0;
        self.hand.clear();
        self.reserve.clear();
        self.defenders.clear();
        self.attackers.clear();
    }

    fn cards_in_zone(&self, zone: ZoneName) -> &[Card] {
        match zone {
            ZoneName::Hand => &self.hand,
            ZoneName::Reserves => &self.reserve,
            ZoneName::Attackers => &self.attackers,
            ZoneName::Defenders => &self.defenders,
        }
    }

    fn cards_in_zone_mut(&mut self, zone: ZoneName) -> &mut Vec<Card> {
        match zone {
            ZoneName::Hand => &mut self.hand,
            ZoneName::Reserves => &mut self.reserve,
            ZoneName::Attackers => &mut self.attackers,
            ZoneName::Defenders => &mut self.defenders,
        }
    }

    fn find_card_position(&self, identifier: &str, zone: ZoneName) -> Result<usize> {
        self.cards_in_zone(zone)
            .iter()
            .position(|x| x.identifier == identifier.to_uppercase())
            .ok_or(InterfaceError::new(format!(
                "Identifier not found {:?} for zone {:?} of player {:?}",
                identifier, zone, self
            )))
    }
}

impl Default for PlayerState {
    fn default() -> Self {
        PlayerState {
            mana: 0,
            hand: Vec::new(),
            reserve: Vec::new(),
            defenders: Vec::new(),
            attackers: Vec::new(),
        }
    }
}

#[derive(Serialize, Deserialize, Debug)]
pub struct InterfaceOptions {
    pub auto_advance: bool,
}

#[derive(Serialize, Deserialize, Debug)]
pub struct InterfaceState {
    pub options: InterfaceOptions,
    pub phase: GamePhase,
    pub player: PlayerState,
    pub enemy: PlayerState,
}

impl Default for InterfaceState {
    fn default() -> Self {
        InterfaceState {
            options: InterfaceOptions {
                auto_advance: false,
            },
            phase: GamePhase::Main,
            player: PlayerState::default(),
            enemy: PlayerState::default(),
        }
    }
}

impl InterfaceState {
    fn get_mana(&self, player_name: PlayerName) -> i32 {
        match player_name {
            PlayerName::Player => self.player.mana,
            PlayerName::Enemy => self.enemy.mana,
        }
    }

    pub fn reset(&mut self) {
        self.phase = GamePhase::Main;
        self.player.reset();
        self.enemy.reset();
    }

    pub fn update(&mut self, other: InterfaceState) {
        self.options = other.options;
        self.phase = other.phase;
        self.player = other.player;
        self.enemy = other.enemy;
    }

    pub fn move_card(
        &mut self,
        identifier: &str,
        index: usize,
        from: ZoneName,
        to: ZoneName,
        player: PlayerName,
    ) -> Result<()> {
        let player = self.player_mut(player);
        if index <= player.cards_in_zone(to).len() {
            let position = player.find_card_position(identifier, from)?;
            let removed = player.cards_in_zone_mut(from).remove(position);
            player.cards_in_zone_mut(to).insert(index, removed);
            Ok(())
        } else {
            InterfaceError::result(format!(
                "Position {:?} is outside range for player {:?} zone {:?}",
                index, player, to
            ))
        }
    }

    fn player(&self, player: PlayerName) -> &PlayerState {
        match player {
            PlayerName::Player => &self.player,
            PlayerName::Enemy => &self.enemy,
        }
    }

    fn player_mut(&mut self, player: PlayerName) -> &mut PlayerState {
        match player {
            PlayerName::Player => &mut self.player,
            PlayerName::Enemy => &mut self.enemy,
        }
    }
}
