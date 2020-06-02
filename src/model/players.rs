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
    cards::{Card, Deck, HasCardData, HasCardId, Scroll},
    creatures::Creature,
    primitives::{CardId, Influence, LifeValue, ManaValue, PlayerName, PowerValue, School},
};
use crate::rules::{effects::UnderflowBehavior, engine::Rule};

pub trait HasOwner {
    fn owner(&self) -> PlayerName;

    fn is_user_owned(&self) -> bool {
        self.owner() == PlayerName::User
    }
}

#[derive(Serialize, Deserialize, Debug, Clone)]
pub struct PlayerState {
    pub current_life: LifeValue,
    pub maximum_life: LifeValue,
    pub current_power: PowerValue,
    pub maximum_power: ManaValue,
    pub current_influence: Influence,
    pub maximum_influence: Influence,
    pub current_scroll_plays: u32,
    pub maximum_scroll_plays: u32,
}

impl Default for PlayerState {
    fn default() -> Self {
        PlayerState {
            current_life: 25,
            maximum_life: 25,
            current_power: 1,
            maximum_power: 1,
            current_influence: Influence::single(1, School::Flame),
            maximum_influence: Influence::single(1, School::Flame),
            current_scroll_plays: 1,
            maximum_scroll_plays: 1,
        }
    }
}

#[derive(Debug, Clone, Copy)]
pub enum PlayerAttribute {
    CurrentLife(LifeValue),
    MaximumLife(LifeValue),
    CurrentPower(PowerValue),
    MaximumPower(PowerValue),
    CurrentInfluence(Influence),
    MaximumInfluence(Influence),
    CurrentScrollPlays(u32),
    MaximumScrollPlays(u32),
}

#[derive(Serialize, Deserialize, Debug, Clone)]
pub struct Player {
    pub name: PlayerName,
    pub state: PlayerState,
    pub deck: Deck,
    pub hand: Vec<Card>,
    pub creatures: Vec<Creature>,
    pub scrolls: Vec<Scroll>,
    pub rules: Vec<Box<dyn Rule>>,
}

impl Player {
    pub fn card(&self, card_id: CardId) -> Result<&Card> {
        self.hand
            .iter()
            .find(|c| c.card_id() == card_id)
            .ok_or_else(|| eyre!("Card not found: {}", card_id))
    }

    pub fn remove_from_hand(&mut self, card_id: CardId) -> Result<Card> {
        let position = self
            .hand
            .iter()
            .position(|c| c.card_data().id == card_id)
            .ok_or_else(|| eyre!("Card ID not found: {:?}", card_id))?;

        Ok(self.hand.remove(position))
    }

    pub fn set_attribute(&mut self, attribute: PlayerAttribute) -> Result<PlayerAttribute> {
        self.modify_attribute(
            attribute,
            |int_ref, int_val| {
                *int_ref = int_val;
                Ok(())
            },
            |influence_ref, influence_val| {
                *influence_ref = influence_val;
                Ok(())
            },
        )
    }

    pub fn add_attribute(&mut self, attribute: PlayerAttribute) -> Result<PlayerAttribute> {
        self.modify_attribute(
            attribute,
            |int_ref, int_val| {
                *int_ref += int_val;
                Ok(())
            },
            |influence_ref, influence_val| {
                influence_ref.add(&influence_val);
                Ok(())
            },
        )
    }

    pub fn subtract_attribute(
        &mut self,
        attribute: PlayerAttribute,
        underflow_behavior: UnderflowBehavior,
    ) -> Result<PlayerAttribute> {
        self.modify_attribute(
            attribute,
            |int_ref, int_val| {
                if int_val > *int_ref {
                    match underflow_behavior {
                        UnderflowBehavior::Error => Err(eyre!(
                            "Cannot subtract, {} is larger than {}",
                            int_val,
                            *int_ref
                        )),
                        UnderflowBehavior::SetZero => {
                            *int_ref = 0;
                            Ok(())
                        }
                    }
                } else {
                    *int_ref -= int_val;
                    Ok(())
                }
            },
            |influence_ref, influence_val| {
                influence_ref.subtract(&influence_val, underflow_behavior)
            },
        )
    }
    fn modify_attribute(
        &mut self,
        attribute: PlayerAttribute,
        int_function: impl FnOnce(&mut u32, u32) -> Result<()>,
        influence_function: impl FnOnce(&mut Influence, Influence) -> Result<()>,
    ) -> Result<PlayerAttribute> {
        match attribute {
            PlayerAttribute::CurrentLife(life) => {
                int_function(&mut self.state.current_life, life)?;
                Ok(PlayerAttribute::CurrentLife(self.state.current_life))
            }
            PlayerAttribute::MaximumLife(life) => {
                int_function(&mut self.state.maximum_life, life)?;
                Ok(PlayerAttribute::MaximumLife(self.state.maximum_life))
            }
            PlayerAttribute::CurrentPower(power) => {
                int_function(&mut self.state.current_power, power)?;
                Ok(PlayerAttribute::CurrentPower(self.state.current_power))
            }
            PlayerAttribute::MaximumPower(power) => {
                int_function(&mut self.state.maximum_power, power)?;
                Ok(PlayerAttribute::MaximumPower(self.state.maximum_power))
            }
            PlayerAttribute::CurrentInfluence(influence) => {
                influence_function(&mut self.state.current_influence, influence)?;
                Ok(PlayerAttribute::CurrentInfluence(
                    self.state.current_influence,
                ))
            }
            PlayerAttribute::MaximumInfluence(influence) => {
                influence_function(&mut self.state.maximum_influence, influence)?;
                Ok(PlayerAttribute::MaximumInfluence(
                    self.state.maximum_influence,
                ))
            }
            PlayerAttribute::CurrentScrollPlays(plays) => {
                int_function(&mut self.state.current_scroll_plays, plays)?;
                Ok(PlayerAttribute::CurrentScrollPlays(
                    self.state.current_scroll_plays,
                ))
            }
            PlayerAttribute::MaximumScrollPlays(plays) => {
                int_function(&mut self.state.maximum_scroll_plays, plays)?;
                Ok(PlayerAttribute::MaximumScrollPlays(
                    self.state.maximum_scroll_plays,
                ))
            }
        }
    }
}

impl HasOwner for Player {
    fn owner(&self) -> PlayerName {
        self.name
    }
}
