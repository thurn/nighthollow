// Copyright © 2020-present Derek Thurn

// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at

//    https://www.apache.org/licenses/LICENSE-2.0

// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.

use color_eyre::Result;
use eyre::eyre;
use serde::{Deserialize, Serialize};

use super::stats::{Stat, StatName, Tag, TagName};
use crate::{model::primitives::*, rules::rules::Rule};
use std::slice::IterMut;

#[derive(Serialize, Deserialize, Debug)]
pub struct ManaCost {
    pub mana: i32,
    pub influence: Influence,
}

#[derive(Serialize, Deserialize, Debug)]
pub enum Cost {
    None,
    ManaCost(ManaCost),
}

#[derive(Serialize, Deserialize, Debug)]
pub struct CardData {
    pub id: CardId,
    pub owner: PlayerName,
    pub cost: Cost,
    pub name: String,
    pub school: School,
    pub text: String,
}

pub trait HasCardData {
    fn card_data(&self) -> &CardData;
}

impl HasCardData for CardData {
    fn card_data(&self) -> &CardData {
        self
    }
}

pub trait HasOwner {
    fn owner(&self) -> PlayerName;
}

impl<T: HasCardData> HasOwner for T {
    fn owner(&self) -> PlayerName {
        self.card_data().owner
    }
}

#[derive(Serialize, Deserialize, Debug, PartialEq, Eq, Copy, Clone)]
pub enum CreatureType {
    Berserker,
    Mage,
}

#[derive(Serialize, Deserialize, Debug)]
pub struct Damage {
    pub values: Vec<(u32, DamageType)>,
}

impl Damage {
    pub fn total(&self) -> u32 {
        self.values
            .iter()
            .fold(0, |accum, (value, _)| accum + *value)
    }
}

#[derive(Serialize, Deserialize, Debug)]
pub struct DamageStat {
    pub value: Stat,
    pub damage_type: DamageType,
}

#[derive(Serialize, Deserialize, Debug)]
pub struct CreatureStats {
    pub health_total: Stat,
    pub health_regeneration: Stat,
    pub damage: HealthValue,
    pub maximum_mana: Stat,
    pub mana_regeneration: Stat,
    pub mana: ManaValue,
    pub initiative: Stat,
    pub crit_chance: Stat,
    pub crit_multiplier: Stat,
    pub accuracy: Stat,
    pub evasion: Stat,
    pub base_damage: Vec<DamageStat>,
    pub damage_resistance: Vec<DamageStat>,
    pub damage_reduction: Vec<DamageStat>,

    tags: Vec<(TagName, Tag)>,
    dynamic_stats: Vec<(StatName, Stat)>,
}

impl CreatureStats {
    pub fn tag(&self, name: TagName) -> bool {
        todo!()
    }

    pub fn get(&self, name: StatName) -> i32 {
        todo!()
    }

    pub fn get_mut(&mut self, name: StatName) -> &mut Stat {
        &mut self.maximum_mana
    }
}

impl Default for CreatureStats {
    fn default() -> Self {
        CreatureStats {
            health_total: Stat::new(100),
            health_regeneration: Stat::new(0),
            damage: 0,
            maximum_mana: Stat::new(100),
            mana_regeneration: Stat::new(100),
            mana: 0,
            initiative: Stat::new(100),
            crit_chance: Stat::new(50),
            crit_multiplier: Stat::new(1000),
            accuracy: Stat::new(100),
            evasion: Stat::new(100),
            base_damage: vec![],
            damage_resistance: vec![],
            damage_reduction: vec![],
            tags: vec![],
            dynamic_stats: vec![],
        }
    }
}

#[derive(Serialize, Deserialize, Debug)]
pub struct CreatureArchetype {
    pub card_data: CardData,
    pub base_type: CreatureType,
    pub stats: CreatureStats,
    pub rules: Vec<Box<dyn Rule>>,
}

impl HasCardData for CreatureArchetype {
    fn card_data(&self) -> &CardData {
        &self.card_data
    }
}

pub enum DamageResult {
    StillAlive,
    AlreadyDead,
    Killed,
}

#[derive(Serialize, Deserialize, Debug)]
pub struct Creature {
    pub archetype: CreatureArchetype,
    pub position: BoardPosition,
    pub spells: Vec<Spell>,
}

impl Creature {
    pub fn creature_id(&self) -> CreatureId {
        self.card_data().id
    }

    pub fn stats(&self) -> &CreatureStats {
        &self.archetype.stats
    }

    pub fn stats_mut(&mut self) -> &mut CreatureStats {
        &mut self.archetype.stats
    }

    pub fn is_alive(&self) -> bool {
        self.stats().health_total.value() > self.stats().damage
    }

    pub fn apply_damage(&mut self, value: HealthValue) -> DamageResult {
        let was_alive = self.is_alive();
        self.stats_mut().damage += value;

        if self.is_alive() {
            return DamageResult::StillAlive;
        } else if !was_alive {
            return DamageResult::AlreadyDead;
        } else {
            return DamageResult::Killed;
        }
    }

    pub fn heal(&mut self, value: HealthValue) {
        if value > self.stats().damage {
            self.stats_mut().damage = 0
        } else {
            self.stats_mut().damage -= value
        }
    }

    pub fn lose_mana(&mut self, value: ManaValue) -> Result<()> {
        if value < self.stats().mana {
            Err(eyre!(
                "Cannot lose {} mana, only {} available",
                value,
                self.stats().mana
            ))
        } else {
            Ok(self.stats_mut().mana -= value)
        }
    }

    pub fn gain_mana(&mut self, value: ManaValue) {
        self.stats_mut().mana += value
    }
}

impl HasCardData for Creature {
    fn card_data(&self) -> &CardData {
        &self.archetype.card_data()
    }
}

#[derive(Serialize, Deserialize, Debug, PartialEq, Eq, Copy, Clone)]
pub enum SpellType {
    Rage,
}

#[derive(Serialize, Deserialize, Debug)]
pub struct Spell {
    pub card_data: CardData,
    pub base_type: SpellType,
}

impl HasCardData for Spell {
    fn card_data(&self) -> &CardData {
        &self.card_data
    }
}

#[derive(Serialize, Deserialize, Debug, PartialEq, Eq, Copy, Clone)]
pub enum ScrollType {
    FlameScroll,
}

#[derive(Serialize, Deserialize, Debug)]
pub struct Scroll {
    pub card_data: CardData,
    pub base_type: ScrollType,
}

impl HasCardData for Scroll {
    fn card_data(&self) -> &CardData {
        &self.card_data
    }
}

#[derive(Serialize, Deserialize, Debug)]
pub enum Card {
    Creature(CreatureArchetype),
    Spell(Spell),
    Scroll(Scroll),
}

impl HasCardData for Card {
    fn card_data(&self) -> &CardData {
        match self {
            Card::Creature(c) => c.card_data(),
            Card::Spell(s) => s.card_data(),
            Card::Scroll(s) => s.card_data(),
        }
    }
}

#[derive(Serialize, Deserialize, Debug)]
pub struct PlayerState {
    pub current_life: i32,
    pub maximum_life: i32,
    pub current_mana: i32,
    pub maximum_mana: i32,
    pub current_influence: Influence,
    pub maximum_influence: Influence,
}

impl Default for PlayerState {
    fn default() -> Self {
        PlayerState {
            current_life: 25,
            maximum_life: 25,
            current_mana: 0,
            maximum_mana: 0,
            current_influence: Influence::default(),
            maximum_influence: Influence::default(),
        }
    }
}

#[derive(Serialize, Deserialize, Debug)]
pub struct Player {
    pub name: PlayerName,
    pub state: PlayerState,
    pub hand: Vec<Card>,
    pub creatures: Vec<Creature>,
    pub scrolls: Vec<Scroll>,
}

impl HasOwner for Player {
    fn owner(&self) -> PlayerName {
        self.name
    }
}

#[derive(Serialize, Deserialize, Debug)]
pub struct GameState {
    pub phase: GamePhase,
    pub turn: TurnNumber,
}

#[derive(Serialize, Deserialize, Debug)]
pub struct Game {
    pub state: GameState,
    pub user: Player,
    pub enemy: Player,
}

impl Game {
    pub fn all_creatures(&self) -> impl Iterator<Item = &Creature> {
        self.user
            .creatures
            .iter()
            .chain(self.enemy.creatures.iter())
    }

    pub fn creature(&self, creature_id: CreatureId) -> Result<&Creature> {
        self.all_creatures()
            .find(|c| c.creature_id() == creature_id)
            .ok_or(eyre!("Creature ID {} not found", creature_id))
    }

    pub fn creature_mut(&mut self, creature_id: CreatureId) -> Result<&mut Creature> {
        self.user
            .creatures
            .iter_mut()
            .chain(self.enemy.creatures.iter_mut())
            .find(|c| c.creature_id() == creature_id)
            .ok_or(eyre!("Creature ID {} not found", creature_id))
    }
}
