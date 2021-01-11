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
using System.Collections.Immutable;
using Nighthollow.Components;
using Nighthollow.Data;
using Nighthollow.Delegates.Effects;
using Nighthollow.Stats2;
using UnityEngine;

#nullable enable

namespace Nighthollow.Delegates.Core
{
  public abstract class AbstractDelegate : IDelegate
  {
    public virtual string? Describe(IStatDescriptionProvider provider) => "<Delegate>";

    public virtual void OnActivate(CreatureContext c)
    {
    }

    public virtual void OnDeath(CreatureContext c)
    {
    }

    public virtual void OnKilledEnemy(CreatureContext c, Creature enemy, int damageAmount)
    {
    }

    public virtual void OnFiredProjectile(SkillContext c, FireProjectileEffect effect)
    {
    }

    public virtual void OnHitTarget(SkillContext c, Creature target, int damage)
    {
    }

    public virtual void OnStart(SkillContext c)
    {
    }

    public virtual void OnUse(SkillContext c)
    {
    }

    public virtual void OnImpact(SkillContext c)
    {
    }

    public virtual void OnApplyToTarget(SkillContext c, Creature target)
    {
    }

    public virtual bool MeleeCouldHit(CreatureContext c) => c.MarkNotImplemented<bool>();

    public virtual bool ProjectileCouldHit(CreatureContext c) => c.MarkNotImplemented<bool>();

    public virtual bool ShouldSkipProjectileImpact(SkillContext c) => c.MarkNotImplemented<bool>();

    public virtual SkillData? SelectSkill(CreatureContext c) => c.MarkNotImplemented<SkillData>();

    public virtual Collider2D GetCollider(SkillContext c) => c.MarkNotImplemented<Collider2D>();

    public virtual IEnumerable<Creature> FindTargets(SkillContext c) => c.MarkNotImplemented<IEnumerable<Creature>>();

    public virtual IEnumerable<Creature> FilterTargets(SkillContext c, IEnumerable<Creature> hits) =>
      c.MarkNotImplemented<IEnumerable<Creature>>();

    public virtual bool RollForHit(SkillContext c, Creature target) => c.MarkNotImplemented<bool>();

    public virtual bool RollForCrit(SkillContext c, Creature target) => c.MarkNotImplemented<bool>();

    public virtual ImmutableDictionary<DamageType, int> RollForBaseDamage(SkillContext c, Creature target) =>
      c.MarkNotImplemented<ImmutableDictionary<DamageType, int>>();

    public virtual ImmutableDictionary<DamageType, int> TransformDamage(
      SkillContext c, Creature target, ImmutableDictionary<DamageType, int> damage) =>
      c.MarkNotImplemented<ImmutableDictionary<DamageType, int>>();

    public virtual ImmutableDictionary<DamageType, int> ApplyDamageReduction(
      SkillContext c, Creature target, ImmutableDictionary<DamageType, int> damage) =>
      c.MarkNotImplemented<ImmutableDictionary<DamageType, int>>();

    public virtual ImmutableDictionary<DamageType, int> ApplyDamageResistance(
      SkillContext c, Creature target, ImmutableDictionary<DamageType, int> damage) =>
      c.MarkNotImplemented<ImmutableDictionary<DamageType, int>>();

    public virtual int ComputeFinalDamage(
      SkillContext c, Creature target, ImmutableDictionary<DamageType, int> damage, bool isCriticalHit) =>
      c.MarkNotImplemented<int>();

    public virtual int ComputeHealthDrain(SkillContext c, Creature creature, int damageAmount) =>
      c.MarkNotImplemented<int>();

    public virtual bool RollForStun(SkillContext c, Creature target, int damageAmount) => c.MarkNotImplemented<bool>();
  }
}
