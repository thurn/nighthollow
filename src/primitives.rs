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

#[derive(Serialize, Deserialize, Debug)]
pub enum School {
    Light,
    Sky,
    Flame,
    Ice,
    Earth,
    Shadow,
}
