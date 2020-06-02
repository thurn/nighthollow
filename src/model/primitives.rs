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

use std::collections::BTreeMap;

use crate::{prelude::*, rules::effects::UnderflowBehavior};

pub type GameId = i32;
pub type CardId = i32;
pub type CreatureId = i32;
pub type SpellId = i32;
pub type ScrollId = i32;
pub type TurnNumber = u32;
pub type RoundNumber = u32;
pub type ActionNumber = u32;
pub type HealthValue = u32;
pub type ManaValue = u32;
pub type PowerValue = u32;
pub type LifeValue = u32;
pub type InfluenceValue = u32;

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

#[derive(Serialize, Deserialize, Debug, PartialEq, Eq, Copy, Clone, Ord, PartialOrd)]
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

#[derive(Serialize, Deserialize, Debug, PartialEq, Eq, Copy, Clone, Ord, PartialOrd)]
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

#[derive(Serialize, Deserialize, Debug, Copy, Clone)]
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

#[derive(Serialize, Deserialize, Debug, Clone, Copy, Eq, PartialEq)]
pub struct Influence {
    light: InfluenceValue,
    sky: InfluenceValue,
    flame: InfluenceValue,
    ice: InfluenceValue,
    earth: InfluenceValue,
    shadow: InfluenceValue,
}

impl Influence {
    pub fn single(value: InfluenceValue, school: School) -> Self {
        let mut result = Influence::default();
        *result.mut_ref(school) = value;
        result
    }

    fn mut_ref(&mut self, school: School) -> &mut InfluenceValue {
        use School::*;
        match school {
            Light => &mut self.light,
            Sky => &mut self.sky,
            Flame => &mut self.flame,
            Ice => &mut self.ice,
            Earth => &mut self.earth,
            Shadow => &mut self.shadow,
        }
    }

    pub fn value(&self, school: School) -> InfluenceValue {
        use School::*;
        match school {
            Light => self.light,
            Sky => self.sky,
            Flame => self.flame,
            Ice => self.ice,
            Earth => self.earth,
            Shadow => self.shadow,
        }
    }

    /// Returns true if every contained Influence value in this object is less
    /// than or equal to its corresponding value in 'other'
    pub fn less_than_or_equal_to(&self, other: &Influence) -> bool {
        for school in SCHOOLS.iter() {
            if self.value(*school) > other.value(*school) {
                return false;
            }
        }
        true
    }

    /// Adds each Influence value in 'other' to the corresponding value in self
    pub fn add(&mut self, other: &Influence) {
        for school in SCHOOLS.iter() {
            *self.mut_ref(*school) = self.value(*school) + other.value(*school);
        }
    }

    /// Decrements each Influence value in this object by the amount present
    /// in 'other', returning an error if this would result in a negative
    /// value.
    pub fn subtract(
        &mut self,
        other: &Influence,
        underflow_behavior: UnderflowBehavior,
    ) -> Result<()> {
        if !other.less_than_or_equal_to(self) {
            match underflow_behavior {
                UnderflowBehavior::Error => {
                    Err(eyre!("Can't subtract {:?} from {:?}", other, self))
                }
                UnderflowBehavior::SetZero => {
                    *self = Influence::default();
                    Ok(())
                }
            }
        } else {
            for school in SCHOOLS.iter() {
                *self.mut_ref(*school) = self.value(*school) - other.value(*school);
            }
            Ok(())
        }
    }
}

impl Default for Influence {
    fn default() -> Self {
        Influence {
            light: 0,
            sky: 0,
            flame: 0,
            ice: 0,
            earth: 0,
            shadow: 0,
        }
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

lazy_static! {
    pub static ref DAMAGE_TYPES: Vec<DamageType> = vec![
        DamageType::Radiant,
        DamageType::Electric,
        DamageType::Fire,
        DamageType::Cold,
        DamageType::Physical,
        DamageType::Necrotic
    ];
}

#[derive(Serialize, Deserialize, Debug, Clone, Copy, Eq, PartialEq)]
pub struct Damage {
    pub radiant: HealthValue,
    pub electric: HealthValue,
    pub fire: HealthValue,
    pub cold: HealthValue,
    pub physical: HealthValue,
    pub necrotic: HealthValue,
}

impl Damage {
    pub fn single(value: HealthValue, damage_type: DamageType) -> Self {
        let mut result = Damage::default();
        *result.mut_ref(damage_type) = value;
        result
    }

    pub fn value(&self, damage_type: DamageType) -> HealthValue {
        use DamageType::*;
        match damage_type {
            Radiant => self.radiant,
            Electric => self.electric,
            Fire => self.fire,
            Cold => self.cold,
            Physical => self.physical,
            Necrotic => self.necrotic,
        }
    }

    fn mut_ref(&mut self, damage_type: DamageType) -> &mut HealthValue {
        use DamageType::*;
        match damage_type {
            Radiant => &mut self.radiant,
            Electric => &mut self.electric,
            Fire => &mut self.fire,
            Cold => &mut self.cold,
            Physical => &mut self.physical,
            Necrotic => &mut self.necrotic,
        }
    }

    pub fn total(&self) -> HealthValue {
        DAMAGE_TYPES.iter().map(|t| self.value(*t)).sum()
    }
}

impl Default for Damage {
    fn default() -> Self {
        Self {
            radiant: 0,
            electric: 0,
            fire: 0,
            cold: 0,
            physical: 0,
            necrotic: 0,
        }
    }
}
