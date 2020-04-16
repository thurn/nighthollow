use rustyline::Editor;
use termion::{color, style};

mod card;
use card::Card;

#[derive(Debug, PartialEq, Eq)]
enum GamePhase {
    Attackers,
    Defenders,
    PreCombat,
    Main,
    End,
}

struct InterfaceState {
    phase: GamePhase,
    hand: Vec<Card>,
    reserve: Vec<Card>,
    defenders: Vec<Card>,
    attackers: Vec<Card>,
}

fn main() {
    let mut rl = Editor::<()>::new();
    if rl.load_history("history.txt").is_err() {
        println!("No previous history.");
    }

    println!("Welcome to the Magewatch shell. Input 'help' to see commands or 'quit' to quit\n");
    let mut interface_state = InterfaceState {
        phase: GamePhase::Attackers,
        hand: vec![
            Card::new("Demon Wolf", "1F", 100),
            Card::new("Cyclops", "FF", 200),
            Card::new("Metalon", "2FF", 250),
        ],
        reserve: vec![],
        defenders: vec![],
        attackers: vec![],
    };

    loop {
        draw_interface_state(&interface_state);

        let readline = rl.readline(">> ");
        match readline {
            Ok(line) => {
                rl.add_history_entry(line.as_str());
                if line.starts_with('q') {
                    break;
                } else {
                    handle_command(line, &mut interface_state);
                }
            }
            Err(_) => break,
        }
    }
    rl.save_history("history.txt").unwrap();
}

fn draw_interface_state(interface_state: &InterfaceState) {
    if interface_state.attackers.len() > 0 {
        println!(
            "{}═════════════════════════════════ Attackers ═════════════════════════════════{}",
            style::Bold,
            style::Reset
        );
        render_card_row(&interface_state.attackers, false);
    }
    if interface_state.defenders.len() > 0 {
        println!(
            "{}═════════════════════════════════ Defenders ═════════════════════════════════{}",
            style::Bold,
            style::Reset
        );
        render_card_row(&interface_state.defenders, false);
    }
    if interface_state.reserve.len() > 0 {
        println!(
            "{}═════════════════════════════════ Reserves ═════════════════════════════════{}",
            style::Bold,
            style::Reset
        );
        render_card_row(&interface_state.reserve, false);
    }
    if interface_state.hand.len() > 0 {
        println!(
            "{}═══════════════════════════════════ Hand ═══════════════════════════════════{}",
            style::Bold,
            style::Reset
        );
        render_card_row(&interface_state.hand, true);
    }

    println!(
        "{}",
        match interface_state.phase {
            GamePhase::Attackers => "Attackers Phase. Add units to your Attack Group.",
            GamePhase::Defenders => "Defenders Phase. Add units to your Defense Group.",
            GamePhase::PreCombat => "Pre-Combat Phase. You may use fast effects.",
            GamePhase::Main => "Main Phase. Play units and cast spells.",
            GamePhase::End => "End Phase. You may use fast effects.",
        }
    );
}

fn render_card_row(cards: &Vec<Card>, include_cost: bool) {
    for _ in cards.iter() {
        print!("┌────────┐");
    }
    println!();

    if include_cost {
        for card in cards.iter() {
            print!("│{}{:<8.8}{}│", card.foreground, card.cost, style::Reset)
        }
        println!();
    }

    for index in 0..2 {
        for card in cards.iter() {
            print!(
                "│{}{:<8.8}{}│",
                card.foreground,
                get_word_at_index(&card.name, index),
                style::Reset
            );
        }
        println!();
    }

    for card in cards.iter() {
        print!(
            "│{}{:<4}{:>4.4}{}│",
            card.foreground,
            format!(
                "{:.0}%",
                100.0 * card.current_health as f64 / card.total_health as f64
            ),
            card.identifier,
            style::Reset
        )
    }
    println!();

    for _ in cards.iter() {
        print!("└────────┘");
    }
    println!();
}

fn get_word_at_index(string: &String, index: usize) -> String {
    string.split(' ').nth(index).unwrap_or("").to_string()
}

fn handle_command(command: String, interface_state: &mut InterfaceState) {
    if command.starts_with('h') {
        print_help();
    } else if command.starts_with('p') && interface_state.phase == GamePhase::Main {
        handle_play_command(command, interface_state);
    } else if command.starts_with('a') && interface_state.phase == GamePhase::Attackers {
        handle_attack_defend_command(command, interface_state, true);
    } else if command.starts_with('d') && interface_state.phase == GamePhase::Defenders {
        handle_attack_defend_command(command, interface_state, false);
    } else if command == "" {
        handle_advance_command(interface_state);
    } else {
        error1("Invalid command", &command);
    }
}

fn handle_play_command(command: String, interface_state: &mut InterfaceState) {
    if let Some(identifier) = command.split(' ').nth(1) {
        move_card(
            identifier,
            &mut interface_state.hand,
            &mut interface_state.reserve,
            "0",
        );
    } else {
        error("Expected argument to command");
    }
}

fn handle_attack_defend_command(
    command: String,
    interface_state: &mut InterfaceState,
    is_attack: bool,
) {
    if let [_, identifier, position] = command.split(' ').collect::<Vec<&str>>()[..] {
        move_card(
            identifier,
            &mut interface_state.reserve,
            if is_attack {
                &mut interface_state.attackers
            } else {
                &mut interface_state.defenders
            },
            position,
        );
    } else {
        error1("Invalid attack/defend command", &command);
    }
}

fn handle_advance_command(interface_state: &mut InterfaceState) {
    use GamePhase::*;
    interface_state.phase = match interface_state.phase {
        Attackers => Defenders,
        Defenders => PreCombat,
        PreCombat => Main,
        Main => End,
        End => Attackers,
    };
}

fn move_card(identifier: &str, source: &mut Vec<Card>, destination: &mut Vec<Card>, index: &str) {
    if let Some(card_position) = source
        .iter()
        .position(|x| x.identifier == identifier.to_uppercase())
    {
        match index.parse::<usize>() {
            Ok(position) if position <= destination.len() => {
                destination.insert(position, source.remove(card_position))
            }
            _ => error1("Invalid position", index),
        }
    } else {
        error1("Identifier not found", identifier);
    }
}

fn error(message: &str) {
    eprintln!(
        "{}{}ERROR: {}{}",
        style::Bold,
        color::Fg(color::Red),
        message,
        style::Reset
    );
}

fn error1(message: &str, argument: &str) {
    eprintln!(
        "{}{}ERROR: {} '{}'{}",
        style::Bold,
        color::Fg(color::Red),
        message,
        argument,
        style::Reset
    );
}

fn print_help() {
    println!(
        r#"
Commands for the Magewatch shell.

Notes:
    - Commands are case-insensitive
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

The game takes place over a sequence of rounds, which are broken up into
phases. Each player proceeds through phases simultaneously, with their
decisions being revealed concurrently at the end of each phase.

Game Round Structure:
    1) Attackers Phase: Players gain mana for their active mana crystals and
       draw one card. Each player can assign up to 5 creatures to positions on
       their attacking line. They may use cards tagged as "fast".
    2) Defenders Phase: Opposing attackers are revealed. Players can assign up
       to 5 creatures to positions on their defending line. They may use cards
       tagged as "fast".
    3) Pre-Combat Phase: Defenders are revealed. Players may use cards tagged as
       "fast".
    4) Main Phase: Attackers and defenders in the same column fight each other,
       dealing damage. Attackers with no assigned defender deal damage to
       the opposing castle. Afterwards, players can place new creatures and
       cast spells ("fast" or regular)
    5) End Phase: Players may use cards tagged as "fast".
    "#
    );
}
