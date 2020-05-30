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

use eyre::Result;

use serde::{Deserialize, Serialize};

use super::{effects::EffectSource, engine2::{RuleContext, PlayerRule, Rule, CreatureRule}};
use crate::model::{
    games::{Game, Player},
    primitives::PlayerName, creatures::Creature,
};

pub struct RuleData<'a, RuleType, DataType> {
    pub index: usize,
    pub rule: &'a RuleType,
    pub data: &'a DataType,
    pub source: EffectSource,
}

pub trait RuleScope<RuleType, DataType> {
    fn rules<'a>(&self, game: &'a Game) -> Vec<RuleData<'a, RuleType, DataType>>;
}

pub struct GlobalRuleScope {}

impl Default for GlobalRuleScope {
    fn default() -> Self {
        GlobalRuleScope {}
    }
}

impl RuleScope<Box<dyn PlayerRule>, Player> for GlobalRuleScope {
    fn rules<'a>(&self, game: &'a Game) -> Vec<RuleData<'a, Box<dyn PlayerRule>, Player>> {
        game.user
            .rules
            .iter()
            .chain(game.enemy.rules.iter())
            .enumerate()
            .map(|(index, rule)| RuleData {
                rule,
                index,
                data: &game.user,
                source: EffectSource::Player(PlayerName::User),
            })
            .collect::<Vec<_>>()
    }
}

pub struct PlayerRuleScope {
    player_name: PlayerName,
}

impl PlayerRuleScope {
    pub fn new(player_name: PlayerName) -> Self {
        Self { player_name }
    }
}

impl RuleScope<Box<dyn PlayerRule>, Player> for PlayerRuleScope {
    fn rules<'a>(&self, game: &'a Game) -> Vec<RuleData<'a, Box<dyn PlayerRule>, Player>> {
        game.player(self.player_name)
            .rules
            .iter()
            .enumerate()
            .map(|(index, rule)| RuleData {
                rule,
                index,
                data: game.player(self.player_name),
                source: EffectSource::Player(self.player_name),
            })
            .collect::<Vec<_>>()
    }
}

#[derive(Serialize, Deserialize, Debug, Clone)]
pub struct CRule {}

impl Rule<Creature> for CRule {}

#[typetag::serde]
impl CreatureRule for CRule {}

#[derive(Serialize, Deserialize, Debug, Clone)]
pub struct PRule {}

impl Rule<Player> for PRule {}

#[typetag::serde]
impl PlayerRule for PRule {}
