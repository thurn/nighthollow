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
    creature_skills::{CreatureMutation, CreatureSkill, MutationResult},
    effects::{AttributeMutationResult, Effects},
    engine::{RuleIdentifier, RulesEngine, Trigger},
};
use crate::{
    api,
    model::{
        games::Game,
        players::PlayerAttribute,
        primitives::{CardId, CreatureId, PlayerName},
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
    PlayerAttributeModified(PlayerAttributeModified),
    CreatureMutated(CreatureMutated),
    CreatureSkillUsed(CreatureSkill),
    CanPlayCardModified(CardId),
}

#[derive(Debug, Copy, Clone)]
pub struct PlayerAttributeModified {
    pub player_name: PlayerName,
    pub attribute: PlayerAttribute,
    pub result: AttributeMutationResult,
}

#[derive(Debug, Clone)]
pub struct CreatureMutated {
    pub source_creature: CreatureId,
    pub mutation: CreatureMutation,
    pub result: MutationResult,
}

pub fn populate_event_rule_effects(
    engine: &RulesEngine,
    effects: &mut Effects,
    event_data: &EventData,
) -> Result<()> {
    match &event_data.event {
        Event::CardDrawn(player_name, card_id) => {
            println!("CardDrawn event {:?}", card_id);
            engine.populate_rule_effects(effects, Trigger::CardDrawn(*player_name, *card_id))
        }
        Event::PlayerAttributeModified(modified) => {
            populate_player_attribute_events(engine, effects, modified)
        }
        Event::CreatureMutated(CreatureMutated {
            source_creature,
            mutation,
            result,
        }) => {
            populate_creature_mutation_events(engine, effects, *source_creature, mutation, result)
        }
        _ => Ok(()),
    }
}

fn populate_player_attribute_events(
    engine: &RulesEngine,
    effects: &mut Effects,
    modified: &PlayerAttributeModified,
) -> Result<()> {
    if modified.result == AttributeMutationResult::GameOver {
        engine.populate_rule_effects(effects, Trigger::PlayerLifeZero(modified.player_name))?;
    }

    match modified.attribute {
        PlayerAttribute::CurrentLife(life) => engine.populate_rule_effects(
            effects,
            Trigger::PlayerLifeChanged(modified.player_name, life),
        ),
        PlayerAttribute::CurrentPower(power) => engine.populate_rule_effects(
            effects,
            Trigger::PlayerPowerChanged(modified.player_name, power),
        ),
        PlayerAttribute::CurrentInfluence(influence) => engine.populate_rule_effects(
            effects,
            Trigger::PlayerInfluenceChanged(modified.player_name, influence),
        ),
        _ => Ok(()),
    }
}

fn populate_creature_mutation_events(
    engine: &RulesEngine,
    effects: &mut Effects,
    source_id: CreatureId,
    mutation: &CreatureMutation,
    result: &MutationResult,
) -> Result<()> {
    let target_id = mutation.target_id;
    for modifier in &mutation.set_modifiers {
        engine.populate_rule_effects(
            effects,
            Trigger::CreatureStatModifierSet {
                source: target_id,
                target: target_id,
                modifier: *modifier,
            },
        )?;
    }

    if let Some(amount) = mutation.heal_damage {
        engine.populate_rule_effects(
            effects,
            Trigger::CreatureHealed {
                source: target_id,
                target: target_id,
                amount,
            },
        )?;
    }

    if let Some(damage) = &mutation.apply_damage {
        engine.populate_rule_effects(
            effects,
            Trigger::CreatureDamaged {
                attacker: target_id,
                defender: target_id,
                damage: *damage,
            },
        )?;

        if *result == MutationResult::Killed {
            engine.populate_rule_effects(
                effects,
                Trigger::CreatureKilled {
                    killed_by: target_id,
                    defender: target_id,
                    damage: *damage,
                },
            )?;
        }
    }

    if let Some(amount) = mutation.gain_mana {
        engine.populate_rule_effects(
            effects,
            Trigger::CreatureManaGained {
                source: target_id,
                target: target_id,
                amount,
            },
        )?;
    }

    if let Some(amount) = mutation.lose_mana {
        engine.populate_rule_effects(
            effects,
            Trigger::CreatureManaLost {
                source: target_id,
                target: target_id,
                amount,
            },
        )?;
    }

    Ok(())
}
