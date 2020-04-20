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

use crate::{
    card::{Card, CardVariant},
    combat,
    primitives::{CombatPosition, GamePhase, InterfaceError, PlayerName, Result},
    scenarios,
    state::{Game, PlayerState, Zone},
};

pub fn draw_interface_state(game: &Game) {
    if game.enemy.hand.len() > 0 {
        println!("{}{:═^80}{}", style::Bold, " Enemy Hand ", style::Reset);
        render_card_row(game.enemy.hand.iter().map(Some).collect(), true);
    }

    if game.enemy.reserve.len() > 0 {
        println!("{}{:═^80}{}", style::Bold, " Enemy Reserves ", style::Reset);
        render_card_row(game.enemy.reserve.iter().map(Some).collect(), false);
    }

    if game.enemy.attackers.len() > 0 || game.enemy.defenders.len() > 0 {
        println!(
            "{}{:═^80}{}",
            style::Bold,
            " Enemy Combatants ",
            style::Reset
        );
        render_card_row(combatants_vector(&game.enemy, PlayerName::Enemy), false);
    }

    if game.user.attackers.len() > 0 || game.user.defenders.len() > 0 {
        println!("{}{:═^80}{}", style::Bold, " Combatants ", style::Reset);
        render_card_row(combatants_vector(&game.user, PlayerName::User), false);
    }

    if game.user.reserve.len() > 0 {
        println!("{}{:═^80}{}", style::Bold, " Reserves ", style::Reset);
        render_card_row(game.user.reserve.iter().map(Some).collect(), false);
    }

    if game.user.hand.len() > 0 {
        println!("{}{:═^80}{}", style::Bold, " Hand ", style::Reset);
        render_card_row(game.user.hand.iter().map(Some).collect(), true);
    }

    println!(
        "{}",
        match game.state.phase {
            GamePhase::Attackers => "Attackers Phase. Add units to your Attack Group.",
            GamePhase::Defenders => "Defenders Phase. Add units to your Defense Group.",
            GamePhase::PreCombat => "Pre-Combat Phase. You may use fast effects.",
            GamePhase::Main => "Main Phase. Play units and cast spells.",
            GamePhase::End => "End Phase. You may use fast effects.",
        }
    );
}

fn combatants_vector(player: &PlayerState, player_name: PlayerName) -> Vec<Option<&Card>> {
    let mut attackers: Vec<Option<&Card>> = Vec::new();
    for i in 0..5 {
        attackers.push(
            if let Some(card) = player
                .attackers
                .iter()
                .find(|c| c.unit().position.map_or(false, |p| i == p as usize))
            {
                Some(card)
            } else {
                None
            },
        );
    }

    let mut defenders: Vec<Option<&Card>> = Vec::new();
    for i in 0..5 {
        defenders.push(
            if let Some(card) = player
                .defenders
                .iter()
                .find(|c| c.unit().position.map_or(false, |p| i == p as usize))
            {
                Some(card)
            } else {
                None
            },
        );
    }

    if player_name == PlayerName::User {
        attackers.append(&mut defenders);
        attackers
    } else {
        defenders.append(&mut attackers);
        defenders
    }
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
                    format!("{}", c.cost),
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
                c.variant.display_status(),
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

pub fn handle_command(command: String, game: &mut Game, player_name: PlayerName) -> Result<()> {
    let phase = game.state.phase;
    let player = game.player_mut(player_name);
    if command.starts_with('h') {
        print_help();
        Ok(())
    } else if command.starts_with('p') && phase == GamePhase::Main {
        handle_move_command(command, &mut player.hand, &mut player.reserve)
    } else if command.starts_with('a') && phase == GamePhase::Attackers {
        handle_move_command(command, &mut player.reserve, &mut player.attackers)
    } else if command.starts_with('d') && phase == GamePhase::Defenders {
        handle_move_command(command, &mut player.reserve, &mut player.defenders)
    } else if command.starts_with('e') {
        if let Some(index) = command.find(' ') {
            handle_command(command[index + 1..].to_string(), game, PlayerName::Enemy)
        } else {
            InterfaceError::result(format!(
                "Expected additional arguments to enemy command {}",
                command
            ))
        }
    } else if command.starts_with('l') {
        if let Some(index) = command.find(' ') {
            scenarios::load_scenario(game, command[index + 1..].to_string())
        } else {
            InterfaceError::result(format!("Expected additional arguments to load {}", command))
        }
    } else if command == "" {
        handle_advance_command(game)
    } else {
        InterfaceError::result(format!("Unknown command {}", command))
    }
}

fn handle_move_command(command: String, from: &mut Zone, to: &mut Zone) -> Result<()> {
    let parts = command.split(' ').collect::<Vec<&str>>();
    if let [_, identifier, position] = *parts {
        let card = PlayerState::find_card(identifier, from);
        let position = CombatPosition::parse(position)?;
        match &mut card.variant {
            CardVariant::Unit(u) => Ok(u.position = Some(position)),
            CardVariant::Spell => InterfaceError::result(format!(
                "Expected a unit card, but {} is a spell card",
                identifier
            )),
        }?;
        PlayerState::move_card(identifier, from, to)
    } else if let [_, identifier] = *parts {
        PlayerState::move_card(identifier, from, to)
    } else {
        Err(InterfaceError::new(format!(
            "Invalid move command: {}",
            command
        )))
    }
}

fn handle_advance_command(game: &mut Game) -> Result<()> {
    let has_fast_effect = game.user.hand.iter().any(|x| x.fast);
    loop {
        match game.state.phase {
            GamePhase::Attackers => {
                game.state.phase = GamePhase::Defenders;
                if game.enemy.attackers.len() > 0 || has_fast_effect {
                    break;
                }
            }
            GamePhase::Defenders => {
                game.state.phase = GamePhase::PreCombat;
                if has_fast_effect {
                    break;
                }
            }
            GamePhase::PreCombat => {
                combat::apply_combat(game);
                game.state.phase = GamePhase::Main;
                break;
            }
            GamePhase::Main => {
                game.state.phase = GamePhase::End;
                if has_fast_effect {
                    break;
                }
            }
            GamePhase::End => {
                game.state.phase = GamePhase::Attackers;
                if game.user.reserve.len() > 0 || has_fast_effect {
                    break;
                }
            }
        }

        if !game.state.auto_advance {
            break;
        }
    }

    Ok(())
}

pub fn print_error(message: String) {
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
  [l]oad x: Load the scenario named 'x'
      Example: 'load attackers'
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
