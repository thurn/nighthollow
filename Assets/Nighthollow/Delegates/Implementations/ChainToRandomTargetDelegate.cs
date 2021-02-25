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
using Nighthollow.Components;
using Nighthollow.Delegates.Effects;
using Nighthollow.Delegates.Handlers;
using Nighthollow.Services;
using Nighthollow.State;
using Nighthollow.Stats;
using UnityEngine;

#nullable enable

namespace Nighthollow.Delegates.Implementations
{
  public sealed class ChainToRandomTargetDelegate : AbstractDelegate, IShouldSkipProjectileImpact, IOnHitTarget
  {
    public override string Describe(IStatDescriptionProvider provider) =>
      $"Projectiles Chain {provider.Get(Stat.MaxProjectileTimesChained)} Times to Random Targets";

    public bool ShouldSkipProjectileImpact(GameContext c, int delegateIndex, IShouldSkipProjectileImpact.Data d)
    {
      if (d.Projectile && d.Projectile!.KeyValueStore.Get(Key.TimesChained) > 0)
      {
        // We skip impact for the projectile for creatures which have already been hit by a chaining projectile
        var targets = d.Skill.DelegateList.FirstNonNull(c, new IFindTargets.Data(d.Self, d.Skill, d.Projectile));
        return !(targets ?? Enumerable.Empty<Creature>())
          .Except(d.Projectile.KeyValueStore.Get(Key.SkipProjectileImpacts))
          .Any();
      }

      return false;
    }

    public IEnumerable<Effect> OnHitTarget(GameContext c, int delegateIndex, IOnHitTarget.Data d)
    {
      if (d.Projectile &&
          d.Projectile!.KeyValueStore.Get(Key.TimesChained) < d.Skill.GetInt(Stat.MaxProjectileTimesChained))
      {
        var enemies = Root.Instance.CreatureService.EnemyCreatures()
          .Except(d.Projectile.KeyValueStore.Get(Key.SkipProjectileImpacts))
          .ToList();
        if (enemies.Count > 0)
        {
          var enemy = enemies[Random.Range(minInclusive: 0, enemies.Count)];
          yield return new FireProjectileEffect(
            d.Self,
            d.Skill,
            delegateIndex,
            d.Projectile.transform.position,
            trackCreature: enemy,
            values: d.Projectile.KeyValueStore
              .Increment(Key.TimesChained)
              .Append(Key.SkipProjectileImpacts, d.Target.Creature));
        }
      }
    }
  }
}