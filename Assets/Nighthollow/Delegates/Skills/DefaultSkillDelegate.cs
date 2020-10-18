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
using Nighthollow.Delegates.Core;
using Nighthollow.Stats;
using Nighthollow.Utils;
using UnityEngine;
using Stat = Nighthollow.Generated.Stat;
using DamageType = Nighthollow.Generated.DamageType;

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
      if (!c.Skill.Delegate.RollForHit(c, target))
      {
        results.Add(new SkillEventEffect(SkillEventEffect.Event.Missed));
        return;
      }

      int damage;
      if (c.Skill.Delegate.RollForCrit(c, target))
      {
        results.Add(new SkillEventEffect(SkillEventEffect.Event.Crit));
        damage = c.Skill.Delegate.RollForCritDamage(c, target, c.Skill.Stats.Get(Stat.BaseDamage));
      }
      else
      {
        damage = c.Skill.Delegate.RollForDamage(c, target, c.Skill.Stats.Get(Stat.BaseDamage));
      }

      results.Add(new ApplyDamageEffect(c.Self, target, damage));

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

    public override bool RollForHit(SkillContext c, Creature target) => true;

    public override bool RollForCrit(SkillContext c, Creature target) => false;

    public override int RollForDamage(
      SkillContext c,
      Creature target,
      TaggedStats<DamageType, IntRangeStat> damage) => damage.AllEntries.Sum(e => e.Value.HighValue);

    public override int RollForCritDamage(
      SkillContext c,
      Creature target,
      TaggedStats<DamageType, IntRangeStat> damage) => damage.AllEntries.Sum(e => e.Value.HighValue);

    public override int ComputeLifeDrain(SkillContext c, Creature creature, int damageAmount) => 0;

    public override bool CheckForStun(SkillContext c, Creature target, int damageAmount) => false;
  }
}