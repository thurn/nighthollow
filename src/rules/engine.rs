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
    effects::{self, Effects, SetModifier},
    events::{self, Events},
    responses,
};
use crate::{
    api, commands,
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
pub enum Trigger {
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
    SpellPlayed(PlayerName, CreatureId, SpellId),
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

impl Trigger {
    fn trigger_name(&self) -> TriggerName {
        match self {
            Trigger::GameStart => TriggerName::GameStart,
            Trigger::GameEnd => TriggerName::GameEnd,
            Trigger::TurnStart => TriggerName::TurnStart,
            Trigger::TurnEnd => TriggerName::TurnEnd,
            Trigger::PlayerLifeChanged(_, _) => TriggerName::PlayerLifeChanged,
            Trigger::PlayerPowerChanged(_, _) => TriggerName::PlayerPowerChanged,
            Trigger::PlayerInfluenceChanged(_, _) => TriggerName::PlayerInfluenceChanged,
            Trigger::CardDrawn(_, _) => TriggerName::CardDrawn,
            Trigger::CardPlayed(_, _) => TriggerName::CardPlayed,
            Trigger::CreaturePlayed(_, _) => TriggerName::CreaturePlayed,
            Trigger::ScrollPlayed(_, _) => TriggerName::ScrollPlayed,
            Trigger::SpellPlayed(_, _, _) => TriggerName::SpellPlayed,
            Trigger::CombatStart => TriggerName::CombatStart,
            Trigger::CombatEnd => TriggerName::CombatEnd,
            Trigger::RoundStart(_) => TriggerName::RoundStart,
            Trigger::RoundEnd(_) => TriggerName::RoundEnd,
            Trigger::ActionStart(_) => TriggerName::ActionStart,
            Trigger::InvokeSkill(_) => TriggerName::InvokeSkill,
            Trigger::ActionEnd(_) => TriggerName::ActionEnd,
            Trigger::CreatureDamaged {
                attacker,
                defender,
                damage,
            } => TriggerName::CreatureDamaged,
            Trigger::CreatureKilled {
                killed_by,
                defender,
                damage,
            } => TriggerName::CreatureKilled,
            Trigger::CreatureHealed {
                source,
                target,
                amount,
            } => TriggerName::CreatureHealed,
            Trigger::CreatureManaChanged {
                source,
                target,
                amount,
            } => TriggerName::CreatureManaChanged,
            Trigger::CreatureStatModifierSet {
                source,
                target,
                modifier,
            } => TriggerName::CreatureStatModifierSet,
        }
    }

    fn entity_id(&self) -> Option<EntityId> {
        match self {
            Trigger::GameStart => None,
            Trigger::GameEnd => None,
            Trigger::TurnStart => None,
            Trigger::TurnEnd => None,
            Trigger::PlayerLifeChanged(p, _) => Some(EntityId::PlayerName(*p)),
            Trigger::PlayerPowerChanged(p, _) => Some(EntityId::PlayerName(*p)),
            Trigger::PlayerInfluenceChanged(p, _) => Some(EntityId::PlayerName(*p)),
            Trigger::CardDrawn(p, _) => Some(EntityId::PlayerName(*p)),
            Trigger::CardPlayed(p, _) => Some(EntityId::PlayerName(*p)),
            Trigger::CreaturePlayed(_, c) => Some(EntityId::CreatureId(*c)),
            Trigger::ScrollPlayed(p, _) => Some(EntityId::PlayerName(*p)),
            Trigger::SpellPlayed(p, _, _) => Some(EntityId::PlayerName(*p)),
            Trigger::CombatStart => None,
            Trigger::CombatEnd => None,
            Trigger::RoundStart(_) => None,
            Trigger::RoundEnd(_) => None,
            Trigger::ActionStart(c) => Some(EntityId::CreatureId(*c)),
            Trigger::InvokeSkill(c) => Some(EntityId::CreatureId(*c)),
            Trigger::ActionEnd(c) => Some(EntityId::CreatureId(*c)),
            Trigger::CreatureDamaged {
                attacker,
                defender,
                damage,
            } => Some(EntityId::CreatureId(*defender)),
            Trigger::CreatureKilled {
                killed_by,
                defender,
                damage,
            } => Some(EntityId::CreatureId(*defender)),
            Trigger::CreatureHealed {
                source,
                target,
                amount,
            } => Some(EntityId::CreatureId(*target)),
            Trigger::CreatureManaChanged {
                source,
                target,
                amount,
            } => Some(EntityId::CreatureId(*target)),
            Trigger::CreatureStatModifierSet {
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

#[derive(Debug, Clone)]
pub struct RuleIdentifier {
    pub index: usize,
    pub entity_id: EntityId,
}

#[derive(Debug)]
pub struct TriggerContext<'a> {
    pub this: Entity<'a>,
    pub data: &'a Trigger,
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
    fn context<'a>(&'a self, game: &'a Game, data: &'a Trigger) -> Result<TriggerContext<'a>> {
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
        data: &'a Trigger,
        effects: &mut Effects,
    ) -> Result<()> {
        self.rule.on_trigger(self.context(game, data)?, effects)
    }
}

#[derive(Debug, Clone)]
pub struct RulesEngine {
    pub game: Game,
    rule_map: BTreeMap<TriggerKey, Vec<RuleData>>,
}

impl RulesEngine {
    pub fn new(game: Game) -> Self {
        let user_rules = game.user.rules.clone();
        let enemy_rules = game.enemy.rules.clone();

        let mut result = Self {
            game,
            rule_map: BTreeMap::new(),
        };

        result.add_rules(EntityId::PlayerName(PlayerName::User), user_rules);
        result.add_rules(EntityId::PlayerName(PlayerName::Enemy), enemy_rules);
        result
    }

    pub fn add_rules(&mut self, entity_id: EntityId, rules: Vec<Box<dyn Rule>>) {
        for (index, rule) in rules.iter().enumerate() {
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
        result: &mut Vec<api::Command>,
        trigger: Trigger,
    ) -> Result<()> {
        let mut effects = Effects::default();
        self.populate_rule_effects(&mut effects, trigger)?;

        let mut event_index = 0;
        let mut events = Events::new();

        while effects.len() > 0 {
            for effect in effects.iter() {
                effects::apply_effect(&mut self.game, &mut events, effect)?;
            }

            effects = Effects::default();

            for event in &events.data[event_index..] {
                events::populate_event_rule_effects(self, &mut effects, event)?;
            }
            event_index = events.data.len();
        }

        responses::generate(&self.game, events, result)?;

        Ok(())
    }

    pub fn invoke_as_group(
        &mut self,
        result: &mut Vec<api::CommandGroup>,
        trigger: Trigger,
    ) -> Result<()> {
        let mut output = vec![];
        self.invoke_trigger(&mut output, trigger)?;

        if output.is_empty() {
            result.push(commands::group(output));
        }
        Ok(())
    }

    pub fn populate_rule_effects(&self, effects: &mut Effects, trigger: Trigger) -> Result<()> {
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
