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
    model::{
        creatures::{Creature, Damage, HasCreatureData},
        players::Player,
        primitives::{FileValue, SkillAnimation},
    },
    rules::{
        creature_skills::{CreatureMutation, CreatureSkill},
        effects::{Effect, Effects},
        engine::{Rule, Trigger, TriggerCondition, TriggerContext, TriggerName},
    },
};

#[derive(Serialize, Deserialize, Debug, Clone)]
pub struct BasicMeleeAttack {
    pub animation: SkillAnimation,
}

impl BasicMeleeAttack {
    pub fn new(animation: SkillAnimation) -> Box<Self> {
        Box::from(BasicMeleeAttack { animation })
    }
}

#[typetag::serde]
impl Rule for BasicMeleeAttack {
    fn triggers(&self) -> Vec<TriggerCondition> {
        vec![
            TriggerName::CreaturePlayed.this(),
            TriggerName::InvokeSkill.this(),
        ]
    }

    fn on_trigger(&self, context: &TriggerContext, effects: &mut Effects) -> Result<()> {
        match context.trigger {
            Trigger::CreaturePlayed(_, creature_id) => {
                println!("Creature played {}", creature_id);
                effects.push_effect(
                    context,
                    Effect::SetSkillPriority(*creature_id, *context.identifier, 1),
                );
            }
            Trigger::InvokeSkill(creature_id) => {
                println!("Invoke skill {}", creature_id);
                if let Some(target) =
                    next_target_for_file(context.opponent(), context.this.creature()?.position.file)
                {
                    effects.push_effect(
                        context,
                        CreatureSkill::simple_melee(
                            *creature_id,
                            self.animation,
                            CreatureMutation {
                                apply_damage: Some(Damage::from(
                                    &context.this.creature()?.stats().base_damage,
                                )),
                                ..CreatureMutation::new(target.creature_id())
                            },
                        ),
                    );
                }
            }
            _ => {}
        }
        Ok(())
    }
}

/// Returns the living creature owned by Player which is on the closest file to
/// 'file' and which is closest to the front of that file
fn next_target_for_file(player: &Player, file: FileValue) -> Option<&Creature> {
    // rank 5 is closest to front actually
    player
        .creatures
        .iter()
        .filter(|c| c.is_alive())
        .min_by_key(|c| (file_distance(c.position.file, file), c.position.rank))
}

/// Returns the numerical distance between two files
fn file_distance(a: FileValue, b: FileValue) -> i32 {
    ((a as i32) - (b as i32)).abs()
}
