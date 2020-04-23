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

use crate::{
    combat,
    model::{CreatureState, Game, Target},
    primitives::{CombatPosition, GamePhase, InterfaceError, PlayerName, Result},
    scenarios, zones,
};

pub fn handle_command(command: &str, game: &mut Game, player_name: PlayerName) -> Result<()> {
    let phase = game.status.phase;
    let player = game.player_mut(player_name);

    if command.starts_with('h') {
        print_help();
        Ok(())
    } else if command.starts_with('p') && phase == GamePhase::Main {
        zones::play_card(player, &arg(&command, 1)?.to_uppercase(), &Target::None)
    } else if command.starts_with('a') && phase == GamePhase::Attackers {
        let position = CombatPosition::parse(arg(&command, 2)?)?;
        let index = player.find_creature(&arg(&command, 1)?.to_uppercase())?;
        player.creatures[index].state = CreatureState::Attacking(position);
        Ok(())
    } else if command.starts_with('d') && phase == GamePhase::Defenders {
        let position = CombatPosition::parse(arg(&command, 2)?)?;
        let index = player.find_creature(&arg(&command, 1)?.to_uppercase())?;
        player.creatures[index].state = CreatureState::Defending(position);
        Ok(())
    } else if command.starts_with('e') {
        let index = command.find(' ').ok_or(InterfaceError::new(format!(
            "Expected arguments: {}",
            command
        )))?;
        handle_command(&command[index + 1..], game, PlayerName::Enemy)
    } else if command.starts_with('l') {
        scenarios::load_scenario(game, arg(command, 1)?)
    } else if command == "" {
        advance_game(game)
    } else {
        InterfaceError::result(format!("Unknown command {}", command))
    }
}

pub fn arg(command: &str, i: usize) -> Result<&str> {
    command.split(' ').nth(i).ok_or(InterfaceError::new(format!(
        "Expected an argument to command {}",
        command
    )))
}

fn advance_game(game: &mut Game) -> Result<()> {
    match game.status.phase {
        GamePhase::Attackers => {
            game.status.phase = GamePhase::Defenders;
        }
        GamePhase::Defenders => {
            game.status.phase = GamePhase::PreCombat;
        }
        GamePhase::PreCombat => {
            combat::apply_combat(game);
            game.status.phase = GamePhase::Main;
        }
        GamePhase::Main => {
            game.status.phase = GamePhase::End;
        }
        GamePhase::End => {
            game.status.phase = GamePhase::Attackers;
        }
    }

    Ok(())
}

// fn handle_advance_command(game: &mut Game) -> Result<()> {
//     let has_fast_effect = game.user.hand.iter().any(|x| x.fast);
//     loop {
//         match game.state.phase {
//             GamePhase::Attackers => {
//                 game.state.phase = GamePhase::Defenders;
//                 if game.enemy.attackers.len() > 0 || has_fast_effect {
//                     break;
//                 }
//             }
//             GamePhase::Defenders => {
//                 game.state.phase = GamePhase::PreCombat;
//                 if has_fast_effect {
//                     break;
//                 }
//             }
//             GamePhase::PreCombat => {
//                 combat::apply_combat(game);
//                 game.state.phase = GamePhase::Main;
//                 break;
//             }
//             GamePhase::Main => {
//                 game.state.phase = GamePhase::End;
//                 if has_fast_effect {
//                     break;
//                 }
//             }
//             GamePhase::End => {
//                 game.state.phase = GamePhase::Attackers;
//                 if game.user.reserve.len() > 0 || has_fast_effect {
//                     break;
//                 }
//             }
//         }

//         if !game.state.auto_advance {
//             break;
//         }
//     }

//     Ok(())
// }

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
