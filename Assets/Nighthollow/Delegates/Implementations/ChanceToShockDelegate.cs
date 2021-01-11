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
using Nighthollow.Stats;
using UnityEngine;

#nullable enable

namespace Nighthollow.Delegates.Implementations
{
  public sealed class ChanceToShockDelegate : AbstractDelegate
  {
    public override string Describe(IStatDescriptionProvider provider) =>
      $"{provider.Get(Stat.ShockChance)} Chance to Shock";

    public override void OnApplyToTarget(SkillContext c, Creature target)
    {
      // if (Random.value > c.Get(OldStat.ShockChance).AsMultiplier())
      // {
      //   return;
      // }
      //
      // var lifetime = new TimedLifetime(c.GetDurationMilliseconds(OldStat.ShockDuration));
      // target.Data.Stats.InsertModifier(OldStat.IsShocked, new BooleanOperation(setBoolean: true), lifetime);
      // target.Data.Stats.InsertModifier(
      //   OldStat.ReceiveCritsChance,
      //   NumericOperation.Add(c.GetStat(OldStat.ShockAddedReceiveCritsChance)),
      //   lifetime);
    }
  }
}
