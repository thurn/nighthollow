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
using Nighthollow.Generated;
using Nighthollow.Stats;
using Nighthollow.Utils;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Nighthollow.Delegates.Skills
{
  public sealed class DefaultSkillDelegate : SkillDelegate
  {
    public override void OnUse(SkillContext c, Results results)
    {
      c.Self.MarkSkillUsed(c.Skill.BaseType);
      if (c.Skill.BaseType.IsProjectile)
      {
        results.Add(new FireProjectileEffect(c.Skill, c.Self.ProjectileSource.position, Vector2.zero));
      }
    }

    public override void OnImpact(SkillContext c, Results results)
    {
      var targets = c.Skill.Delegate.PopulateTargets(c);
      foreach (var target in targets)
      {
        c.Skill.Delegate.ApplyToTarget(c, target, results);
      }
    }

    public override void PopulateTargets(SkillContext c, List<Creature> targets)
    {
      var filter = new ContactFilter2D
      {
        layerMask = Constants.LayerMaskForCreatures(c.Self.Owner.GetOpponent()),
        useTriggers = true
      };
      var sourceCollider = c.Skill.Delegate.GetCollider(c);
      if (!sourceCollider)
      {
        return;
      }

      var colliders = new List<Collider2D>();
      sourceCollider!.OverlapCollider(filter, colliders);
      targets.AddRange(c.Skill.Delegate.SelectTargets(
        c, colliders.Select(ComponentUtils.GetComponent<Creature>)));
    }

    public override IEnumerable<Creature> SelectTargets(SkillContext c, IEnumerable<Creature> hits) =>
      c.Skill.BaseType.IsMelee ? hits.Take(Errors.CheckNonzero(c.Skill.GetInt(Stat.MaxMeleeAreaTargets))) : hits;

    public override Collider2D? GetCollider(SkillContext c) => c.Self.Collider;

    public override void ApplyToTarget(SkillContext c, Creature target, Results results)
    {
      if (c.Skill.GetBool(Stat.UsesAccuracy) && !c.Skill.Delegate.RollForHit(c, target))
      {
        results.Add(c.Self.Owner == PlayerName.User
          ? new SkillEventEffect(SkillEventEffect.Event.Missed, c.Self)
          : new SkillEventEffect(SkillEventEffect.Event.Evade, target));
        return;
      }

      var isCriticalHit = false;
      if (c.Skill.GetBool(Stat.CanCrit) && c.Skill.Delegate.RollForCrit(c, target))
      {
        results.Add(new SkillEventEffect(SkillEventEffect.Event.Crit, c.Self));
        isCriticalHit = true;
      }

      var damage = c.Skill.Delegate.RollForBaseDamage(c, target);
      damage = c.Skill.GetBool(Stat.IgnoresDamageReduction)
        ? damage
        : c.Skill.Delegate.ApplyDamageReduction(c, target, damage);
      damage = c.Skill.GetBool(Stat.IgnoresDamageResistance)
        ? damage
        : c.Skill.Delegate.ApplyDamageResistance(c, target, damage);
      var totalDamage = c.Skill.Delegate.ComputeFinalDamage(c, target, damage, isCriticalHit);

      results.Add(new ApplyDamageEffect(c.Self, target, totalDamage));
      results.Add(new DamageTextEffect(target, totalDamage));

      var healthDrain = c.Skill.Delegate.ComputeHealthDrain(c, target, totalDamage);
      if (healthDrain > 0)
      {
        results.Add(new HealEffect(c.Self, healthDrain));
      }

      if (c.Skill.GetBool(Stat.CanStun) && c.Skill.Delegate.CheckForStun(c, target, totalDamage))
      {
        results.Add(new StunEffect(target, c.Self.Data.GetDurationSeconds(Stat.StunDurationOnEnemies)));
        results.Add(new SkillEventEffect(SkillEventEffect.Event.Stun, target));
      }
    }

    public override TaggedValues<DamageType, IntValue>? RollForBaseDamage(
      SkillContext c, Creature target) =>
      new TaggedValues<DamageType, IntValue>(
        c.Skill.Stats.Get(Stat.BaseDamage).Values.ToDictionary(
          k => k.Key,
          v => new IntValue(Random.Range(v.Value.Low, v.Value.High))));

    public override TaggedValues<DamageType, IntValue>? ApplyDamageReduction(
      SkillContext c,
      Creature target,
      TaggedValues<DamageType, IntValue> damage) =>
      new TaggedValues<DamageType, IntValue>(
        damage.Values.ToDictionary(
          k => k.Key,
          v => ApplyReduction(
            c,
            v.Value.Int,
            target.Data.Stats.Get(Stat.DamageReduction).Get(v.Key, IntValue.Zero).Int)));

    static IntValue ApplyReduction(SkillContext c, int damage, int reduction) =>
      new IntValue(Math.Max(
        // Apply maximum reduction
        Mathf.RoundToInt(damage * (1f - c.Self.GetOwnerStats().Get(Stat.MaximumDamageReduction).AsMultiplier())),
        damage - reduction));

    public override TaggedValues<DamageType, IntValue>? ApplyDamageResistance(
      SkillContext c,
      Creature target,
      TaggedValues<DamageType, IntValue> damage) =>
      new TaggedValues<DamageType, IntValue>(
        damage.Values.ToDictionary(
          k => k.Key,
          v => ApplyResistance(
            c,
            v.Value.Int,
            target.Data.Stats.Get(Stat.DamageResistance).Get(v.Key, IntValue.Zero).Int)));

    static IntValue ApplyResistance(SkillContext c, int damageValue, float resistance) =>
      new IntValue(Mathf.RoundToInt(Math.Max(
        // Apply maximum resistance
        damageValue * (1f - c.Self.GetOwnerStats().Get(Stat.MaximumDamageResistance).AsMultiplier()),
        Mathf.Clamp01(1f - (resistance / (resistance + (2.0f * damageValue)))) * damageValue)));

    public override int? ComputeFinalDamage(SkillContext c,
      Creature target,
      TaggedValues<DamageType, IntValue> damage,
      bool isCriticalHit)
    {
      var total = damage.Values.Values.Select(v => v.Int).Sum();
      return isCriticalHit ? c.Skill.Stats.Get(Stat.CritMultiplier).CalculateFraction(total) : total;
    }

    public override bool RollForHit(SkillContext c, Creature target)
    {
      var accuracy = c.Skill.GetInt(Stat.Accuracy);
      var hitChance = Mathf.Clamp(
        0.1f,
        accuracy / (accuracy + Mathf.Pow((target.Data.GetInt(Stat.Evasion) / 4.0f), 0.8f)),
        0.95f);
      return Random.value <= hitChance;
    }

    public override bool RollForCrit(SkillContext c, Creature target) =>
      Random.value <= c.Skill.Stats.Get(Stat.CritChance).AsMultiplier();

    public override int ComputeHealthDrain(SkillContext c, Creature creature, int damageAmount) =>
      c.Skill.BaseType.IsMelee ? c.Skill.Stats.Get(Stat.MeleeHealthDrainPercent).CalculateFraction(damageAmount) : 0;

    public override bool CheckForStun(SkillContext c, Creature target, int damageAmount)
    {
      var stunChance = c.Skill.Stats.Get(Stat.StunChance).AsMultiplier() +
                       (damageAmount / (float) target.Data.GetInt(Stat.Health));
      return Random.value <= Mathf.Clamp(
        stunChance,
        0,
        c.Self.GetOwnerStats().Get(Stat.MaximumStunChance).AsMultiplier());
    }
  }
}