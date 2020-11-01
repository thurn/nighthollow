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
using System.Linq;
using Nighthollow.Components;
using Nighthollow.Data;
using Nighthollow.Delegates.Effects;
using Nighthollow.Delegates.Skills;
using Nighthollow.Generated;
using Nighthollow.Stats;
using UnityEngine;

namespace Nighthollow.Delegates.Core
{
  public sealed class SkillDelegateList : AbstractDelegateList<SkillContext, ISkillDelegate>, ISkillDelegate
  {
    public SkillDelegateList(IEnumerable<ISkillDelegate> delegates) :
      base(delegates.Append(new DefaultSkillDelegate()).ToList())
    {
    }

    public void OnActivate(SkillContext context) =>
      ExecuteEvent(context, (d, c) => d.OnActivate(c));

    public void OnDeath(SkillContext context) =>
      ExecuteEvent(context, (d, c) => d.OnDeath(c));

    public void OnKilledEnemy(SkillContext context, Creature enemy, int damageAmount) =>
      ExecuteEvent(context, (d, c) => d.OnKilledEnemy(c, enemy, damageAmount));

    public void OnFiredProjectile(SkillContext context, FireProjectileEffect effect) =>
      ExecuteEvent(context, (d, c) => d.OnFiredProjectile(c, effect));

    public void OnHitTarget(SkillContext context, SkillData skill, Creature target) =>
      ExecuteEvent(context, (d, c) => d.OnHitTarget(c, skill, target));

    public void OnStart(SkillContext context) =>
      ExecuteEvent(context, (d, c) => d.OnStart(c));

    public void OnUse(SkillContext context) =>
      ExecuteEvent(context, (d, c) => d.OnUse(c));

    public void OnImpact(SkillContext context) =>
      ExecuteEvent(context, (d, c) => d.OnImpact(c));

    public void OnApplyToTarget(SkillContext context, Creature target) =>
      ExecuteEvent(context, (d, c) => d.OnApplyToTarget(c, target));

    public Collider2D GetCollider(SkillContext context) =>
      GetFirstImplemented(context, (d, c) => d.GetCollider(c));

    public IEnumerable<Creature> PopulateTargets(SkillContext context) =>
      GetFirstImplemented(context, (d, c) => d.PopulateTargets(c));

    public IEnumerable<Creature> SelectTargets(SkillContext context, IEnumerable<Creature> hits) =>
      GetFirstImplemented(context, (d, c) => d.SelectTargets(c, hits));

    public bool RollForHit(SkillContext context, Creature target) =>
      GetFirstImplemented(context, (d, c) => d.RollForHit(c, target));

    public bool RollForCrit(SkillContext context, Creature target) =>
      GetFirstImplemented(context, (d, c) => d.RollForCrit(c, target));

    public TaggedValues<DamageType, IntValue> RollForBaseDamage(SkillContext context, Creature target) =>
      GetFirstImplemented(context, (d, c) => d.RollForBaseDamage(c, target));

    public TaggedValues<DamageType, IntValue> ApplyDamageReduction(
      SkillContext context, Creature target, TaggedValues<DamageType, IntValue> damage) =>
      GetFirstImplemented(context, (d, c) => d.ApplyDamageReduction(c, target, damage));

    public TaggedValues<DamageType, IntValue> ApplyDamageResistance(
      SkillContext context, Creature target, TaggedValues<DamageType, IntValue> damage) =>
      GetFirstImplemented(context, (d, c) => d.ApplyDamageResistance(c, target, damage));

    public int ComputeFinalDamage(
      SkillContext context, Creature target, TaggedValues<DamageType, IntValue> damage, bool isCriticalHit) =>
      GetFirstImplemented(context, (d, c) => d.ComputeFinalDamage(c, target, damage, isCriticalHit));

    public int ComputeHealthDrain(
      SkillContext context, Creature creature, int damageAmount) =>
      GetFirstImplemented(context, (d, c) => d.ComputeHealthDrain(c, creature, damageAmount));

    public bool RollForStun(SkillContext context, Creature target, int damageAmount) =>
      GetFirstImplemented(context, (d, c) => d.RollForStun(c, target, damageAmount));
  }
}