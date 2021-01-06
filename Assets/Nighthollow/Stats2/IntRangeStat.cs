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
using MessagePack;
using Nighthollow.Data;
using Nighthollow.Generated;

#nullable enable

namespace Nighthollow.Stats2
{
  [MessagePackObject]
  public readonly struct IntRangeValue : IValueData
  {
    public IntRangeValue(int low, int high)
    {
      Low = low;
      High = high;
    }

    [Key(0)] public int Low { get; }
    [Key(1)] public int High { get; }

    public object Get() => this;

    public static readonly IntRangeValue Zero = new IntRangeValue(low: 0, high: 0);

    public override string ToString() => $"{Low}-{High}";

    public bool Equals(IntRangeValue other) => Low == other.Low && High == other.High;

    public override bool Equals(object? obj) => obj is IntRangeValue other && Equals(other);

    public override int GetHashCode()
    {
      unchecked
      {
        return (Low * 397) ^ High;
      }
    }

    public static bool operator ==(IntRangeValue left, IntRangeValue right) => left.Equals(right);

    public static bool operator !=(IntRangeValue left, IntRangeValue right) => !left.Equals(right);

    public static bool TryParse(string value, out IntRangeValue result)
    {
      var split = value.Split('-');
      if (split.Length == 2 && int.TryParse(split[0], out var low) && int.TryParse(split[1], out var high))
      {
        result = new IntRangeValue(low, high);
        return true;
      }
      else
      {
        result = Zero;
        return false;
      }
    }
  }

  public sealed class IntRangeStat : NumericStat<IntRangeValue>
  {
    public IntRangeStat(StatId statId) : base(statId)
    {
    }

    public override IntRangeValue ComputeValue(
      IReadOnlyDictionary<ModifierType, IEnumerable<NumericStatModifier<IntRangeValue>>> groups)
    {
      return new IntRangeValue(
        IntStat.Compute(groups, range => range.Low),
        IntStat.Compute(groups, range => range.High));
    }
  }
}