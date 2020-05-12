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

use super::{
    mutations::Mutation,
    primitives::GamePhase,
    rules::{CreatureId, DiscardCardId, HandCardId, RuleContext, RuleId},
};

pub enum Event<'a> {
    StartGame,
    Resolve,
    Mutation(&'a RuleContext<'a>, &'a Mutation),
    PassPriority,
    AttackersSelected(&'a Vec<CreatureId>),
    DefendersSelected(&'a Vec<CreatureId>),
    CardPlayed(HandCardId),

    // These UI events are only sent to the Rule which requested the user interaction in question
    OpponentSelected,
    SelfSelected,
    CreatureTargetSelected(CreatureId),
    MultipleCreatureTargetsSelected(&'a Vec<CreatureId>),
    HandCardTargetSelected(HandCardId),
    MultipleHandCardTargetsSelected(&'a Vec<HandCardId>),
    DiscardCardTargetSelected(DiscardCardId),
    MultipleDiscardCardTargetsSelected(&'a Vec<DiscardCardId>),
}
