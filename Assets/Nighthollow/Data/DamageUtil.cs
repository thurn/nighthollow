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
using Nighthollow.Generated;
using Nighthollow.Stats;
using Nighthollow.Utils;
using UnityEngine;

#nullable enable

namespace Nighthollow.Data
{
  public static class DamageUtil
  {
    public static TaggedValues<DamageType, IntValue> RollForDamage(TaggedValues<DamageType, IntRangeValue> damage) =>
      new TaggedValues<DamageType, IntValue>(
        damage.Values.ToDictionary(
          k => k.Key,
          v => new IntValue(Random.Range(v.Value.Low, v.Value.High))));

    public static TaggedValues<DamageType, IntValue> Multiply(
      int multiplier, TaggedValues<DamageType, IntValue> damage) =>
      new TaggedValues<DamageType, IntValue>(
        damage.Values.ToDictionary(
          pair => pair.Key,
          pair => new IntValue(pair.Value.Int * multiplier)));

    public static TaggedValues<DamageType, IntValue> Add(
      TaggedValues<DamageType, IntValue> a, TaggedValues<DamageType, IntValue> b)
    {
      var result = a.Values.Clone();
      foreach (var pair in b.Values)
      {
        if (result.ContainsKey(pair.Key))
        {
          result[pair.Key] = new IntValue(result[pair.Key].Int + pair.Value.Int);
        }
        else
        {
          result[pair.Key] = pair.Value;
        }
      }

      return new TaggedValues<DamageType, IntValue>(result);
    }
  }
}