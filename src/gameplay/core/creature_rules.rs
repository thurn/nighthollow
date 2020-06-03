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
    model::{creatures::HasCreatureData, players::PlayerAttribute},
    rules::{
        creature_skills::CreatureSkill,
        effects::{Effect, Effects, Operator, UnderflowBehavior},
        engine::{Rule, Trigger, TriggerCondition, TriggerContext, TriggerName},
    },
};

#[derive(Serialize, Deserialize, Debug, Clone)]
pub struct CoreCreatureRules {}

impl CoreCreatureRules {
    pub fn new() -> Box<Self> {
        Box::from(CoreCreatureRules {})
    }
}

#[typetag::serde]
impl Rule for CoreCreatureRules {
    fn triggers(&self) -> Vec<TriggerCondition> {
        vec![TriggerName::CombatEnd.any()]
    }

    fn on_trigger(&self, context: &TriggerContext, effects: &mut Effects) -> Result<()> {
        let creature = context.this.creature()?;
        match context.trigger {
            Trigger::CombatEnd => {
                if creature.is_alive() {
                    effects.push_effect(
                        context,
                        Effect::ModifyPlayerAttribute(
                            context.opponent().name,
                            Operator::Subtract(UnderflowBehavior::SetZero),
                            PlayerAttribute::CurrentLife(
                                creature.stats().opponent_life_reduction.value(),
                            ),
                        ),
                    );

                    effects.push_effect(
                        context,
                        Effect::UseCreatureSkill(CreatureSkill {
                            source_creature: creature.creature_id(),
                            animation: creature.data.base_type.victory_skill(),
                            on_impact: vec![],
                            melee_target: None,
                        }),
                    );
                }
            }
            _ => {}
        }
        Ok(())
    }
}
