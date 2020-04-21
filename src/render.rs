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

use termion::{color, style};

use std::{
    collections::hash_map::DefaultHasher,
    fmt::Display,
    hash::{Hash, Hasher},
    iter::Filter,
    slice::Iter,
};

use crate::{
    model::{Card, CardVariant, Creature, CreatureState, Game, Player},
    primitives::{GamePhase, PlayerName},
};

pub fn draw_interface_state(game: &Game) {
    println!(
        "{}Enemy: {}/{}{}",
        color::Fg(color::Red),
        game.enemy.status.current_health,
        game.enemy.status.maximum_health,
        style::Reset
    );

    if game.enemy.hand.len() > 0 {
        println!("{}{:═^80}{}", style::Bold, " Enemy Hand ", style::Reset);
        render_card_row(game.enemy.hand.iter().map(Some).collect(), true);
    }

    if game.enemy.non_combatants().count() > 0 {
        println!("{}{:═^80}{}", style::Bold, " Enemy Reserves ", style::Reset);
        render_card_row(game.enemy.non_combatants().map(Some).collect(), false);
    }

    if game.enemy.attackers().count() > 0 || game.enemy.defenders().count() > 0 {
        println!(
            "{}{:═^80}{}",
            style::Bold,
            " Enemy Combatants ",
            style::Reset
        );
        render_card_row(combatants_vector(&game.enemy, PlayerName::Enemy), false);
    }

    if game.user.attackers().count() > 0 || game.user.defenders().count() > 0 {
        println!("{}{:═^80}{}", style::Bold, " Combatants ", style::Reset);
        render_card_row(combatants_vector(&game.user, PlayerName::User), false);
    }

    if game.user.non_combatants().count() > 0 {
        println!("{}{:═^80}{}", style::Bold, " Reserves ", style::Reset);
        render_card_row(game.user.non_combatants().map(Some).collect(), false);
    }

    if game.user.hand.len() > 0 {
        println!("{}{:═^80}{}", style::Bold, " Hand ", style::Reset);
        render_card_row(game.user.hand.iter().map(Some).collect(), true);
    }

    println!(
        "{}User: {}/{}{}",
        color::Fg(color::Red),
        game.user.status.current_health,
        game.user.status.maximum_health,
        style::Reset
    );

    println!(
        "{}",
        match game.status.phase {
            GamePhase::Attackers => "Attackers Phase. Add units to your Attack Group.",
            GamePhase::Defenders => "Defenders Phase. Add units to your Defense Group.",
            GamePhase::PreCombat => "Pre-Combat Phase. You may use fast effects.",
            GamePhase::Main => "Main Phase. Play units and cast spells.",
            GamePhase::End => "End Phase. You may use fast effects.",
        }
    );
}

fn combatants_vector(
    player: &Player,
    player_name: PlayerName,
) -> Vec<Option<&impl CardRowElement>> {
    let mut attackers = Vec::new();
    for i in 0..5 {
        attackers.push(player.attackers().find_map(
            |(c, p)| {
                if p as usize == i {
                    Some(c)
                } else {
                    None
                }
            },
        ));
    }

    let mut defenders = Vec::new();
    for i in 0..5 {
        defenders.push(player.defenders().find_map(
            |(c, p)| {
                if p as usize == i {
                    Some(c)
                } else {
                    None
                }
            },
        ));
    }

    if player_name == PlayerName::User {
        attackers.append(&mut defenders);
        attackers
    } else {
        defenders.append(&mut attackers);
        defenders
    }
}

trait CardRowElement {
    fn card(&self) -> &Card;
    fn as_creature(&self) -> Option<&Creature>;
}

impl CardRowElement for CardVariant {
    fn card(&self) -> &Card {
        CardVariant::card(self)
    }

    fn as_creature(&self) -> Option<&Creature> {
        match self {
            CardVariant::Creature(c) => Some(c),
            _ => None,
        }
    }
}

impl CardRowElement for Creature {
    fn card(&self) -> &Card {
        &self.card
    }

    fn as_creature(&self) -> Option<&Creature> {
        Some(self)
    }
}

fn render_card_row(cards: Vec<Option<&impl CardRowElement>>, include_cost: bool) {
    for card in &cards {
        if let Some(_) = card {
            print!("┌────────┐");
        } else {
            print!("          ");
        }
    }
    println!();

    if include_cost {
        for card in &cards {
            if let Some(c) = card {
                print!(
                    "│{}{:<8.8}{}│",
                    get_terminal_color(&c.card().id),
                    format!("{}", c.card().cost),
                    style::Reset
                )
            } else {
                print!("          ");
            }
        }
        println!();
    }

    for index in 0..2 {
        for card in &cards {
            if let Some(c) = card {
                print!(
                    "│{}{:<8.8}{}│",
                    get_terminal_color(&c.card().id),
                    get_word_at_index(&c.card().name, index),
                    style::Reset
                );
            } else {
                print!("          ");
            }
        }
        println!();
    }

    for card in &cards {
        if let Some(c) = *card {
            print!(
                "│{}{:<4}{:>4.4}{}│",
                get_terminal_color(&c.card().id),
                render_status(c),
                c.card().id,
                style::Reset
            )
        } else {
            print!("          ");
        }
    }
    println!();

    for card in &cards {
        if let Some(_) = card {
            print!("└────────┘");
        } else {
            print!("          ");
        }
    }
    println!();
}

fn get_word_at_index(string: &String, index: usize) -> String {
    string.split(' ').nth(index).unwrap_or("").to_string()
}

fn get_terminal_color(identifier: &String) -> Box<dyn Display> {
    let mut hasher = DefaultHasher::new();
    identifier.hash(&mut hasher);
    match hasher.finish() % 10 {
        0 => Box::from(color::Fg(color::LightRed)),
        1 => Box::from(color::Fg(color::LightMagenta)),
        2 => Box::from(color::Fg(color::LightYellow)),
        3 => Box::from(color::Fg(color::LightGreen)),
        4 => Box::from(color::Fg(color::LightBlue)),
        5 => Box::from(color::Fg(color::Red)),
        6 => Box::from(color::Fg(color::Magenta)),
        7 => Box::from(color::Fg(color::Yellow)),
        8 => Box::from(color::Fg(color::Green)),
        _ => Box::from(color::Fg(color::Blue)),
    }
}

fn render_status(card: &impl CardRowElement) -> String {
    match card.as_creature() {
        Some(c) => format!(
            "{:.0}%",
            100.0 * i32::from(c.current_health) as f64 / i32::from(c.maximum_health) as f64
        ),
        _ => String::from(""),
    }
}
