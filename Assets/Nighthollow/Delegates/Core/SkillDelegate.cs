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
  public abstract class SkillDelegate
  {
    /// <summary>
    /// Called when a skill's animation begins.
    /// </summary>
    public virtual void OnStart(SkillContext c, Results results)
    {
    }

    /// <summary>
    /// Called when a skill's initial animation reaches its impact frame. For spells, this is the place to create and
    /// fire projectiles. For melee skills, this will be called immediately before <see cref="OnImpact"/>.
    /// </summary>
    public virtual void OnUse(SkillContext c, Results results)
    {
    }

    /// <summary>
    /// Called to apply the effect of a skill, either on melee hit or on projectile impact.
    /// </summary>
    public virtual void OnImpact(SkillContext c, Results results)
    {
    }

    /// <summary>
    /// Returns the collider to use for hit-testing this skill's impact. The *first* value returned will
    /// be used, subsequent delegates will not be invoked.
    /// </summary>
    public virtual Collider2D? GetCollider(SkillContext c) => null;

    /// <summary>
    /// Appends new creatures to the list of targets for this skill. Normally this is invoked by the default skill
    /// delegate's "OnImpact" implementation.
    /// </summary>
    public virtual void PopulateTargets(SkillContext c, List<Creature> targets)
    {
    }

    /// <summary>
    /// Adds the effects for this skill as a result of a hit.
    /// </summary>
    ///
    /// Normally this is invoked by the default skill delegate's "OnImpact" implementation for each target returned from
    /// "PopulateTargets" for a skill. The default implementation implements the standard algorithm for applying the
    /// skill's BaseDamage, including things like checking for hit, checking for critical hit, applying damage,
    /// applying health drain, and applying stun.
    public virtual void ApplyToTarget(SkillContext c, Creature target, Results results)
    {
    }

    /// <summary>
    /// Should return true if a hit from this creature should hit. A hit is registered if *any* delegate returns
    /// true from this method.
    /// </summary>
    public virtual bool RollForHit(SkillContext c, Creature target) => false;

    /// <summary>
    /// Should return true if a hit from this creature should critically hit. A critical hit is registered if *any*
    /// delegate returns true from this method.
    /// </summary>
    public virtual bool RollForCrit(SkillContext c, Creature target) => false;

    /// <summary>
    /// Should compute the base damage for this skill, randomly selecting from within its base damage ranges. The first
    /// delegate to return a non-null value with be used.
    /// </summary>
    public virtual TaggedStatListValue<DamageType, IntValue, IntStat>? RollForBaseDamage(
      SkillContext c, Creature target) => null;

    /// <summary>
    /// Should apply damage reduction for this skill, reducing the damage value based on the target's reduction. The
    /// first delegate to return a non-null value with be used.
    /// </summary>
    public virtual TaggedStatListValue<DamageType, IntValue, IntStat>? ApplyDamageReduction(
      SkillContext c,
      Creature target,
      TaggedStatListValue<DamageType, IntValue, IntStat> damage) => null;

    /// <summary>
    /// Should apply damage resistance for this skill, reducing the damage value based on the target's resistance. The
    /// first delegate to return a non-null value with be used.
    /// </summary>
    public virtual TaggedStatListValue<DamageType, IntValue, IntStat>? ApplyDamageResistance(
      SkillContext c,
      Creature target,
      TaggedStatListValue<DamageType, IntValue, IntStat> damage) => null;

    /// <summary>
    /// Should compute the final damage value for this skill based on the value adjusted by damage resistance and
    /// reduction. Should apply the critical hit multiplier if "isCriticalHit" is true. The first delegate to return a
    /// non-null value with be used.
    /// </summary>
    public virtual int? ComputeFinalDamage(SkillContext c,
      Creature target,
      TaggedStatListValue<DamageType, IntValue, IntStat> damage,
      bool isCriticalHit) => null;

    /// <summary>
    /// Computes health drain for a hit on 'target' dealing total damage 'damageAmount'. Return values of all delegates
    /// are summed together.
    /// </summary>
    public virtual int ComputeHealthDrain(SkillContext c, Creature creature, int damageAmount) => 0;

    /// <summary>
    /// Should return true if a hit from this creature should stun 'target'. A stun is invoked if *any* delegate returns
    /// true from this method.
    /// </summary>
    public virtual bool CheckForStun(SkillContext c, Creature target, int damageAmount) => false;
  }
}