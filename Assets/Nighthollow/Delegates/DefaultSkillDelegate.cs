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
using Nighthollow.Delegates.Effects;
using Nighthollow.Delegates.Handlers;
using Nighthollow.Services;
using Nighthollow.Stats;
using Nighthollow.Utils;
using UnityEngine;
using Random = UnityEngine.Random;

#nullable enable

namespace Nighthollow.Delegates
{
  public sealed class DefaultSkillDelegate : IDelegate,
    IOnSkillUsed, IOnSkillImpact, IOnApplySkillToTarget, IFindTargets, IFilterTargets, IGetCollider, IRollForBaseDamage,
    IApplyDamageReduction, IApplyDamageResistance, IComputeFinalDamage, IRollForHit, IRollForCrit, IComputeHealthDrain,
    IRollForStun
  {
    public string Describe(IStatDescriptionProvider provider) => "Default Skill Delegate";

    public IEnumerable<Effect> OnSkillUsed(IGameContext c, IOnSkillUsed.Data d)
    {
      yield return new MarkSkillUsedEffect(d.Self, d.Skill.BaseTypeId);
      switch (d.Skill.BaseType.SkillType)
      {
        case SkillType.Projectile:
          yield return new FireProjectileEffect(
            d.Self,
            d.Skill,
            DelegateId.DefaultSkillDelegate,
            c.Creatures.GetProjectileSourcePosition(d.Self),
            Vector2.zero);
          break;
        case SkillType.Melee when d.Skill.BaseType.Address != null:
          yield return
            new PlayTimedEffectEffect(d.Skill.BaseType.Address,
              c.Creatures.GetCollider(d.Self).bounds.center);
          break;
        case SkillType.Area when d.Skill.BaseType.Address != null:
          var position = c.Creatures.GetPosition(d.Self);
          yield return
            new PlayTimedEffectEffect(d.Skill.BaseType.Address,
              new Vector2(
                c[d.Self].RankPosition?.ToCenterXPosition() ?? position.x,
                c[d.Self].FilePosition?.ToCenterYPosition() ?? position.y));
          break;
      }
    }

    public IEnumerable<Effect> OnSkillImpact(IGameContext c, IOnSkillImpact.Data d)
    {
      var targets = d.Skill.DelegateList.FirstNonNull(c, new IFindTargets.Data(d.Self, d.Skill, d.Projectile));

      foreach (var target in targets ?? Enumerable.Empty<CreatureId>())
      {
        yield return new EventEffect<IOnApplySkillToTarget>(new IOnApplySkillToTarget.Data(
          d.Self,
          d.Skill,
          target,
          d.Projectile));
      }
    }

    public IEnumerable<Effect> OnApplySkillToTarget(IGameContext c, IOnApplySkillToTarget.Data d)
    {
      if (d.Skill.GetBool(Stat.UsesAccuracy) &&
          !d.Skill.DelegateList.First(c, new IRollForHit.Data(d.Self, d.Skill, d.Target), notFound: false))
      {
        yield return c[d.Self].Owner == PlayerName.User
          ? new SkillEventEffect(SkillEventEffect.Event.Missed, d.Self)
          : new SkillEventEffect(SkillEventEffect.Event.Evade, d.Target);
        yield break;
      }

      var isCriticalHit = false;
      if (d.Skill.GetBool(Stat.CanCrit) &&
          d.Skill.DelegateList.First(c, new IRollForCrit.Data(d.Self, d.Skill, d.Target), notFound: false))
      {
        yield return new SkillEventEffect(SkillEventEffect.Event.Crit, d.Self);
        isCriticalHit = true;
      }

      var baseDamage = d.Skill.DelegateList.First(c,
        new IRollForBaseDamage.Data(d.Self, d.Skill, d.Target),
        notFound: ImmutableDictionary<DamageType, int>.Empty);
      var damage = d.Skill.DelegateList.Iterate(c,
        new ITransformDamage.Data(d.Self, d.Skill, d.Target),
        initialValue: baseDamage);
      var totalDamage = d.Skill.DelegateList.First(c,
        new IComputeFinalDamage.Data(d.Self, d.Skill, d.Target, damage, isCriticalHit),
        notFound: 0);

      yield return new EventEffect<IOnHitTarget>(
        new IOnHitTarget.Data(d.Self, d.Skill, d.Target, d.Projectile, totalDamage));

      if (totalDamage == 0)
      {
        yield break;
      }

      yield return new ApplyDamageEffect(d.Self, d.Target, totalDamage);
      yield return new DamageTextEffect(d.Target, totalDamage);

      var healthDrain = d.Skill.DelegateList.First(c,
        new IComputeHealthDrain.Data(d.Self, d.Skill, d.Target, totalDamage),
        notFound: 0);
      if (healthDrain > 0)
      {
        yield return new HealEffect(d.Self, healthDrain);
      }

      if (d.Skill.GetBool(Stat.CanStun) &&
          d.Skill.DelegateList.First(c, new IRollForStun.Data(d.Self, d.Skill, d.Target, totalDamage), notFound: false))
      {
        yield return new StunEffect(d.Target, d.Skill.GetDurationSeconds(Stat.StunDurationOnEnemies));
        yield return new SkillEventEffect(SkillEventEffect.Event.Stun, d.Target);
      }
    }

    public IEnumerable<CreatureId> FindTargets(IGameContext c, IFindTargets.Data d)
    {
      var filter = new ContactFilter2D
      {
        layerMask = Constants.LayerMaskForCreatures(c[d.Self].Owner.GetOpponent()),
        useLayerMask = true,
        useTriggers = true
      };

      var sourceCollider = d.Skill.DelegateList.FirstNonNull(c, new IGetCollider.Data(d.Self, d.Skill, d.Projectile));
      if (!sourceCollider)
      {
        return Enumerable.Empty<CreatureId>();
      }

      var colliders = new List<Collider2D>();
      sourceCollider!.OverlapCollider(filter, colliders);

      var creatures = colliders
        // Filter out trigger colliders
        .Where(collider => collider.GetComponent<Creature>())
        .Select(collider => ComponentUtils.GetComponent<Creature>(collider).CreatureId);
      return d.Skill.DelegateList.First(c,
        new IFilterTargets.Data(d.Self, d.Skill, creatures),
        notFound: Enumerable.Empty<CreatureId>());
    }

    public IEnumerable<CreatureId> FilterTargets(IGameContext c, IFilterTargets.Data d) =>
      d.Skill.IsMelee()
        ? d.Hits.Take(Errors.CheckPositive(d.Skill.GetInt(Stat.MaxMeleeAreaTargets)))
        : d.Hits;

    public Collider2D GetCollider(IGameContext c, IGetCollider.Data d) =>
      d.Projectile ? d.Projectile!.Collider : c.Creatures.GetCollider(d.Self);

    public ImmutableDictionary<DamageType, int> RollForBaseDamage(
      IGameContext c, IRollForBaseDamage.Data d) =>
      DamageUtil.RollForDamage(d.Skill.Get(Stat.BaseDamage));

    public ImmutableDictionary<DamageType, int> ApplyDamageReduction(
      IGameContext c, IApplyDamageReduction.Data d)
    {
      return d.Damage.ToImmutableDictionary(
        pair => pair.Key,
        pair => ApplyReduction(
          d.Skill,
          pair.Value,
          c[d.Target].Data.Get(Stat.DamageReduction).GetOrReturnDefault(pair.Key, defaultValue: 0)));
    }

    static int ApplyReduction(SkillData skill, int damage, int reduction) =>
      Math.Max(
        // Apply maximum reduction
        Mathf.RoundToInt(damage * (1f - skill.Get(Stat.MaximumDamageReduction).AsMultiplier())),
        damage - reduction);

    public ImmutableDictionary<DamageType, int> ApplyDamageResistance(
      IGameContext c, IApplyDamageResistance.Data d)
    {
      return d.Damage.ToImmutableDictionary(
        pair => pair.Key,
        pair => ApplyResistance(
          d.Skill,
          pair.Value,
          c[d.Target].Data.Get(Stat.DamageResistance).GetOrReturnDefault(pair.Key, defaultValue: 0)));
    }

    static int ApplyResistance(SkillData skill, int damageValue, float resistance) =>
      Mathf.RoundToInt(Math.Max(
        // Apply maximum resistance
        damageValue * (1f - skill.Get(Stat.MaximumDamageResistance).AsMultiplier()),
        Mathf.Clamp01(1f - resistance / (resistance + 2.0f * damageValue)) * damageValue));

    public int ComputeFinalDamage(IGameContext c, IComputeFinalDamage.Data d)
    {
      var damage = d.Skill.GetBool(Stat.IgnoresDamageReduction)
        ? d.Damage
        : d.Skill.DelegateList.First(c,
          new IApplyDamageReduction.Data(d.Self, d.Skill, d.Target, d.Damage),
          notFound: d.Damage);

      damage = d.Skill.GetBool(Stat.IgnoresDamageResistance)
        ? damage
        : d.Skill.DelegateList.First(c,
          new IApplyDamageResistance.Data(d.Self, d.Skill, d.Target, damage),
          notFound: damage);

      var total = damage.Values.Sum();
      total = d.IsCriticalHit ? d.Skill.Get(Stat.CritMultiplier).CalculateFraction(total) : total;

      if (d.Skill.IsMelee())
      {
        total = d.Skill.Get(Stat.MeleeDamageMultiplier).CalculateFraction(total);
      }

      if (d.Skill.IsProjectile())
      {
        total = d.Skill.Get(Stat.ProjectileDamageMultiplier).CalculateFraction(total);
      }

      return total;
    }

    public bool RollForHit(IGameContext c, IRollForHit.Data d)
    {
      var accuracy = d.Skill.GetInt(Stat.Accuracy);
      var hitChance = Mathf.Clamp(
        value: 0.1f,
        accuracy / (accuracy + Mathf.Pow(c[d.Target].GetInt(Stat.Evasion) / 4.0f, p: 0.8f)),
        max: 0.95f);
      return Random.value <= hitChance;
    }

    public bool RollForCrit(IGameContext c, IRollForCrit.Data d) =>
      Random.value <= d.Skill.Get(Stat.CritChance).AsMultiplier() +
      c[d.Target].Stats.Get(Stat.ReceiveCritsChance).AsMultiplier();

    public int ComputeHealthDrain(IGameContext c, IComputeHealthDrain.Data d) => d.Skill.IsMelee()
      ? d.Skill.Get(Stat.MeleeHealthDrainPercent).CalculateFraction(d.TotalDamage)
      : 0;

    public bool RollForStun(IGameContext c, IRollForStun.Data d)
    {
      var stunChance = d.Skill.Get(Stat.AddedStunChance).AsMultiplier() +
                       d.DamageAmount / (float) c[d.Target].GetInt(Stat.Health);
      return Random.value <= Mathf.Clamp(
        stunChance,
        min: 0,
        d.Skill.Get(Stat.MaximumStunChance).AsMultiplier());
    }
  }
}