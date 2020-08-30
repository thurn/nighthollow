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

using Nighthollow.Components;
using Nighthollow.Data;
using Nighthollow.Services;
using Nighthollow.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Nighthollow.Delegate
{
  public sealed class DefaultCreatureDelegate : AbstractCreatureDelegate
  {
    public override void OnActivate(Creature self)
    {
    }

    public override void OnDeath(Creature self)
    {
    }

    public override bool ShouldUseUntargetedSkill(Creature self)
    {
      return self.Data.Skills.Any(s =>
        s.SkillType == SkillType.Untargeted &&
        s.EnergyCost <= self.CurrentEnergy);
    }

    public override bool ShouldUseMeleeSkill(Creature self)
    {
      return self.HasMeleeSkill() && GetCollidingCreatures(self.Owner, self.Collider).Any();
    }

    public override bool ShouldUseProjectileSkill(Creature self)
    {
      return self.HasProjectileSkill() &&
             GetCollidingCreatures(self.Owner, self.ProjectileCollider.Collider).Any();
    }

    public override SkillData ChooseSkill(Creature self, SkillType skillType)
    {
      return self.Data.Skills
        .Where(s => s.SkillType == skillType && s.EnergyCost <= self.CurrentEnergy)
        .OrderByDescending(s => s.EnergyCost)
        .ThenBy(s => Random.Range(0f, 1f))
        .FirstOrDefault();
    }

    public override ProjectileData ChooseProjectile(Creature self)
    {
      return self.Data.Projectiles
        .Where(p => p.EnergyCost <= self.CurrentEnergy)
        .OrderByDescending(p => p.EnergyCost)
        .ThenBy(p => Random.Range(0f, 1f))
        .First();
    }

    public override void OnFireProjectile(
      Creature self,
      ProjectileData projectileData,
      Vector2 firingPoint,
      Vector2? direction = null)
    {
      var projectile = Root.Instance.ObjectPoolService.Create(projectileData.Prefab, firingPoint);
      projectile.Initialize(self, projectileData, firingPoint, direction);
    }

    public override void OnUseUntargetedSkill(Creature self, SkillData skill)
    {
      throw new InvalidOperationException($"No implementation for skill {skill}");
    }

    public override void OnMeleeHit(Creature self)
    {
      foreach (var creature in GetCollidingCreatures(self.Owner, self.Collider))
      {
        ExecuteMeleeAttack(self, creature, self.Data.BaseAttack);
      }
    }

    public override void OnProjectileImpact(Creature self, Projectile projectile)
    {
      foreach (var creature in OverlapBox(
        self.Owner,
        projectile.transform.position,
        new Vector2(projectile.Data.HitboxSize / 1000f, projectile.Data.HitboxSize / 1000f)))
      {
        ExecuteSpellAttack(self, creature, self.Data.BaseAttack);
      }
    }

    public override void ExecuteSpellAttack(
      Creature self,
      Creature target,
      Damage damage)
    {
      var total = Mathf.RoundToInt(damage.Total(target.Data.DamageResistance));
      target.AddDamage(self, total);
    }

    public override void ExecuteMeleeAttack(
      Creature self,
      Creature target,
      Damage damage)
    {
      var accuracy = self.Data.Accuracy.Value;
      var hitChance = Mathf.Clamp(
        0.1f,
        accuracy / (accuracy + Mathf.Pow((target.Data.Evasion.Value / 4.0f), 0.8f)),
        0.95f);
      if (Random.Range(0f, 1f) > hitChance)
      {
        // Miss
        if (self.Owner == PlayerName.User)
        {
          Root.Instance.Prefabs.CreateMiss(RandomEffectPoint(self));
        }
        else
        {
          Root.Instance.Prefabs.CreateEvade(RandomEffectPoint(target));
        }

        return;
      }

      var multiplier = 1.0f;
      if (Random.Range(0f, 1f) < Constants.MultiplierBasisPoints(self.Data.CritChance.Value))
      {
        // Critical hit
        multiplier = Constants.MultiplierBasisPoints(self.Data.CritMultiplier.Value);
        if (self.Owner == PlayerName.User)
        {
          Root.Instance.Prefabs.CreateCrit(RandomEffectPoint(self));
        }
      }

      var total = Mathf.RoundToInt(multiplier * damage.Total(target.Data.DamageResistance));
      target.AddDamage(self, total);

      var lifeDrain = Constants.FractionBasisPoints(total, self.Data.MeleeLifeDrainBp.Value);
      self.Heal(lifeDrain);
    }

    public override void OnKilledEnemy(Creature self, Creature enemy, int damageAmount)
    {
    }

    public static IEnumerable<Creature> GetCollidingCreatures(
      PlayerName owner,
      Collider2D collider)
    {
      return OverlapBox(
        owner,
        collider.bounds.center,
        collider.bounds.size);
    }

    /// <summary>
    /// Returns a list of the enemies of 'owner' overlapping with a rectangle at 'center' of
    /// size 'size'.
    /// </summary>
    public static IEnumerable<Creature> OverlapBox(
      PlayerName owner,
      Vector2 center,
      Vector2 size)
    {
      var hits = Physics2D.OverlapBoxAll(
        center,
        size,
        angle: 0,
        Constants.LayerMaskForCreatures(owner.GetOpponent()));

      foreach (var hit in hits)
      {
        var creature = hit.GetComponent<Creature>();
        if (creature)
        {
          yield return creature;
        }
      }
    }

    public static Vector3 RandomEffectPoint(Creature creature)
    {
      var bounds = creature.Collider.bounds;
      return bounds.center + new Vector3(
               (Random.value - 0.5f) * bounds.size.x,
               (Random.value - 0.5f) * bounds.size.y,
               0
             );
    }
  }
}