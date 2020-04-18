use serde::{Deserialize, Serialize};

use crate::primitives::HealthValue;

#[derive(Serialize, Deserialize, Debug)]
pub enum Attack {
    BasicAttack(HealthValue),
}

#[derive(Serialize, Deserialize, Debug)]
pub struct Unit {
    current_health: HealthValue,
    maximum_health: HealthValue,
    attacks: Vec<Attack>,
}

impl Unit {
    pub fn new(health: i32, attack: i32) -> Unit {
        Unit {
            current_health: HealthValue::new(health),
            maximum_health: HealthValue::new(health),
            attacks: vec![Attack::BasicAttack(HealthValue::new(attack))],
        }
    }

    pub fn display_health(&self) -> String {
        format!(
            "{}%",
            100.0 * i32::from(self.current_health) as f64 / i32::from(self.maximum_health) as f64
        )
    }
}
