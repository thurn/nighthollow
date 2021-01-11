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

using System.Collections.Immutable;
using System.Linq;
using Nighthollow.Data;
using Nighthollow.Stats;
using Nighthollow.Utils;
using UnityEngine;

#nullable enable

namespace Nighthollow.Data
{
  public static class DamageUtil
  {
    public static ImmutableDictionary<DamageType, int> RollForDamage(
      ImmutableDictionary<DamageType, IntRangeValue> damage)
    {
      return damage.ToImmutableDictionary(
        pair => pair.Key,
        pair => Random.Range(pair.Value.Low, pair.Value.High));
    }

    public static ImmutableDictionary<DamageType, int> Multiply(
      int multiplier, ImmutableDictionary<DamageType, int> damage)
    {
      return damage.ToImmutableDictionary(
        pair => pair.Key,
        pair => pair.Value * multiplier);
    }

    public static ImmutableDictionary<DamageType, int> Add(
      ImmutableDictionary<DamageType, int> a, ImmutableDictionary<DamageType, int> b)
    {
      var result = a.ToBuilder();
      foreach (var pair in b)
      {
        if (result.ContainsKey(pair.Key))
        {
          result[pair.Key] = result[pair.Key] + pair.Value;
        }
        else
        {
          result[pair.Key] = pair.Value;
        }
      }

      return result.ToImmutable();
    }
  }
}
