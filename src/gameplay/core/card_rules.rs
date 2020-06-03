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

use crate::{
    model::{
        cards::{Cost, HasCardData, HasCardId},
        games::Game,
        players::Player,
    },
    prelude::*,
    rules::{
        effects::{Effect, Effects},
        engine::{Rule, TriggerCondition, TriggerContext, TriggerName},
    },
};

#[derive(Serialize, Deserialize, Debug, Clone)]
pub struct CoreCardRules {}

impl CoreCardRules {
    pub fn new() -> Box<Self> {
        Box::from(CoreCardRules {})
    }
}

#[typetag::serde]
impl Rule for CoreCardRules {
    fn triggers(&self) -> Vec<TriggerCondition> {
        vec![
            TriggerName::CardDrawn.this(),
            TriggerName::PlayerInfluenceChanged.any(),
            TriggerName::PlayerPowerChanged.any(),
        ]
    }
    fn on_trigger(&self, context: &TriggerContext, effects: &mut Effects) -> Result<()> {
        let card = context.this.card()?;
        println!("Card drawn: {:?}", card);
        Ok(effects.push_effect(
            context,
            Effect::SetCanPlayCard(
                card.card_id(),
                can_pay_cost(context.owner(), &card.card_data().cost),
            ),
        ))
    }
}

fn can_pay_cost(player: &Player, cost: &Cost) -> bool {
    match cost {
        Cost::ScrollPlay => player.state.current_scroll_plays > 0,
        Cost::StandardCost(standard_cost) => {
            standard_cost.power <= player.state.current_power
                && standard_cost
                    .influence
                    .less_than_or_equal_to(&player.state.current_influence)
        }
    }
}
