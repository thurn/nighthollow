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
    primitives::{Damage, GamePhase, HealthValue, Influence, ManaValue, PlayerName},
    rules::{CreatureId, DiscardCardId, HandCardId, Request},
    types::Attack,
};
use std::marker::PhantomData;

pub enum MutationType {
    SetBase,
    Increase,
    Reduce,
    MoreMultiplier,
    LessMultiplier,
}

pub enum VectorMutationType {
    Overwrite,
    Add,
    Remove,
}

pub enum CardSelectorType {
    MyCards,
    OpponentCards,
    MyDiscard,
    OpponetDiscard,
    SpecificCardInHand(Vec<HandCardId>),
    SpecificCardInMyDiscard(Vec<DiscardCardId>),
    SpecificCardInOpponentDiscard(Vec<DiscardCardId>),
}

pub enum CreaturePlayerSelectorType {
    MyCreatures,
    MyCreaturesOrMe,
    OpponentCreatures,
    OpponentCreaturesAndOpponent,
    SpecificCreatures(Vec<CreatureId>),
    SpecificCreaturesAndMe(Vec<CreatureId>),
    SpecificCreaturesAndOpponent(Vec<CreatureId>),
}

pub enum Mutation {
    // UI Changes
    DisplayHandCardSelector(CardSelectorType),
    DisplayCreatureAndPlayerSelector(CreaturePlayerSelectorType),

    // Player Mutations
    DrawCard(PlayerName),
    DiscardCard(PlayerName, HandCardId),
    DamagePlayer(PlayerName, MutationType, Damage),
    PlayerHealth(PlayerName, MutationType, HealthValue),
    PlayerMaxHealth(PlayerName, MutationType, HealthValue),
    PlayerMana(PlayerName, MutationType, ManaValue),
    PlayerInfluence(PlayerName, MutationType, Influence),

    // Creature Mutations
    DamageCreature(CreatureId, MutationType, Damage),
    CreatureHealth(CreatureId, MutationType, HealthValue),
    CreatureMaxHealth(CreatureId, MutationType, HealthValue),
    CreatureAttack(CreatureId, MutationType, Attack),
    CreatureAttributes(CreatureId, VectorMutationType, Vec<Attribute>),
}
