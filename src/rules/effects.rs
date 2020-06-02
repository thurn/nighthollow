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
    engine::{RuleIdentifier, TriggerContext},
    events::{Event, Events, PlayerAttributeModified},
};
use crate::{
    api,
    model::{
        cards::HasCardId,
        games::Game,
        players::PlayerAttribute,
        primitives::{CreatureId, PlayerName},
        stats::{Operation, StatName},
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
    ModifyPlayerAttribute(PlayerName, Operator, PlayerAttribute),
    SetSkillPriority(CreatureId, RuleIdentifier, u32),
    UseCreatureSkill(CreatureSkill),
}

pub fn apply_effect(game: &mut Game, events: &mut Events, effect_data: &EffectData) -> Result<()> {
    let identifier = effect_data.identifier();
    match &effect_data.effect {
        Effect::DrawCard(player_name) => {
            let player = game.player_mut(*player_name);
            let card = player.deck.draw_card()?;
            events.push_event(identifier, Event::CardDrawn(player.name, card.card_id()));
            player.hand.push(card);
        }
        Effect::ModifyPlayerAttribute(player_name, operator, player_attribute) => {
            modify_player_attribute(
                game,
                events,
                identifier,
                *player_name,
                *operator,
                *player_attribute,
            )?;
        }
        Effect::SetSkillPriority(creature_id, rule_identifier, priority) => game
            .creature_mut(*creature_id)?
            .set_skill_priority(*rule_identifier, *priority),
        Effect::UseCreatureSkill(skill) => {
            creature_skills::apply_creature_skill(game, events, identifier, skill)?;
        }
    }
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
