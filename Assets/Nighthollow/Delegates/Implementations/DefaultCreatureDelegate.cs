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
using System.Linq;
using Nighthollow.Data;
using Nighthollow.Delegates.Core;
using Nighthollow.Delegates.Effects;
using Nighthollow.Generated;
using Nighthollow.Model;
using Nighthollow.Stats;
using Nighthollow.Utils;
using UnityEngine;
using SkillData = Nighthollow.Model.SkillData;

#nullable enable

namespace Nighthollow.Delegates.Implementations
{
  public sealed class DefaultCreatureDelegate : AbstractDelegate
  {
    public override string DescribeOld(StatEntity entity) => "Default Creature Delegate";

    public override void OnDeath(CreatureContext c)
    {
      if (c.Self.Owner == PlayerName.Enemy)
      {
        c.Results.Add(new EnemyRemovedEffect());
      }
    }

    public override SkillData? SelectSkill(CreatureContext c)
    {
      if (c.Delegate.MeleeCouldHit(c))
      {
        var skill = SelectMatching(c, s => s.IsMelee());
        if (skill != null)
        {
          return skill;
        }
      }

      if (c.Self.HasProjectileSkill() && c.Delegate.ProjectileCouldHit(c))
      {
        var skill = SelectMatching(c, s => s.IsProjectile());
        if (skill != null)
        {
          return skill;
        }
      }

      return SelectMatching(c, s => !s.IsMelee() && !s.IsProjectile());
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

      var maxCooldown = available.Max(s => s.GetDurationSeconds(OldStat.Cooldown));
      return available.FirstOrDefault(s => s.GetDurationSeconds(OldStat.Cooldown) >= maxCooldown);
    }

    static bool CooldownAvailable(CreatureContext c, SkillData skill)
    {
      var lastUsed = c.Self.TimeLastUsedSeconds(skill.BaseType);
      return !lastUsed.HasValue || skill.GetDurationSeconds(OldStat.Cooldown) <= Time.time - lastUsed.Value;
    }

    public override bool MeleeCouldHit(CreatureContext c) =>
      c.Self.Collider && HasOverlap(c.Self.Owner, c.Self.Collider);

    public override bool ProjectileCouldHit(CreatureContext c)
    {
      var hit = Physics2D.Raycast(
        c.Self.ProjectileSource.position,
        Constants.ForwardDirectionForPlayer(c.Self.Owner),
        Mathf.Infinity,
        Constants.LayerMaskForCreatures(c.Self.Owner.GetOpponent()));
      return hit.collider;
    }

    static bool HasOverlap(PlayerName owner, Collider2D collider2D) =>
      collider2D.IsTouchingLayers(Constants.LayerMaskForCreatures(owner.GetOpponent()));
  }
}
