// Copyright 2020 © Derek Thurn

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
    rules::{CreatureId, DiscardCardId, HandCardId, Request, SpellId},
    types::Attack,
};
use std::marker::PhantomData;

pub enum MutationType {
    Apply,
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

pub enum SelectorType {
    Me,
    Opponent,
    Creatures(Vec<CreatureId>),
    MyCreatures,
    OpponentCreatures,
    Spells(Vec<SpellId>),
    MySpells,
    OpponentSpells,
    CardsInHand(Vec<HandCardId>),
    MyCardsInHand,
    OpponentCardsInHand,
    CardsInDiscard(Vec<DiscardCardId>),
    MyCardsInDiscard,
    OpponentCardsInDiscard,
}

pub struct AttackersSelector {
    creatures: Vec<CreatureId>,
}

pub enum Mutation {
    AdvanceGamePhase,

    // UI Changes
    DisplayTargetSelector(Vec<SelectorType>),
    DisplayAttackersSelector(AttackersSelector),

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
