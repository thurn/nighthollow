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
using Nighthollow.Delegates.Effects;
using Nighthollow.Delegates.Handlers;
using Nighthollow.Services;
using Nighthollow.State;
using Nighthollow.Stats;
using Nighthollow.Utils;
using UnityEngine;

#nullable enable

namespace Nighthollow.Delegates.Implementations
{
  public sealed class ChainingProjectilesDelegate : IDelegate, IShouldSkipProjectileImpact, IOnHitTarget
  {
    public string Describe(IStatDescriptionProvider provider) =>
      $"Projectiles Chain {provider.Get(Stat.ProjectileChainCount)} Times on Hit";

    public bool ShouldSkipProjectileImpact(IGameContext c, int delegateIndex, IShouldSkipProjectileImpact.Data d) =>
      ShouldSkipChaininingImpacts(c, d);

    public static bool ShouldSkipChaininingImpacts(IGameContext c, IShouldSkipProjectileImpact.Data d)
    {
      if (d.Projectile && d.Projectile!.KeyValueStore.Get(Key.TimesChained) > 0)
        // We skip impact for the projectile for creatures which have already been hit by a chaining projectile
      {
        var targets = d.Skill.DelegateList.FirstNonNull(c, new IFindTargets.Data(d.Self, d.Skill, d.Projectile));
        if (targets is { } t)
        {
          return t
            .Except(d.Projectile.KeyValueStore.Get(Key.SkipProjectileImpacts))
            .Any();
        }
        else
        {
          return false;
        }
      }

      return false;
    }

    public IEnumerable<Effect> OnHitTarget(IGameContext c, int delegateIndex, IOnHitTarget.Data d)
    {
      Errors.CheckPositive(d.Skill.GetInt(Stat.ProjectileChainCount));
      if (d.Projectile &&
          d.Projectile!.KeyValueStore.Get(Key.TimesChained) < d.Skill.GetInt(Stat.MaxProjectileTimesChained))
      {
        // TODO: This works, but it probably shouldn't...
        var chainCount = d.Skill.GetInt(Stat.ProjectileChainCount);
        var add = chainCount % 2 == 0 ? 1f : 0f;
        for (var i = 0; i < chainCount; ++i)
        {
          var direction = Quaternion.AngleAxis(
            Mathf.Lerp(a: 0f, b: 360f, (i + add) / (chainCount + add)),
            Vector3.forward) * d.Projectile.transform.forward;
          yield return new FireProjectileEffect(
            d.Self,
            d.Skill,
            DelegateId.ChainingProjectilesDelegate,
            d.Projectile.transform.position,
            direction,
            values: d.Projectile.KeyValueStore
              .Increment(Key.TimesChained)
              .Append(Key.SkipProjectileImpacts, d.Target));
        }
      }
    }
  }
}
