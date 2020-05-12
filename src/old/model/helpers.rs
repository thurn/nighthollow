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

use super::{
    events::Event,
    mutations::{Mutation, SelectorType},
    primitives::Result,
    rules::{CreatureId, Request},
};

pub fn one_shot(
    r: &Request,
    function: impl Fn() -> Result<Vec<Mutation>>,
) -> Result<Vec<Mutation>> {
    match r.event {
        Event::Resolve => function(),
        _ => Ok(vec![]),
    }
}

pub fn single_target_opponent_creature(
    r: &Request,
    function: impl Fn(CreatureId) -> Result<Vec<Mutation>>,
) -> Result<Vec<Mutation>> {
    match r.event {
        Event::Resolve => Ok(vec![Mutation::DisplayTargetSelector(vec![
            SelectorType::OpponentCreatures,
        ])]),
        Event::CreatureTargetSelected(c) => function(*c),
        _ => Ok(vec![]),
    }
}
