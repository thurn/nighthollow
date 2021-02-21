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
using System.Collections.Immutable;
using System.Linq;
using Nighthollow.Components;
using Nighthollow.Data;
using Nighthollow.Delegates2.Core;
using Nighthollow.Delegates2.Effects;
using Nighthollow.Stats;
using Nighthollow.Utils;
using UnityEngine;
using Random = UnityEngine.Random;

#nullable enable

namespace Nighthollow.Delegates2.Implementations
{
  public sealed class DefaultSkillDelegate : AbstractDelegate
  {
    public override string Describe(IStatDescriptionProvider provider) => "Default Skill Delegate";

    public override void OnUse(SkillContext c)
    {
      c.Self.MarkSkillUsed(c.Skill.BaseTypeId);
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
      var targets = context.Delegate.FindTargets(context);

      foreach (var target in targets ?? Enumerable.Empty<Creature>())
      {
        context.Results.Add(Events.Effect(context, (d, c) => d.OnApplyToTarget(c, target)));
      }
    }

    public override void OnApplyToTarget(SkillContext c, Creature target)
    {
      if (c.GetBool(Stat.UsesAccuracy) && c.Delegate.RollForHit(c, target) == false)
      {
        c.Results.Add(c.Self.Owner == PlayerName.User
          ? new SkillEventEffect(SkillEventEffect.Event.Missed, c.Self)
          : new SkillEventEffect(SkillEventEffect.Event.Evade, target));
        return;
      }

      var isCriticalHit = false;
      if (c.GetBool(Stat.CanCrit) && c.Delegate.RollForCrit(c, target) == true)
      {
        c.Results.Add(new SkillEventEffect(SkillEventEffect.Event.Crit, c.Self));
        isCriticalHit = true;
      }

      var baseDamage = c.Delegate.RollForBaseDamage(c, target) ?? ImmutableDictionary<DamageType, int>.Empty;
      var damage = c.Delegate.TransformDamage(c, target, baseDamage);
      var totalDamage = c.Delegate.ComputeFinalDamage(c, target, damage ?? baseDamage, isCriticalHit) ?? 0;

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
        c.Results.Add(new HealEffect(c.Self, healthDrain.Value));
      }

      if (c.GetBool(Stat.CanStun) && c.Delegate.RollForStun(c, target, totalDamage) == true)
      {
        c.Results.Add(new StunEffect(target, c.GetDurationSeconds(Stat.StunDurationOnEnemies)));
        c.Results.Add(new SkillEventEffect(SkillEventEffect.Event.Stun, target));
      }
    }

    public override IEnumerable<Creature>? FindTargets(SkillContext c)
    {
      var filter = new ContactFilter2D
      {
        layerMask = Constants.LayerMaskForCreatures(c.Self.Owner.GetOpponent()),
        useLayerMask = true,
        useTriggers = true
      };
      var sourceCollider = c.Delegate.GetCollider(c);
      if (!sourceCollider || sourceCollider == null)
      {
        return Enumerable.Empty<Creature>();
      }

      var colliders = new List<Collider2D>();
      sourceCollider.OverlapCollider(filter, colliders);

      return c.Delegate.FilterTargets(c, colliders
        // Filter out trigger colliders
        .Where(collider => collider.GetComponent<Creature>())
        .Select(ComponentUtils.GetComponent<Creature>));
    }

    public override IEnumerable<Creature> FilterTargets(SkillContext c, IEnumerable<Creature> hits) => c.Skill.IsMelee()
      ? hits.Take(Errors.CheckPositive(c.GetInt(Stat.MaxMeleeAreaTargets)))
      : hits;

    public override Collider2D GetCollider(SkillContext c) => c.Projectile ? c.Projectile!.Collider : c.Self.Collider;

    public override ImmutableDictionary<DamageType, int> RollForBaseDamage(SkillContext c, Creature target) =>
      DamageUtil.RollForDamage(c.Get(Stat.BaseDamage));

    public override ImmutableDictionary<DamageType, int> ApplyDamageReduction(
      SkillContext c,
      Creature target,
      ImmutableDictionary<DamageType, int> damage)
    {
      return damage.ToImmutableDictionary(
          pair => pair.Key,
          pair => ApplyReduction(
            c,
            pair.Value,
            target.Data.Get(Stat.DamageReduction).GetOrReturnDefault(pair.Key, defaultValue: 0)));
    }

    static int ApplyReduction(SkillContext c, int damage, int reduction) =>
      Math.Max(
        // Apply maximum reduction
        Mathf.RoundToInt(damage * (1f - c.Get(Stat.MaximumDamageReduction).AsMultiplier())),
        damage - reduction);

    public override ImmutableDictionary<DamageType, int> ApplyDamageResistance(
      SkillContext c,
      Creature target,
      ImmutableDictionary<DamageType, int> damage)
    {
      return damage.ToImmutableDictionary(
          pair => pair.Key,
          pair => ApplyResistance(
            c,
            pair.Value,
            target.Data.Get(Stat.DamageResistance).GetOrReturnDefault(pair.Key, defaultValue: 0)));
    }

    static int ApplyResistance(SkillContext c, int damageValue, float resistance) =>
      Mathf.RoundToInt(Math.Max(
        // Apply maximum resistance
        damageValue * (1f - c.Get(Stat.MaximumDamageResistance).AsMultiplier()),
        Mathf.Clamp01(1f - resistance / (resistance + 2.0f * damageValue)) * damageValue));

    public override int? ComputeFinalDamage(
      SkillContext c,
      Creature target,
      ImmutableDictionary<DamageType, int> damage,
      bool isCriticalHit)
    {
      damage = c.GetBool(Stat.IgnoresDamageReduction)
        ? damage
        : (c.Delegate.ApplyDamageReduction(c, target, damage) ?? damage);

      damage = c.GetBool(Stat.IgnoresDamageResistance)
        ? damage
        : (c.Delegate.ApplyDamageResistance(c, target, damage) ?? damage);

      var total = damage.Values.Sum();
      total = isCriticalHit ? c.Get(Stat.CritMultiplier).CalculateFraction(total) : total;

      if (c.Skill.IsMelee())
      {
        total = c.Get(Stat.MeleeDamageMultiplier).CalculateFraction(total);
      }

      if (c.Skill.IsProjectile())
      {
        total = c.Get(Stat.ProjectileDamageMultiplier).CalculateFraction(total);
      }

      return total;
    }

    public override bool? RollForHit(SkillContext c, Creature target)
    {
      var accuracy = c.GetInt(Stat.Accuracy);
      var hitChance = Mathf.Clamp(
        value: 0.1f,
        accuracy / (accuracy + Mathf.Pow(target.Data.GetInt(Stat.Evasion) / 4.0f, p: 0.8f)),
        max: 0.95f);
      return Random.value <= hitChance;
    }

    public override bool? RollForCrit(SkillContext c, Creature target) =>
      Random.value <= c.Get(Stat.CritChance).AsMultiplier() +
      target.Data.Stats.Get(Stat.ReceiveCritsChance).AsMultiplier();

    public override int? ComputeHealthDrain(SkillContext c, Creature creature, int damageAmount) => c.Skill.IsMelee()
      ? c.Get(Stat.MeleeHealthDrainPercent).CalculateFraction(damageAmount)
      : 0;

    public override bool? RollForStun(SkillContext c, Creature target, int damageAmount)
    {
      var stunChance = c.Get(Stat.AddedStunChance).AsMultiplier() +
                       damageAmount / (float) target.Data.GetInt(Stat.Health);
      return Random.value <= Mathf.Clamp(
        stunChance,
        min: 0,
        c.Get(Stat.MaximumStunChance).AsMultiplier());
    }
  }
}
