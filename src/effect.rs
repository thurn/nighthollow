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

use std::fmt::Debug;

use serde::{Deserialize, Serialize};

use crate::{
    attributes::Attribute,
    primitives::{Damage, HealthValue},
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
pub struct DamageCreatureEffect(pub Damage);

#[typetag::serde]
impl Effect for DamageCreatureEffect {}

#[derive(Serialize, Deserialize, Debug, Clone)]
pub struct DamagePlayerEffect(pub Damage);

#[typetag::serde]
impl Effect for DamagePlayerEffect {}

#[derive(Serialize, Deserialize, Debug, Clone)]
pub struct DamageCreatureOrPlayerEffect(pub Damage);

#[typetag::serde]
impl Effect for DamageCreatureOrPlayerEffect {}

#[derive(Serialize, Deserialize, Debug, Clone)]
pub struct BonusAttackEffect(pub HealthValue);

#[typetag::serde]
impl Effect for BonusAttackEffect {}

#[derive(Serialize, Deserialize, Debug, Clone)]
pub struct BonusAttackThisTurnEffect(pub HealthValue);

#[typetag::serde]
impl Effect for BonusAttackThisTurnEffect {}

#[derive(Serialize, Deserialize, Debug, Clone)]
pub struct BonusHealthEffect(pub HealthValue);

#[typetag::serde]
impl Effect for BonusHealthEffect {}

#[derive(Serialize, Deserialize, Debug, Clone)]
pub struct BonusHealthThisTurnEffect(pub HealthValue);

#[typetag::serde]
impl Effect for BonusHealthThisTurnEffect {}

/// Gains bonus attack while attacking for each other creature you control
/// with a given tag
#[derive(Serialize, Deserialize, Debug, Clone)]
pub struct BrawlerEffect {
    pub tag: CreatureTag,
    pub bonus: HealthValue,
}

#[typetag::serde]
impl Effect for BrawlerEffect {}

#[derive(Serialize, Deserialize, Debug, Clone)]
pub struct MayDiscardToDrawEffect();

#[typetag::serde]
impl Effect for MayDiscardToDrawEffect {}

#[derive(Serialize, Deserialize, Debug, Clone)]
pub struct ApplyAttributeEffect(pub Attribute);

#[typetag::serde]
impl Effect for ApplyAttributeEffect {}
