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

use crate::{
    model::games::Player,
    rules::{
        effects::{Effect, Effects},
        engine::{Rule, TriggerCondition, TriggerContext, TriggerName},
    },
};

#[derive(Serialize, Deserialize, Debug, Clone)]
pub struct CorePlayerRules {}

impl CorePlayerRules {
    pub fn new() -> Box<Self> {
        Box::from(CorePlayerRules {})
    }
}

#[typetag::serde]
impl Rule for CorePlayerRules {
    fn triggers(&self) -> Vec<TriggerCondition> {
        vec![TriggerCondition::Any(TriggerName::GameStart)]
    }

    fn on_trigger(&self, context: TriggerContext, effects: &mut Effects) -> Result<()> {
        for i in 0..6 {
            effects.push_effect(
                context.identifier,
                Effect::DrawCard(context.this.player()?.name),
            );
        }

        Ok(())
    }
}
