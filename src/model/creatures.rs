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

use crate::prelude::*;

use super::{
    assets::CreatureType,
    cards::{CardData, HasCardData, HasCardId, Spell},
    stats::{Stat, StatName, Tag, TagName},
};
use crate::{
    model::primitives::*,
    rules::engine::{Rule, RuleIdentifier},
};
use std::{
    cmp,
    collections::{BTreeMap, BTreeSet},
};

#[derive(Serialize, Deserialize, Debug, Clone)]
pub struct DamageStats {
    pub radiant: Stat,
    pub electric: Stat,
    pub fire: Stat,
    pub cold: Stat,
    pub physical: Stat,
    pub necrotic: Stat,
}

impl DamageStats {
    pub fn damage(&self) -> Damage {
        Damage {
            radiant: self.radiant.value(),
            electric: self.electric.value(),
            fire: self.fire.value(),
            cold: self.cold.value(),
            physical: self.physical.value(),
            necrotic: self.necrotic.value(),
        }
    }
}

impl Default for DamageStats {
    fn default() -> Self {
        Self {
            radiant: Stat::new(0),
            electric: Stat::new(0),
            fire: Stat::new(0),
            cold: Stat::new(0),
            physical: Stat::new(0),
            necrotic: Stat::new(0),
        }
    }
}

#[derive(Serialize, Deserialize, Debug, Clone)]
pub struct CreatureStats {
    pub health_total: Stat,
    pub health_regeneration_per_round: Stat,
    pub starting_mana: Stat,
    pub maximum_mana: Stat,
    pub mana_regeneration_per_round: Stat,
    pub initiative: Stat,
    pub crit_chance: Stat,
    pub crit_multiplier: Stat,
    pub accuracy: Stat,
    pub evasion: Stat,
    pub opponent_life_reduction: Stat,
    pub base_damage: DamageStats,
    pub damage_resistance: DamageStats,
    pub damage_reduction: DamageStats,

    pub tags: Vec<(TagName, Tag)>,
    pub dynamic_stats: Vec<(StatName, Stat)>,
}

impl CreatureStats {
    pub fn tag(&self, _name: TagName) -> bool {
        todo!()
    }

    pub fn get(&self, _name: StatName) -> i32 {
        todo!()
    }

    pub fn get_mut(&mut self, _name: StatName) -> &mut Stat {
        &mut self.maximum_mana
    }
}

impl Default for CreatureStats {
    fn default() -> Self {
        CreatureStats {
            health_total: Stat::new(0),
            health_regeneration_per_round: Stat::new(0),
            starting_mana: Stat::new(0),
            maximum_mana: Stat::new(0),
            mana_regeneration_per_round: Stat::new(0),
            initiative: Stat::new(0),
            crit_chance: Stat::new(0),
            crit_multiplier: Stat::new(0),
            accuracy: Stat::new(0),
            evasion: Stat::new(0),
            opponent_life_reduction: Stat::new(1),
            base_damage: DamageStats::default(),
            damage_resistance: DamageStats::default(),
            damage_reduction: DamageStats::default(),
            tags: vec![],
            dynamic_stats: vec![],
        }
    }
}

#[derive(Serialize, Deserialize, Debug, Clone)]
pub struct CreatureData {
    pub card_data: CardData,
    pub base_type: CreatureType,
    pub stats: CreatureStats,
    pub rules: Vec<Box<dyn Rule>>,
}

impl HasCardData for CreatureData {
    fn card_data(&self) -> &CardData {
        &self.card_data
    }

    fn card_data_mut(&mut self) -> &mut CardData {
        &mut self.card_data
    }
}

pub enum DamageResult {
    StillAlive,
    Killed,
    AlreadyDead,
}

#[derive(Serialize, Deserialize, Debug, Clone)]
pub struct CreatureState {
    is_alive: bool,
    damage: HealthValue,
    mana: ManaValue,
    can_reposition: bool,
    skills: BTreeMap<RuleIdentifier, u32>,
}

impl Default for CreatureState {
    fn default() -> Self {
        CreatureState {
            is_alive: true,
            damage: 0,
            mana: 0,
            can_reposition: true,
            skills: BTreeMap::new(),
        }
    }
}

#[derive(Serialize, Deserialize, Debug, Clone)]
pub struct Creature {
    pub data: CreatureData,
    pub position: BoardPosition,
    pub spells: Vec<Spell>,
    pub state: CreatureState,
}

impl Creature {
    pub fn stats(&self) -> &CreatureStats {
        &self.data.stats
    }

    pub fn stats_mut(&mut self) -> &mut CreatureStats {
        &mut self.data.stats
    }

    pub fn is_alive(&self) -> bool {
        self.state.is_alive
    }

    pub fn current_mana(&self) -> ManaValue {
        self.state.mana
    }

    pub fn can_reposition(&self) -> bool {
        self.state.can_reposition
    }

    pub fn current_health(&self) -> u32 {
        let health = self.stats().health_total.value();
        if self.state.damage > health {
            0
        } else {
            health - self.state.damage
        }
    }

    pub fn apply_damage(&mut self, value: HealthValue) -> DamageResult {
        if !self.state.is_alive {
            return DamageResult::AlreadyDead;
        }

        self.state.damage += value;

        if self.stats().health_total.value() > self.state.damage {
            DamageResult::StillAlive
        } else {
            self.state.is_alive = false;
            DamageResult::Killed
        }
    }

    pub fn heal(&mut self, value: HealthValue) {
        if !self.state.is_alive {
            return;
        }

        if value > self.state.damage {
            self.state.damage = 0
        } else {
            self.state.damage -= value
        }
    }

    pub fn lose_mana(&mut self, value: ManaValue) -> Result<()> {
        if !self.state.is_alive {
            return Ok(());
        }

        if value < self.state.mana {
            Err(eyre!(
                "Cannot lose {} mana, only {} available",
                value,
                self.state.mana
            ))
        } else {
            self.state.mana -= value;
            Ok(())
        }
    }

    pub fn gain_mana(&mut self, value: ManaValue) {
        if !self.state.is_alive {
            return;
        }

        self.state.mana = cmp::min(self.stats().maximum_mana.value(), self.state.mana + value);
    }

    pub fn highest_priority_skill(&self) -> Option<RuleIdentifier> {
        self.state
            .skills
            .iter()
            .max_by_key(|(_, p)| *p)
            .map(|(id, _)| *id)
    }

    pub fn set_skill_priority(&mut self, rule_identifier: RuleIdentifier, priority: u32) {
        println!(
            "Setting priority for {:?} to {:?}",
            rule_identifier, priority
        );
        self.state.skills.insert(rule_identifier, priority);
    }

    pub fn reset(&mut self) {
        self.state.is_alive = true;
        self.state.damage = 0;
        self.state.mana = self.stats().starting_mana.value();
    }
}

impl HasCardData for Creature {
    fn card_data(&self) -> &CardData {
        self.data.card_data()
    }

    fn card_data_mut(&mut self) -> &mut CardData {
        self.data.card_data_mut()
    }
}

pub trait HasCreatureData {
    fn creature_id(&self) -> CreatureId;

    fn base_type(&self) -> CreatureType;
}

impl HasCreatureData for Creature {
    fn creature_id(&self) -> CreatureId {
        self.card_data().card_id()
    }

    fn base_type(&self) -> CreatureType {
        self.data.base_type
    }
}

impl HasCreatureData for CreatureData {
    fn creature_id(&self) -> CreatureId {
        self.card_data().card_id()
    }

    fn base_type(&self) -> CreatureType {
        self.base_type
    }
}
