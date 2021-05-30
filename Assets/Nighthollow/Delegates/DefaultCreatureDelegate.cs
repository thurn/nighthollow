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
using Nighthollow.Services;
using Nighthollow.Stats;
using Nighthollow.Utils;
using UnityEngine;

#nullable enable

namespace Nighthollow.Delegates
{
  public sealed class DefaultCreatureDelegate : IDelegate,
    IOnCreatureDeath, ISelectSkill, IMeleeSkillCouldHit, IProjectileSkillCouldHit
  {
    public string Describe(IStatDescriptionProvider provider) => "Default Creature Delegate";

    public IEnumerable<Effect> OnCreatureDeath(IGameContext c, IOnCreatureDeath.Data d)
    {
      if (c[d.Self].Owner == PlayerName.Enemy)
      {
        // TODO: should be a triggered event
        yield return new EnemyRemovedEffect();
      }
    }

    public SkillData? SelectSkill(IGameContext c, ISelectSkill.Data d)
    {
      if (AnyMeleeSkillCouldHit(c, c[d.Self]))
      {
        var skill = SelectMatching(c, c[d.Self], s => s.IsMelee());
        if (skill != null)
        {
          return skill;
        }
      }

      if (AnyProjectileSkillCouldHit(c, c[d.Self]))
      {
        var skill = SelectMatching(c, c[d.Self], s => s.IsProjectile());
        if (skill != null)
        {
          return skill;
        }
      }

      return SelectMatching(c, c[d.Self], s => !s.IsMelee() && !s.IsProjectile());
    }

    static bool AnyMeleeSkillCouldHit(IGameContext c, CreatureState self)
    {
      return self.Data.Skills
        .Where(s => s.IsMelee())
        .Any(skill => skill.DelegateList.Any(c, new IMeleeSkillCouldHit.Data(self.CreatureId, skill)));
    }

    static bool AnyProjectileSkillCouldHit(IGameContext c, CreatureState self)
    {
      return self.Data.Skills
        .Where(s => s.IsProjectile())
        .Any(skill => skill.DelegateList.Any(c, new IProjectileSkillCouldHit.Data(self.CreatureId, skill)));
    }

    static SkillData? SelectMatching(IGameContext c, CreatureState self, Func<SkillData, bool> predicate)
    {
      var available = self.Data.Skills
        .Where(predicate)
        .Where(s => CooldownAvailable(c, self, s))
        .ToList();
      if (available.Count == 0)
      {
        return null;
      }

      var maxCooldown = available.Max(s => s.GetDurationSeconds(Stat.Cooldown));
      return available.FirstOrDefault(s => s.GetDurationSeconds(Stat.Cooldown) >= maxCooldown);
    }

    static bool CooldownAvailable(IGameContext c, CreatureState self, SkillData skill)
    {
      var lastUsed = self.SkillLastUsedTimeSeconds(skill.BaseTypeId);
      return !lastUsed.HasValue || skill.GetDurationSeconds(Stat.Cooldown) <= Time.time - lastUsed.Value;
    }

    public bool MeleeSkillCouldHit(IGameContext c, IMeleeSkillCouldHit.Data d) =>
      c.Creatures.GetCollider(d.Self).IsTouchingLayers(
        Constants.LayerMaskForCreatures(c[d.Self].Owner.GetOpponent()));

    public bool ProjectileSkillCouldHit(IGameContext c, IProjectileSkillCouldHit.Data d)
    {
      var hit = Physics2D.Raycast(
        c.Creatures.GetProjectileSourcePosition(d.Self),
        Constants.ForwardDirectionForPlayer(c[d.Self].Owner),
        Mathf.Infinity,
        Constants.LayerMaskForCreatures(c[d.Self].Owner.GetOpponent()));
      return hit.collider;
    }
  }
}