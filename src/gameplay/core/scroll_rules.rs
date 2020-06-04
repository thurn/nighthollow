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

use crate::{
    model::players::PlayerAttribute,
    rules::{
        effects::{Effect, Effects, Operator},
        engine::{Rule, Trigger, TriggerCondition, TriggerContext, TriggerName},
    },
};

#[derive(Serialize, Deserialize, Debug, Clone)]
pub struct CoreScrollRules {}

impl CoreScrollRules {
    pub fn new() -> Box<Self> {
        Box::from(CoreScrollRules {})
    }
}

#[typetag::serde]
impl Rule for CoreScrollRules {
    fn triggers(&self) -> Vec<TriggerCondition> {
        vec![TriggerName::ScrollPlayed.this()]
    }

    fn on_trigger(&self, context: &TriggerContext, effects: &mut Effects) -> Result<()> {
        match context.trigger {
            Trigger::ScrollPlayed(player_name, scroll_id) => {
                let scroll = context.this.scroll()?;
                effects.push_effect(
                    context,
                    Effect::ModifyPlayerAttribute(
                        *player_name,
                        Operator::Add,
                        PlayerAttribute::CurrentPower(scroll.stats.added_current_power),
                    ),
                );
                effects.push_effect(
                    context,
                    Effect::ModifyPlayerAttribute(
                        *player_name,
                        Operator::Add,
                        PlayerAttribute::MaximumPower(scroll.stats.added_maximum_power),
                    ),
                );
                effects.push_effect(
                    context,
                    Effect::ModifyPlayerAttribute(
                        *player_name,
                        Operator::Add,
                        PlayerAttribute::CurrentInfluence(scroll.stats.added_current_influence),
                    ),
                );
                effects.push_effect(
                    context,
                    Effect::ModifyPlayerAttribute(
                        *player_name,
                        Operator::Add,
                        PlayerAttribute::MaximumInfluence(scroll.stats.added_maximum_influence),
                    ),
                );
            }
            _ => {}
        }
        Ok(())
    }
}
