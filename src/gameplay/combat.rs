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

use crate::model::types::{Attack, Attackable, AttackerMut, CreatureState, Game, Player};

pub fn apply_combat(game: &mut Game) {
    for attacker in game.user.attackers_mut() {
        apply_combat_for_attacker(attacker, &mut game.enemy);
    }

    for attacker in game.enemy.attackers_mut() {
        apply_combat_for_attacker(attacker, &mut game.user);
    }

    game.user
        .creatures
        .iter_mut()
        .for_each(|c| c.state = CreatureState::Default);
    game.enemy
        .creatures
        .iter_mut()
        .for_each(|c| c.state = CreatureState::Default);
}

fn apply_combat_for_attacker((attacker, position): AttackerMut, defending_player: &mut Player) {
    if let Some((defender, _)) = defending_player
        .defenders_mut()
        .find(|(_, p)| position == *p)
    {
        apply_attack(&attacker.attack, defender);
        apply_attack(&defender.attack, attacker);
    }

    apply_attack(&attacker.attack, &mut defending_player.status);
}

fn apply_attack(attack: &Attack, defender: &mut impl Attackable) {
    match attack {
        Attack::BasicAttack(damage) => defender.apply_damage(damage),
    }
}
