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
using System.Linq;
using Nighthollow.Data;
using Nighthollow.Delegates.Effects;
using Nighthollow.Delegates.Handlers;
using Nighthollow.Services;
using Nighthollow.Stats;
using Nighthollow.Utils;
using UnityEngine;

#nullable enable

namespace Nighthollow.Delegates.Implementations
{
  public abstract class AbstractProjectileOffsetDelegate : IDelegate, IProjectileSkillCouldHit,
    IOnFiredProjectile
  {
    public abstract string Describe(IStatDescriptionProvider provider);

    protected abstract Vector2 GetOrigin(IGameContext c, CreatureState creature, SkillData skill, int projectileNumber);

    protected abstract Vector2 GetDirection(
      IGameContext c, CreatureState creature, SkillData skill, int projectileNumber);

    /// <summary>Count of projectiles to fire, *including* the initial projectile.</summary>
    protected abstract int GetProjectileCount(IGameContext c, CreatureState creature, SkillData skill);

    protected abstract DelegateId DelegateId { get; }

    public bool ProjectileSkillCouldHit(IGameContext c, IProjectileSkillCouldHit.Data d)
    {
      return CollectionUtils.AlternatingIntegers()
        .Take(GetProjectileCount(c, c[d.Self], d.Skill) - 1)
        .Select(i =>
          Physics2D.Raycast(
            GetOrigin(c, c[d.Self], d.Skill, i),
            GetDirection(c, c[d.Self], d.Skill, i),
            Mathf.Infinity,
            Constants.LayerMaskForCreatures(c[d.Self].Owner.GetOpponent())))
        .Any(hit => hit.collider);
    }

    public IEnumerable<Effect> OnFiredProjectile(IGameContext c, IOnFiredProjectile.Data d)
    {
      // Skip effect created by this delegate to avoid infinite loops
      if (d.Effect.CreatedBy == DelegateId)
      {
        return Enumerable.Empty<Effect>();
      }

      return CollectionUtils.AlternatingIntegers()
        .Take(GetProjectileCount(c, c[d.Self], d.Skill) - 1)
        .Select(i => Result(c, c[d.Self], d.Skill, i));
    }

    FireProjectileEffect Result(
      IGameContext c, CreatureState self, SkillData skill, int offsetCount) =>
      new FireProjectileEffect(
        self.CreatureId,
        skill,
        DelegateId,
        GetOrigin(c, self, skill, offsetCount),
        GetDirection(c, self, skill, offsetCount));
  }
}