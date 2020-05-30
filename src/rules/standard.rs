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

use super::{
    effects2::{Effect, Effects},
    rules::{BaseRule, GameRule, RuleContext},
};
use crate::{api, commands, model::{primitives::PlayerName, games::Player}};

#[derive(Serialize, Deserialize, Debug, Clone)]
pub struct StandardGameRules {}

impl StandardGameRules {
    pub fn new() -> Box<Self> {
        Box::from(StandardGameRules {})
    }
}

impl BaseRule<Player> for StandardGameRules {}

#[typetag::serde]
impl GameRule for StandardGameRules {
    fn on_start_game_request(&self, context: &RuleContext<Player>, effects: &mut Effects) {
        for i in 0..6 {
            effects.push_effect(context, Effect::DrawCard(PlayerName::User));
            effects.push_effect(context, Effect::DrawCard(PlayerName::Enemy));
        }
    }
}
