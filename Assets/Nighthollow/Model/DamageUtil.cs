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
using Nighthollow.Data;
using Nighthollow.Stats;
using Nighthollow.Utils;
using UnityEngine;

#nullable enable

namespace Nighthollow.Model
{
  public static class DamageUtil
  {
    public static TaggedValues<DamageType, int> RollForDamage(TaggedValues<DamageType, IntRangeValue> damage)
    {
      return new TaggedValues<DamageType, int>(
        damage.Values.ToDictionary(
          k => k.Key,
          v => Random.Range((int) v.Value.Low, (int) v.Value.High)));
    }

    public static TaggedValues<DamageType, int> Multiply(
      int multiplier, TaggedValues<DamageType, int> damage)
    {
      return new TaggedValues<DamageType, int>(
        damage.Values.ToDictionary(
          pair => pair.Key,
          pair => pair.Value * multiplier));
    }

    public static TaggedValues<DamageType, int> Add(
      TaggedValues<DamageType, int> a, TaggedValues<DamageType, int> b)
    {
      var result = a.Values.Clone();
      foreach (var pair in b.Values)
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

      return new TaggedValues<DamageType, int>(result);
    }
  }
}
