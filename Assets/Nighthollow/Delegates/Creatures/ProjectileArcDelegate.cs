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

using Nighthollow.Data;
using Nighthollow.Delegates.Core;
using Nighthollow.Delegates.Effects;
using Nighthollow.Generated;
using UnityEngine;

namespace Nighthollow.Delegates.Creatures
{
  public sealed class ProjectileArcDelegate : AbstractCreatureDelegate
  {
    public override void OnFiredProjectile(CreatureContext c, FireProjectileEffect effect)
    {
      if (effect.Identifier.DelegateType == DelegateType.Creature && effect.Identifier.Index <= c.DelegateIndex)
      {
        // Only process projectiles fired by *later* creature delegates in order to avoid infinite loops and such.
        return;
      }

      var toFire = c.GetInt(Stat.ProjectileArcCount) - 1;
      var projectileCount = 0;
      var offsetCount = 1;

      while (true)
      {
        if (projectileCount == toFire)
        {
          break;
        }

        c.Results.Add(new FireProjectileEffect(
          c.Self,
          effect.SkillData,
          new FireProjectileEffect.DelegateIdentifier(c.DelegateIndex, DelegateType.Creature),
          c.Self.ProjectileSource.position,
          effect.FiringDirectionOffset +
          offsetCount * new Vector2(0, c.GetInt(Stat.ProjectileArcRotationOffset) / 1000f)));

        projectileCount++;
        if (projectileCount == toFire)
        {
          break;
        }

        c.Results.Add(new FireProjectileEffect(
          c.Self,
          effect.SkillData,
          new FireProjectileEffect.DelegateIdentifier(c.DelegateIndex, DelegateType.Creature),
          c.Self.ProjectileSource.position,
          effect.FiringDirectionOffset -
          offsetCount * new Vector2(0, c.GetInt(Stat.ProjectileArcRotationOffset) / 1000f)));

        projectileCount++;
        offsetCount++;
      }

      // 1 less projectile since we already fired one
      for (var i = 1; i < c.GetInt(Stat.ProjectileSequenceCount); ++i)
      {
      }
    }
  }
}