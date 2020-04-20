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

use crate::{
    card::{Card, CardVariant},
    state::{Game, PlayerState},
    unit::{Attack, Attackable, Unit},
};

pub fn apply_combat(game: &mut Game) {
    for attacker in &mut game.user.attackers {
        apply_combat_for_card(attacker, &mut game.enemy)
    }

    for attacker in &mut game.enemy.attackers {
        apply_combat_for_card(attacker, &mut game.user)
    }

    game.user.reserve.extend(game.user.attackers.drain(..));
    game.user.reserve.extend(game.user.defenders.drain(..));
    game.enemy.reserve.extend(game.enemy.attackers.drain(..));
    game.enemy.reserve.extend(game.enemy.defenders.drain(..));
}

fn apply_combat_for_card(card: &mut Card, defending_player: &mut PlayerState) {
    if let CardVariant::Unit(Unit {
        position: Some(p1), ..
    }) = card.variant
    {
        let attacker = card.expect_unit_mut();
        let defenders = &mut defending_player.defenders;
        if let Some(defender) = defenders.into_iter().find(|c| match c.variant {
            CardVariant::Unit(Unit {
                position: Some(p2), ..
            }) => p1 == p2,
            _ => false,
        }) {
            let defender = defender.expect_unit_mut();
            for attack in &attacker.attacks {
                apply_attack(attack, defender)
            }
            for attack in &defender.attacks {
                apply_attack(attack, attacker)
            }
        } else {
            for attack in &attacker.attacks {
                apply_attack(attack, defending_player.hero.expect_hero_mut())
            }
        }
    } else {
        panic!(
            "Expected an attacker with a combat position but got {:?}",
            card
        );
    }
}

fn apply_attack(attack: &Attack, defender: &mut impl Attackable) {
    match attack {
        Attack::BasicAttack(damage) => defender.apply_health_change(-*damage),
    }
}
