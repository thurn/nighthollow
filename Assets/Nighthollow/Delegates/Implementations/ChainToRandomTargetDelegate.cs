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
using Nighthollow.Components;
using Nighthollow.Delegates.Core;
using Nighthollow.Delegates.Effects;
using Nighthollow.Generated;
using Nighthollow.Services;
using Nighthollow.State;
using Nighthollow.Stats;
using UnityEngine;

#nullable enable

namespace Nighthollow.Delegates.Implementations
{
  public sealed class ChainToRandomTargetDelegate : AbstractDelegate
  {
    public override string DescribeOld(StatEntity entity) =>
      $"Projectiles Chain {entity.GetInt(OldStat.MaxProjectileTimesChained)} Times to Random Targets";

    public override bool ShouldSkipProjectileImpact(SkillContext c)
    {
      if (c.Projectile && c.Projectile!.Values.Get(Key.TimesChained) > 0)
        // We skip impact for the projectile for creatures which have already been hit by a chaining projectile
      {
        return !c.Delegate.FindTargets(c)
          .Except(c.Projectile.Values.Get(Key.SkipProjectileImpacts)).Any();
      }

      return false;
    }

    public override void OnHitTarget(SkillContext c, Creature target, int damage)
    {
      if (c.Projectile && c.Projectile!.Values.Get(Key.TimesChained) < c.GetInt(OldStat.MaxProjectileTimesChained))
      {
        var enemies = Root.Instance.CreatureService.EnemyCreatures()
          .Except(c.Projectile.Values.Get(Key.SkipProjectileImpacts))
          .ToList();
        if (enemies.Count > 0)
        {
          var enemy = enemies[Random.Range(minInclusive: 0, enemies.Count)];
          c.Results.Add(new FireProjectileEffect(
            c.Self,
            c,
            c.DelegateIndex,
            c.Projectile.transform.position,
            trackCreature: enemy,
            values: c.Projectile.Values.Copy()
              .Increment(Key.TimesChained)
              .Append(Key.SkipProjectileImpacts, target)));
        }
      }
    }
  }
}
