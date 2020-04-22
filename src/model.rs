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

use std::fmt::{Display, Formatter};

use serde::{Deserialize, Serialize};

use crate::primitives::{
    CombatPosition, GamePhase, HealthValue, Influence, InterfaceError, ManaValue, PlayerName,
    Result, School,
};

#[derive(Serialize, Deserialize, Debug)]
pub struct Game {
    pub status: GameStatus,
    pub user: Player,
    pub enemy: Player,
}

impl Game {
    pub fn player(&self, name: PlayerName) -> &Player {
        match name {
            PlayerName::User => &self.user,
            PlayerName::Enemy => &self.enemy,
        }
    }

    pub fn player_mut(&mut self, name: PlayerName) -> &mut Player {
        match name {
            PlayerName::User => &mut self.user,
            PlayerName::Enemy => &mut self.enemy,
        }
    }
}

impl Default for Game {
    fn default() -> Self {
        Game {
            status: GameStatus {
                phase: GamePhase::Main,
            },
            user: Player::default(),
            enemy: Player::default(),
        }
    }
}

#[derive(Serialize, Deserialize, Debug)]
pub struct GameStatus {
    pub phase: GamePhase,
}

#[derive(Serialize, Deserialize, Debug)]
pub struct Player {
    pub status: PlayerStatus,
    pub hand: Vec<CardVariant>,
    pub creatures: Vec<Creature>,
    pub crystals: Vec<Crystal>,
    pub structures: Vec<Structure>,
    pub discard: Vec<CardVariant>,
}

pub type Attacker<'a> = (&'a Creature, CombatPosition);
pub type Defender<'a> = (&'a Creature, CombatPosition);
pub type AttackerMut<'a> = (&'a mut Creature, CombatPosition);
pub type DefenderMut<'a> = (&'a mut Creature, CombatPosition);

impl Player {
    pub fn attackers(&self) -> impl Iterator<Item = Attacker> {
        self.creatures.iter().filter_map(|c| match c.state {
            CreatureState::Attacking(p) => Some((c, p)),
            _ => None,
        })
    }

    pub fn attackers_mut(&mut self) -> impl Iterator<Item = AttackerMut> {
        self.creatures.iter_mut().filter_map(|c| match c.state {
            CreatureState::Attacking(p) => Some((c, p)),
            _ => None,
        })
    }

    pub fn defenders(&self) -> impl Iterator<Item = Defender> {
        self.creatures.iter().filter_map(|c| match c.state {
            CreatureState::Defending(p) => Some((c, p)),
            _ => None,
        })
    }

    pub fn defenders_mut(&mut self) -> impl Iterator<Item = DefenderMut> {
        self.creatures.iter_mut().filter_map(|c| match c.state {
            CreatureState::Defending(p) => Some((c, p)),
            _ => None,
        })
    }

    pub fn non_combatants(&self) -> impl Iterator<Item = &Creature> {
        self.creatures
            .iter()
            .filter(|c| matches!(c.state, CreatureState::Default | CreatureState::Stunned))
    }

    pub fn non_combatants_mut(&mut self) -> impl Iterator<Item = &mut Creature> {
        self.creatures
            .iter_mut()
            .filter(|c| matches!(c.state, CreatureState::Default | CreatureState::Stunned))
    }

    pub fn play_permanent(&mut self, identifier: &str) -> Result<()> {
        let card_position = self
            .hand
            .iter()
            .position(|c| match c {
                CardVariant::Spell(_) => false,
                _ => c.card().id == identifier,
            })
            .ok_or(InterfaceError::new(format!(
                "Permanent card not found with ID {}",
                identifier
            )))?;
        let card = self.hand.remove(card_position);
        match card {
            CardVariant::Creature(c) => Ok(self.creatures.push(c)),
            CardVariant::Crystal(c) => Ok(self.crystals.push(c)),
            CardVariant::Structure(s) => Ok(self.structures.push(s)),
            CardVariant::Spell(s) => panic!("Value cannot be a spell here"),
        }
    }

    pub fn set_creature_state(&mut self, identifier: &str, state: CreatureState) -> Result<()> {
        self.creatures
            .iter_mut()
            .find(|x| x.card.id == identifier)
            .map_or(
                InterfaceError::result(format!("Creature not found: {}", identifier)),
                |c| Ok(c.state = state),
            )
    }
}

impl Default for Player {
    fn default() -> Self {
        Player {
            status: PlayerStatus::default(),
            hand: vec![],
            creatures: vec![],
            crystals: vec![],
            structures: vec![],
            discard: vec![],
        }
    }
}

#[derive(Serialize, Deserialize, Debug)]
pub struct PlayerStatus {
    pub mana: ManaValue,
    pub influence: Influence,
    pub current_health: HealthValue,
    pub maximum_health: HealthValue,
}

impl Default for PlayerStatus {
    fn default() -> Self {
        PlayerStatus {
            mana: ManaValue::from(0),
            influence: Influence::default(),
            current_health: HealthValue::from(100),
            maximum_health: HealthValue::from(100),
        }
    }
}

#[derive(Serialize, Deserialize, Debug)]
pub enum CardVariant {
    Creature(Creature),
    Spell(Spell),
    Structure(Structure),
    Crystal(Crystal),
}

impl CardVariant {
    pub fn card(&self) -> &Card {
        match self {
            CardVariant::Creature(u) => &u.card,
            CardVariant::Spell(s) => &s.card,
            CardVariant::Structure(s) => &s.card,
            CardVariant::Crystal(c) => &c.card,
        }
    }

    pub fn card_mut(&mut self) -> &mut Card {
        match self {
            CardVariant::Creature(u) => &mut u.card,
            CardVariant::Spell(s) => &mut s.card,
            CardVariant::Structure(s) => &mut s.card,
            CardVariant::Crystal(c) => &mut c.card,
        }
    }
}

#[derive(Serialize, Deserialize, Debug)]
pub struct Card {
    pub id: String,
    pub cost: Cost,
    pub name: String,
    pub school: School,
}

#[derive(Serialize, Deserialize, Debug)]
pub enum Cost {
    None,
    ManaCost(ManaCost),
}

impl Display for Cost {
    fn fmt(&self, f: &mut Formatter<'_>) -> std::fmt::Result {
        match self {
            Cost::None => Ok(()),
            Cost::ManaCost(mana_cost) => write!(f, "{}", mana_cost),
        }
    }
}

#[derive(Serialize, Deserialize, Debug)]
pub struct ManaCost {
    pub mana: ManaValue,
    pub influence: Influence,
}

impl Display for ManaCost {
    fn fmt(&self, f: &mut Formatter<'_>) -> std::fmt::Result {
        write!(f, "{} {}", self.mana, self.influence)
    }
}

#[derive(Serialize, Deserialize, Debug)]
pub struct Creature {
    pub card: Card,
    pub state: CreatureState,
    pub current_health: HealthValue,
    pub maximum_health: HealthValue,
    pub attacks: Vec<Attack>,
}

impl Creature {
    pub fn expect_attacking(&self) -> CombatPosition {
        match self.state {
            CreatureState::Attacking(p) => p,
            _ => panic!("Expected an attacking creature but got {:?}", self),
        }
    }

    pub fn expect_defending(&self) -> CombatPosition {
        match self.state {
            CreatureState::Defending(p) => p,
            _ => panic!("Expected a defending creature but got {:?}", self),
        }
    }
}

pub trait Attackable {
    fn apply_health_change(&mut self, value: HealthValue);
}

impl Attackable for PlayerStatus {
    fn apply_health_change(&mut self, value: HealthValue) {
        self.current_health += value
    }
}

impl Attackable for Creature {
    fn apply_health_change(&mut self, value: HealthValue) {
        self.current_health += value
    }
}

#[derive(Serialize, Deserialize, Debug)]
pub enum CreatureState {
    Default, // Can be in play, in hand, or in discard
    Stunned,
    Attacking(CombatPosition),
    Defending(CombatPosition),
}

#[derive(Serialize, Deserialize, Debug)]
pub enum Attack {
    BasicAttack(HealthValue),
}

#[derive(Serialize, Deserialize, Debug)]
pub struct Spell {
    pub card: Card,
}

#[derive(Serialize, Deserialize, Debug)]
pub struct Structure {
    pub card: Card,
}

#[derive(Serialize, Deserialize, Debug)]
pub struct Crystal {
    pub card: Card,
    pub mana_per_turn: ManaValue,
    pub influence_per_turn: Influence,
}
