use std::fmt::Display;
use std::sync::atomic::{AtomicI32, Ordering};

use serde::{Deserialize, Serialize};
use termion::color;

use crate::primitives::ManaValue;
use crate::unit::Unit;

static NEXT_IDENTIFIER_INDEX: AtomicI32 = AtomicI32::new(1);

#[derive(Serialize, Deserialize, Debug)]
pub struct ManaCost {
    mana: ManaValue,
}

pub enum Cost {
    ManaCost(ManaCost),
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
pub enum Variant {
    Unit(Unit),
}

impl Variant {
    pub fn display_status(&self) -> String {
        match self {
            Variant::Unit(unit) => unit.display_health(),
        }
    }
}

#[derive(Serialize, Deserialize, Debug)]
pub struct Card {
    pub cost: String,
    pub name: String,
    pub identifier: String,
    pub variant: Variant,
    pub fast: bool,
    pub foreground: ForegroundColor,
}

impl Card {
    pub fn new_unit(name: &str, cost: &str, health: i32, attack: i32) -> Card {
        let index = NEXT_IDENTIFIER_INDEX.fetch_add(1, Ordering::Relaxed);

        Card {
            cost: cost.to_string(),
            variant: Variant::Unit(Unit::new(health, attack)),
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
