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


using System.Linq;
using Nighthollow.Delegates.Core;
using Nighthollow.Delegates.Effects;
using Nighthollow.Data;
using Nighthollow.Utils;
using UnityEngine;

#nullable enable

namespace Nighthollow.Delegates.Implementations
{
  public abstract class AbstractProjectileOffsetDelegate : AbstractDelegate
  {
    protected abstract Vector2 GetOrigin(DelegateContext c, int projectileNumber);

    protected abstract Vector2 GetDirection(DelegateContext c, int projectileNumber);

    /// <summary>Count of projectiles to fire, *including* the initial projectile.</summary>
    protected abstract int GetProjectileCount(DelegateContext c);

    public override bool ProjectileCouldHit(CreatureContext c)
    {
      return CollectionUtils.AlternatingIntegers()
        .Take(GetProjectileCount(c) - 1)
        .Select(i =>
          Physics2D.Raycast(
            GetOrigin(c, i),
            GetDirection(c, i),
            Mathf.Infinity,
            Constants.LayerMaskForCreatures(c.Self.Owner.GetOpponent())))
        .Any(hit => hit.collider);
    }

    public override void OnFiredProjectile(SkillContext c, FireProjectileEffect effect)
    {
      if (effect.DelegateIndex <= c.DelegateIndex)
        // Only process projectiles fired by *later* creature delegates in order to avoid infinite loops and such.
      {
        return;
      }

      c.Results.AddRange(
        CollectionUtils.AlternatingIntegers()
          .Take(GetProjectileCount(c) - 1)
          .Select(i => Result(c, effect, i)));
    }

    FireProjectileEffect Result(SkillContext c, FireProjectileEffect effect, int offsetCount) =>
      new FireProjectileEffect(
        c.Self,
        c,
        c.DelegateIndex,
        GetOrigin(c, offsetCount),
        GetDirection(c, offsetCount));
  }
}
