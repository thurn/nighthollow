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

using System.Linq;
using Nighthollow.Data;
using Nighthollow.Delegates.Core;
using Nighthollow.Delegates.Effects;
using Nighthollow.Generated;
using Nighthollow.Utils;
using UnityEngine;

namespace Nighthollow.Delegates.Creatures
{
  public sealed class ProjectileArcDelegate : AbstractCreatureDelegate
  {
    public override bool ProjectileCouldHit(CreatureContext c)
    {
      return CollectionUtils.AlternatingIntegers()
        .Take(c.GetInt(Stat.ProjectileArcCount) - 1)
        .Select(i =>
          Physics2D.Raycast(
            origin: c.Self.ProjectileSource.position,
            direction: FiringDirection(c, i),
            distance: Mathf.Infinity,
            layerMask: Constants.LayerMaskForCreatures(c.Self.Owner.GetOpponent())))
        .Any(hit => hit.collider);
    }

    public override void OnFiredProjectile(CreatureContext c, FireProjectileEffect effect)
    {
      if (effect.Identifier.DelegateType == DelegateType.Creature && effect.Identifier.Index <= c.DelegateIndex)
      {
        // Only process projectiles fired by *later* creature delegates in order to avoid infinite loops and such.
        return;
      }

      c.Results.AddRange(
        CollectionUtils.AlternatingIntegers()
          .Take(c.GetInt(Stat.ProjectileArcCount) - 1)
          .Select(i => Result(c, effect, i)));
    }

    static FireProjectileEffect Result(CreatureContext c, FireProjectileEffect effect, int offsetCount)
    {
      return new FireProjectileEffect(
        c.Self,
        effect.SkillData,
        new FireProjectileEffect.DelegateIdentifier(c.DelegateIndex, DelegateType.Creature),
        c.Self.ProjectileSource.position,
        FiringDirection(c, offsetCount));
    }

    static Vector2 FiringDirection(CreatureContext c, int offsetCount)
    {
      return Constants.ForwardDirectionForPlayer(c.Self.Owner) +
             offsetCount * new Vector2(0, c.GetInt(Stat.ProjectileArcRotationOffset) / 1000f);
    }
  }
}