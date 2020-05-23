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

use serde::{Deserialize, Serialize};

use super::{
    effects::{CreatureMutation, CreatureSkill, Effect, Effects},
    rules::{Rule, RuleContext},
};
use crate::model::{
    primitives::{FileValue, SkillAnimation},
    types::{Creature, Damage, DamageAmount, Player},
};

#[derive(Serialize, Deserialize, Debug)]
pub struct BaseMeleeDamageAttack {
    pub animation: SkillAnimation,
}

impl BaseMeleeDamageAttack {
    pub fn new(animation: SkillAnimation) -> Box<Self> {
        Box::from(BaseMeleeDamageAttack { animation })
    }
}

#[typetag::serde]
impl Rule for BaseMeleeDamageAttack {
    fn on_calculate_skill_priority(&self, context: &RuleContext) -> Option<u32> {
        Some(1)
    }

    fn on_invoke_skill(&self, context: &RuleContext, effects: &mut Effects) {
        if let Some(target) =
            next_target_for_file(context.opponent(), context.creature.position.file)
        {
            effects.push_effect(
                context,
                CreatureSkill::simple_melee(
                    context.creature,
                    self.animation,
                    target.creature_id(),
                    CreatureMutation {
                        apply_damage: Some(Damage::from(&context.creature.stats().base_damage)),
                        ..CreatureMutation::default()
                    },
                ),
            )
        }
    }
}

/// Returns the living creature owned by Player which is on the closest file to
/// 'file' and which is closest to the front of that file
fn next_target_for_file(player: &Player, file: FileValue) -> Option<&Creature> {
    player
        .creatures
        .iter()
        .filter(|c| c.is_alive)
        .min_by_key(|c| (file_distance(c.position.file, file), c.position.rank))
}

/// Returns the numerical distance between two files
fn file_distance(a: FileValue, b: FileValue) -> i32 {
    ((a as i32) - (b as i32)).abs()
}
