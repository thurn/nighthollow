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
    effects::Effects,
    engine::{RuleIdentifier, RulesEngine, Trigger},
};
use crate::{
    api,
    model::{
        games::{Game, PlayerAttribute},
        primitives::{CardId, PlayerName},
    },
};

#[derive(Debug, Clone)]
pub struct EventData {
    pub event: Event,
    pub rule_identifier: RuleIdentifier,
}

pub struct Events {
    pub data: Vec<EventData>,
}

impl Events {
    pub fn new() -> Events {
        Events { data: vec![] }
    }

    pub fn push_event(&mut self, rule_identifier: RuleIdentifier, event: Event) {
        self.data.push(EventData {
            event,
            rule_identifier,
        });
    }
}

#[derive(Debug, Clone)]
pub enum Event {
    CardDrawn(PlayerName, CardId),
    PlayerAttributeSet(PlayerName, PlayerAttribute),
}

pub fn populate_event_rule_effects(
    engine: &RulesEngine,
    effects: &mut Effects,
    event_data: &EventData,
) -> Result<()> {
    match &event_data.event {
        Event::CardDrawn(player_name, card_id) => {
            engine.populate_rule_effects(effects, Trigger::CardDrawn(*player_name, *card_id))?;
        }
        Event::PlayerAttributeSet(player_name, attribute) => {
            populate_player_attribute_events(engine, effects, *player_name, attribute.clone())?
        }
    }

    Ok(())
}

fn populate_player_attribute_events(
    engine: &RulesEngine,
    effects: &mut Effects,
    player_name: PlayerName,
    attribute: PlayerAttribute,
) -> Result<()> {
    match attribute {
        PlayerAttribute::CurrentLife(life) => {
            engine.populate_rule_effects(effects, Trigger::PlayerLifeChanged(player_name, life))
        }
        PlayerAttribute::CurrentPower(power) => {
            engine.populate_rule_effects(effects, Trigger::PlayerPowerChanged(player_name, power))
        }
        PlayerAttribute::CurrentInfluence(influence) => engine.populate_rule_effects(
            effects,
            Trigger::PlayerInfluenceChanged(player_name, influence),
        ),
        _ => Ok(()),
    }
}
