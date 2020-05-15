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

use serde::{Deserialize, Serialize};

extern crate derive_more;

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

#[derive(Serialize, Deserialize, Debug, PartialEq, Eq, Copy, Clone)]
pub enum RankValue {
    Rank0,
    Rank1,
    Rank2,
    Rank3,
    Rank4,
    Rank5,
}

#[derive(Serialize, Deserialize, Debug, PartialEq, Eq, Copy, Clone)]
pub enum FileValue {
    File0,
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

#[derive(Serialize, Deserialize, Debug, Ord, PartialOrd, Eq, PartialEq, Clone)]
pub struct Influence {
    pub light: i32,
    pub sky: i32,
    pub flame: i32,
    pub ice: i32,
    pub earth: i32,
    pub shadow: i32,
}

impl Influence {
    pub fn new(school: School, amount: i32) -> Influence {
        Influence {
            light: if school == School::Light { amount } else { 0 },
            sky: if school == School::Sky { amount } else { 0 },
            flame: if school == School::Flame { amount } else { 0 },
            ice: if school == School::Ice { amount } else { 0 },
            earth: if school == School::Earth { amount } else { 0 },
            shadow: if school == School::Shadow { amount } else { 0 },
        }
    }

    pub fn light(i: i32) -> Influence {
        Influence {
            light: i,
            ..Influence::default()
        }
    }

    pub fn sky(i: i32) -> Influence {
        Influence {
            sky: i,
            ..Influence::default()
        }
    }

    pub fn flame(i: i32) -> Influence {
        Influence {
            flame: i,
            ..Influence::default()
        }
    }

    pub fn value(&self, school: &School) -> i32 {
        match school {
            School::Light => self.light,
            School::Sky => self.sky,
            School::Flame => self.flame,
            School::Ice => self.ice,
            School::Earth => self.earth,
            School::Shadow => self.shadow,
        }
    }

    pub fn add(&mut self, other: &Influence) {
        self.light += other.light;
        self.sky += other.sky;
        self.flame += other.flame;
        self.ice += other.ice;
        self.earth += other.earth;
        self.shadow += other.shadow;
    }

    pub fn subtract(&mut self, other: &Influence) {
        self.light -= other.light;
        self.sky -= other.sky;
        self.flame -= other.flame;
        self.ice -= other.ice;
        self.earth -= other.earth;
        self.shadow -= other.shadow;
    }

    pub fn set_to(&mut self, other: &Influence) {
        self.light = other.light;
        self.sky = other.sky;
        self.flame = other.flame;
        self.ice = other.ice;
        self.earth = other.earth;
        self.shadow = other.shadow;
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

impl std::fmt::Display for Influence {
    fn fmt(&self, f: &mut std::fmt::Formatter<'_>) -> std::fmt::Result {
        for school in SCHOOLS.iter() {
            if self.value(school) > 0 {
                write!(f, "{}{}", self.value(school), school.abbreviation())?;
            }
        }
        Ok(())
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
