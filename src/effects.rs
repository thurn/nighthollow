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

use crate::primitives::{Damage, HealthValue};
use serde::{Deserialize, Serialize};

#[derive(Serialize, Deserialize, Debug, Clone)]
pub enum CreatureTag {
    Elemental,
}

#[derive(Serialize, Deserialize, Debug, Clone)]
pub enum Effect {
    DamageCreature(Damage),
    DamagePlayer(Damage),
    DamageCreatureOrPlayer(Damage),
    BonusAttackThisTurn(HealthValue),
    BonusHealthThisTurn(HealthValue),
    BonusAttackThisTurnForEachCreatureYouControlWithTag(HealthValue, CreatureTag),
    MayDiscardToDraw,
}

#[derive(Serialize, Deserialize, Debug, Clone)]
pub enum TriggerName {
    Play,
    Death,
    Attack,
}

#[derive(Serialize, Deserialize, Debug, Clone)]
pub struct Trigger(pub TriggerName, pub Vec<Effect>);
