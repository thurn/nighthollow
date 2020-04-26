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
    model::effect::{CreatureTag, Effect, Request, Response},
    model::primitives::{Damage, Influence, ManaValue},
};

#[derive(Serialize, Deserialize, Debug, Clone)]
pub struct ExhaustSoTargetWithAttackLessThanCantBeBlocked(pub Damage);

#[typetag::serde]
impl Effect for ExhaustSoTargetWithAttackLessThanCantBeBlocked {
    fn evaluate(&self, request: &Request) -> Response {
        Response::default()
    }
}

#[derive(Serialize, Deserialize, Debug, Clone)]
pub struct ExhaustToAddManaOnlyForCreaturesWithTag {
    pub mana: ManaValue,
    pub influence: Influence,
    pub tag: CreatureTag,
}

#[typetag::serde]
impl Effect for ExhaustToAddManaOnlyForCreaturesWithTag {
    fn evaluate(&self, request: &Request) -> Response {
        Response::default()
    }
}
