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

use std::error;
use std::fmt;

use serde::{Deserialize, Serialize};

extern crate derive_more;

use derive_more::{Add, AddAssign, Constructor, Display, From, Into, Neg, Sub, SubAssign, Sum};

pub type Result<T> = std::result::Result<T, Box<dyn error::Error>>;

#[derive(Debug)]
pub struct InterfaceError {
    pub message: String,
}

impl InterfaceError {
    pub fn new(message: String) -> Box<InterfaceError> {
        Box::from(InterfaceError {
            message: message.to_string(),
        })
    }

    pub fn result<T>(message: String) -> Result<T> {
        Err(InterfaceError::new(message))
    }
}

impl fmt::Display for InterfaceError {
    fn fmt(&self, f: &mut fmt::Formatter) -> fmt::Result {
        write!(f, "{}", self.message)
    }
}

impl error::Error for InterfaceError {
    fn source(&self) -> Option<&(dyn error::Error + 'static)> {
        None
    }
}

#[derive(
    Copy,
    Clone,
    From,
    Into,
    Serialize,
    Deserialize,
    Debug,
    Display,
    Eq,
    PartialEq,
    Ord,
    PartialOrd,
    Constructor,
    Add,
    AddAssign,
    SubAssign,
    Neg,
    Sum,
)]
pub struct HealthValue(i32);

#[derive(
    Copy,
    Clone,
    From,
    Into,
    Serialize,
    Deserialize,
    Debug,
    Display,
    Eq,
    PartialEq,
    Ord,
    PartialOrd,
    Constructor,
    Add,
    AddAssign,
    SubAssign,
    Neg,
    Sum,
)]
pub struct ManaValue(i32);

#[derive(Serialize, Deserialize, Debug, PartialEq, Eq, Copy, Clone)]
pub enum GamePhase {
    Attackers,
    Defenders,
    PreCombat,
    Main,
    End,
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

#[derive(Serialize, Deserialize, Debug, PartialEq, Eq, Copy, Clone)]
pub enum CombatPosition {
    Position0,
    Position1,
    Position2,
    Position3,
    Position4,
}

impl CombatPosition {
    pub fn parse(input: &str) -> Result<CombatPosition> {
        CombatPosition::from(input.parse::<i32>()?)
    }

    pub fn from(input: i32) -> Result<CombatPosition> {
        match input {
            0 => Ok(CombatPosition::Position0),
            1 => Ok(CombatPosition::Position1),
            2 => Ok(CombatPosition::Position2),
            3 => Ok(CombatPosition::Position3),
            4 => Ok(CombatPosition::Position4),
            _ => Err(InterfaceError::new(format!(
                "Invalid combat position: {}",
                input
            ))),
        }
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

#[derive(Serialize, Deserialize, Debug, Ord, PartialOrd, Eq, PartialEq, Clone)]
pub struct Damage {
    pub physical: i32,
    pub fire: i32,
    pub cold: i32,
    pub lightning: i32,
    pub necrotic: i32,
}

impl Default for Damage {
    fn default() -> Self {
        Damage {
            physical: 0,
            fire: 0,
            cold: 0,
            lightning: 0,
            necrotic: 0,
        }
    }
}

impl Damage {
    pub fn physical(i: i32) -> Damage {
        Damage {
            physical: i,
            ..Damage::default()
        }
    }

    pub fn fire(i: i32) -> Damage {
        Damage {
            fire: i,
            ..Damage::default()
        }
    }

    pub fn cold(i: i32) -> Damage {
        Damage {
            cold: i,
            ..Damage::default()
        }
    }

    pub fn lightning(i: i32) -> Damage {
        Damage {
            lightning: i,
            ..Damage::default()
        }
    }

    pub fn necrotic(i: i32) -> Damage {
        Damage {
            necrotic: i,
            ..Damage::default()
        }
    }
}
