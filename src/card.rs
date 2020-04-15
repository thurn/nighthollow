use std::fmt::Display;
use std::sync::atomic::{AtomicI32, Ordering};
use termion::color;

static NEXT_IDENTIFIER_INDEX: AtomicI32 = AtomicI32::new(1);

pub struct Card {
    pub cost: String,
    pub name: String,
    pub identifier: String,
    pub total_health: i32,
    pub current_health: i32,
    pub foreground: Box<dyn Display>,
}

impl Card {
    pub fn new(name: &str, cost: &str, health: i32) -> Card {
        let index = NEXT_IDENTIFIER_INDEX.fetch_add(1, Ordering::Relaxed);
        let foreground: Box<dyn Display> = match index % 11 {
            0 => Box::from(color::Fg(color::LightRed)),
            1 => Box::from(color::Fg(color::Cyan)),
            2 => Box::from(color::Fg(color::LightMagenta)),
            3 => Box::from(color::Fg(color::LightYellow)),
            4 => Box::from(color::Fg(color::LightCyan)),
            5 => Box::from(color::Fg(color::LightGreen)),
            6 => Box::from(color::Fg(color::Green)),
            7 => Box::from(color::Fg(color::Blue)),
            8 => Box::from(color::Fg(color::LightBlue)),
            9 => Box::from(color::Fg(color::Magenta)),
            _ => Box::from(color::Fg(color::Red)),
        };

        Card {
            cost: cost.to_string(),
            total_health: health,
            current_health: health,
            name: name.to_string(),
            identifier: to_identifier(index),
            foreground,
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
