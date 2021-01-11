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


using Nighthollow.Components;
using Nighthollow.Delegates.Core;
using Nighthollow.Delegates.Effects;
using Nighthollow.Stats;

#nullable enable

namespace Nighthollow.Delegates.Implementations
{
  public sealed class KnockbackOnHitDelegate : AbstractDelegate
  {
    public override string Describe(IStatDescriptionProvider provider) =>
      $"Knocks Back Targets for {provider.Get(Stat.KnockbackDuration)} on Hit";

    public override void OnHitTarget(SkillContext c, Creature target, int damage)
    {
      var duration = c.GetDurationSeconds(Stat.KnockbackDuration);
      c.Results.Add(new KnockbackEffect(
        target,
        c.Get(Stat.KnockbackDistanceMultiplier).AsMultiplier() * duration,
        duration));
    }
  }
}
