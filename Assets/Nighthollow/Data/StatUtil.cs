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
          return new PercentageValue(double.Parse(value));
        case StatType.Duration:
          return new DurationValue(double.Parse(value));
        case StatType.IntRange:
          var split = value.Split(',');
          return new IntRangeValue(int.Parse(split[0]), int.Parse(split[1]));
        case StatType.SchoolInts:
        case StatType.DamageTypeInts:
        case StatType.DamageTypeIntRanges:
          throw new NotSupportedException("Not implemented.");
        case StatType.Unknown:
        default:
          throw new ArgumentOutOfRangeException(nameof(type), type, null);
      }
    }
  }
}