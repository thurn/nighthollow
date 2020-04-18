#![allow(dead_code)]
use std::fs::File;
use std::io::BufReader;

use rustyline::Editor;
use serde_json::{de, ser};

mod card;
mod render;
mod state;

use state::PlayerName;

fn main() {
    let mut rl = Editor::<()>::new();
    if rl.load_history("history.txt").is_err() {
        println!("No previous history.");
    }

    println!("Welcome to the Magewatch shell. Input 'help' to see commands or 'quit' to quit\n");
    let mut interface_state = state::initial_state();

    if let Ok(input_file) = File::open("state.json") {
        if let Ok(state) = de::from_reader(BufReader::new(input_file)) {
            interface_state = state;
        }
    }

    loop {
        let file = File::create("state.json").expect("Unable to open state.json!");
        ser::to_writer_pretty(&file, &interface_state).expect("Error writing to state.json!");

        render::draw_interface_state(&interface_state);

        let readline = rl.readline(">> ");
        match readline {
            Ok(line) => {
                rl.add_history_entry(line.as_str());
                if line.starts_with('q') {
                    break;
                } else {
                    if let Err(e) =
                        render::handle_command(line, &mut interface_state, PlayerName::Player)
                    {
                        render::print_error(format!("{}", e))
                    }
                }
            }
            Err(_) => break,
        }
    }
    rl.save_history("history.txt").unwrap();
}
