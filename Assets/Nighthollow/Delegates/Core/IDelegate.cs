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

using System.Collections.Generic;
using Nighthollow.Components;
using Nighthollow.Data;
using Nighthollow.Delegates.Effects;
using Nighthollow.Generated;
using Nighthollow.Stats;
using UnityEngine;

#nullable enable

namespace Nighthollow.Delegates.Core
{
  public interface IDelegate
  {
    #region Events

    /// <summary>Called when a creature is first placed.</summary>
    void OnActivate(CreatureContext c);

    /// <summary>Called when a creature dies.</summary>
    void OnDeath(CreatureContext c);

    /// <summary>Called when a creature kills an enemy creature.</summary>
    void OnKilledEnemy(
      CreatureContext c,
      Creature enemy,
      int damageAmount);

    /// <summary>Called when the creature fires a projectile.</summary>
    void OnFiredProjectile(SkillContext c, FireProjectileEffect effect);

    /// <summary>Called when one of the creature's skills hits a target.</summary>
    void OnHitTarget(SkillContext c, Creature target);

    /// <summary>
    /// Called when a skill's animation begins.
    /// </summary>
    void OnStart(SkillContext c);

    /// <summary>
    /// Called when a skill's initial animation reaches its impact frame. The default implementation handles firing a
    /// projectile for projectile skills. For non-projectile skills, this will be called immediately before the
    /// <see cref="OnImpact"/> event.
    /// </summary>
    void OnUse(SkillContext c);

    /// <summary>
    /// Called to apply the effect of a skill on a melee hit or projectile impact.
    /// </summary>
    void OnImpact(SkillContext c);

    /// <summary>
    /// Adds the effects for this skill as a result of a hit.
    /// </summary>
    ///
    /// Normally this is invoked by the default skill delegate's <see cref="OnImpact"/> implementation for each target
    /// returned from <see cref="FindTargets"/> for a skill. The default implementation implements the standard
    /// algorithm for applying the skill's BaseDamage, including things like checking for hit, checking for critical
    /// hit, applying damage, applying health drain, and applying stun.
    void OnApplyToTarget(SkillContext c, Creature target);

    #endregion

    #region Queries

    /// <summary>
    /// Should check if the creature could currently hit with a melee skill. Will return true if any delegate returns
    /// a true value.
    /// </summary>
    bool MeleeCouldHit(CreatureContext c);

    /// <summary>
    /// Called to check if a projectile fired by this creature would currently hit a target. Will return true if
    /// any delegate returns a true value.
    /// </summary>
    bool ProjectileCouldHit(CreatureContext c);

    /// <summary>
    /// Should check if the current projectile should *not* trigger an impact in its current position. Will return
    /// true if any delegate returns a true value.
    /// </summary>
    bool ShouldSkipProjectileImpact(SkillContext c);

    /// <summary>
    /// Called when a creature wants to decide which skill to use. Should return null if there is no appropriate skill
    /// available.
    /// </summary>
    SkillData? SelectSkill(CreatureContext c);

    /// <summary>
    /// Returns the collider to use for hit-testing this skill's impact.
    /// </summary>
    Collider2D GetCollider(SkillContext c);

    /// <summary>
    /// Returns the creatures to target for this skill. Normally this is invoked by the default skill delegate's
    /// <see cref="OnImpact"/> implementation. Default implementation uses @<see cref="GetCollider"/> to find all
    /// creatures in the impact area and then adds targets returned by <see cref="FilterTargets"/>.
    /// </summary>
    IEnumerable<Creature> FindTargets(SkillContext c);

    /// <summary>
    /// Given a list of creatures hit by a skill, returns a list of the creatures which should have the skill effect
    /// applied by <see cref="OnApplyToTarget"/>.
    /// </summary>
    IEnumerable<Creature> FilterTargets(SkillContext c, IEnumerable<Creature> hits);

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
    /// reduction. Should apply the critical hit multiplier if <paramref name="isCriticalHit"/> is true.
    /// </summary>
    public int ComputeFinalDamage(SkillContext c,
      Creature target,
      TaggedValues<DamageType, IntValue> damage,
      bool isCriticalHit);

    /// <summary>
    /// Computes health drain for a hit on <paramref name="target"/> dealing total damage
    /// <paramref name="damageAmount"/>.
    /// </summary>
    int ComputeHealthDrain(SkillContext c, Creature target, int damageAmount);

    /// <summary>
    /// Should return true if a hit from this creature for <paramref name="damageAmount"/> damage should stun
    /// <paramref name="target"/>.
    /// </summary>
    bool RollForStun(SkillContext c, Creature target, int damageAmount);

    #endregion
  }
}