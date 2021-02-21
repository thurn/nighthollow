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


using Nighthollow.Delegates.Effects;
using Nighthollow.Delegates2.Core;
using Nighthollow.Stats;
using UnityEngine;

#nullable enable

namespace Nighthollow.Delegates2.Implementations
{
  public sealed class MultipleProjectilesDelegate : AbstractDelegate
  {
    public override string Describe(IStatDescriptionProvider provider) =>
      $"Fires {provider.Get(Stat.ProjectileSequenceCount)} Projectiles in Sequence";

    public override void OnFiredProjectile(SkillContext c, FireProjectileEffect effect)
    {
      if (effect.DelegateIndex <= c.DelegateIndex)
        // Only process projectiles fired by *later* creature delegates in order to avoid infinite loops and such.
      {
        return;
      }

      // 1 less projectile since we already fired one
      for (var i = 1; i < c.GetInt(Stat.ProjectileSequenceCount); ++i)
      {
        c.Results.Add(new FireProjectileEffect(
          c.Self,
          c,
          c.DelegateIndex,
          c.Self.ProjectileSource.position,
          Vector2.zero,
          firingDelayMs: i * c.Get(Stat.ProjectileSequenceDelay).AsMilliseconds()));
      }
    }
  }
}
