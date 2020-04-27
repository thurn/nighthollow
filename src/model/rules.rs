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

use std::{fmt::Debug, marker::PhantomData};

use serde::{Deserialize, Serialize};

use super::{
    mutations::Mutation,
    primitives::PlayerName,
    types::{CardVariant, Creature, Game},
};

#[derive(Serialize, Deserialize, Debug, Clone)]
pub struct HandCardId(usize);

#[derive(Serialize, Deserialize, Debug, Clone)]
pub struct DiscardCardId(usize);

#[derive(Serialize, Deserialize, Debug, Clone)]
pub struct CreatureId(usize);

#[derive(Serialize, Deserialize, Debug, Clone)]
pub struct RuleId(usize);

#[derive(Serialize, Deserialize, Debug, Clone)]
pub enum TriggerName {
    Play,
    Death,
    Attack,
    PlayerDamaged,
}

pub struct Request<'a> {
    id: RuleId,
    owner: PlayerName,
    source: &'a CardVariant,
    game: &'a Game,
}

pub struct Response {
    pub mutations: Vec<Mutation>,
}

impl Default for Response {
    fn default() -> Self {
        Response { mutations: vec![] }
    }
}

#[derive(Serialize, Deserialize, Debug, Clone)]
pub struct Trigger(pub TriggerName, pub Vec<Box<dyn Rule>>);

#[typetag::serde(tag = "type")]
pub trait Rule: RuleClone + Debug {
    fn evaluate(&self, request: &Request) -> Response;
}

pub trait RuleClone {
    fn clone_box(&self) -> Box<dyn Rule>;
}

impl<T: 'static + Rule + Clone> RuleClone for T {
    fn clone_box(&self) -> Box<dyn Rule> {
        Box::new(self.clone())
    }
}

impl Clone for Box<dyn Rule> {
    fn clone(&self) -> Box<dyn Rule> {
        self.clone_box()
    }
}
