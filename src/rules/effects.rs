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
    creature_skills::{self, CreatureSkill},
    engine::{EntityId, RuleIdentifier, RulesEngine, TriggerContext},
    events::{Event, Events, PlayerAttributeModified},
};
use crate::{
    api,
    model::{
        cards::{Cost, HasCardData, HasCardId},
        games::Game,
        players::PlayerAttribute,
        primitives::{CardId, CreatureId, PlayerName},
        stats::{Operation, StatName, TagModifier},
    },
};

#[derive(Debug, Clone)]
pub struct EffectData {
    pub effect: Effect,
    pub rule_identifier: RuleIdentifier,
}

impl EffectData {
    pub fn identifier(&self) -> RuleIdentifier {
        self.rule_identifier
    }
}

#[derive(Debug)]
pub struct Effects {
    data: Vec<EffectData>,
}

impl Effects {
    pub fn push_effect(&mut self, context: &TriggerContext, effect: Effect) {
        self.data.push(EffectData {
            effect,
            rule_identifier: *context.identifier,
        });
    }

    pub fn len(&self) -> usize {
        self.data.len()
    }

    pub fn iter(&self) -> impl Iterator<Item = &EffectData> {
        self.data.iter()
    }
}

impl Default for Effects {
    fn default() -> Effects {
        Effects { data: vec![] }
    }
}

#[derive(Debug, Clone, Copy)]
pub enum UnderflowBehavior {
    Error,
    SetZero,
}

#[derive(Debug, Clone, Copy)]
pub enum Operator {
    Set,
    Add,
    Subtract(UnderflowBehavior),
}

#[derive(Debug, Clone)]
pub enum Effect {
    DrawCard(PlayerName),
    PlayCard(PlayerName, CardId),
    ModifyPlayerAttribute(PlayerName, Operator, PlayerAttribute),
    SetSkillPriority(CreatureId, RuleIdentifier, u32),
    UseCreatureSkill(CreatureSkill),
    SetCanPlayCard(CardId, bool),
}

pub fn apply_effect(
    engine: &mut RulesEngine,
    events: &mut Events,
    effect_data: &EffectData,
) -> Result<()> {
    let identifier = effect_data.identifier();
    match &effect_data.effect {
        Effect::DrawCard(player_name) => {
            draw_card(engine, events, identifier, *player_name)?;
        }
        Effect::PlayCard(player_name, card_id) => {
            play_card(&mut engine.game, events, identifier, *player_name, *card_id)?;
        }
        Effect::ModifyPlayerAttribute(player_name, operator, player_attribute) => {
            modify_player_attribute(
                &mut engine.game,
                events,
                identifier,
                *player_name,
                *operator,
                *player_attribute,
            )?;
        }
        Effect::SetSkillPriority(creature_id, rule_identifier, priority) => engine
            .game
            .creature_mut(*creature_id)?
            .set_skill_priority(*rule_identifier, *priority),
        Effect::UseCreatureSkill(skill) => {
            creature_skills::apply_creature_skill(&mut engine.game, events, identifier, skill)?;
        }
        Effect::SetCanPlayCard(card_id, value) => {
            set_can_play_card(engine, events, identifier, *card_id, *value)?;
        }
    }
    Ok(())
}

fn draw_card(
    engine: &mut RulesEngine,
    events: &mut Events,
    identifier: RuleIdentifier,
    player_name: PlayerName,
) -> Result<()> {
    let player = engine.game.player_mut(player_name);
    let card = player.deck.draw_card()?;
    let card_id = card.card_id();
    let rules = card.card_data().rules.clone();
    events.push_event(identifier, Event::CardDrawn(player.name, card.card_id()));
    player.hand.push(card);
    engine.add_rules(EntityId::CardId(card_id), rules);
    Ok(())
}

fn play_card(
    game: &mut Game,
    events: &mut Events,
    identifier: RuleIdentifier,
    player_name: PlayerName,
    card_id: CardId,
) -> Result<()> {
    let cost = game.player(player_name).card(card_id)?.card_data().cost;

    match cost {
        Cost::ScrollPlay => {
            modify_player_attribute(
                game,
                events,
                identifier,
                player_name,
                Operator::Subtract(UnderflowBehavior::Error),
                PlayerAttribute::CurrentScrollPlays(1),
            )?;
        }
        Cost::StandardCost(standard_cost) => {
            modify_player_attribute(
                game,
                events,
                identifier,
                player_name,
                Operator::Subtract(UnderflowBehavior::Error),
                PlayerAttribute::CurrentPower(standard_cost.power),
            )?;
            modify_player_attribute(
                game,
                events,
                identifier,
                player_name,
                Operator::Subtract(UnderflowBehavior::Error),
                PlayerAttribute::CurrentInfluence(standard_cost.influence),
            )?;
        }
    }

    events.push_event(identifier, Event::CardPlayed(player_name, card_id));
    Ok(())
}

#[derive(Debug, Copy, Clone, Eq, PartialEq)]
pub enum AttributeMutationResult {
    None,
    GameOver,
}

fn modify_player_attribute(
    game: &mut Game,
    events: &mut Events,
    identifier: RuleIdentifier,
    player_name: PlayerName,
    operator: Operator,
    attribute: PlayerAttribute,
) -> Result<()> {
    let player = &mut game.player_mut(player_name);
    let new_value = match operator {
        Operator::Set => player.set_attribute(attribute),
        Operator::Add => player.add_attribute(attribute),
        Operator::Subtract(underflow_bheavior) => {
            player.subtract_attribute(attribute, underflow_bheavior)
        }
    };

    events.push_event(
        identifier,
        Event::PlayerAttributeModified(PlayerAttributeModified {
            player_name: player.name,
            attribute: new_value?,
            result: if player.state.current_life == 0 {
                AttributeMutationResult::GameOver
            } else {
                AttributeMutationResult::None
            },
        }),
    );

    Ok(())
}

#[derive(Debug, Copy, Clone)]
pub struct SetModifier {
    pub stat: StatName,
    pub value: u32,
    pub operation: Operation,
}

fn set_can_play_card(
    engine: &mut RulesEngine,
    events: &mut Events,
    identifier: RuleIdentifier,
    card_id: CardId,
    value: bool,
) -> Result<()> {
    let card = engine.game.card_mut(card_id)?.card_data_mut();
    let previous_value = card.state.owner_can_play.value();
    card.state.owner_can_play.set_modifier(TagModifier {
        value,
        source: identifier,
    });

    if card.state.owner_can_play.value() != previous_value {
        events.push_event(identifier, Event::CanPlayCardModified(card_id));
    }
    Ok(())
}
