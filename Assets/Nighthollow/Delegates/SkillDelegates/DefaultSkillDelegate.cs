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
using Nighthollow.Components;
using Nighthollow.Delegates.CreatureDelegates;
using Nighthollow.Stats;
using Nighthollow.Utils;
using UnityEngine;
using Nighthollow.Data;
using SkillType = Nighthollow.Generated.SkillType;
using Stat = Nighthollow.Generated.Stat;
using DamageType = Nighthollow.Generated.DamageType;


namespace Nighthollow.Delegates.SkillDelegates
{
  public sealed class DefaultSkillDelegate : SkillDelegate
  {
    public override void OnUse(SkillContext c, Results<TargetedSkillEffect> results)
    {
      if (c.Skill.SkillType == SkillType.Projectile)
      {
        results.Add(new FireProjectileEffect(c.Skill, c.Self.ProjectileSource.position, Vector2.zero));
      }
    }

    public override void OnImpact(SkillContext c, Collider2D collider, Results<TargetedSkillEffect> results)
    {
      var targets = new List<Creature>();
      c.Skill.Delegate.PopulateTargets(c, targets);
      foreach (var target in targets)
      {
        c.Skill.Delegate.ApplyToTarget(c, target, results);
      }
    }

    public override void PopulateTargets(SkillContext c, Collider2D collider, List<Creature> targets)
    {
      var colliders = new List<Collider2D>();
      var filter = new ContactFilter2D {layerMask = Constants.LayerMaskForCreatures(c.Self.Owner.GetOpponent())};
      collider.OverlapCollider(filter, colliders);

      foreach (var result in colliders)
      {
        targets.Add(ComponentUtils.GetComponent<Creature>(result));
      }
    }

    public override void ApplyToTarget(SkillContext c, Creature target, Results<TargetedSkillEffect> results)
    {
      if (!c.Skill.Delegate.CheckForHit(c, target))
      {
        results.Add(new SkillEventEffect(SkillEventEffect.Event.Missed));
        return;
      }

      int damage;
      if (c.Skill.Delegate.CheckForCrit(c, target))
      {
        results.Add(new SkillEventEffect(SkillEventEffect.Event.Crit));
        damage = c.Skill.Delegate.ComputeCritDamage(c, target, c.Skill.Stats.Get(Stat.BaseDamage));
      }
      else
      {
        damage = c.Skill.Delegate.ComputeDamage(c, target, c.Skill.Stats.Get(Stat.BaseDamage));
      }

      results.Add(new ApplyDamageEffect(target, damage));

      var lifeDrain = c.Skill.Delegate.ComputeLifeDrain(c, target, damage);
      if (lifeDrain > 0)
      {
        results.Add(new HealEffect(c.Self, lifeDrain));
      }

      if (c.Skill.Delegate.CheckForStun(c, target, damage))
      {
        results.Add(new StunEffect(target, c.Self.Data.GetDurationSeconds(Stat.StunDurationOnEnemies)));
        results.Add(new SkillEventEffect(SkillEventEffect.Event.Stun));
      }
    }

    public override bool CheckForHit(SkillContext c, Creature target) => false;

    public override bool CheckForCrit(SkillContext c, Creature target) => false;

    public override int ComputeDamage(SkillContext c, Creature target,
      TaggedStats<DamageType, IntRangeStat> damage) => 0;

    public override int ComputeCritDamage(SkillContext c, Creature target,
      TaggedStats<DamageType, IntRangeStat> damage) => 0;

    public override int ComputeLifeDrain(SkillContext c, Creature creature, int damageAmount) => 0;

    public override bool CheckForStun(SkillContext c, Creature target, int damageAmount) => false;
  }
}