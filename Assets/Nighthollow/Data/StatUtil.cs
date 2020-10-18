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

using System;
using System.Linq;
using Nighthollow.Generated;
using Nighthollow.Stats;

#nullable enable

namespace Nighthollow.Data
{
  public static class StatUtil
  {
    public static IStatValue ParseStat(StatType type, string value)
    {
      switch (type)
      {
        case StatType.Int:
          return new IntValue(int.Parse(value));
        case StatType.Bool:
          return new BoolValue(bool.Parse(value));
        case StatType.Percentage:
          return new PercentageValue(double.Parse(value.Replace("%", "")));
        case StatType.Duration:
          return new DurationValue(double.Parse(value));
        case StatType.IntRange:
          return AsIntRange(value);
        case StatType.DamageTypeIntRanges:
          return new TaggedStatListValue<DamageType, IntRangeValue, IntRangeStat>(
            value.Split(',').Select(AsDamageRange).ToList());
        case StatType.SchoolInts:
          return new TaggedStatListValue<School, IntValue, IntStat>(
            value.Split(',').Select(AsSchoolInt).ToList());
        case StatType.DamageTypeInts:
          throw new NotSupportedException("Not implemented.");
        case StatType.Unknown:
        default:
          throw new ArgumentOutOfRangeException(nameof(type), type, null);
      }
    }

    static TaggedStatValue<DamageType, IntRangeValue> AsDamageRange(string value)
    {
      var damageType = value.Last() switch
      {
        'R' => DamageType.Radiant,
        'L' => DamageType.Lightning,
        'F' => DamageType.Fire,
        'C' => DamageType.Cold,
        'P' => DamageType.Physical,
        'N' => DamageType.Necrotic,
        _ => throw new ArgumentException($"Unknown damage type identifier: {value}")
      };
      return new TaggedStatValue<DamageType, IntRangeValue>(damageType, AsIntRange(value.Remove(value.Length - 1)));
    }

    static TaggedStatValue<School, IntValue> AsSchoolInt(string value)
    {
      var damageType = value.Last() switch
      {
        'L' => School.Light,
        'S' => School.Sky,
        'F' => School.Flame,
        'I' => School.Ice,
        'E' => School.Earth,
        'N' => School.Night,
        _ => throw new ArgumentException($"Unknown damage type identifier: {value}")
      };
      return new TaggedStatValue<School, IntValue>(damageType, new IntValue(int.Parse(value.Remove(value.Length - 1))));
    }

    static IntRangeValue AsIntRange(string value)
    {
      var split = value.Split('-');
      return new IntRangeValue(int.Parse(split[0]), int.Parse(split[1]));
    }
  }
}