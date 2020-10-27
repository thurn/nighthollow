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
using System.Linq;
using Nighthollow.Data;
using Nighthollow.Delegates.Core;
using Nighthollow.Generated;
using Nighthollow.Utils;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Nighthollow.Delegates.Creatures
{
  public sealed class DefaultCreatureDelegate : CreatureDelegate
  {
    public override void OnDeath(CreatureContext c, Results results)
    {
      if (c.Self.Owner == PlayerName.Enemy)
      {
        results.Add(new EnemyRemovedEffect());
      }
    }

    public override SkillData? SelectSkill(CreatureContext c)
    {
      var data = c.Self.Data;
      if (data.Delegate.CanUseProjectileSkill(c))
      {
        var skill = SelectMatching(c, s => s.BaseType.IsProjectile);
        if (skill != null)
        {
          return skill;
        }
      }

      if (data.Delegate.CanUseMeleeSkill(c))
      {
        var skill = SelectMatching(c, s => s.BaseType.IsMelee);
        if (skill != null)
        {
          return skill;
        }
      }

      return SelectMatching(c, s => !s.BaseType.IsProjectile && !s.BaseType.IsMelee);
    }

    static SkillData? SelectMatching(CreatureContext c, Func<SkillData, bool> predicate)
    {
      var available = c.Self.Data.Skills
        .Where(predicate)
        .Where(s => CooldownAvailable(c, s))
        .ToList();
      if (available.Count == 0)
      {
        return null;
      }

      var maxCooldown = available.Max(s => s.GetDurationSeconds(Stat.Cooldown));
      var maximums = available.Where(s => s.GetDurationSeconds(Stat.Cooldown) >= maxCooldown).ToList();
      return maximums.ElementAt(Random.Range(0, maximums.Count));
    }

    static bool CooldownAvailable(CreatureContext c, SkillData skill)
    {
      var lastUsed = c.Self.TimeLastUsedSeconds(skill.BaseType);
      return !lastUsed.HasValue || skill.GetDurationSeconds(Stat.Cooldown) <= Time.time - lastUsed.Value;
    }

    public override bool CanUseMeleeSkill(CreatureContext c) =>
      c.Self.Collider && HasOverlap(c.Self.Owner, c.Self.Collider);

    public override bool CanUseProjectileSkill(CreatureContext c) =>
      c.Self.ProjectileCollider && HasOverlap(c.Self.Owner, c.Self.ProjectileCollider.Collider);

    static bool HasOverlap(PlayerName owner, Collider2D collider2D) =>
      collider2D.IsTouchingLayers(Constants.LayerMaskForCreatures(owner.GetOpponent()));
  }
}