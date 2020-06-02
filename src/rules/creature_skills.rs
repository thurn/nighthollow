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
    effects::{Effect, SetModifier},
    engine::RuleIdentifier,
    events::{CreatureMutated, Event, Events},
};
use crate::{
    api::{
        self, MCreatureUpdate, MFireProjectile, MOnImpact, MOnImpactNumber,
        MUseCreatureSkillCommand,
    },
    commands,
    model::{
        assets::{HasAddress, ParticleSystemName, ProjectileName},
        creatures::{Creature, DamageResult, HasCreatureData},
        games::Game,
        primitives::{CreatureId, Damage, HealthValue, ManaValue, SkillAnimation},
        stats::{Modifier, Operation, StatName},
    },
};

#[derive(Debug, Clone)]
pub struct CreatureSkill {
    pub source_creature: CreatureId,

    // A skill animation to play
    pub animation: SkillAnimation,

    // Effects to apply when the animation reaches its impact frame
    pub on_impact: Vec<OnImpactNumber>,

    // Optionally, a target for this skill. The creature will move into
    // melee range with this target before playing the skill animation, but
    // this choice is cosmetic -- it has no gameplay effect.
    pub melee_target: Option<CreatureId>,
}

impl CreatureSkill {
    pub fn simple_melee(
        creature_id: CreatureId,
        animation: SkillAnimation,
        mutation: CreatureMutation,
    ) -> Effect {
        Effect::UseCreatureSkill(CreatureSkill {
            source_creature: creature_id,
            animation,
            melee_target: Some(mutation.target_id),
            on_impact: vec![OnImpactNumber {
                impact_number: 1,
                effect: OnImpact::ApplyCreatureMutation(mutation),
            }],
        })
    }
}

#[derive(Debug, Clone)]
pub struct OnImpactNumber {
    pub impact_number: u32,
    pub effect: OnImpact,
}

#[derive(Debug, Clone)]
pub enum OnImpact {
    ApplyCreatureMutation(CreatureMutation),
    FireProjectile(FireProjectile),
}

#[derive(Debug, Clone)]
pub struct FireProjectile {
    pub projectile: ProjectileName,
    pub on_hit: Vec<OnImpact>,
    pub target: ProjectileTarget,
}

#[derive(Debug, Clone)]
pub enum ProjectileTarget {
    TargetCreature(CreatureId),
}

#[derive(Debug, Clone)]
pub struct CreatureMutation {
    pub target_id: CreatureId,
    pub set_modifiers: Vec<SetModifier>,
    pub heal_damage: Option<HealthValue>,
    pub apply_damage: Option<Damage>,
    pub gain_mana: Option<ManaValue>,
    pub lose_mana: Option<ManaValue>,
    pub play_particle_systems: Vec<ParticleSystemName>,
}

impl CreatureMutation {
    pub fn new(target_id: CreatureId) -> Self {
        CreatureMutation {
            target_id,
            set_modifiers: vec![],
            apply_damage: None,
            heal_damage: None,
            gain_mana: None,
            lose_mana: None,
            play_particle_systems: vec![],
        }
    }
}

pub fn apply_creature_skill(
    game: &mut Game,
    events: &mut Events,
    identifier: RuleIdentifier,
    skill: &CreatureSkill,
) -> Result<()> {
    for on_impact in &skill.on_impact {
        match &on_impact.effect {
            OnImpact::ApplyCreatureMutation(apply) => {
                apply_creature_mutation(game, events, identifier, apply, skill)?;
            }
            OnImpact::FireProjectile(_) => {}
        }
    }

    events.push_event(identifier, Event::CreatureSkillUsed(skill.clone()));
    Ok(())
}

#[derive(Debug, Clone, Eq, PartialEq)]
pub enum MutationResult {
    None,
    Killed,
}

fn apply_creature_mutation(
    game: &mut Game,
    events: &mut Events,
    identifier: RuleIdentifier,
    mutation: &CreatureMutation,
    skill: &CreatureSkill,
) -> Result<()> {
    let target_id = mutation.target_id;
    let creature = game.creature_mut(target_id)?;
    let mut result = MutationResult::None;

    for set_modifier in mutation.set_modifiers.iter() {
        creature
            .stats_mut()
            .get_mut(set_modifier.stat)
            .set_modifier(Modifier {
                value: set_modifier.value,
                operation: set_modifier.operation,
                source: identifier,
            });
    }

    if let Some(healing) = mutation.heal_damage {
        creature.heal(healing);
    }

    if let Some(damage) = &mutation.apply_damage {
        if let DamageResult::Killed = creature.apply_damage(damage.total()) {
            result = MutationResult::Killed;
        }
    }

    if let Some(mana_gain) = mutation.gain_mana {
        creature.gain_mana(mana_gain);
    }

    if let Some(mana_loss) = mutation.lose_mana {
        creature.lose_mana(mana_loss)?;
    }

    events.push_event(
        identifier,
        Event::CreatureMutated(CreatureMutated {
            source_creature: skill.source_creature,
            mutation: mutation.clone(),
            result,
        }),
    );
    Ok(())
}

pub fn command_for_skill(game: &Game, skill: &CreatureSkill) -> Result<api::Command> {
    Ok(api::Command {
        command: Some(api::command::Command::UseCreatureSkill(use_creature_skill(
            game, skill,
        )?)),
    })
}

fn use_creature_skill(game: &Game, skill: &CreatureSkill) -> Result<MUseCreatureSkillCommand> {
    let mut on_impact = vec![];
    for on_impact_number in skill.on_impact.iter() {
        on_impact.push(adapt_on_impact_number(game, on_impact_number)?)
    }

    Ok(MUseCreatureSkillCommand {
        source_creature: Some(commands::creature_id(skill.source_creature)),
        animation: Some(commands::adapt_skill_animation(
            skill.animation,
            game.creature(skill.source_creature)?.data.base_type,
        )),
        on_impact,
        melee_target: skill.melee_target.map(commands::creature_id),
    })
}

fn adapt_on_impact_number(
    game: &Game,
    on_impact_number: &OnImpactNumber,
) -> Result<MOnImpactNumber> {
    Ok(MOnImpactNumber {
        impact_number: on_impact_number.impact_number,
        effect: Some(adapt_on_impact(game, &on_impact_number.effect)?),
    })
}

fn adapt_on_impact(game: &Game, on_impact: &OnImpact) -> Result<MOnImpact> {
    Ok(MOnImpact {
        on_impact: Some(match on_impact {
            OnImpact::ApplyCreatureMutation(mutation) => {
                api::m_on_impact::OnImpact::Update(adapt_creature_mutation(game, mutation)?)
            }
            OnImpact::FireProjectile(fire_projectile) => {
                api::m_on_impact::OnImpact::FireProjectile(adapt_fire_projectile(
                    game,
                    fire_projectile,
                )?)
            }
        }),
    })
}

fn adapt_creature_mutation(game: &Game, mutation: &CreatureMutation) -> Result<MCreatureUpdate> {
    let creature = game.creature(mutation.target_id)?;
    let health = creature.stats().health_total.value();

    Ok(MCreatureUpdate {
        creature_id: Some(commands::creature_id(mutation.target_id)),
        set_health_percentage: ratio(creature.current_health(), health),
        play_death_animation: !creature.is_alive(),
        set_mana_percentage: ratio(
            creature.current_mana(),
            creature.stats().maximum_mana.value(),
        ),
        play_particle_effects: vec![],
    })
}

fn ratio(a: u32, b: u32) -> f32 {
    if b == 0 {
        0.0
    } else {
        (a as f32 / b as f32).clamp(0.0, 1.0)
    }
}

fn adapt_fire_projectile(game: &Game, fire_projectile: &FireProjectile) -> Result<MFireProjectile> {
    let mut on_hit = vec![];
    for hit in &fire_projectile.on_hit {
        on_hit.push(adapt_on_impact(game, &hit)?);
    }

    Ok(MFireProjectile {
        projectile: Some(commands::prefab(fire_projectile.projectile.address())),
        on_hit,
        target: Some(adapt_projectile_target(&fire_projectile.target)?),
    })
}

fn adapt_projectile_target(target: &ProjectileTarget) -> Result<api::m_fire_projectile::Target> {
    Ok(match target {
        ProjectileTarget::TargetCreature(creature_id) => {
            api::m_fire_projectile::Target::TargetCreature(commands::creature_id(*creature_id))
        }
    })
}
