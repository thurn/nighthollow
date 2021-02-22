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
using Nighthollow.Data;
using Nighthollow.Delegates.Effects;
using Nighthollow.Delegates.Handlers;
using Nighthollow.Stats;
using Nighthollow.Utils;
using UnityEngine;

#nullable enable

namespace Nighthollow.Delegates.Implementations
{
  public sealed class DefaultCreatureDelegate : AbstractDelegate,
    IOnCreatureDeath, ISelectSkill, IMeleeSkillCouldHit, IProjectileSkillCouldHit
  {
    public override string Describe(IStatDescriptionProvider provider) => "Default Creature Delegate";

    public IEnumerable<Effect> OnCreatureDeath(DelegateContext c, IOnCreatureDeath.Data d)
    {
      if (d.Self.Owner == PlayerName.Enemy)
      {
        yield return new EnemyRemovedEffect();
      }
    }

    public SkillData? SelectSkill(DelegateContext c, ISelectSkill.Data d)
    {
      if (AnyMeleeSkillCouldHit(c, d.Self))
      {
        var skill = SelectMatching(d.Self, s => s.IsMelee());
        if (skill != null)
        {
          return skill;
        }
      }

      if (AnyProjectileSkillCouldHit(c, d.Self))
      {
        var skill = SelectMatching(d.Self, s => s.IsProjectile());
        if (skill != null)
        {
          return skill;
        }
      }

      return SelectMatching(d.Self, s => !s.IsMelee() && !s.IsProjectile());
    }

    static bool AnyMeleeSkillCouldHit(DelegateContext c, CreatureState self)
    {
      return self.Data.Skills
        .Where(s => s.IsMelee())
        .Any(skill => skill.NewDelegate.Any(c, new IMeleeSkillCouldHit.Data(self, skill)));
    }

    static bool AnyProjectileSkillCouldHit(DelegateContext c, CreatureState self)
    {
      return self.Data.Skills
        .Where(s => s.IsProjectile())
        .Any(skill => skill.NewDelegate.Any(c, new IProjectileSkillCouldHit.Data(self, skill)));
    }

    static SkillData? SelectMatching(CreatureState self, Func<SkillData, bool> predicate)
    {
      var available = self.Data.Skills
        .Where(predicate)
        .Where(s => CooldownAvailable(self, s))
        .ToList();
      if (available.Count == 0)
      {
        return null;
      }

      var maxCooldown = available.Max(s => s.GetDurationSeconds(Stat.Cooldown));
      return available.FirstOrDefault(s => s.GetDurationSeconds(Stat.Cooldown) >= maxCooldown);
    }

    static bool CooldownAvailable(CreatureState self, SkillData skill)
    {
      var lastUsed = self.Creature.TimeLastUsedSeconds(skill.BaseTypeId);
      return !lastUsed.HasValue || skill.GetDurationSeconds(Stat.Cooldown) <= Time.time - lastUsed.Value;
    }

    public bool MeleeSkillCouldHit(DelegateContext c, IMeleeSkillCouldHit.Data d) =>
      d.Self.Creature.Collider && HasOverlap(d.Self.Owner, d.Self.Creature.Collider);

    public bool ProjectileSkillCouldHit(DelegateContext c, IProjectileSkillCouldHit.Data d)
    {
      var hit = Physics2D.Raycast(
        d.Self.Creature.ProjectileSource.position,
        Constants.ForwardDirectionForPlayer(d.Self.Owner),
        Mathf.Infinity,
        Constants.LayerMaskForCreatures(d.Self.Owner.GetOpponent()));
      return hit.collider;
    }

    static bool HasOverlap(PlayerName owner, Collider2D collider2D) =>
      collider2D.IsTouchingLayers(Constants.LayerMaskForCreatures(owner.GetOpponent()));
  }
}