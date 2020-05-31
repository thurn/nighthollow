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

use eyre::{eyre, Result};

use super::{
    engine::RuleIdentifier,
    events::{Event, Events},
};
use crate::{
    api,
    model::{
        cards::HasCardId,
        games::{Game, PlayerAttribute},
        primitives::{CreatureId, PlayerName},
        stats::{Operation, StatName},
    },
};

#[derive(Debug, Clone)]
pub enum EffectSource {
    Creature(CreatureId),
    Player(PlayerName),
    Game,
}

#[derive(Debug, Clone)]
pub struct EffectData {
    pub effect: Effect,
    pub rule_identifier: RuleIdentifier,
}

impl EffectData {
    pub fn identifier(&self) -> RuleIdentifier {
        self.rule_identifier.clone()
    }
}

#[derive(Debug)]
pub struct Effects {
    data: Vec<EffectData>,
}

impl Effects {
    pub fn push_effect(&mut self, identifier: &RuleIdentifier, effect: Effect) {
        self.data.push(EffectData {
            effect,
            rule_identifier: identifier.clone(),
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

#[derive(Debug, Clone)]
pub enum Effect {
    DrawCard(PlayerName),
    SetPlayerAttribute(PlayerName, PlayerAttribute),
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
        Effect::SetPlayerAttribute(player_name, player_attribute) => {
            set_player_attribute(game, events, identifier, *player_name, player_attribute)
        }
    }
    Ok(())
}

fn set_player_attribute(
    game: &mut Game,
    events: &mut Events,
    identifier: RuleIdentifier,
    player_name: PlayerName,
    attribute: &PlayerAttribute,
) {
    let mut state = &mut game.player_mut(player_name).state;
    match attribute {
        PlayerAttribute::CurrentLife(life) => {
            state.current_life = *life;
        }
        PlayerAttribute::MaximumLife(life) => {
            state.maximum_life = *life;
        }
        PlayerAttribute::CurrentPower(power) => {
            state.current_power = *power;
        }
        PlayerAttribute::MaximumPower(power) => {
            state.maximum_power = *power;
        }
        PlayerAttribute::CurrentInfluence(influence) => {
            state.current_influence = influence.clone();
        }
        PlayerAttribute::MaximumInfluence(influence) => {
            state.maximum_influence = influence.clone();
        }
        PlayerAttribute::CurrentScrollPlays(plays) => {
            state.current_scroll_plays = *plays;
        }
        PlayerAttribute::MaximumScrollPlays(plays) => {
            state.maximum_scroll_plays = *plays;
        }
    }

    events.push_event(
        identifier,
        Event::PlayerAttributeSet(player_name, attribute.clone()),
    );
}

#[derive(Debug, Clone)]
pub struct SetModifier {
    pub stat: StatName,
    pub value: u32,
    pub operation: Operation,
}
