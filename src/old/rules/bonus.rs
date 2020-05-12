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
use std::fmt::Debug;

use crate::old::model::{
    mutations::Mutation,
    primitives::{CreatureTag, Damage, HealthValue, Result},
    rules::{Request, Response, Rule},
    types::Creature,
};

#[derive(Serialize, Deserialize, Debug, Clone)]
pub struct BonusAttackAndHealthThisTurn(pub Damage, pub HealthValue);

#[typetag::serde]
impl Rule for BonusAttackAndHealthThisTurn {
    fn update(&self, r: &Request) -> Result<Vec<Mutation>> {
        Ok(vec![])
    }
}

#[derive(Serialize, Deserialize, Debug, Clone)]
pub struct BonusAttackAndHealthCantDefend(pub Damage, pub HealthValue);

#[typetag::serde]
impl Rule for BonusAttackAndHealthCantDefend {
    fn update(&self, r: &Request) -> Result<Vec<Mutation>> {
        Ok(vec![])
    }
}

/// Gains bonus damage while attacking for each other creature you control
/// with a given tag
#[derive(Serialize, Deserialize, Debug, Clone)]
pub struct AttackingDamageBonusPerTaggedAlly {
    pub tag: CreatureTag,
    pub bonus: Damage,
}

#[typetag::serde]
impl Rule for AttackingDamageBonusPerTaggedAlly {
    fn update(&self, r: &Request) -> Result<Vec<Mutation>> {
        Ok(vec![])
    }
}

#[derive(Serialize, Deserialize, Debug, Clone)]
pub struct CreatureDamageBonusOnOpponentNoncombatDamaged(pub Damage);

#[typetag::serde]
impl Rule for CreatureDamageBonusOnOpponentNoncombatDamaged {
    fn update(&self, r: &Request) -> Result<Vec<Mutation>> {
        Ok(vec![])
    }
}

#[derive(Serialize, Deserialize, Debug, Clone)]
pub struct EachCreatureWithSameNameBonusDamageThisTurn(pub Damage);

#[typetag::serde]
impl Rule for EachCreatureWithSameNameBonusDamageThisTurn {
    fn update(&self, r: &Request) -> Result<Vec<Mutation>> {
        Ok(vec![])
    }
}
