// Copyright The Magewatch Project

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

use rand::{
    distributions::{Distribution, WeightedIndex},
    prelude::thread_rng,
};

use crate::{
    model::primitives::{Influence, InterfaceError, ManaValue, Result},
    model::types::{CardVariant, Cost, Player, Spell},
};

pub fn draw_card(player: &mut Player) -> Result<()> {
    let distribution = WeightedIndex::new(&player.deck.weights)?;
    let mut rng = thread_rng();
    let index = distribution.sample(&mut rng);
    let card = player.deck.cards[index].clone();
    player.hand.push(card);
    let weight = player.deck.weights[index];
    player.deck.weights[index] = match weight {
        // Linear descent for the first 4 draws, then halves
        4000 => 3000,
        3000 => 2000,
        2000 => 1000,
        _ => cmp::max(1, weight / 2),
    };
    Ok(())
}

pub fn play_card(player: &mut Player, card_id: &str) -> Result<()> {
    if !can_pay_cost(player, card_id)? {
        return InterfaceError::result(format!("Cannot pay cost for card {}", card_id));
    }

    pay_costs(player, card_id)?;

    match player.hand.remove(player.find_card_in_hand(card_id)?) {
        CardVariant::Creature(c) => player.creatures.push(c),
        CardVariant::Crystal(c) => player.crystals.push(c),
        CardVariant::Structure(s) => player.structures.push(s),
        CardVariant::Spell(s) => resolve_spell(player, s),
    }
    Ok(())
}

pub fn can_pay_cost(player: &Player, card_id: &str) -> Result<bool> {
    match &player.hand[player.find_card_in_hand(card_id)?].card().cost {
        Cost::None => Ok(true),
        Cost::ManaCost(c) => {
            Ok(c.influence <= player.status.influence && c.mana <= player.status.mana)
        }
    }
}

fn pay_costs(player: &mut Player, card_id: &str) -> Result<()> {
    match &player.hand[player.find_card_in_hand(card_id)?].card().cost {
        Cost::None => Ok(()),
        Cost::ManaCost(c) => {
            player.status.mana -= c.mana;
            player.status.influence.subtract(&c.influence);
            Ok(())
        }
    }
}

fn resolve_spell(player: &mut Player, spell: Spell) {
    todo!("Implement this");
}

pub fn upkeep_mana(player: &mut Player) {
    player.status.mana = ManaValue::from(0);
    player.status.influence = Influence::default();

    for crystal in player.crystals.iter() {
        player.status.mana += crystal.mana_per_turn;
        player.status.influence.add(&crystal.influence_per_turn);
    }
}
