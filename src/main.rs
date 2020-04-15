use rustyline::Editor;
use termion::{color, style};

struct Card {
    name: String,
    identifier: String,
    foreground: String
}

fn main() {
    let mut rl = Editor::<()>::new();
    if rl.load_history("history.txt").is_err() {
        println!("No previous history.");
    }

    println!("Welcome to the Magewatch shell. Input 'help' to see commands or 'quit' to quit\n");

    loop {
        draw_game_state();

        let readline = rl.readline(">> ");
        match readline {
            Ok(line) => {
                rl.add_history_entry(line.as_str());
                if line.starts_with('q') {
                    break;
                } else {
                    handle_command(line);
                }
            }
            Err(_) => break,
        }
    }
    rl.save_history("history.txt").unwrap();
}

fn draw_game_state() {
    println!("{}═══════════════════════════════════ Hand ═══════════════════════════════════{}",
        style::Bold,
        style::Reset);
    render_card_row(vec![
        Card {
            name: "Demon Wolf".to_string(),
            identifier: "#A".to_string(),
            foreground: color::Green.fg_str().to_string()
        },
        Card {
            name: "Cyclops".to_string(),
            identifier: "#B".to_string(),
            foreground: color::Blue.fg_str().to_string()
        },
        Card {
            name: "Metalon".to_string(),
            identifier: "#C".to_string(),
            foreground: color::Red.fg_str().to_string()
        }
    ])
}

fn render_card_row(cards: Vec<Card>) {
    for _ in &cards {
        print!("┌────────┐");
    }
    println!();

    for index in 0..3 {
        for card in &cards {
            print!("│{}{}{}│",
                card.foreground,
                get_word_at_index(&card.name, index),
                style::Reset);
        }
        println!();
    }

    for card in &cards {
        print!("│{}{}{}│",
            card.foreground,
            get_word_at_index(&card.identifier, 0),
            style::Reset)
    }
    println!();

    for _ in &cards {
        print!("└────────┘");
    }
    println!();
}

fn get_word_at_index(string: &String, index: usize) -> String {
    let mut word = string.split(' ').nth(index).unwrap_or("").to_string();
    word.push_str("        ");
    word.truncate(8);
    return word;
}

fn handle_command(command: String) {
    if command.starts_with('h') {
        print_help();
    }
}

fn print_help() {
    println!(
        r#"
Commands for the Magewatch shell.

Notes:
    - All commands can be invoked using only their first letter as a
      mnemonic.
    - Cards and creatures are identified by alphabetic identifiers.
    - Attacker/defender positions are identified by numbers.
    - Arguments to commands can be repeated to perform the same command
      multiple times, for example 'play b d' requests to play the creature
      'b' followed by the creature 'd'.

Commands:
    [h]elp: Print this help message.
    [q]uit: Exit shell.
    [p]lay x: Play creature card with identifier x.
        Example: 'play b'
    [a]ttack x n: Designate creature x as an attacker in column n.
        Example: 'attack b 1'
    [d]efend x n: Designate creature x as a defender in column n.
        Example: 'defend c 2'
    [c]ast x (y): Cast spell x, optionally targeting creature y.
        Example: 'cast f'
        Example: 'cast f s'
    Empty Input: Proceed to the next game phase

The game takes place over a sequence of rounds, which are broken up into 5
phases. Each player proceeds through phases simultaneously, with their
decisions being revealed concurrently at the end of each phase. After each
phase, there is an opportunity for players to use "fast" effects before
proceeding to the next phase.

Game Round Structure:
    1) Start of Round: During this phase, players gain mana from their
       active mana crystals and each draw one card
    2) Choose Attackers: Players can assign up to 5 creatures to positions on
       their attacking line.
    3) Choose Defenders: Opposing attackers are revealed. Players can assign
       up to 5 creatures to positions on their defending line.
    4) Combat Phase: Attackers and defenders in the same column fight each other,
       dealing damage. Attackers with no assigned defender deal damage to
       the opposing castle.
    5) Main Phase: Players can place new creatures and cast spells
    "#
    );
}
