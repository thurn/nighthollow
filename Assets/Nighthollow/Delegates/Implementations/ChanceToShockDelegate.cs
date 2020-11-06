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

using Nighthollow.Components;
using Nighthollow.Delegates.Core;
using Nighthollow.Generated;
using Nighthollow.Stats;
using UnityEngine;

namespace Nighthollow.Delegates.Implementations
{
  public sealed class ChanceToShockDelegate : AbstractDelegate
  {
    public override void OnApplyToTarget(SkillContext c, Creature target)
    {
      if (Random.value > c.GetStat(Stat.ShockChance).AsMultiplier())
      {
        return;
      }

      var lifetime = new TimedLifetime(c.GetDurationMilliseconds(Stat.ShockDuration));
      target.Data.Stats.InsertModifier(Stat.IsShocked, new BooleanOperation(true), lifetime);
      target.Data.Stats.InsertModifier(
        Stat.ReceiveCritsChance,
        NumericOperation.Add(c.GetStat(Stat.ShockAddedReceiveCritsChance)),
        lifetime);
    }
  }
}