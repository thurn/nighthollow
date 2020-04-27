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
    effect::CreatureId,
    primitives::{Damage, GamePhase, PlayerName},
};

pub enum TriggerData {
    PhaseStarted(GamePhase),
    CreaturedEnteredPlay(CreatureId),
    CreaturedDied(CreatureId),
    CreatureDeclaredAttacking(CreatureId),
    PlayerDamaged(PlayerName, Damage),
    CreatureDamaged(CreatureId, Damage),

    // UI Events. Only sent to the Effect which requested the UI interaction
    CreatureSelectedInInterface(CreatureId),
    PlayerSelectedInInterface(PlayerName),
}

pub trait OnPhaseStarted {
    fn on_phase_started(&self, phase: GamePhase);
}

pub trait CanSub {
    fn doit(&self) -> ListenerResponse;
}

pub struct MyComponent {
    pub int: i32,
}

impl OnPhaseStarted for MyComponent {
    fn on_phase_started(&self, phase: GamePhase) {}
}

impl CanSub for MyComponent {
    fn doit(&self) -> ListenerResponse {
        ListenerResponse::Subscribe(self)
    }
}

pub enum ListenerResponse<'a> {
    Subscribe(&'a dyn OnPhaseStarted),
}

pub struct EventDispatcher<'a> {
    handlers: Vec<&'a dyn OnPhaseStarted>,
}

impl<'a> EventDispatcher<'a> {
    pub fn add(&mut self, response: ListenerResponse<'a>) {
        match response {
            ListenerResponse::Subscribe(listener) => self.handlers.push(listener),
        }
    }
}
