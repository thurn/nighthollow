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

use eyre::eyre;
use eyre::Result;
use serde::{Deserialize, Serialize};

extern crate derive_more;

pub type GameId = i32;
pub type CardId = i32;
pub type CreatureId = i32;
pub type TurnNumber = u32;
pub type RoundNumber = u32;
pub type ActionNumber = u32;
pub type HealthValue = u32;
pub type ManaValue = u32;
pub type PowerValue = u32;
pub type LifeValue = u32;
pub type InfluenceValue = u32;

#[derive(Serialize, Deserialize, Debug, PartialEq, Eq, Copy, Clone)]
pub struct RuleId {
    pub creature_id: CreatureId,
    pub index: usize,
}

#[derive(Serialize, Deserialize, Debug, PartialEq, Eq, Copy, Clone)]
pub enum GamePhase {
    Preparation,
    Main,
}

lazy_static! {
    pub static ref SCHOOLS: Vec<School> = vec![
        School::Light,
        School::Sky,
        School::Flame,
        School::Ice,
        School::Earth,
        School::Shadow
    ];
}

#[derive(Serialize, Deserialize, Debug, PartialEq, Eq, Copy, Clone)]
pub enum School {
    Light,
    Sky,
    Flame,
    Ice,
    Earth,
    Shadow,
}

impl School {
    pub fn abbreviation(&self) -> &str {
        match self {
            School::Light => "L",
            School::Sky => "S",
            School::Flame => "F",
            School::Ice => "I",
            School::Earth => "E",
            School::Shadow => "D",
        }
    }
}

#[derive(Serialize, Deserialize, Debug, PartialEq, Eq, Copy, Clone)]
pub enum PlayerName {
    User,
    Enemy,
}

impl PlayerName {
    pub fn opponent(&self) -> PlayerName {
        match self {
            PlayerName::User => PlayerName::Enemy,
            PlayerName::Enemy => PlayerName::User,
        }
    }
}

#[derive(Serialize, Deserialize, Debug, PartialEq, Eq, Copy, Clone, Ord, PartialOrd)]
pub enum RankValue {
    Rank1,
    Rank2,
    Rank3,
    Rank4,
    Rank5,
}

#[derive(Serialize, Deserialize, Debug, PartialEq, Eq, Copy, Clone, Ord, PartialOrd)]
pub enum FileValue {
    File1,
    File2,
    File3,
    File4,
    File5,
}

#[derive(Serialize, Deserialize, Debug, Clone)]
pub struct BoardPosition {
    pub rank: RankValue,
    pub file: FileValue,
}

impl BoardPosition {
    pub fn new(rank: RankValue, file: FileValue) -> BoardPosition {
        BoardPosition { rank, file }
    }
}

#[derive(Serialize, Deserialize, Debug, Clone, Copy)]
pub enum SkillAnimation {
    Skill1,
    Skill2,
    Skill3,
    Skill4,
    Skill5,
}

#[derive(Serialize, Deserialize, Debug, Clone, Eq, PartialEq)]
pub struct InfluenceAmount {
    pub value: InfluenceValue,
    pub school: School,
}

#[derive(Serialize, Deserialize, Debug, Clone, Eq, PartialEq)]
pub struct Influence {
    values: Vec<InfluenceAmount>,
}

impl Influence {
    pub fn single(value: InfluenceValue, school: School) -> Self {
        Influence {
            values: vec![InfluenceAmount { value, school }],
        }
    }

    pub fn value(&self, school: School) -> InfluenceValue {
        self.values
            .iter()
            .find(|v| v.school == school)
            .map_or(0, |v| v.value)
    }

    /// Returns true if every contained Influence value in this object is less
    /// than or equal to its corresponding value in 'other'
    pub fn less_than_or_equal_to(&self, other: &Influence) -> bool {
        self.values
            .iter()
            .all(|amount| amount.value <= other.value(amount.school))
    }

    /// Decrements each Influence value in this object by the amount present
    /// in 'other', returning an error if this would result in a negative
    /// value.
    pub fn subtract(&mut self, other: &Influence) -> Result<()> {
        other
            .values
            .iter()
            .try_for_each(|v| self.subtract_amount(v))
    }

    fn subtract_amount(&mut self, other: &InfluenceAmount) -> Result<()> {
        let position = self
            .values
            .iter()
            .position(|v| v.school == other.school)
            .ok_or(eyre!("Value exceeds available influence: {:?}", other))?;
        let amount = self
            .values
            .get_mut(position)
            .ok_or(eyre!("Not found: {}", position))?;

        if amount.value < other.value {
            Err(eyre!(
                "Value {} exceeds available influence: {}",
                other.value,
                amount.value
            ))
        } else {
            amount.value -= other.value;
            Ok(())
        }
    }
}

impl Default for Influence {
    fn default() -> Self {
        Influence { values: vec![] }
    }
}

#[derive(Serialize, Deserialize, Debug, PartialEq, Eq, Copy, Clone)]
pub enum DamageType {
    Radiant,
    Electric,
    Fire,
    Cold,
    Physical,
    Necrotic,
}
