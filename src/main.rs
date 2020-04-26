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

#![deny(warnings)]
#![allow(dead_code)]
#![allow(unused_variables)]
#![allow(unused_imports)]

use std::fs::File;
use std::io::BufReader;

use rustyline::Editor;
use serde_json::{de, ser};
use termion::{color, style};

#[macro_use]
extern crate lazy_static;

mod attributes;
mod combat;
mod commands;
mod effect;
mod gameplay;
mod model;
mod primitives;
mod render;
mod scenarios;
mod unit;

use model::Game;
use primitives::PlayerName;

fn main() {
    let mut rl = Editor::<()>::new();
    if rl.load_history("history.txt").is_err() {
        println!("No previous history.");
    }

    println!("Welcome to the Magewatch shell. Input 'help' to see commands or 'quit' to quit\n");
    let mut game = Game::default();

    if let Ok(input_file) = File::open("state.json") {
        if let Ok(read) = de::from_reader(BufReader::new(input_file)) {
            game = read;
        }
    }

    loop {
        let file = File::create("state.json").expect("Unable to open state.json!");
        ser::to_writer_pretty(&file, &game).expect("Error writing to state.json!");

        render::draw_interface_state(&game);

        let readline = rl.readline(">> ");
        match readline {
            Ok(line) => {
                rl.add_history_entry(line.as_str());
                if line.starts_with('q') {
                    break;
                } else {
                    if let Err(e) = commands::handle_command(&line, &mut game, PlayerName::User) {
                        eprintln!(
                            "{}{}ERROR: {}{}",
                            style::Bold,
                            color::Fg(color::Red),
                            e,
                            style::Reset
                        );
                    }
                }
            }
            Err(_) => break,
        }
    }
    rl.save_history("history.txt").unwrap();
}
