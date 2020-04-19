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

use std::fmt::{Display, Formatter};
use std::sync::atomic::{AtomicI32, Ordering};

use serde::{Deserialize, Serialize};
use termion::color;

use crate::primitives::{Influence, ManaValue, School};
use crate::unit::Unit;

static NEXT_IDENTIFIER_INDEX: AtomicI32 = AtomicI32::new(1);

#[derive(Serialize, Deserialize, Debug)]
pub struct ManaCost {
    mana: ManaValue,
    influence: Influence,
}

impl Display for ManaCost {
    fn fmt(&self, f: &mut Formatter<'_>) -> std::fmt::Result {
        write!(f, "{} {}", self.mana, self.influence)
    }
}

#[derive(Serialize, Deserialize, Debug)]
pub enum Cost {
    ManaCost(ManaCost),
}

impl Cost {
    pub fn mana_cost(school: School, mana: ManaValue, influence: i32) -> Cost {
        Cost::ManaCost(ManaCost {
            mana,
            influence: Influence::new(school, influence),
        })
    }
}

impl Display for Cost {
    fn fmt(&self, f: &mut Formatter<'_>) -> std::fmt::Result {
        match self {
            Cost::ManaCost(mana_cost) => write!(f, "{}", mana_cost),
        }
    }
}

#[derive(Serialize, Deserialize, Debug)]
pub enum ForegroundColor {
    LightRed,
    LightMagenta,
    LightYellow,
    LightGreen,
    LightBlue,
    Red,
    Magenta,
    Yellow,
    Green,
    Blue,
}

impl ForegroundColor {
    pub fn to_terminal_color(&self) -> Box<dyn Display> {
        match self {
            ForegroundColor::LightRed => Box::from(color::Fg(color::LightRed)),
            ForegroundColor::LightMagenta => Box::from(color::Fg(color::LightMagenta)),
            ForegroundColor::LightYellow => Box::from(color::Fg(color::LightYellow)),
            ForegroundColor::LightGreen => Box::from(color::Fg(color::LightGreen)),
            ForegroundColor::LightBlue => Box::from(color::Fg(color::LightBlue)),
            ForegroundColor::Red => Box::from(color::Fg(color::Red)),
            ForegroundColor::Magenta => Box::from(color::Fg(color::Magenta)),
            ForegroundColor::Yellow => Box::from(color::Fg(color::Yellow)),
            ForegroundColor::Green => Box::from(color::Fg(color::Green)),
            ForegroundColor::Blue => Box::from(color::Fg(color::Blue)),
        }
    }
}

#[derive(Serialize, Deserialize, Debug)]
pub enum CardVariant {
    Unit(Unit),
    Spell,
}

impl CardVariant {
    pub fn display_status(&self) -> String {
        match self {
            CardVariant::Unit(unit) => unit.display_health(),
            CardVariant::Spell => "".to_string(),
        }
    }
}

#[derive(Serialize, Deserialize, Debug)]
pub struct Card {
    pub cost: Cost,
    pub name: String,
    pub identifier: String,
    pub variant: CardVariant,
    pub fast: bool,
    pub foreground: ForegroundColor,
}

impl Card {
    pub fn new_unit(name: &str, cost: Cost, health: i32, attack: i32) -> Card {
        let index = NEXT_IDENTIFIER_INDEX.fetch_add(1, Ordering::Relaxed);

        Card {
            cost,
            variant: CardVariant::Unit(Unit::new(health, attack)),
            name: name.to_string(),
            identifier: to_identifier(index),
            fast: false,
            foreground: match index % 10 {
                0 => ForegroundColor::LightRed,
                1 => ForegroundColor::LightMagenta,
                2 => ForegroundColor::LightYellow,
                3 => ForegroundColor::LightGreen,
                4 => ForegroundColor::LightBlue,
                5 => ForegroundColor::Red,
                6 => ForegroundColor::Magenta,
                7 => ForegroundColor::Yellow,
                8 => ForegroundColor::Green,
                _ => ForegroundColor::Blue,
            },
        }
    }
}

fn to_identifier(index: i32) -> String {
    let mut dividend = index;
    let mut column_name = String::new();
    let mut modulo: u8;

    while dividend > 0 {
        modulo = ((dividend - 1) % 26) as u8;
        column_name.insert(0, (65 + modulo) as char);
        dividend = (dividend - modulo as i32) / 26;
    }

    return column_name;
}
