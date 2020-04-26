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

use super::{mutation::Mutation, types::Game};
use serde::{Deserialize, Serialize};
use std::fmt::Debug;

#[derive(Serialize, Deserialize, Debug, Clone)]
pub enum TriggerName {
    Play,
    Death,
    Attack,
    PlayerDamaged,
}

pub struct Request<'a> {
    game: &'a Game,
}

pub struct Response {
    mutations: Vec<Mutation>,
}

impl Default for Response {
    fn default() -> Self {
        Response { mutations: vec![] }
    }
}

#[derive(Serialize, Deserialize, Debug, Clone)]
pub struct Trigger(pub TriggerName, pub Vec<Box<dyn Effect>>);

#[derive(Serialize, Deserialize, Debug, Clone)]
pub enum CreatureTag {
    Elemental,
}

#[typetag::serde(tag = "type")]
pub trait Effect: EffectClone + Debug {
    fn evaluate(&self, request: &Request) -> Response;
}

pub struct Instant {
    pub effects: Vec<Box<dyn Effect>>,
}

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
