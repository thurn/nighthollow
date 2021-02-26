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
using Nighthollow.Delegates.Effects;
using Nighthollow.Delegates.Handlers;
using Nighthollow.Services;
using Nighthollow.Stats;
using UnityEngine;

#nullable enable

namespace Nighthollow.Delegates.Implementations
{
  public sealed class MultipleProjectilesDelegate : AbstractDelegate, IOnFiredProjectile
  {
    public override string Describe(IStatDescriptionProvider provider) =>
      $"Fires {provider.Get(Stat.ProjectileSequenceCount)} Projectiles in Sequence";

    public IEnumerable<Effect> OnFiredProjectile(GameContext c, int delegateIndex, IOnFiredProjectile.Data d)
    {
      if (d.Effect.DelegateIndex <= delegateIndex)
        // Only process projectiles fired by *later* creature delegates in order to avoid infinite loops and such.
      {
        yield break;
      }

      // 1 less projectile since we already fired one
      for (var i = 1; i < d.Skill.GetInt(Stat.ProjectileSequenceCount); ++i)
      {
        yield return new FireProjectileEffect(
          d.Self,
          d.Skill,
          delegateIndex,
          d.Self.GetProjectileSourcePosition(c),
          Vector2.zero,
          firingDelayMs: i * d.Skill.Get(Stat.ProjectileSequenceDelay).AsMilliseconds());
      }
    }
  }
}
