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

use std::collections::BTreeSet;

use eyre::{eyre, Result};

use super::{
    engine::RuleIdentifier,
    events::{Event, Events},
};
use crate::{
    api,
    model::{
        cards::HasCardId,
        games::Game,
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

#[derive(Debug, Clone, Ord, PartialOrd, Eq, PartialEq)]
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
    data: BTreeSet<EffectData>,
}

impl Effects {
    pub fn new() -> Effects {
        Effects {
            data: BTreeSet::new(),
        }
    }

    pub fn push_effect(&mut self, identifier: &RuleIdentifier, effect: Effect) {
        self.data.insert(EffectData {
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

#[derive(Debug, Clone, Ord, PartialOrd, Eq, PartialEq)]
pub enum Effect {
    DrawCard(PlayerName),
}

pub fn apply_effect(game: &mut Game, events: &mut Events, effect_data: &EffectData) -> Result<()> {
    match &effect_data.effect {
        Effect::DrawCard(player_name) => {
            let player = game.player_mut(*player_name);
            let card = player.deck.draw_card()?;
            events.push_event(
                effect_data.identifier(),
                Event::CardDrawn(player.name, card.card_id()),
            );
            player.hand.push(card);
        }
    }
    Ok(())
}

#[derive(Debug, Clone)]
pub struct SetModifier {
    pub stat: StatName,
    pub value: u32,
    pub operation: Operation,
}
