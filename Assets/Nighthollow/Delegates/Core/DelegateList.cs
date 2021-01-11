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
  public abstract class DelegateList : AbstractDelegateList, IDelegate
  {
    protected DelegateList(IReadOnlyList<IDelegate> delegates) : base(delegates)
    {
    }

    public string? Describe(IStatDescriptionProvider provider) => "<Delegate List>";

    public string DescribeOld(StatEntity entity) => "Delegate List";

    public void OnActivate(CreatureContext context) =>
      ExecuteEvent(context, (d, c) => d.OnActivate(c));

    public void OnDeath(CreatureContext context) =>
      ExecuteEvent(context, (d, c) => d.OnDeath(c));

    public void OnKilledEnemy(CreatureContext context, Creature enemy, int damageAmount) =>
      ExecuteEvent(context, (d, c) => d.OnKilledEnemy(c, enemy, damageAmount));

    public void OnFiredProjectile(SkillContext context, FireProjectileEffect effect) =>
      ExecuteEvent(context, (d, c) => d.OnFiredProjectile(c, effect));

    public void OnHitTarget(SkillContext context, Creature target, int damage) =>
      ExecuteEvent(context, (d, c) => d.OnHitTarget(c, target, damage));

    public void OnStart(SkillContext context) =>
      ExecuteEvent(context, (d, c) => d.OnStart(c));

    public void OnUse(SkillContext context) =>
      ExecuteEvent(context, (d, c) => d.OnUse(c));

    public void OnImpact(SkillContext context) =>
      ExecuteEvent(context, (d, c) => d.OnImpact(c));

    public void OnApplyToTarget(SkillContext context, Creature target) =>
      ExecuteEvent(context, (d, c) => d.OnApplyToTarget(c, target));

    public bool MeleeCouldHit(CreatureContext context) =>
      GetFirstImplemented(context, (d, c) => d.MeleeCouldHit(c));

    public bool ProjectileCouldHit(CreatureContext context) =>
      AnyReturnedTrue(context, (d, c) => d.ProjectileCouldHit(c));

    public bool ShouldSkipProjectileImpact(SkillContext context) =>
      AnyReturnedTrue(context, (d, c) => d.ShouldSkipProjectileImpact(c));

    public SkillData? SelectSkill(CreatureContext context) =>
      GetFirstImplemented(context, (d, c) => d.SelectSkill(c));

    public Collider2D GetCollider(SkillContext context) =>
      GetFirstImplemented(context, (d, c) => d.GetCollider(c));

    public IEnumerable<Creature> FindTargets(SkillContext context) =>
      GetFirstImplemented(context, (d, c) => d.FindTargets(c));

    public IEnumerable<Creature> FilterTargets(SkillContext context, IEnumerable<Creature> hits) =>
      GetFirstImplemented(context, (d, c) => d.FilterTargets(c, hits));

    public bool RollForHit(SkillContext context, Creature target) =>
      GetFirstImplemented(context, (d, c) => d.RollForHit(c, target));

    public bool RollForCrit(SkillContext context, Creature target) =>
      GetFirstImplemented(context, (d, c) => d.RollForCrit(c, target));

    public ImmutableDictionary<DamageType, int> RollForBaseDamage(SkillContext context, Creature target) =>
      GetFirstImplemented(context, (d, c) => d.RollForBaseDamage(c, target));

    public ImmutableDictionary<DamageType, int> TransformDamage(
      SkillContext context, Creature target, ImmutableDictionary<DamageType, int> damage) =>
      AggregateDelegates(context, damage, (d, c, value) => d.TransformDamage(c, target, value));

    public ImmutableDictionary<DamageType, int> ApplyDamageReduction(
      SkillContext context, Creature target, ImmutableDictionary<DamageType, int> damage) =>
      GetFirstImplemented(context, (d, c) => d.ApplyDamageReduction(c, target, damage));

    public ImmutableDictionary<DamageType, int> ApplyDamageResistance(
      SkillContext context, Creature target, ImmutableDictionary<DamageType, int> damage) =>
      GetFirstImplemented(context, (d, c) => d.ApplyDamageResistance(c, target, damage));

    public int ComputeFinalDamage(
      SkillContext context, Creature target, ImmutableDictionary<DamageType, int> damage, bool isCriticalHit) =>
      GetFirstImplemented(context, (d, c) => d.ComputeFinalDamage(c, target, damage, isCriticalHit));

    public int ComputeHealthDrain(
      SkillContext context, Creature creature, int damageAmount) =>
      GetFirstImplemented(context, (d, c) => d.ComputeHealthDrain(c, creature, damageAmount));

    public bool RollForStun(SkillContext context, Creature target, int damageAmount) =>
      GetFirstImplemented(context, (d, c) => d.RollForStun(c, target, damageAmount));
  }
}
