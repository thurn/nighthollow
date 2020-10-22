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
      targets.AddRange(colliders.Select(ComponentUtils.GetComponent<Creature>));
    }

    public override Collider2D? GetCollider(SkillContext c) => c.Self.Collider;

    public override void ApplyToTarget(SkillContext c, Creature target, Results results)
    {
      if (GetStat(c, Stat.UsesAccuracy).Value && !c.Skill.Delegate.RollForHit(c, target))
      {
        results.Add(c.Self.Owner == PlayerName.User
          ? new SkillEventEffect(SkillEventEffect.Event.Missed, c.Self)
          : new SkillEventEffect(SkillEventEffect.Event.Evade, target));
        return;
      }

      var isCriticalHit = false;
      if (GetStat(c, Stat.CanCrit).Value && c.Skill.Delegate.RollForCrit(c, target))
      {
        results.Add(new SkillEventEffect(SkillEventEffect.Event.Crit, c.Self));
        isCriticalHit = true;
      }

      var damage = c.Skill.Delegate.RollForBaseDamage(c, target);
      damage = GetStat(c, Stat.IgnoresDamageReduction).Value
        ? damage
        : c.Skill.Delegate.ApplyDamageReduction(c, target, damage);
      damage = GetStat(c, Stat.IgnoresDamageResistance).Value
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

      if (GetStat(c, Stat.CanStun).Value && c.Skill.Delegate.CheckForStun(c, target, totalDamage))
      {
        results.Add(new StunEffect(target, c.Self.Data.GetDurationSeconds(Stat.StunDurationOnEnemies)));
        results.Add(new SkillEventEffect(SkillEventEffect.Event.Stun, target));
      }
    }

    public override TaggedStatListValue<DamageType, IntValue, IntStat>? RollForBaseDamage(
      SkillContext c, Creature target) =>
      new TaggedStatListValue<DamageType, IntValue, IntStat>(
        GetStat(c, Stat.BaseDamage).AllEntries
          .Select(pair => new TaggedStatValue<DamageType, IntValue>(
            pair.Key,
            new IntValue(Random.Range(pair.Value.LowValue, pair.Value.HighValue))))
          .ToList());

    public override TaggedStatListValue<DamageType, IntValue, IntStat>? ApplyDamageReduction(
      SkillContext c,
      Creature target,
      TaggedStatListValue<DamageType, IntValue, IntStat> damage) =>
      new TaggedStatListValue<DamageType, IntValue, IntStat>(
        damage.Values
          .Select(pair => new TaggedStatValue<DamageType, IntValue>(
            pair.Tag,
            ApplyReduction(
              c,
              pair.Value.Value,
              target.Data.Stats.Get(Stat.DamageReduction).Get(pair.Tag).Value)))
          .ToList());

    static IntValue ApplyReduction(SkillContext c, int damage, int reduction) =>
      new IntValue(Math.Max(
        // Apply maximum reduction
        Mathf.RoundToInt(damage * (1f - c.Self.GetOwnerStats().Get(Stat.MaximumDamageReduction).AsMultiplier())),
        damage - reduction));

    public override TaggedStatListValue<DamageType, IntValue, IntStat>? ApplyDamageResistance(
      SkillContext c,
      Creature target,
      TaggedStatListValue<DamageType, IntValue, IntStat> damage) =>
      new TaggedStatListValue<DamageType, IntValue, IntStat>(
        damage.Values
          .Select(pair => new TaggedStatValue<DamageType, IntValue>(
            pair.Tag,
            ApplyResistance(
              c,
              pair.Value.Value,
              target.Data.Stats.Get(Stat.DamageResistance).Get(pair.Tag).Value)))
          .ToList());

    static IntValue ApplyResistance(SkillContext c, int damageValue, float resistance) =>
      new IntValue(Mathf.RoundToInt(Math.Max(
        // Apply maximum reduction
        damageValue * (1f - c.Self.GetOwnerStats().Get(Stat.MaximumDamageResistance).AsMultiplier()),
        Mathf.Clamp01(resistance / (resistance + (2.0f * damageValue))) * damageValue)));

    public override int? ComputeFinalDamage(SkillContext c,
      Creature target,
      TaggedStatListValue<DamageType, IntValue, IntStat> damage,
      bool isCriticalHit)
    {
      var total = damage.Values.Select(v => v.Value.Value).Sum();
      return isCriticalHit ? GetStat(c, Stat.CritMultiplier).CalculateFraction(total) : total;
    }

    public override bool RollForHit(SkillContext c, Creature target)
    {
      var accuracy = GetStat(c, Stat.Accuracy).Value;
      var hitChance = Mathf.Clamp(
        0.1f,
        accuracy / (accuracy + Mathf.Pow((target.Data.GetInt(Stat.Evasion) / 4.0f), 0.8f)),
        0.95f);
      return Random.value <= hitChance;
    }

    public override bool RollForCrit(SkillContext c, Creature target) =>
      Random.value <= GetStat(c, Stat.CritChance).AsMultiplier();

    public override int ComputeHealthDrain(SkillContext c, Creature creature, int damageAmount) =>
      c.Skill.BaseType.IsMelee ? GetStat(c, Stat.MeleeHealthDrainPercent).CalculateFraction(damageAmount) : 0;

    public override bool CheckForStun(SkillContext c, Creature target, int damageAmount)
    {
      var stunChance = GetStat(c, Stat.StunChance).AsMultiplier() +
                           (damageAmount / (float) target.Data.GetInt(Stat.Health));
      return Random.value <= Mathf.Clamp(
        stunChance,
        0,
        c.Self.GetOwnerStats().Get(Stat.MaximumStunChance).AsMultiplier());
    }

    static T GetStat<T>(SkillContext c, IStatId<T> statId) where T : IStat =>
      c.Skill.Stats.GetWithParent(statId, c.Self.Data);
  }
}