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

use std::cmp;
use std::sync::atomic::{AtomicI32, Ordering};

use eyre::eyre;
use eyre::Result;
use rand::{
    distributions::{Distribution, WeightedIndex},
    prelude::thread_rng,
};
use serde::{Deserialize, Serialize};

use super::{
    assets::{ScrollType, SpellType},
    creatures::{Creature, CreatureData},
    games::HasOwner,
    primitives::{
        CardId, FileValue, Influence, PlayerName, PowerValue, RankValue, School, ScrollId, SpellId,
    },
};

static NEXT_CARD_ID: AtomicI32 = AtomicI32::new(1);

#[derive(Serialize, Deserialize, Debug, Clone)]
pub struct StandardCost {
    pub power: PowerValue,
    pub influence: Influence,
}

#[derive(Serialize, Deserialize, Debug, Clone)]
pub enum Cost {
    ScrollPlay,
    StandardCost(StandardCost),
}

#[derive(Serialize, Deserialize, Debug, Clone)]
pub struct CardState {
    pub owner_can_play: bool,
    pub revealed_to_opponent: bool,
}

impl Default for CardState {
    fn default() -> Self {
        CardState {
            owner_can_play: true, // tmp
            revealed_to_opponent: false,
        }
    }
}

#[derive(Serialize, Deserialize, Debug, Clone)]
pub struct CardData {
    pub id: CardId,
    pub owner: PlayerName,
    pub state: CardState,
    pub cost: Cost,
    pub name: String,
    pub school: School,
    pub text: String,
}

pub trait HasCardData {
    fn card_data(&self) -> &CardData;
    fn card_data_mut(&mut self) -> &mut CardData;
}

impl HasCardData for CardData {
    fn card_data(&self) -> &CardData {
        self
    }

    fn card_data_mut(&mut self) -> &mut CardData {
        self
    }
}

impl<T: HasCardData> HasOwner for T {
    fn owner(&self) -> PlayerName {
        self.card_data().owner
    }
}

pub trait HasCardState {
    fn card_state(&self) -> &CardState;
    fn card_state_mut(&mut self) -> &mut CardState;
}

impl<T: HasCardData> HasCardState for T {
    fn card_state(&self) -> &CardState {
        &self.card_data().state
    }

    fn card_state_mut(&mut self) -> &mut CardState {
        &mut self.card_data_mut().state
    }
}

pub trait HasCardId {
    fn card_id(&self) -> CardId;
}

impl<T: HasCardData> HasCardId for T {
    fn card_id(&self) -> CardId {
        self.card_data().id
    }
}

pub trait HasSchool {
    fn school(&self) -> School;
}

impl<T: HasCardData> HasSchool for T {
    fn school(&self) -> School {
        self.card_data().school
    }
}

#[derive(Serialize, Deserialize, Debug, Clone)]
pub struct Spell {
    pub card_data: CardData,
    pub base_type: SpellType,
}

impl Spell {
    pub fn spell_id(&self) -> SpellId {
        self.card_data.id
    }
}

impl HasCardData for Spell {
    fn card_data(&self) -> &CardData {
        &self.card_data
    }

    fn card_data_mut(&mut self) -> &mut CardData {
        &mut self.card_data
    }
}

#[derive(Serialize, Deserialize, Debug, Clone)]
pub struct ScrollStats {
    pub added_current_power: PowerValue,
    pub added_maximum_power: PowerValue,
    pub added_current_influence: Influence,
    pub added_maximum_influence: Influence,
}

#[derive(Serialize, Deserialize, Debug, Clone)]
pub struct Scroll {
    pub card_data: CardData,
    pub base_type: ScrollType,
    pub stats: ScrollStats,
}

impl Scroll {
    pub fn scroll_id(&self) -> ScrollId {
        self.card_data.id
    }
}

impl HasCardData for Scroll {
    fn card_data(&self) -> &CardData {
        &self.card_data
    }

    fn card_data_mut(&mut self) -> &mut CardData {
        &mut self.card_data
    }
}

#[derive(Serialize, Deserialize, Debug, Clone)]
pub enum Card {
    Creature(CreatureData),
    Spell(Spell),
    Scroll(Scroll),
}

impl HasCardData for Card {
    fn card_data(&self) -> &CardData {
        match self {
            Card::Creature(c) => c.card_data(),
            Card::Spell(s) => s.card_data(),
            Card::Scroll(s) => s.card_data(),
        }
    }

    fn card_data_mut(&mut self) -> &mut CardData {
        match self {
            Card::Creature(c) => c.card_data_mut(),
            Card::Spell(s) => s.card_data_mut(),
            Card::Scroll(s) => s.card_data_mut(),
        }
    }
}

#[derive(Serialize, Deserialize, Debug, Clone)]
pub struct Deck {
    cards: Vec<Card>,
    weights: Vec<u32>,
}

impl Deck {
    /// Instantiates a new deck given one copy of each card it should contain.
    /// The IDs and Owners on the provided cards are overwritten with new
    /// values.
    pub fn new(mut cards: Vec<Card>, owner: PlayerName) -> Self {
        let len = cards.len();
        cards.iter_mut().for_each(|c| {
            c.card_data_mut().id = next_card_id();
            c.card_data_mut().owner = owner;
        });
        Deck {
            cards,
            weights: vec![4000; len],
        }
    }

    /// Iterates over the types of card in this deck
    pub fn cards(&self) -> impl Iterator<Item = &Card> {
        self.cards.iter()
    }

    /// Randomly draws a card from this deck and updates the card draw weights
    pub fn draw_card(&mut self) -> Result<Card> {
        let distribution = WeightedIndex::new(&self.weights)?;
        let mut rng = thread_rng();
        self.draw_card_at_index(distribution.sample(&mut rng))
    }

    /// Draws a card with a specific ID from the deck, updating draw weights
    /// as appropriate
    pub fn draw_specific_card(&mut self, card_id: CardId) -> Result<Card> {
        let card_index = self
            .cards
            .iter()
            .position(|c| c.card_data().id == card_id)
            .ok_or_else(|| eyre!("Card not found: {}", card_id))?;
        self.draw_card_at_index(card_index)
    }

    /// Draws a card at a specific index position within the deck, updating
    /// draw weights as appropriate.
    pub fn draw_card_at_index(&mut self, card_index: usize) -> Result<Card> {
        let mut card = self
            .cards
            .get(card_index)
            .ok_or_else(|| eyre!("Card at index {} not found", card_index))?
            .clone();
        self.decrement_weights(card_index)?;
        card.card_data_mut().id = next_card_id();
        Ok(card)
    }

    fn decrement_weights(&mut self, index: usize) -> Result<()> {
        let weight = self
            .weights
            .get(index)
            .ok_or_else(|| eyre!("Index not found {}", index))?;
        self.weights[index] = match weight {
            // Linear descent for the first 4 draws, then halves
            4000 => 3000,
            3000 => 2000,
            2000 => 1000,
            _ => cmp::max(1, weight / 2),
        };
        Ok(())
    }
}

// Resets ID generation so subsequent cards will have deterministic IDs
pub fn debug_reset_id_generation() {
    NEXT_CARD_ID.store(1, Ordering::SeqCst)
}

fn next_card_id() -> CardId {
    NEXT_CARD_ID.fetch_add(1, Ordering::SeqCst)
}
