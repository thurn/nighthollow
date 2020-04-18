use serde::{Deserialize, Serialize};

extern crate derive_more;

use derive_more::{Add, Constructor, Display, From, Into};

#[derive(
    Copy, Clone, From, Into, Serialize, Deserialize, Debug, Display, PartialEq, Constructor, Add,
)]
pub struct HealthValue(i32);

#[derive(
    Copy, Clone, From, Into, Serialize, Deserialize, Debug, Display, PartialEq, Constructor, Add,
)]
pub struct ManaValue(i32);

#[derive(PartialEq, Serialize, Deserialize, Debug)]
pub enum School {
    Light,
    Sky,
    Flame,
    Ice,
    Earth,
    Shadow,
}

#[derive(Serialize, Deserialize, Debug)]
pub struct Influence {
    light: i32,
    sky: i32,
    flame: i32,
    ice: i32,
    earth: i32,
    shadow: i32,
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

    pub fn get(&self, school: School) -> i32 {
        match school {
            School::Light => self.light,
            School::Sky => self.sky,
            School::Flame => self.flame,
            School::Ice => self.ice,
            School::Earth => self.earth,
            School::Shadow => self.shadow,
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

impl std::fmt::Display for Influence {
    fn fmt(&self, f: &mut std::fmt::Formatter<'_>) -> std::fmt::Result {
        if self.light > 0 {
            write!(f, "{}L", self.light)?
        }
        if self.sky > 0 {
            write!(f, "{}S", self.sky)?
        }
        if self.flame > 0 {
            write!(f, "{}F", self.flame)?
        }
        if self.ice > 0 {
            write!(f, "{}I", self.ice)?
        }
        if self.earth > 0 {
            write!(f, "{}E", self.earth)?
        }
        if self.shadow > 0 {
            write!(f, "{}D", self.shadow)?
        }
        Ok(())
    }
}
