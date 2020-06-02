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
pub struct CorePlayerRules {}

impl CorePlayerRules {
    pub fn new() -> Box<Self> {
        Box::from(CorePlayerRules {})
    }
}

#[typetag::serde]
impl Rule for CorePlayerRules {
    fn triggers(&self) -> Vec<TriggerCondition> {
        vec![TriggerName::GameStart.any(), TriggerName::TurnStart.any()]
    }

    fn on_trigger(&self, context: &TriggerContext, effects: &mut Effects) -> Result<()> {
        match context.trigger {
            Trigger::GameStart => {
                for i in 0..6 {
                    effects.push_effect(context, Effect::DrawCard(context.this.player()?.name));
                }
            }
            Trigger::TurnStart => on_advance_turn(context, effects)?,
            _ => {}
        }

        Ok(())
    }
}

fn on_advance_turn(context: &TriggerContext, effects: &mut Effects) -> Result<()> {
    let player = context.this.player()?;
    let name = player.name;

    effects.push_effect(
        context,
        Effect::ModifyPlayerAttribute(
            name,
            Operator::Set,
            PlayerAttribute::CurrentPower(player.state.maximum_power),
        ),
    );

    effects.push_effect(
        context,
        Effect::ModifyPlayerAttribute(
            name,
            Operator::Set,
            PlayerAttribute::CurrentInfluence(player.state.maximum_influence),
        ),
    );

    effects.push_effect(
        context,
        Effect::ModifyPlayerAttribute(
            name,
            Operator::Set,
            PlayerAttribute::CurrentScrollPlays(player.state.maximum_scroll_plays),
        ),
    );

    effects.push_effect(context, Effect::DrawCard(name));

    Ok(())
}
