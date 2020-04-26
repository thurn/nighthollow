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
    attributes::Attribute,
    primitives::{HealthValue, Influence, ManaValue, PlayerName},
    types::{Attack, CardVariant, Creature},
};
use std::marker::PhantomData;

pub struct Id<T>(usize, PhantomData<*const T>);
pub type HandCardId = Id<CardVariant>;
pub type CreatureId = Id<Creature>;

pub enum MutationType {
    SetBase,
    Increase,
    Reduce,
    MoreMultiplier,
    LessMultiplier,
}

pub enum SetMutationType {
    Overwrite,
    Add,
    Remove,
}

pub enum Mutation {
    // UI Changes
    DisplayHandCardSelector(Vec<HandCardId>),
    DisplayCreatureSelector(Vec<CreatureId>),

    // Player Mutations
    DrawCard(PlayerName),
    DiscardCard(PlayerName, HandCardId),
    PlayerHealth(PlayerName, MutationType, HealthValue),
    PlayerMaxHealth(PlayerName, MutationType, HealthValue),
    PlayerMana(PlayerName, MutationType, ManaValue),
    PlayerInfluence(PlayerName, MutationType, Influence),

    // Creature Mutations
    CreatureHealth(CreatureId, MutationType, HealthValue),
    CreatureMaxHealth(CreatureId, MutationType, HealthValue),
    CreatureAttack(CreatureId, MutationType, Attack),
    CreatureAttributes(CreatureId, SetMutationType, Vec<Attribute>),
}
