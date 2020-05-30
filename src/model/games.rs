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

use eyre::eyre;
use eyre::Result;
use serde::{Deserialize, Serialize};

use super::{
    cards::{Card, Cost, Deck, HasCardData, HasCardId, HasCardState, Scroll},
    creatures::{Creature, CreatureData, HasCreatureData},
    stats::{Stat, StatName, Tag, TagName},
};
use crate::{
    api, commands,
    model::primitives::*,
    rules::engine::{Rule, RulesEngine},
};

pub trait HasOwner {
    fn owner(&self) -> PlayerName;

    fn is_user_owned(&self) -> bool {
        self.owner() == PlayerName::User
    }
}

#[derive(Serialize, Deserialize, Debug, Clone)]
pub struct PlayerState {
    current_life: LifeValue,
    maximum_life: LifeValue,
    current_power: PowerValue,
    maximum_power: ManaValue,
    current_influence: Influence,
    maximum_influence: Influence,
    available_scroll_plays: u32,
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
            available_scroll_plays: 1,
        }
    }
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
            .ok_or(eyre!("Card not found: {}", card_id))
    }

    pub fn draw_card(&mut self) -> Result<&Card> {
        let card = self.deck.draw_card()?;
        Ok(self.add_to_hand(card))
    }

    pub fn draw_specific_card(&mut self, id: CardId) -> Result<&Card> {
        let card = self.deck.draw_specific_card(id)?;
        Ok(self.add_to_hand(card))
    }

    pub fn draw_card_at_index(&mut self, index: usize) -> Result<&Card> {
        let card = self.deck.draw_card_at_index(index)?;
        Ok(self.add_to_hand(card))
    }

    fn add_to_hand(&mut self, mut card: Card) -> &Card {
        Self::update_can_play(&self.state, &mut card);
        self.hand.push(card);
        self.hand.last().expect("Card not found?")
    }

    pub fn decrement_life(&mut self, amount: LifeValue) {
        self.state.current_life = if amount > self.state.current_life {
            0
        } else {
            self.state.current_life - amount
        }
    }

    pub fn pay_cost(&mut self, cost: &Cost, result: &mut Vec<api::CommandGroup>) -> Result<()> {
        match cost {
            Cost::ScrollPlay => {}
            Cost::StandardCost(cost) => {
                if cost.power > self.state.current_power {
                    return Err(eyre!(
                        "Can't pay power cost {}, have only {}",
                        cost.power,
                        self.state.current_power
                    ));
                }
                self.state.current_power -= cost.power;
                self.state.current_influence.subtract(&cost.influence)?;
            }
        }

        self.update_cards(result);
        Ok(())
    }

    pub fn add_scroll(&mut self, scroll: Scroll, result: &mut Vec<api::CommandGroup>) {
        self.state.current_power += scroll.stats.added_current_power;
        self.state.maximum_power += scroll.stats.added_maximum_power;
        self.state
            .current_influence
            .add(&scroll.stats.added_current_influence);
        self.state
            .maximum_influence
            .add(&scroll.stats.added_maximum_influence);
        self.state.available_scroll_plays -= 1;
        self.scrolls.push(scroll);

        self.update_cards(result);
    }

    pub fn upkeep(&mut self, result: &mut Vec<api::CommandGroup>) -> Result<()> {
        self.state.current_power = self.state.maximum_power;
        self.state.current_influence = self.state.maximum_influence.clone();
        self.state.available_scroll_plays = 1;
        self.update_cards(result);
        let card = self.draw_card()?;
        result.push(commands::single(commands::draw_or_update_card_command(
            card,
        )));
        Ok(())
    }

    fn update_cards(&mut self, result: &mut Vec<api::CommandGroup>) {
        let state = &self.state;
        let mut group = vec![];
        for card in self.hand.iter_mut() {
            if Self::update_can_play(state, card) {
                group.push(commands::update_can_play_card_command(
                    self.name,
                    card.card_id(),
                    card.card_state().owner_can_play,
                ))
            }
        }

        group.push(commands::update_player_command(self));
        result.push(commands::group(group));
    }

    pub fn player_data(&self) -> api::PlayerData {
        api::PlayerData {
            player_name: commands::player_name(self.name).into(),
            current_life: self.state.current_life,
            maximum_life: self.state.maximum_life,
            current_power: self.state.current_power,
            maximum_power: self.state.maximum_power,
            current_influence: commands::influence(&self.state.current_influence),
            maximum_influence: commands::influence(&self.state.maximum_influence),
        }
    }

    /// Returns true if the 'can_play' value changed
    fn update_can_play(state: &PlayerState, card: &mut Card) -> bool {
        let old_value = card.card_state().owner_can_play;
        let new_value = match &card.card_data().cost {
            super::cards::Cost::ScrollPlay => state.available_scroll_plays > 0,
            super::cards::Cost::StandardCost(cost) => {
                cost.influence
                    .less_than_or_equal_to(&state.current_influence)
                    && cost.power <= state.current_power
            }
        };

        card.card_state_mut().owner_can_play = new_value;
        old_value != new_value
    }
}

impl HasOwner for Player {
    fn owner(&self) -> PlayerName {
        self.name
    }
}

#[derive(Serialize, Deserialize, Debug, Clone)]
pub enum MainButtonState {
    ToCombat,
    EndTurn,
}

#[derive(Serialize, Deserialize, Debug, Clone)]
pub struct GameState {
    pub phase: GamePhase,
    pub turn: TurnNumber,
    pub main_button: MainButtonState,
}

#[derive(Serialize, Deserialize, Debug, Clone)]
pub struct Game {
    pub id: GameId,
    pub state: GameState,
    pub user: Player,
    pub enemy: Player,
}

impl Game {
    pub fn all_creatures(&self) -> impl Iterator<Item = &Creature> {
        self.user
            .creatures
            .iter()
            .chain(self.enemy.creatures.iter())
    }

    pub fn all_creatures_mut(&mut self) -> impl Iterator<Item = &mut Creature> {
        self.user
            .creatures
            .iter_mut()
            .chain(self.enemy.creatures.iter_mut())
    }

    pub fn player(&self, player: PlayerName) -> &Player {
        match player {
            PlayerName::User => &self.user,
            PlayerName::Enemy => &self.enemy,
        }
    }

    pub fn player_mut(&mut self, player: PlayerName) -> &mut Player {
        match player {
            PlayerName::User => &mut self.user,
            PlayerName::Enemy => &mut self.enemy,
        }
    }

    pub fn creature(&self, creature_id: CreatureId) -> Result<&Creature> {
        self.all_creatures()
            .find(|c| c.creature_id() == creature_id)
            .ok_or(eyre!("Creature ID {} not found", creature_id))
    }

    pub fn creature_mut(&mut self, creature_id: CreatureId) -> Result<&mut Creature> {
        self.user
            .creatures
            .iter_mut()
            .chain(self.enemy.creatures.iter_mut())
            .find(|c| c.creature_id() == creature_id)
            .ok_or(eyre!("Creature ID {} not found", creature_id))
    }
}
