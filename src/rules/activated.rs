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

use crate::model::{
    mutations::Mutation,
    primitives::{CreatureTag, Damage, Influence, ManaValue, Result},
    rules::{Request, Response, Rule},
    types::Creature,
};

#[derive(Serialize, Deserialize, Debug, Clone)]
pub struct ExhaustSoTargetWithAttackLessThanCantBeBlocked(pub Damage);

#[typetag::serde]
impl Rule for ExhaustSoTargetWithAttackLessThanCantBeBlocked {
    fn update(&self, r: &Request) -> Result<Vec<Mutation>> {
        Ok(vec![])
    }
}

#[derive(Serialize, Deserialize, Debug, Clone)]
pub struct ExhaustToAddManaOnlyForCreaturesWithTag {
    pub mana: ManaValue,
    pub influence: Influence,
    pub tag: CreatureTag,
}

#[typetag::serde]
impl Rule for ExhaustToAddManaOnlyForCreaturesWithTag {
    fn update(&self, r: &Request) -> Result<Vec<Mutation>> {
        Ok(vec![])
    }
}