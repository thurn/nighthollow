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
use std::fmt::Debug;

use crate::{
    attributes::Attribute,
    primitives::{Damage, HealthValue, Influence, ManaValue},
};

#[derive(Serialize, Deserialize, Debug, Clone)]
pub enum TriggerName {
    Play,
    Death,
    Attack,
    PlayerDamaged,
}

#[derive(Serialize, Deserialize, Debug, Clone)]
pub struct Trigger(pub TriggerName, pub Vec<Box<dyn Effect>>);

#[derive(Serialize, Deserialize, Debug, Clone)]
pub enum CreatureTag {
    Elemental,
}

#[typetag::serde(tag = "type")]
pub trait Effect: EffectClone + Debug {}

pub trait EffectClone {
    fn clone_box(&self) -> Box<dyn Effect>;
}

impl<T: 'static + Effect + Clone> EffectClone for T {
    fn clone_box(&self) -> Box<dyn Effect> {
        Box::new(self.clone())
    }
}

impl Clone for Box<dyn Effect> {
    fn clone(&self) -> Box<dyn Effect> {
        self.clone_box()
    }
}

#[derive(Serialize, Deserialize, Debug, Clone)]
pub struct BonusAttackAndHealthThisTurn(pub Damage, pub HealthValue);

#[typetag::serde]
impl Effect for BonusAttackAndHealthThisTurn {}

#[derive(Serialize, Deserialize, Debug, Clone)]
pub struct BonusAttackAndHealthCantDefend(pub Damage, pub HealthValue);

#[typetag::serde]
impl Effect for BonusAttackAndHealthCantDefend {}

/// Gains bonus damage while attacking for each other creature you control
/// with a given tag
#[derive(Serialize, Deserialize, Debug, Clone)]
pub struct AttackingDamageBonusPerTaggedAlly {
    pub tag: CreatureTag,
    pub bonus: Damage,
}

#[typetag::serde]
impl Effect for AttackingDamageBonusPerTaggedAlly {}

#[derive(Serialize, Deserialize, Debug, Clone)]
pub struct MayDiscardToDraw();

#[typetag::serde]
impl Effect for MayDiscardToDraw {}

#[derive(Serialize, Deserialize, Debug, Clone)]
pub struct CreatureDamageBonusOnOpponentNoncombatDamaged(pub Damage);

#[typetag::serde]
impl Effect for CreatureDamageBonusOnOpponentNoncombatDamaged {}

#[derive(Serialize, Deserialize, Debug, Clone)]
pub struct EachCreatureWithSameNameBonusDamageThisTurn(pub Damage);

#[typetag::serde]
impl Effect for EachCreatureWithSameNameBonusDamageThisTurn {}

#[derive(Serialize, Deserialize, Debug, Clone)]
pub struct ExhaustSoTargetWithAttackLessThanCantBeBlocked(pub Damage);

#[typetag::serde]
impl Effect for ExhaustSoTargetWithAttackLessThanCantBeBlocked {}

#[derive(Serialize, Deserialize, Debug, Clone)]
pub struct ExhaustToAddManaOnlyForCreaturesWithTag {
    pub mana: ManaValue,
    pub influence: Influence,
    pub tag: CreatureTag,
}

#[typetag::serde]
impl Effect for ExhaustToAddManaOnlyForCreaturesWithTag {}
