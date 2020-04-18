#![allow(dead_code)]

use std::error;
use std::fmt;
use std::fs::File;
use std::io::BufReader;

use rustyline::Editor;
use serde::{Deserialize, Serialize};
use serde_json::{de, ser};
use termion::{color, style};

mod card;
use card::Card;

type Result<T> = std::result::Result<T, Box<dyn error::Error>>;

#[derive(Serialize, Deserialize, Debug, PartialEq, Eq, Copy, Clone)]
enum PlayerName {
    Player,
    Enemy,
}

#[derive(Serialize, Deserialize, Debug, PartialEq, Eq, Copy, Clone)]
enum Zone {
    Hand,
    Reserves,
    Attackers,
    Defenders,
}

#[derive(Serialize, Deserialize, Debug, PartialEq, Eq, Copy, Clone)]
enum GamePhase {
    Attackers,
    Defenders,
    PreCombat,
    Main,
    End,
}

#[derive(Debug)]
struct InterfaceError {
    message: String,
}

impl InterfaceError {
    fn new(message: String) -> Box<InterfaceError> {
        Box::from(InterfaceError {
            message: message.to_string(),
        })
    }

    fn result(message: String) -> Result<()> {
        Err(InterfaceError::new(message))
    }
}

impl fmt::Display for InterfaceError {
    fn fmt(&self, f: &mut fmt::Formatter) -> fmt::Result {
        write!(f, "{}", self.message)
    }
}

impl error::Error for InterfaceError {
    fn source(&self) -> Option<&(dyn error::Error + 'static)> {
        None
    }
}

#[derive(Serialize, Deserialize, Debug)]
struct PlayerState {
    mana: i32,
    hand: Vec<Card>,
    reserve: Vec<Card>,
    defenders: Vec<Card>,
    attackers: Vec<Card>,
}

impl PlayerState {
    fn cards_in_zone(&self, zone: Zone) -> &[Card] {
        match zone {
            Zone::Hand => &self.hand,
            Zone::Reserves => &self.reserve,
            Zone::Attackers => &self.attackers,
            Zone::Defenders => &self.defenders,
        }
    }

    fn cards_in_zone_mut(&mut self, zone: Zone) -> &mut Vec<Card> {
        match zone {
            Zone::Hand => &mut self.hand,
            Zone::Reserves => &mut self.reserve,
            Zone::Attackers => &mut self.attackers,
            Zone::Defenders => &mut self.defenders,
        }
    }

    fn find_card_position(&self, identifier: &str, zone: Zone) -> Result<usize> {
        self.cards_in_zone(zone)
            .iter()
            .position(|x| x.identifier == identifier.to_uppercase())
            .ok_or(InterfaceError::new(format!(
                "Identifier not found {:?} for zone {:?} of player {:?}",
                identifier, zone, self
            )))
    }
}

#[derive(Serialize, Deserialize, Debug)]
struct InterfaceOptions {
    auto_advance: bool,
}

#[derive(Serialize, Deserialize, Debug)]
struct InterfaceState {
    options: InterfaceOptions,
    phase: GamePhase,
    player: PlayerState,
    enemy: PlayerState,
}

impl InterfaceState {
    fn get_mana(&self, player_name: PlayerName) -> i32 {
        match player_name {
            PlayerName::Player => self.player.mana,
            PlayerName::Enemy => self.enemy.mana,
        }
    }

    fn move_card(
        &mut self,
        identifier: &str,
        index: usize,
        from: Zone,
        to: Zone,
        player: PlayerName,
    ) -> Result<()> {
        let player = self.player_mut(player);
        if index <= player.cards_in_zone(to).len() {
            let position = player.find_card_position(identifier, from)?;
            let removed = player.cards_in_zone_mut(from).remove(position);
            player.cards_in_zone_mut(to).insert(index, removed);
            Ok(())
        } else {
            InterfaceError::result(format!(
                "Position {:?} is outside range for player {:?} zone {:?}",
                index, player, to
            ))
        }
    }

    fn player(&self, player: PlayerName) -> &PlayerState {
        match player {
            PlayerName::Player => &self.player,
            PlayerName::Enemy => &self.enemy,
        }
    }

    fn player_mut(&mut self, player: PlayerName) -> &mut PlayerState {
        match player {
            PlayerName::Player => &mut self.player,
            PlayerName::Enemy => &mut self.enemy,
        }
    }
}

fn main() {
    let mut rl = Editor::<()>::new();
    if rl.load_history("history.txt").is_err() {
        println!("No previous history.");
    }

    println!("Welcome to the Magewatch shell. Input 'help' to see commands or 'quit' to quit\n");
    let mut interface_state = starting_game_state();

    if let Ok(input_file) = File::open("state.json") {
        if let Ok(state) = de::from_reader(BufReader::new(input_file)) {
            interface_state = state;
        }
    }

    loop {
        let file = File::create("state.json").expect("Unable to open state.json!");
        ser::to_writer_pretty(&file, &interface_state).expect("Error writing to state.json!");

        draw_interface_state(&interface_state);

        let readline = rl.readline(">> ");
        match readline {
            Ok(line) => {
                rl.add_history_entry(line.as_str());
                if line.starts_with('q') {
                    break;
                } else {
                    if let Err(e) = handle_command(line, &mut interface_state, PlayerName::Player) {
                        print_error(format!("{}", e))
                    }
                }
            }
            Err(_) => break,
        }
    }
    rl.save_history("history.txt").unwrap();
}

fn starting_game_state() -> InterfaceState {
    InterfaceState {
        options: InterfaceOptions {
            auto_advance: false,
        },
        phase: GamePhase::Main,
        player: PlayerState {
            mana: 0,
            hand: vec![
                Card::new("Demon Wolf", "1F", 100),
                Card::new("Cyclops", "FF", 200),
                Card::new("Metalon", "2FF", 250),
            ],
            reserve: vec![],
            defenders: vec![],
            attackers: vec![],
        },
        enemy: PlayerState {
            mana: 0,
            hand: vec![
                Card::new("Demon Wolf", "1F", 100),
                Card::new("Cyclops", "FF", 200),
                Card::new("Metalon", "2FF", 250),
            ],
            reserve: vec![],
            defenders: vec![],
            attackers: vec![],
        },
    }
}

fn draw_interface_state(interface_state: &InterfaceState) {
    if interface_state.enemy.hand.len() > 0 {
        println!("{}{:═^80}{}", style::Bold, " Enemy Hand ", style::Reset);
        render_card_row(interface_state.enemy.hand.iter().map(Some).collect(), true);
    }

    if interface_state.enemy.reserve.len() > 0 {
        println!("{}{:═^80}{}", style::Bold, " Enemy Reserves ", style::Reset);
        render_card_row(
            interface_state.enemy.reserve.iter().map(Some).collect(),
            false,
        );
    }

    if interface_state.enemy.attackers.len() > 0 || interface_state.enemy.defenders.len() > 0 {
        println!(
            "{}{:═^80}{}",
            style::Bold,
            " Enemy Combatants ",
            style::Reset
        );
        render_card_row(combatants_vector(&interface_state.enemy), false);
    }

    if interface_state.player.attackers.len() > 0 || interface_state.player.defenders.len() > 0 {
        println!("{}{:═^80}{}", style::Bold, " Combatants ", style::Reset);
        render_card_row(combatants_vector(&interface_state.player), false);
    }

    if interface_state.player.reserve.len() > 0 {
        println!("{}{:═^80}{}", style::Bold, " Reserves ", style::Reset);
        render_card_row(
            interface_state.player.reserve.iter().map(Some).collect(),
            false,
        );
    }

    if interface_state.player.hand.len() > 0 {
        println!("{}{:═^80}{}", style::Bold, " Hand ", style::Reset);
        render_card_row(interface_state.player.hand.iter().map(Some).collect(), true);
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

fn combatants_vector(player: &PlayerState) -> Vec<Option<&Card>> {
    let mut result = Vec::new();
    for i in 0..5 {
        result.push(if i < player.attackers.len() {
            Some(&player.attackers[i])
        } else {
            None
        });
    }

    for i in 0..5 {
        result.push(if i < player.defenders.len() {
            Some(&player.defenders[i])
        } else {
            None
        });
    }
    result
}

fn render_card_row<'a>(cards: Vec<Option<&Card>>, include_cost: bool) {
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
                    c.foreground.to_terminal_color(),
                    c.cost,
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
                    c.foreground.to_terminal_color(),
                    get_word_at_index(&c.name, index),
                    style::Reset
                );
            } else {
                print!("          ");
            }
        }
        println!();
    }

    for card in &cards {
        if let Some(c) = card {
            print!(
                "│{}{:<4}{:>4.4}{}│",
                c.foreground.to_terminal_color(),
                format!(
                    "{:.0}%",
                    100.0 * c.current_health as f64 / c.total_health as f64
                ),
                c.identifier,
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

fn handle_command(
    command: String,
    interface_state: &mut InterfaceState,
    player: PlayerName,
) -> Result<()> {
    if command.starts_with('h') {
        print_help();
        Ok(())
    } else if command.starts_with('p') && interface_state.phase == GamePhase::Main {
        handle_move_command(command, interface_state, Zone::Hand, Zone::Reserves, player)
    } else if command.starts_with('a') && interface_state.phase == GamePhase::Attackers {
        handle_move_command(
            command,
            interface_state,
            Zone::Reserves,
            Zone::Attackers,
            player,
        )
    } else if command.starts_with('d') && interface_state.phase == GamePhase::Defenders {
        handle_move_command(
            command,
            interface_state,
            Zone::Reserves,
            Zone::Defenders,
            player,
        )
    } else if command == "" {
        handle_advance_command(interface_state)
    } else if command.starts_with('e') {
        if let Some(index) = command.find(' ') {
            handle_command(
                command[index + 1..].to_string(),
                interface_state,
                PlayerName::Enemy,
            )
        } else {
            InterfaceError::result(format!(
                "Expected additional arguments to command {}",
                command
            ))
        }
    } else {
        InterfaceError::result(format!("Unknown command {}", command))
    }
}

fn handle_move_command(
    command: String,
    interface_state: &mut InterfaceState,
    from: Zone,
    to: Zone,
    player: PlayerName,
) -> Result<()> {
    let parts = command.split(' ').collect::<Vec<&str>>();
    if let [_, identifier, position] = *parts {
        let p = position.parse::<usize>()?;
        interface_state.move_card(identifier, p, from, to, player)
    } else if let [_, identifier] = *parts {
        interface_state.move_card(identifier, 0, from, to, player)
    } else {
        Err(InterfaceError::new(format!(
            "Invalid move command: {}",
            command
        )))
    }
}

fn handle_advance_command(interface_state: &mut InterfaceState) -> Result<()> {
    let has_fast_effect = interface_state.player.hand.iter().any(|x| x.fast);
    loop {
        match interface_state.phase {
            GamePhase::Attackers => {
                interface_state.phase = GamePhase::Defenders;
                if interface_state.enemy.attackers.len() > 0 || has_fast_effect {
                    break;
                }
            }
            GamePhase::Defenders => {
                interface_state.phase = GamePhase::PreCombat;
                if has_fast_effect {
                    break;
                }
            }
            GamePhase::PreCombat => {
                interface_state.phase = GamePhase::Main;
                break;
            }
            GamePhase::Main => {
                interface_state.phase = GamePhase::End;
                if has_fast_effect {
                    break;
                }
            }
            GamePhase::End => {
                interface_state.phase = GamePhase::Attackers;
                if interface_state.player.reserve.len() > 0 || has_fast_effect {
                    break;
                }
            }
        }

        if !interface_state.options.auto_advance {
            break;
        }
    }

    Ok(())
}

fn print_error(message: String) {
    eprintln!(
        "{}{}ERROR: {}{}",
        style::Bold,
        color::Fg(color::Red),
        message,
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
    [e]nemy <command>: Invoke another command, but it apply it to the opponent
    [r]eset: Reset saved state to default
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
