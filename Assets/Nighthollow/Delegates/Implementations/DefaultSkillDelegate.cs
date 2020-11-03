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

using System;
using System.Collections.Generic;
using System.Linq;
using Nighthollow.Components;
using Nighthollow.Data;
using Nighthollow.Delegates.Core;
using Nighthollow.Delegates.Effects;
using Nighthollow.Generated;
using Nighthollow.Stats;
using Nighthollow.Utils;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Nighthollow.Delegates.Implementations
{
  public sealed class DefaultSkillDelegate : AbstractDelegate
  {
    public override void OnUse(SkillContext c)
    {
      c.Self.MarkSkillUsed(c.Skill.BaseType);
      if (c.Skill.BaseType.IsProjectile)
      {
        c.Results.Add(new FireProjectileEffect(
          c.Self,
          c,
          c.DelegateIndex,
          c.Self.ProjectileSource.position,
          Vector2.zero));
      }
    }

    public override void OnImpact(SkillContext context)
    {
      if (context.GetBool(Stat.Untargeted))
      {
        return;
      }

      var targets = context.Delegate.FindTargets(context);
      foreach (var target in targets)
      {
        context.Results.Add(Events.Effect(context, (d, c) => d.OnApplyToTarget(c, target)));
      }
    }

    public override void OnApplyToTarget(SkillContext c, Creature target)
    {
      if (c.GetBool(Stat.UsesAccuracy) && !c.Delegate.RollForHit(c, target))
      {
        c.Results.Add(c.Self.Owner == PlayerName.User
          ? new SkillEventEffect(SkillEventEffect.Event.Missed, c.Self)
          : new SkillEventEffect(SkillEventEffect.Event.Evade, target));
        return;
      }

      var isCriticalHit = false;
      if (c.GetBool(Stat.CanCrit) && c.Delegate.RollForCrit(c, target))
      {
        c.Results.Add(new SkillEventEffect(SkillEventEffect.Event.Crit, c.Self));
        isCriticalHit = true;
      }

      var damage = c.Delegate.RollForBaseDamage(c, target);
      damage = c.Delegate.TransformDamage(c, target, damage);
      var totalDamage = c.Delegate.ComputeFinalDamage(c, target, damage, isCriticalHit);

      c.Results.Add(Events.Effect(c, (d, sc) => d.OnHitTarget(sc, target, totalDamage)));

      if (totalDamage == 0)
      {
        return;
      }

      c.Results.Add(new ApplyDamageEffect(c.Self, target, totalDamage));
      c.Results.Add(new DamageTextEffect(target, totalDamage));

      var healthDrain = c.Delegate.ComputeHealthDrain(c, target, totalDamage);
      if (healthDrain > 0)
      {
        c.Results.Add(new HealEffect(c.Self, healthDrain));
      }

      if (c.GetBool(Stat.CanStun) && c.Delegate.RollForStun(c, target, totalDamage))
      {
        c.Results.Add(new StunEffect(target, c.GetDurationSeconds(Stat.StunDurationOnEnemies)));
        c.Results.Add(new SkillEventEffect(SkillEventEffect.Event.Stun, target));
      }
    }

    public override IEnumerable<Creature> FindTargets(SkillContext c)
    {
      var filter = new ContactFilter2D
      {
        layerMask = Constants.LayerMaskForCreatures(c.Self.Owner.GetOpponent()),
        useLayerMask = true,
        useTriggers = true
      };
      var sourceCollider = c.Delegate.GetCollider(c);
      var colliders = new List<Collider2D>();
      sourceCollider.OverlapCollider(filter, colliders);

      return c.Delegate.FilterTargets(c, colliders
        // Filter out trigger colliders
        .Where(collider => collider.GetComponent<Creature>())
        .Select(ComponentUtils.GetComponent<Creature>));
    }

    public override IEnumerable<Creature> FilterTargets(SkillContext c, IEnumerable<Creature> hits) =>
      c.Skill.BaseType.IsMelee ? hits.Take(Errors.CheckPositive(c.GetInt(Stat.MaxMeleeAreaTargets))) : hits;

    public override Collider2D GetCollider(SkillContext c) =>
      c.Projectile ? c.Projectile!.Collider : c.Self.Collider;

    public override TaggedValues<DamageType, int> RollForBaseDamage(SkillContext c, Creature target) =>
      DamageUtil.RollForDamage(c.GetStat(Stat.BaseDamage));

    public override TaggedValues<DamageType, int> ApplyDamageReduction(
      SkillContext c,
      Creature target,
      TaggedValues<DamageType, int> damage) =>
      new TaggedValues<DamageType, int>(
        damage.Values.ToDictionary(
          k => k.Key,
          v => ApplyReduction(
            c,
            v.Value,
            target.Data.GetStat(Stat.DamageReduction).Get(v.Key, 0))));

    static int ApplyReduction(SkillContext c, int damage, int reduction) =>
      Math.Max(
        // Apply maximum reduction
        Mathf.RoundToInt(damage * (1f - c.GetStat(Stat.MaximumDamageReduction).AsMultiplier())),
        damage - reduction);

    public override TaggedValues<DamageType, int> ApplyDamageResistance(
      SkillContext c,
      Creature target,
      TaggedValues<DamageType, int> damage) =>
      new TaggedValues<DamageType, int>(
        damage.Values.ToDictionary(
          k => k.Key,
          v => ApplyResistance(
            c,
            v.Value,
            target.Data.GetStat(Stat.DamageResistance).Get(v.Key, 0))));

    static int ApplyResistance(SkillContext c, int damageValue, float resistance) =>
      Mathf.RoundToInt(Math.Max(
        // Apply maximum resistance
        damageValue * (1f - c.GetStat(Stat.MaximumDamageResistance).AsMultiplier()),
        Mathf.Clamp01(1f - (resistance / (resistance + (2.0f * damageValue)))) * damageValue));

    public override int ComputeFinalDamage(
      SkillContext c,
      Creature target,
      TaggedValues<DamageType, int> damage,
      bool isCriticalHit)
    {
      damage = c.GetBool(Stat.IgnoresDamageReduction)
        ? damage
        : c.Delegate.ApplyDamageReduction(c, target, damage);

      damage = c.GetBool(Stat.IgnoresDamageResistance)
        ? damage
        : c.Delegate.ApplyDamageResistance(c, target, damage);

      var total = damage.Values.Values.Sum();
      total = isCriticalHit ? c.GetStat(Stat.CritMultiplier).CalculateFraction(total) : total;

      if (c.Skill.BaseType.IsMelee)
      {
        total = c.GetStat(Stat.MeleeDamageMultiplier).CalculateFraction(total);
      }

      if (c.Skill.BaseType.IsProjectile)
      {
        total = c.GetStat(Stat.ProjectileDamageMultiplier).CalculateFraction(total);
      }

      return total;
    }

    public override bool RollForHit(SkillContext c, Creature target)
    {
      var accuracy = c.GetInt(Stat.Accuracy);
      var hitChance = Mathf.Clamp(
        0.1f,
        accuracy / (accuracy + Mathf.Pow((target.Data.GetInt(Stat.Evasion) / 4.0f), 0.8f)),
        0.95f);
      return Random.value <= hitChance;
    }

    public override bool RollForCrit(SkillContext c, Creature target) =>
      Random.value <= (c.GetStat(Stat.CritChance).AsMultiplier() +
                       target.Data.Stats.Get(Stat.ReceiveCritsChance).AsMultiplier());

    public override int ComputeHealthDrain(SkillContext c, Creature creature, int damageAmount) =>
      c.Skill.BaseType.IsMelee ? c.GetStat(Stat.MeleeHealthDrainPercent).CalculateFraction(damageAmount) : 0;

    public override bool RollForStun(SkillContext c, Creature target, int damageAmount)
    {
      var stunChance = c.GetStat(Stat.AddedStunChance).AsMultiplier() +
                       (damageAmount / (float) target.Data.GetInt(Stat.Health));
      return Random.value <= Mathf.Clamp(
        stunChance,
        0,
        c.GetStat(Stat.MaximumStunChance).AsMultiplier());
    }
  }
}