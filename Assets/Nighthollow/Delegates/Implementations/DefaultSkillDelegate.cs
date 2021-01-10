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


using System;
using System.Collections.Generic;
using System.Linq;
using Nighthollow.Components;
using Nighthollow.Data;
using Nighthollow.Delegates.Core;
using Nighthollow.Delegates.Effects;
using Nighthollow.Generated;
using Nighthollow.Model;
using Nighthollow.Stats;
using Nighthollow.Utils;
using UnityEngine;
using Random = UnityEngine.Random;

#nullable enable

namespace Nighthollow.Delegates.Implementations
{
  public sealed class DefaultSkillDelegate : AbstractDelegate
  {
    public override string Describe(IStatDescriptionProvider provider) => "Default Skill Delegate";

    public override string DescribeOld(StatEntity entity) => "Default Skill Delegate";

    public override void OnUse(SkillContext c)
    {
      c.Self.MarkSkillUsed(c.Skill.BaseType);
      switch (c.Skill.BaseType.SkillType)
      {
        case SkillType.Projectile:
          c.Results.Add(new FireProjectileEffect(
            c.Self,
            c,
            c.DelegateIndex,
            c.Self.ProjectileSource.position,
            Vector2.zero));
          break;
        case SkillType.Melee when c.Skill.BaseType.Address != null:
          c.Results.Add(
            new PlayTimedEffectEffect(c.Skill.BaseType.Address, c.Self.Collider.bounds.center));
          break;
        case SkillType.Area when c.Skill.BaseType.Address != null:
          c.Results.Add(
            new PlayTimedEffectEffect(c.Skill.BaseType.Address,
              new Vector2(
                c.Self.RankPosition?.ToCenterXPosition() ?? c.Self.transform.position.x,
                c.Self.FilePosition.ToCenterYPosition())));
          break;
      }
    }

    public override void OnImpact(SkillContext context)
    {
      if (context.GetBool(OldStat.Untargeted))
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
      if (c.GetBool(OldStat.UsesAccuracy) && !c.Delegate.RollForHit(c, target))
      {
        c.Results.Add(c.Self.Owner == PlayerName.User
          ? new SkillEventEffect(SkillEventEffect.Event.Missed, c.Self)
          : new SkillEventEffect(SkillEventEffect.Event.Evade, target));
        return;
      }

      var isCriticalHit = false;
      if (c.GetBool(OldStat.CanCrit) && c.Delegate.RollForCrit(c, target))
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

      if (c.GetBool(OldStat.CanStun) && c.Delegate.RollForStun(c, target, totalDamage))
      {
        c.Results.Add(new StunEffect(target, c.GetDurationSeconds(OldStat.StunDurationOnEnemies)));
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

    public override IEnumerable<Creature> FilterTargets(SkillContext c, IEnumerable<Creature> hits) => c.Skill.IsMelee()
      ? hits.Take(Errors.CheckPositive(c.GetInt(OldStat.MaxMeleeAreaTargets)))
      : hits;

    public override Collider2D GetCollider(SkillContext c) => c.Projectile ? c.Projectile!.Collider : c.Self.Collider;

    public override TaggedValues<DamageType, int> RollForBaseDamage(SkillContext c, Creature target) =>
      DamageUtil.RollForDamage(c.GetStat(OldStat.BaseDamage));

    public override TaggedValues<DamageType, int> ApplyDamageReduction(
      SkillContext c,
      Creature target,
      TaggedValues<DamageType, int> damage)
    {
      return new TaggedValues<DamageType, int>(
        damage.Values.ToDictionary(
          k => k.Key,
          v => ApplyReduction(
            c,
            v.Value,
            target.Data.GetStat(OldStat.DamageReduction).Get(v.Key, notFound: 0))));
    }

    static int ApplyReduction(SkillContext c, int damage, int reduction) =>
      Math.Max(
        // Apply maximum reduction
        Mathf.RoundToInt(damage * (1f - c.GetStat(OldStat.MaximumDamageReduction).AsMultiplier())),
        damage - reduction);

    public override TaggedValues<DamageType, int> ApplyDamageResistance(
      SkillContext c,
      Creature target,
      TaggedValues<DamageType, int> damage)
    {
      return new TaggedValues<DamageType, int>(
        damage.Values.ToDictionary(
          k => k.Key,
          v => ApplyResistance(
            c,
            v.Value,
            target.Data.GetStat(OldStat.DamageResistance).Get(v.Key, notFound: 0))));
    }

    static int ApplyResistance(SkillContext c, int damageValue, float resistance) =>
      Mathf.RoundToInt(Math.Max(
        // Apply maximum resistance
        damageValue * (1f - c.GetStat(OldStat.MaximumDamageResistance).AsMultiplier()),
        Mathf.Clamp01(1f - resistance / (resistance + 2.0f * damageValue)) * damageValue));

    public override int ComputeFinalDamage(
      SkillContext c,
      Creature target,
      TaggedValues<DamageType, int> damage,
      bool isCriticalHit)
    {
      damage = c.GetBool(OldStat.IgnoresDamageReduction)
        ? damage
        : c.Delegate.ApplyDamageReduction(c, target, damage);

      damage = c.GetBool(OldStat.IgnoresDamageResistance)
        ? damage
        : c.Delegate.ApplyDamageResistance(c, target, damage);

      var total = damage.Values.Values.Sum();
      total = isCriticalHit ? c.GetStat(OldStat.CritMultiplier).CalculateFraction(total) : total;

      if (c.Skill.IsMelee())
      {
        total = c.GetStat(OldStat.MeleeDamageMultiplier).CalculateFraction(total);
      }

      if (c.Skill.IsProjectile())
      {
        total = c.GetStat(OldStat.ProjectileDamageMultiplier).CalculateFraction(total);
      }

      return total;
    }

    public override bool RollForHit(SkillContext c, Creature target)
    {
      var accuracy = c.GetInt(OldStat.Accuracy);
      var hitChance = Mathf.Clamp(
        value: 0.1f,
        accuracy / (accuracy + Mathf.Pow(target.Data.GetInt(OldStat.Evasion) / 4.0f, p: 0.8f)),
        max: 0.95f);
      return Random.value <= hitChance;
    }

    public override bool RollForCrit(SkillContext c, Creature target) =>
      Random.value <= c.GetStat(OldStat.CritChance).AsMultiplier() +
      target.Data.Stats.Get(OldStat.ReceiveCritsChance).AsMultiplier();

    public override int ComputeHealthDrain(SkillContext c, Creature creature, int damageAmount) => c.Skill.IsMelee()
      ? c.GetStat(OldStat.MeleeHealthDrainPercent).CalculateFraction(damageAmount)
      : 0;

    public override bool RollForStun(SkillContext c, Creature target, int damageAmount)
    {
      var stunChance = c.GetStat(OldStat.AddedStunChance).AsMultiplier() +
                       damageAmount / (float) target.Data.GetInt(OldStat.Health);
      return Random.value <= Mathf.Clamp(
        stunChance,
        min: 0,
        c.GetStat(OldStat.MaximumStunChance).AsMultiplier());
    }
  }
}
