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

use crate::prelude::*;
use dyn_clone::DynClone;

use super::{
    effects::{self, Effects, SetModifier},
    events::{self, Events},
    responses,
};
use crate::{
    api, commands,
    model::{
        cards::{Card, Scroll, Spell},
        creatures::{Creature, HasCreatureData},
        games::Game,
        players::{HasOwner, Player},
        primitives::{
            CardId, CreatureId, Damage, HealthValue, Influence, LifeValue, ManaValue, PlayerName,
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
    PlayerLifeZero,
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
    CreatureManaGained,
    CreatureManaLost,
    CreatureStatModifierSet,
}

impl TriggerName {
    pub fn this(self) -> TriggerCondition {
        TriggerCondition::This(self)
    }

    pub fn any(self) -> TriggerCondition {
        TriggerCondition::Any(self)
    }

    pub fn source(self) -> TriggerCondition {
        TriggerCondition::Source(self)
    }
}

#[derive(Debug, Copy, Clone)]
pub enum Trigger {
    GameStart,
    GameEnd,
    TurnStart,
    TurnEnd,
    PlayerLifeChanged(PlayerName, LifeValue),
    PlayerLifeZero(PlayerName),
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
    CreatureManaGained {
        source: CreatureId,
        target: CreatureId,
        amount: ManaValue,
    },
    CreatureManaLost {
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
            Trigger::PlayerLifeZero(_) => TriggerName::PlayerLifeZero,
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
            Trigger::CreatureManaGained {
                source,
                target,
                amount,
            } => TriggerName::CreatureManaGained,
            Trigger::CreatureManaLost {
                source,
                target,
                amount,
            } => TriggerName::CreatureManaLost,
            Trigger::CreatureStatModifierSet {
                source,
                target,
                modifier,
            } => TriggerName::CreatureStatModifierSet,
        }
    }

    fn entity_ids(&self) -> EntityIds {
        match self {
            Trigger::GameStart => EntityIds::default(),
            Trigger::GameEnd => EntityIds::default(),
            Trigger::TurnStart => EntityIds::default(),
            Trigger::TurnEnd => EntityIds::default(),
            Trigger::PlayerLifeChanged(p, _) => EntityIds::player(*p),
            Trigger::PlayerLifeZero(p) => EntityIds::player(*p),
            Trigger::PlayerPowerChanged(p, _) => EntityIds::player(*p),
            Trigger::PlayerInfluenceChanged(p, _) => EntityIds::player(*p),
            Trigger::CardDrawn(p, _) => EntityIds::player(*p),
            Trigger::CardPlayed(p, _) => EntityIds::player(*p),
            Trigger::CreaturePlayed(p, c) => EntityIds {
                player: Some(*p),
                source_creature: None,
                target_creature: Some(*c),
            },
            Trigger::ScrollPlayed(p, _) => EntityIds::player(*p),
            Trigger::SpellPlayed(p, _, _) => EntityIds::player(*p),
            Trigger::CombatStart => EntityIds::default(),
            Trigger::CombatEnd => EntityIds::default(),
            Trigger::RoundStart(_) => EntityIds::default(),
            Trigger::RoundEnd(_) => EntityIds::default(),
            Trigger::ActionStart(c) => EntityIds::creature(*c),
            Trigger::InvokeSkill(c) => EntityIds::creature(*c),
            Trigger::ActionEnd(c) => EntityIds::creature(*c),
            Trigger::CreatureDamaged {
                attacker,
                defender,
                damage,
            } => EntityIds {
                player: None,
                source_creature: Some(*attacker),
                target_creature: Some(*defender),
            },
            Trigger::CreatureKilled {
                killed_by,
                defender,
                damage,
            } => EntityIds {
                player: None,
                source_creature: Some(*killed_by),
                target_creature: Some(*defender),
            },
            Trigger::CreatureHealed {
                source,
                target,
                amount,
            } => EntityIds {
                player: None,
                source_creature: Some(*source),
                target_creature: Some(*target),
            },
            Trigger::CreatureManaGained {
                source,
                target,
                amount,
            } => EntityIds {
                player: None,
                source_creature: Some(*source),
                target_creature: Some(*target),
            },
            Trigger::CreatureManaLost {
                source,
                target,
                amount,
            } => EntityIds {
                player: None,
                source_creature: Some(*source),
                target_creature: Some(*target),
            },
            Trigger::CreatureStatModifierSet {
                source,
                target,
                modifier,
            } => EntityIds {
                player: None,
                source_creature: Some(*source),
                target_creature: Some(*target),
            },
        }
    }
}

#[derive(Debug, Copy, Clone)]
struct EntityIds {
    player: Option<PlayerName>,
    source_creature: Option<CreatureId>,
    target_creature: Option<CreatureId>,
}

impl EntityIds {
    fn player(player_name: PlayerName) -> Self {
        Self {
            player: Some(player_name),
            source_creature: None,
            target_creature: None,
        }
    }

    fn creature(creature_id: CreatureId) -> Self {
        Self {
            player: None,
            source_creature: None,
            target_creature: Some(creature_id),
        }
    }
}

impl Default for EntityIds {
    fn default() -> Self {
        Self {
            player: None,
            source_creature: None,
            target_creature: None,
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

#[derive(Serialize, Deserialize, Debug, Copy, Clone, PartialOrd, Ord, Eq, PartialEq)]
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
    Source(TriggerName),
    Any(TriggerName),
}

impl TriggerCondition {
    fn to_key(self, entity_id: EntityId) -> TriggerKey {
        match self {
            TriggerCondition::This(name) => TriggerKey::Entity(name, entity_id),
            TriggerCondition::Any(name) => TriggerKey::Global(name),
            TriggerCondition::Source(name) => TriggerKey::Source(name, entity_id),
        }
    }
}

#[derive(Debug, Copy, Clone, Ord, PartialOrd, Eq, PartialEq)]
pub enum TriggerKey {
    Global(TriggerName),
    Entity(TriggerName, EntityId),
    Source(TriggerName, EntityId),
}

#[derive(Serialize, Deserialize, Debug, Clone, Copy, Ord, PartialOrd, Eq, PartialEq)]
pub struct RuleIdentifier {
    pub index: usize,
    pub entity_id: EntityId,
}

#[derive(Debug)]
pub struct TriggerContext<'a> {
    pub this: Entity<'a>,
    pub trigger: &'a Trigger,
    pub identifier: &'a RuleIdentifier,
    pub game: &'a Game,
}

impl<'a> TriggerContext<'a> {
    pub fn opponent(&self) -> &Player {
        self.game.player(match self.this {
            Entity::Creature(c) => c.owner().opponent(),
            Entity::Player(p) => p.name.opponent(),
        })
    }
}

#[typetag::serde(tag = "type")]
pub trait Rule: Debug + Send + DynClone {
    fn triggers(&self) -> Vec<TriggerCondition>;

    fn on_trigger(&self, context: &TriggerContext, effects: &mut Effects) -> Result<()>;
}

dyn_clone::clone_trait_object!(Rule);

#[derive(Debug, Clone)]
struct RuleData {
    rule: Box<dyn Rule>,
    identifier: RuleIdentifier,
}

impl RuleData {
    fn context<'a>(
        &'a self,
        this: Entity<'a>,
        game: &'a Game,
        trigger: &'a Trigger,
    ) -> Result<TriggerContext<'a>> {
        Ok(TriggerContext {
            this,
            trigger,
            identifier: &self.identifier,
            game,
        })
    }

    fn on_trigger<'a>(
        &self,
        game: &'a Game,
        trigger: &'a Trigger,
        effects: &mut Effects,
    ) -> Result<()> {
        self.rule.on_trigger(
            &self.context(self.identifier.entity_id.find_in(game)?, game, trigger)?,
            effects,
        )
    }
}

#[derive(Debug, Clone)]
pub struct RulesEngine {
    pub game: Game,
    rules: BTreeMap<RuleIdentifier, RuleData>,
    rule_map: BTreeMap<TriggerKey, Vec<RuleData>>,
}

impl RulesEngine {
    pub fn new(game: Game) -> Self {
        let (rules, rule_map) = create_maps(&game);
        Self {
            game,
            rules,
            rule_map,
        }
    }

    pub fn add_rules(&mut self, entity_id: EntityId, input: Vec<Box<dyn Rule>>) {
        add_rules(&mut self.rules, &mut self.rule_map, entity_id, input);
    }

    pub fn invoke_trigger(
        &mut self,
        result: &mut Vec<api::Command>,
        trigger: Trigger,
    ) -> Result<()> {
        let mut effects = Effects::default();
        self.populate_rule_effects(&mut effects, trigger)?;
        self.apply_effects(result, effects)
    }

    pub fn invoke_as_group(
        &mut self,
        result: &mut Vec<api::CommandGroup>,
        trigger: Trigger,
    ) -> Result<()> {
        let mut output = vec![];
        self.invoke_trigger(&mut output, trigger)?;

        if !output.is_empty() {
            result.push(commands::group(output));
        }
        Ok(())
    }

    pub fn invoke_rule(
        &mut self,
        rule_identifier: RuleIdentifier,
        result: &mut Vec<api::Command>,
        trigger: Trigger,
    ) -> Result<()> {
        let mut effects = Effects::default();
        if let Some(r) = self.rules.get(&rule_identifier) {
            r.on_trigger(&self.game, &trigger, &mut effects)?;
        }
        self.apply_effects(result, effects)
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

        let entity_ids = trigger.entity_ids();
        let ids = vec![
            entity_ids.player.map(EntityId::PlayerName),
            entity_ids.target_creature.map(EntityId::CreatureId),
        ];

        for entity_id in ids.into_iter().filter_map(|x| x) {
            if let Some(rules) = self
                .rule_map
                .get(&TriggerKey::Entity(trigger.trigger_name(), entity_id))
            {
                for rule_data in rules {
                    rule_data.on_trigger(&self.game, &trigger, effects)?;
                }
            }
        }

        if let Some(creature_id) = entity_ids.source_creature {
            if let Some(rules) = self.rule_map.get(&TriggerKey::Source(
                trigger.trigger_name(),
                EntityId::CreatureId(creature_id),
            )) {
                for rule_data in rules {
                    rule_data.on_trigger(&self.game, &trigger, effects)?;
                }
            }
        }

        Ok(())
    }

    fn apply_effects(
        &mut self,
        result: &mut Vec<api::Command>,
        mut effects: Effects,
    ) -> Result<()> {
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
}

fn create_maps(
    game: &Game,
) -> (
    BTreeMap<RuleIdentifier, RuleData>,
    BTreeMap<TriggerKey, Vec<RuleData>>,
) {
    let mut rules = BTreeMap::new();
    let mut rule_maps = BTreeMap::new();

    add_rules(
        &mut rules,
        &mut rule_maps,
        EntityId::PlayerName(PlayerName::User),
        game.user.rules.clone(),
    );

    add_rules(
        &mut rules,
        &mut rule_maps,
        EntityId::PlayerName(PlayerName::Enemy),
        game.enemy.rules.clone(),
    );

    (rules, rule_maps)
}

fn add_rules(
    rules: &mut BTreeMap<RuleIdentifier, RuleData>,
    rule_map: &mut BTreeMap<TriggerKey, Vec<RuleData>>,
    entity_id: EntityId,
    input: Vec<Box<dyn Rule>>,
) {
    for (index, rule) in input.iter().enumerate() {
        let identifier = RuleIdentifier { index, entity_id };
        let data = RuleData {
            rule: rule.clone(),
            identifier,
        };

        rules.insert(identifier, data.clone());

        let conditions = rule.triggers();
        for condition in conditions {
            let key = condition.to_key(entity_id);
            if let Some(r) = rule_map.get_mut(&key) {
                r.push(data.clone());
            } else {
                rule_map.insert(key, vec![data.clone()]);
            }
        }
    }
}
