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

use std::{collections::BTreeMap, fmt::Debug};

use dyn_clone::DynClone;
use eyre::{eyre, Result};

use super::{
    command_generation,
    effects::{self, Effects, SetModifier},
    events::{self, Events},
};
use crate::{
    api,
    model::{
        cards::{Card, Scroll, Spell},
        creatures::{Creature, Damage, HasCreatureData},
        games::{Game, Player},
        primitives::{
            CardId, CreatureId, HealthValue, Influence, LifeValue, ManaValue, PlayerName,
            PowerValue, RoundNumber, ScrollId, SpellId,
        },
        stats::StatName,
    },
};

#[derive(Debug, Copy, Clone, Ord, PartialOrd, Eq, PartialEq)]
pub enum TriggerName {
    GameStart,
    GameEnd,
    TurnStart,
    TurnEnd,
    PlayerLifeChanged,
    PlayerPowerChanged,
    PlayerInfluenceChanged,
    CardDrawn,
    CardPlayed,
    CreaturePlayed,
    ScrollPlayed,
    SpellPlayed,
    CombatStart,
    CombatEnd,
    ActionStart,
    InvokeSkill,
    ActionEnd,
    RoundStart,
    RoundEnd,
    CreatureDamaged,
    CreatureKilled,
    CreatureHealed,
    CreatureManaChanged,
    CreatureStatModifierSet,
}

#[derive(Debug, Clone)]
pub enum TriggerData {
    GameStart,
    GameEnd,
    TurnStart,
    TurnEnd,
    PlayerLifeChanged(PlayerName, LifeValue),
    PlayerPowerChanged(PlayerName, PowerValue),
    PlayerInfluenceChanged(PlayerName, Influence),
    CardDrawn(PlayerName, CardId),
    CardPlayed(PlayerName, CardId),
    CreaturePlayed(PlayerName, CreatureId),
    ScrollPlayed(PlayerName, ScrollId),
    SpellPlayed(PlayerName, SpellId),
    CombatStart,
    CombatEnd,
    RoundStart(RoundNumber),
    RoundEnd(RoundNumber),
    ActionStart(CreatureId),
    InvokeSkill(CreatureId),
    ActionEnd(CreatureId),
    CreatureDamaged {
        attacker: CreatureId,
        defender: CreatureId,
        damage: Damage,
    },
    CreatureKilled {
        killed_by: CreatureId,
        defender: CreatureId,
        damage: Damage,
    },
    CreatureHealed {
        source: CreatureId,
        target: CreatureId,
        amount: HealthValue,
    },
    CreatureManaChanged {
        source: CreatureId,
        target: CreatureId,
        amount: ManaValue,
    },
    CreatureStatModifierSet {
        source: CreatureId,
        target: CreatureId,
        modifier: SetModifier,
    },
}

impl TriggerData {
    fn trigger_name(&self) -> TriggerName {
        match self {
            TriggerData::GameStart => TriggerName::GameStart,
            TriggerData::GameEnd => TriggerName::GameEnd,
            TriggerData::TurnStart => TriggerName::TurnStart,
            TriggerData::TurnEnd => TriggerName::TurnEnd,
            TriggerData::PlayerLifeChanged(_, _) => TriggerName::PlayerLifeChanged,
            TriggerData::PlayerPowerChanged(_, _) => TriggerName::PlayerPowerChanged,
            TriggerData::PlayerInfluenceChanged(_, _) => TriggerName::PlayerInfluenceChanged,
            TriggerData::CardDrawn(_, _) => TriggerName::CardDrawn,
            TriggerData::CardPlayed(_, _) => TriggerName::CardPlayed,
            TriggerData::CreaturePlayed(_, _) => TriggerName::CreaturePlayed,
            TriggerData::ScrollPlayed(_, _) => TriggerName::ScrollPlayed,
            TriggerData::SpellPlayed(_, _) => TriggerName::SpellPlayed,
            TriggerData::CombatStart => TriggerName::CombatStart,
            TriggerData::CombatEnd => TriggerName::CombatEnd,
            TriggerData::RoundStart(_) => TriggerName::RoundStart,
            TriggerData::RoundEnd(_) => TriggerName::RoundEnd,
            TriggerData::ActionStart(_) => TriggerName::ActionStart,
            TriggerData::InvokeSkill(_) => TriggerName::InvokeSkill,
            TriggerData::ActionEnd(_) => TriggerName::ActionEnd,
            TriggerData::CreatureDamaged {
                attacker,
                defender,
                damage,
            } => TriggerName::CreatureDamaged,
            TriggerData::CreatureKilled {
                killed_by,
                defender,
                damage,
            } => TriggerName::CreatureKilled,
            TriggerData::CreatureHealed {
                source,
                target,
                amount,
            } => TriggerName::CreatureHealed,
            TriggerData::CreatureManaChanged {
                source,
                target,
                amount,
            } => TriggerName::CreatureManaChanged,
            TriggerData::CreatureStatModifierSet {
                source,
                target,
                modifier,
            } => TriggerName::CreatureStatModifierSet,
        }
    }

    fn entity_id(&self) -> Option<EntityId> {
        match self {
            TriggerData::GameStart => None,
            TriggerData::GameEnd => None,
            TriggerData::TurnStart => None,
            TriggerData::TurnEnd => None,
            TriggerData::PlayerLifeChanged(p, _) => Some(EntityId::PlayerName(*p)),
            TriggerData::PlayerPowerChanged(p, _) => Some(EntityId::PlayerName(*p)),
            TriggerData::PlayerInfluenceChanged(p, _) => Some(EntityId::PlayerName(*p)),
            TriggerData::CardDrawn(p, _) => Some(EntityId::PlayerName(*p)),
            TriggerData::CardPlayed(p, _) => Some(EntityId::PlayerName(*p)),
            TriggerData::CreaturePlayed(_, c) => Some(EntityId::CreatureId(*c)),
            TriggerData::ScrollPlayed(p, _) => Some(EntityId::PlayerName(*p)),
            TriggerData::SpellPlayed(p, _) => Some(EntityId::PlayerName(*p)),
            TriggerData::CombatStart => None,
            TriggerData::CombatEnd => None,
            TriggerData::RoundStart(_) => None,
            TriggerData::RoundEnd(_) => None,
            TriggerData::ActionStart(c) => Some(EntityId::CreatureId(*c)),
            TriggerData::InvokeSkill(c) => Some(EntityId::CreatureId(*c)),
            TriggerData::ActionEnd(c) => Some(EntityId::CreatureId(*c)),
            TriggerData::CreatureDamaged {
                attacker,
                defender,
                damage,
            } => Some(EntityId::CreatureId(*defender)),
            TriggerData::CreatureKilled {
                killed_by,
                defender,
                damage,
            } => Some(EntityId::CreatureId(*defender)),
            TriggerData::CreatureHealed {
                source,
                target,
                amount,
            } => Some(EntityId::CreatureId(*target)),
            TriggerData::CreatureManaChanged {
                source,
                target,
                amount,
            } => Some(EntityId::CreatureId(*target)),
            TriggerData::CreatureStatModifierSet {
                source,
                target,
                modifier,
            } => Some(EntityId::CreatureId(*target)),
        }
    }
}

#[derive(Debug, Clone)]
pub enum Entity<'a> {
    Creature(&'a Creature),
    Player(&'a Player),
}

impl<'a> Entity<'a> {
    pub fn creature(&self) -> Result<&Creature> {
        match self {
            Entity::Creature(c) => Ok(c),
            Entity::Player(p) => Err(eyre!("Expected creature but got player {:?}", p.name)),
        }
    }

    pub fn player(&self) -> Result<&Player> {
        match self {
            Entity::Player(p) => Ok(p),
            Entity::Creature(c) => Err(eyre!(
                "Expected player but got creature {:?}",
                c.creature_id()
            )),
        }
    }
}

#[derive(Debug, Copy, Clone, PartialOrd, Ord, Eq, PartialEq)]
pub enum EntityId {
    PlayerName(PlayerName),
    CreatureId(CreatureId),
}

impl EntityId {
    pub fn find_in<'a>(&self, game: &'a Game) -> Result<Entity<'a>> {
        Ok(match self {
            EntityId::PlayerName(player_name) => Entity::Player(game.player(*player_name)),
            EntityId::CreatureId(creature_id) => Entity::Creature(game.creature(*creature_id)?),
        })
    }
}

#[derive(Debug, Copy, Clone, Ord, PartialOrd, Eq, PartialEq)]
pub enum TriggerCondition {
    This(TriggerName),
    Any(TriggerName),
}

impl TriggerCondition {
    fn to_key(self, entity_id: EntityId) -> TriggerKey {
        match self {
            TriggerCondition::This(name) => TriggerKey::Entity(name, entity_id),
            TriggerCondition::Any(name) => TriggerKey::Global(name),
        }
    }
}

#[derive(Debug, Copy, Clone, Ord, PartialOrd, Eq, PartialEq)]
pub enum TriggerKey {
    Global(TriggerName),
    Entity(TriggerName, EntityId),
}

#[derive(Debug, Clone, Ord, PartialOrd, Eq, PartialEq)]
pub struct RuleIdentifier {
    pub index: usize,
    pub entity_id: EntityId,
}

#[derive(Debug)]
pub struct TriggerContext<'a> {
    pub this: Entity<'a>,
    pub data: &'a TriggerData,
    pub identifier: &'a RuleIdentifier,
    pub game: &'a Game,
}

#[typetag::serde(tag = "type")]
pub trait Rule: Debug + Send + DynClone {
    fn triggers(&self) -> Vec<TriggerCondition>;

    fn on_trigger(&self, context: TriggerContext, effects: &mut Effects) -> Result<()>;
}

dyn_clone::clone_trait_object!(Rule);

#[derive(Debug, Clone)]
struct RuleData {
    rule: Box<dyn Rule>,
    identifier: RuleIdentifier,
}

impl RuleData {
    fn context<'a>(&'a self, game: &'a Game, data: &'a TriggerData) -> Result<TriggerContext<'a>> {
        Ok(TriggerContext {
            this: self.identifier.entity_id.find_in(game)?,
            data,
            identifier: &self.identifier,
            game,
        })
    }

    fn on_trigger<'a>(
        &self,
        game: &'a Game,
        data: &'a TriggerData,
        effects: &mut Effects,
    ) -> Result<()> {
        self.rule.on_trigger(self.context(game, data)?, effects)
    }
}

#[derive(Debug, Clone)]
pub struct RulesEngine {
    game: Game,
    rule_map: BTreeMap<TriggerKey, Vec<RuleData>>,
}

impl RulesEngine {
    pub fn new(game: Game) -> Self {
        Self {
            game,
            rule_map: BTreeMap::new(),
        }
    }

    pub fn add_rules(&mut self, entity_id: EntityId, rules: impl Iterator<Item = Box<dyn Rule>>) {
        for (index, rule) in rules.enumerate() {
            let conditions = rule.triggers();
            for condition in conditions {
                let data = RuleData {
                    rule: rule.clone(),
                    identifier: RuleIdentifier { index, entity_id },
                };
                let key = condition.to_key(entity_id);
                if let Some(rules) = self.rule_map.get_mut(&key) {
                    rules.push(data);
                } else {
                    self.rule_map.insert(key, vec![data]);
                }
            }
        }
    }

    pub fn invoke_trigger(
        &mut self,
        commands: &mut Vec<api::Command>,
        trigger: TriggerData,
    ) -> Result<()> {
        let mut effects = Effects::new();
        self.populate_rule_effects(&mut effects, trigger)?;

        let mut event_index = 0;
        let mut events = Events::new();

        while effects.len() > 0 {
            for effect in effects.iter() {
                effects::apply_effect(&mut self.game, &mut events, effect)?;
            }

            effects = Effects::new();

            for event in &events.data[event_index..] {
                events::populate_event_rule_effects(self, &mut effects, event)?;
            }
            event_index = events.data.len();
        }

        command_generation::generate(&self.game, events, commands)?;

        Ok(())
    }

    pub fn populate_rule_effects(&self, effects: &mut Effects, trigger: TriggerData) -> Result<()> {
        if let Some(rules) = self
            .rule_map
            .get(&TriggerKey::Global(trigger.trigger_name()))
        {
            for rule_data in rules {
                rule_data.on_trigger(&self.game, &trigger, effects)?;
            }
        }

        if let Some(entity_id) = trigger.entity_id() {
            if let Some(rules) = self
                .rule_map
                .get(&TriggerKey::Entity(trigger.trigger_name(), entity_id))
            {
                for rule_data in rules {
                    rule_data.on_trigger(&self.game, &trigger, effects)?;
                }
            }
        }

        Ok(())
    }
}
