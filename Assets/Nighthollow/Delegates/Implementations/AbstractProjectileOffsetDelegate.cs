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

    protected abstract Vector2 GetOrigin(GameContext c, CreatureState creature, SkillData skill, int projectileNumber);

    protected abstract Vector2 GetDirection(
      GameContext c, CreatureState creature, SkillData skill, int projectileNumber);

    /// <summary>Count of projectiles to fire, *including* the initial projectile.</summary>
    protected abstract int GetProjectileCount(GameContext c, CreatureState creature, SkillData skill);

    public bool ProjectileSkillCouldHit(GameContext c, int delegateIndex, IProjectileSkillCouldHit.Data d)
    {
      return CollectionUtils.AlternatingIntegers()
        .Take(GetProjectileCount(c, d.Self, d.Skill) - 1)
        .Select(i =>
          Physics2D.Raycast(
            GetOrigin(c, d.Self, d.Skill, i),
            GetDirection(c, d.Self, d.Skill, i),
            Mathf.Infinity,
            Constants.LayerMaskForCreatures(d.Self.Owner.GetOpponent())))
        .Any(hit => hit.collider);
    }

    public IEnumerable<Effect> OnFiredProjectile(GameContext c, int delegateIndex, IOnFiredProjectile.Data d)
    {
      // Only process projectiles fired by *later* creature delegates in order to avoid infinite loops and such.
      if (d.Effect.DelegateIndex <= delegateIndex)
      {
        return Enumerable.Empty<Effect>();
      }

      return CollectionUtils.AlternatingIntegers()
        .Take(GetProjectileCount(c, d.Self, d.Skill) - 1)
        .Select(i => Result(c, delegateIndex, d.Self, d.Skill, i));
    }

    FireProjectileEffect Result(
      GameContext c, int delegateIndex, CreatureState self, SkillData skill, int offsetCount) =>
      new FireProjectileEffect(
        self,
        skill,
        delegateIndex,
        GetOrigin(c, self, skill, offsetCount),
        GetDirection(c, self, skill, offsetCount));
  }
}