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

use super::rules::RuleContext;
use crate::{
    api::{
        self, m_on_impact, MCreatureUpdate, MFireProjectile, MOnImpact, MOnImpactNumber,
        MSkillAnimation, MSkillAnimationNumber, MUseCreatureSkillCommand,
    },
    commands,
    model::{
        assets::{CreatureType, ParticleSystemName, ProjectileName},
        creatures::{Creature, Damage, DamageResult, HasCreatureData},
        games::Game,
        primitives::{CreatureId, HealthValue, ManaValue, PlayerName, RuleId, SkillAnimation},
        stats::{Modifier, Operation, StatName},
    },
};

#[derive(Copy, Clone)]
pub enum EffectSource {
    Creature(CreatureId),
    Player(PlayerName),
    Game,
}

pub struct EffectData {
    pub effect: Effect,
    pub rule_id: RuleId,
    pub source: EffectSource,
}

pub struct Effects {
    effects: Vec<EffectData>,
}

impl Effects {
    pub fn new() -> Effects {
        Effects { effects: vec![] }
    }

    pub fn push_effect<T>(&mut self, context: &RuleContext<T>, effect: Effect) {
        self.effects.push(EffectData {
            effect,
            rule_id: context.rule_id,
            source: context.source,
        });
    }

    pub fn len(&self) -> usize {
        self.effects.len()
    }

    pub fn iter(&self) -> impl Iterator<Item = &EffectData> {
        self.effects.iter()
    }
}

pub enum Effect {
    InitiateGame,
    DrawCard(PlayerName),
    UseCreatureSkill(CreatureSkill),
}

pub struct CreatureSkill {
    source_creature: CreatureId,

    // A skill animation to play
    animation: SkillAnimation,

    // Effects to apply when the animation reaches its impact frame
    on_impact: Vec<OnImpactNumber>,

    // Optionally, a target for this skill. The creature will move into
    // melee range with this target before playing the skill animation.
    melee_target: Option<CreatureId>,
}

impl CreatureSkill {
    pub fn simple_melee(
        creature: &Creature,
        animation: SkillAnimation,
        target: CreatureId,
        mutation: CreatureMutation,
    ) -> Effect {
        Effect::UseCreatureSkill(CreatureSkill {
            source_creature: creature.creature_id(),
            animation,
            melee_target: Some(target),
            on_impact: vec![OnImpactNumber {
                impact_number: 1,
                effect: OnImpact::ApplyCreatureMutation(ApplyCreatureMutation {
                    target_id: target,
                    mutation,
                }),
            }],
        })
    }
}

pub struct OnImpactNumber {
    impact_number: u32,
    effect: OnImpact,
}

pub enum OnImpact {
    ApplyCreatureMutation(ApplyCreatureMutation),
    FireProjectile(FireProjectile),
}

pub struct FireProjectile {
    projectile: ProjectileName,
    on_hit: Vec<OnImpact>,
    target: ProjectileTarget,
}

pub enum ProjectileTarget {
    TargetCreature(CreatureId),
}

pub struct ApplyCreatureMutation {
    pub target_id: CreatureId,
    pub mutation: CreatureMutation,
}

pub struct CreatureMutation {
    pub set_modifiers: Vec<SetModifier>,
    pub apply_damage: Option<Damage>,
    pub heal_damage: Option<HealthValue>,
    pub gain_mana: Option<ManaValue>,
    pub lose_mana: Option<ManaValue>,
    pub play_particle_systems: Vec<ParticleSystemName>,
}

impl Default for CreatureMutation {
    fn default() -> Self {
        CreatureMutation {
            set_modifiers: vec![],
            apply_damage: None,
            heal_damage: None,
            gain_mana: None,
            lose_mana: None,
            play_particle_systems: vec![],
        }
    }
}

#[derive(Debug, Clone)]
pub struct SetModifier {
    pub stat: StatName,
    pub value: u32,
    pub operation: Operation,
}

pub struct MutationEvent {
    pub source: EffectSource,
    pub mutation: Mutation,
}

pub enum Mutation {
    CreatureMutation(CreatureMutationEvent),
}

/// Represents a mutation which ocurred during effect application which should
// be used to trigger further "on mutation x happened" rules
pub struct CreatureMutationEvent {
    pub target_creature: CreatureId,
    pub event_type: CreatureMutationEventType,
}

impl MutationEvent {
    pub fn source_creature(&self) -> Result<CreatureId> {
        match self.source {
            EffectSource::Creature(id) => Ok(id),
            _ => Err(eyre!("Expected a creature source")),
        }
    }
}

/// Possible types of mutation event
pub enum CreatureMutationEventType {
    AppliedDamage(Damage),
    Killed(Damage),
    Healed(HealthValue),
    GainedMana(ManaValue),
    LostMana(ManaValue),
    SetModifier(SetModifier),
}

fn creature_mutation_event(
    source: EffectSource,
    target: CreatureId,
    event_type: CreatureMutationEventType,
) -> MutationEvent {
    MutationEvent {
        source,
        mutation: Mutation::CreatureMutation(CreatureMutationEvent {
            target_creature: target,
            event_type,
        }),
    }
}

/// Applies a specific Effect to the Game, mutating game state. The 'commands'
/// buffer is populated with resulting commands which should be sent to the
/// client. The 'events' buffer is populated with mutation events, used to
/// trigger addtional rules as a result of this effect.
pub fn apply_effect(
    game: &mut Game,
    effect_data: &EffectData,
    commands: &mut Vec<api::Command>,
    events: &mut Vec<MutationEvent>,
) -> Result<()> {
    match &effect_data.effect {
        Effect::InitiateGame => commands.push(commands::initiate_game_command(game)),
        Effect::DrawCard(player_name) => {
            let card = game.player_mut(*player_name).deck.draw_card()?;
            commands.push(commands::draw_or_update_card_command(&card));
        }
        Effect::UseCreatureSkill(skill) => {
            commands.push(api::Command {
                command: Some(api::command::Command::UseCreatureSkill(use_creature_skill(
                    game,
                    effect_data,
                    skill,
                    events,
                )?)),
            });
        }
    }
    Ok(())
}

fn use_creature_skill(
    game: &mut Game,
    effect_data: &EffectData,
    skill: &CreatureSkill,
    events: &mut Vec<MutationEvent>,
) -> Result<MUseCreatureSkillCommand> {
    let mut on_impact = vec![];
    for on_impact_number in skill.on_impact.iter() {
        on_impact.push(adapt_on_impact_number(
            game,
            effect_data,
            on_impact_number,
            events,
        )?)
    }

    Ok(MUseCreatureSkillCommand {
        source_creature: Some(commands::creature_id(skill.source_creature)),
        animation: Some(commands::adapt_skill_animation(
            skill.animation,
            game.creature(skill.source_creature)?.data.base_type,
        )),
        on_impact,
        melee_target: skill.melee_target.map(|cid| commands::creature_id(cid)),
    })
}

fn adapt_on_impact_number(
    game: &mut Game,
    effect_data: &EffectData,
    on_impact_number: &OnImpactNumber,
    events: &mut Vec<MutationEvent>,
) -> Result<MOnImpactNumber> {
    Ok(MOnImpactNumber {
        impact_number: on_impact_number.impact_number,
        effect: Some(adapt_on_impact(
            game,
            effect_data,
            &on_impact_number.effect,
            events,
        )?),
    })
}

fn adapt_on_impact(
    game: &mut Game,
    effect_data: &EffectData,
    on_impact: &OnImpact,
    events: &mut Vec<MutationEvent>,
) -> Result<MOnImpact> {
    Ok(MOnImpact {
        on_impact: Some(match on_impact {
            OnImpact::ApplyCreatureMutation(mutation) => m_on_impact::OnImpact::Update(
                adapt_creature_mutation(game, effect_data, mutation, events)?,
            ),
            OnImpact::FireProjectile(fire_projectile) => m_on_impact::OnImpact::FireProjectile(
                adapt_fire_projectile(game, effect_data, fire_projectile, events)?,
            ),
        }),
    })
}

fn adapt_creature_mutation(
    game: &mut Game,
    effect_data: &EffectData,
    mutation: &ApplyCreatureMutation,
    events: &mut Vec<MutationEvent>,
) -> Result<MCreatureUpdate> {
    let result = apply_mutation(game, effect_data, mutation, events)?;
    let creature = game.creature(mutation.target_id)?;
    let health = creature.stats().health_total.value();

    Ok(MCreatureUpdate {
        creature_id: Some(commands::creature_id(mutation.target_id)),
        set_health_percentage: ratio(creature.current_health(), health),
        play_death_animation: result == MutationResult::Killed,
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

#[derive(Debug, PartialEq, Eq)]
enum MutationResult {
    None,
    Killed,
}

fn apply_mutation(
    game: &mut Game,
    effect_data: &EffectData,
    apply_mutation: &ApplyCreatureMutation,
    events: &mut Vec<MutationEvent>,
) -> Result<MutationResult> {
    let target_id = apply_mutation.target_id;
    let mutation = &apply_mutation.mutation;
    let creature = game.creature_mut(target_id)?;
    let mut result = MutationResult::None;

    for set_modifier in mutation.set_modifiers.iter() {
        creature
            .stats_mut()
            .get_mut(set_modifier.stat)
            .set_modifier(Modifier {
                value: set_modifier.value,
                operation: set_modifier.operation,
                source: effect_data.rule_id,
            });
        events.push(creature_mutation_event(
            effect_data.source,
            target_id,
            CreatureMutationEventType::SetModifier(set_modifier.clone()),
        ));
    }

    if let Some(damage) = &mutation.apply_damage {
        events.push(creature_mutation_event(
            effect_data.source,
            target_id,
            CreatureMutationEventType::AppliedDamage(damage.clone()),
        ));
        match creature.apply_damage(damage.total()) {
            DamageResult::Killed => {
                result = MutationResult::Killed;
                events.push(creature_mutation_event(
                    effect_data.source,
                    target_id,
                    CreatureMutationEventType::Killed(damage.clone()),
                ));
            }
            _ => {}
        }
    }

    if let Some(healing) = mutation.heal_damage {
        creature.heal(healing);
        events.push(creature_mutation_event(
            effect_data.source,
            target_id,
            CreatureMutationEventType::Healed(healing),
        ));
    }

    if let Some(mana_gain) = mutation.gain_mana {
        creature.gain_mana(mana_gain);
        events.push(creature_mutation_event(
            effect_data.source,
            target_id,
            CreatureMutationEventType::GainedMana(mana_gain),
        ));
    }

    if let Some(mana_loss) = mutation.lose_mana {
        creature.lose_mana(mana_loss)?;
        events.push(creature_mutation_event(
            effect_data.source,
            target_id,
            CreatureMutationEventType::LostMana(mana_loss),
        ));
    }

    Ok(result)
}

fn adapt_fire_projectile(
    game: &mut Game,
    effect_data: &EffectData,
    fire_projectile: &FireProjectile,
    events: &mut Vec<MutationEvent>,
) -> Result<MFireProjectile> {
    todo!()
}
