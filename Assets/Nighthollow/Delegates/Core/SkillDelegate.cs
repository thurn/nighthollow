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
    public virtual void OnImpact(SkillContext c,  Results results)
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
    /// Appends new effects to the list of effects to apply to the target of this skill. Normally this is invoked
    /// by the default skill delegate's "OnImpact" implementation for each target returned from "PopulateTargets". The
    /// default implementation implements the standard algorithm for applying the skill's BaseDamage, including
    /// things like checking for hit, checking for critical hit, applying damage, applying life drain, and applying
    /// stun.
    /// </summary>
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
    /// Adds final damage for a hit on 'target' dealing base damage 'damage'. Return values of all delegates are summed
    /// together.
    /// </summary>
    public virtual int RollForDamage(SkillContext c, Creature target,
      TaggedStats<DamageType, IntRangeStat> damage) => 0;

    /// <summary>
    /// Adds final critical hit damage for a critical hit on 'target' dealing base damage 'damage'. Return values of all
    /// delegates are summed together.
    /// </summary>
    public virtual int RollForCritDamage(SkillContext c, Creature target,
      TaggedStats<DamageType, IntRangeStat> damage) => 0;

    /// <summary>
    /// Computes life drain for a hit on 'target' dealing total damage 'damageAmount'. Return values of all delegates
    /// are summed together.
    /// </summary>
    public virtual int ComputeLifeDrain(SkillContext c, Creature creature, int damageAmount) => 0;

    /// <summary>
    /// Should return true if a hit from this creature should stun 'target'. A stun is invoked if *any* delegate returns
    /// true from this method.
    /// </summary>
    public virtual bool CheckForStun(SkillContext c, Creature target, int damageAmount) => false;
  }
}