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
using Nighthollow.Components;
using Nighthollow.Delegates.Core;
using Nighthollow.Delegates.Effects;
using Nighthollow.Generated;
using Nighthollow.State;
using Nighthollow.Stats;
using Nighthollow.Utils;
using UnityEngine;

namespace Nighthollow.Delegates.Implementations
{
  public sealed class ChainingProjectilesDelegate : AbstractDelegate
  {
    public override string Describe(StatEntity entity) =>
      $"Projectiles Chain {entity.GetInt(Stat.ProjectileChainCount)} Times on Hit";

    public override bool ShouldSkipProjectileImpact(SkillContext c)
    {
      if (c.Projectile && c.Projectile!.Values.Get(Key.TimesChained) > 0)
      {
        // We skip impact for the projectile for creatures which have already been hit by a chaining projectile
        return !c.Delegate.FindTargets(c)
          .Except(c.Projectile.Values.Get(Key.SkipProjectileImpacts)).Any();
      }

      return false;
    }

    public override void OnHitTarget(SkillContext c, Creature target, int damage)
    {
      Errors.CheckPositive(c.GetInt(Stat.ProjectileChainCount));
      if (c.Projectile && c.Projectile!.Values.Get(Key.TimesChained) < c.GetInt(Stat.MaxProjectileTimesChained))
      {
        // TODO: This works, but it probably shouldn't...
        var chainCount = c.GetInt(Stat.ProjectileChainCount);
        var add = chainCount % 2 == 0 ? 1f : 0f;
        for (var i = 0; i < chainCount; ++i)
        {
          var direction = Quaternion.AngleAxis(
            Mathf.Lerp(0f, 360f, (i + add) / (chainCount + add)),
            Vector3.forward) * c.Projectile.transform.forward;
          c.Results.Add(new FireProjectileEffect(
            c.Self,
            c,
            c.DelegateIndex,
            c.Projectile.transform.position,
            direction,
            values: c.Projectile.Values.Copy()
              .Increment(Key.TimesChained)
              .Append(Key.SkipProjectileImpacts, target)));
        }
      }
    }
  }
}
