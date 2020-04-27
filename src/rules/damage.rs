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
    model::rules::{CreatureId, Request, Response, Rule},
    model::{
        mutations::{CreaturePlayerSelectorType, Mutation, MutationType},
        primitives::Damage,
        types::Creature,
    },
};

#[derive(Serialize, Deserialize, Debug, Clone)]
pub struct DamageTargetCreature(pub Damage);

#[typetag::serde]
impl Rule for DamageTargetCreature {
    fn evaluate(&self, request: &Request) -> Response {
        Response::default()
    }
}

#[derive(Serialize, Deserialize, Debug, Clone)]
pub struct DamageOpponent(pub Damage);

#[typetag::serde]
impl Rule for DamageOpponent {
    fn evaluate(&self, request: &Request) -> Response {
        Response::default()
    }
}

#[derive(Serialize, Deserialize, Debug, Clone)]
pub struct DamageTargetCreatureAndItsOwner(pub Damage, pub Damage);

#[typetag::serde]
impl Rule for DamageTargetCreatureAndItsOwner {
    fn evaluate(&self, request: &Request) -> Response {
        Response::default()
    }
}

#[derive(Serialize, Deserialize, Debug, Clone)]
pub struct DamageTargetCreatureOrOpponent(pub Damage);

#[typetag::serde]
impl Rule for DamageTargetCreatureOrOpponent {
    fn evaluate(&self, request: &Request) -> Response {
        Response::default()
    }
}

#[derive(Serialize, Deserialize, Debug, Clone)]
pub struct DamageTargetCreatureAndExileIfDies(pub Damage);

#[typetag::serde]
impl Rule for DamageTargetCreatureAndExileIfDies {
    fn evaluate(&self, request: &Request) -> Response {
        Response::default()
    }
}
