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

#nullable enable

using System.Collections.Generic;
using Nighthollow.Components;
using Nighthollow.Generated;
using Nighthollow.Stats;
using UnityEngine;

namespace Nighthollow.Delegates.Core
{
  public interface ISkillDelegate : ICreatureEventsDelegate<SkillContext>
  {
    /// <summary>
    /// Called when a skill's animation begins.
    /// </summary>
    void OnStart(SkillContext c);

    /// <summary>
    /// Called when a skill's initial animation reaches its impact frame. For spells, this is the place to create and
    /// fire projectiles. For melee skills, this will be called immediately before <see cref="OnImpact"/>.
    /// </summary>
    void OnUse(SkillContext c);

    /// <summary>
    /// Called to apply the effect of a skill, either on melee hit or on projectile impact.
    /// </summary>
    void OnImpact(SkillContext c);

    /// <summary>
    /// Adds the effects for this skill as a result of a hit.
    /// </summary>
    ///
    /// Normally this is invoked by the default skill delegate's "OnImpact" implementation for each target returned from
    /// "PopulateTargets" for a skill. The default implementation implements the standard algorithm for applying the
    /// skill's BaseDamage, including things like checking for hit, checking for critical hit, applying damage,
    /// applying health drain, and applying stun.
    void OnApplyToTarget(SkillContext c, Creature target);

    /// <summary>
    /// Returns the collider to use for hit-testing this skill's impact.
    /// </summary>
    Collider2D GetCollider(SkillContext c);

    /// <summary>
    /// Returns the creatures to target for this skill. Normally this is invoked by the default skill delegate's
    /// <see cref="OnImpact"/> implementation. Default implementation uses @<see cref="GetCollider"/> to find all
    /// creatures in the impact area and then adds targets returned by <see cref="SelectTargets"/>.
    /// </summary>
    IEnumerable<Creature> PopulateTargets(SkillContext c);

    /// <summary>
    /// Given a list of creatures hit by a skill, returns a list of the creatures which should have the skill effect
    /// applied by <see cref="OnApplyToTarget"/>.
    /// </summary>
    IEnumerable<Creature> SelectTargets(SkillContext c, IEnumerable<Creature> hits);

    /// <summary>
    /// Should return true if a hit from this creature should hit.
    /// </summary>
    bool RollForHit(SkillContext c, Creature target);

    /// <summary>
    /// Should return true if a hit from this creature should critically hit.
    /// </summary>
    bool RollForCrit(SkillContext c, Creature target);

    /// <summary>
    /// Should compute the base damage for this skill, randomly selecting from within its base damage ranges.
    /// </summary>
    TaggedValues<DamageType, IntValue> RollForBaseDamage(SkillContext c, Creature target);

    /// <summary>
    /// Should apply damage reduction for this skill, reducing the damage value based on the target's reduction.
    /// </summary>
    TaggedValues<DamageType, IntValue> ApplyDamageReduction(
      SkillContext c,
      Creature target,
      TaggedValues<DamageType, IntValue> damage);

    /// <summary>
    /// Should apply damage resistance for this skill, reducing the damage value based on the target's resistance.
    /// </summary>
    TaggedValues<DamageType, IntValue> ApplyDamageResistance(
      SkillContext c,
      Creature target,
      TaggedValues<DamageType, IntValue> damage);

    /// <summary>
    /// Should compute the final damage value for this skill based on the value adjusted by damage resistance and
    /// reduction. Should apply the critical hit multiplier if "isCriticalHit" is true.
    /// </summary>
    public int ComputeFinalDamage(SkillContext c,
      Creature target,
      TaggedValues<DamageType, IntValue> damage,
      bool isCriticalHit);

    /// <summary>
    /// Computes health drain for a hit on 'target' dealing total damage 'damageAmount'.
    /// </summary>
    int ComputeHealthDrain(SkillContext c, Creature creature, int damageAmount);

    /// <summary>
    /// Should return true if a hit from this creature should stun 'target'.
    /// </summary>
    bool RollForStun(SkillContext c, Creature target, int damageAmount);
  }
}