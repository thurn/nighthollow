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
    effect::{CreatureId, Request},
    mutation::Mutation,
    primitives::{Damage, GamePhase},
    types::{CardVariant, Creature, Player},
};

pub struct EventRegistry {}

pub enum EventResult {
    Continue,
    Unsubscribe,
}

pub struct EventResponse {
    pub mutations: Vec<Mutation>,
    pub result: EventResult,
}

pub trait OnPhaseStarted {
    fn on_phase_started(&self, new_phase: &GamePhase) -> Vec<Mutation>;
}

pub trait OnCreaturePlayed {
    fn on_creature_played(&self, creature: &Creature) -> Vec<Mutation>;
}

pub trait OnCreatureDied {
    fn on_creature_died(&self, creature: &Creature) -> Vec<Mutation>;
}

pub trait OnCreatureAttack {
    fn on_creature_attack(&self, creature: &Creature) -> Vec<Mutation>;
}

pub trait OnCreatureDefend {
    fn on_creature_defend(&self, creature: &Creature, defending: &Creature) -> Vec<Mutation>;
}

pub trait OnCreatureDamaged {
    fn on_creature_damaged(
        &self,
        creature: &Creature,
        damage: &Damage,
        source: &CardVariant,
    ) -> Vec<Mutation>;
}

pub trait OnPlayerDamaged {
    fn on_player_damaged(
        &self,
        player: &Player,
        damage: &Damage,
        source: &CardVariant,
    ) -> Vec<Mutation>;
}

pub trait OnCreatureTargetSelected {
    fn on_creature_target_selected(&self, request: &Request, creature: CreatureId)
        -> EventResponse;
}

pub trait OnPlayerTargetSelected {
    fn on_player_target_selected(&self, request: &Request, player: &Player) -> EventResponse;
}
