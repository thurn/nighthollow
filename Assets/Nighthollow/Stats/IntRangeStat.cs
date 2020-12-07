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
using System.Collections.Generic;
using Nighthollow.Generated;
using UnityEngine;

#nullable enable

namespace Nighthollow.Stats
{
  public readonly struct IntRangeValue
  {
    public static readonly IntRangeValue Zero = new IntRangeValue(low: 0, high: 0);
    public int Low { get; }
    public int High { get; }

    public override string ToString() => $"{Low}-{High}";

    public IntRangeValue(int low, int high)
    {
      Low = low;
      High = high;
    }

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
  }

  public sealed class IntRangeStat : NumericStat<IntRangeValue>
  {
    public IntRangeStat(StatId id) : base(id)
    {
    }

    public override IntRangeValue ComputeValue(IReadOnlyList<NumericOperation<IntRangeValue>> operations) =>
      Compute(operations);

    public static IntRangeValue Compute(IReadOnlyList<NumericOperation<IntRangeValue>> operations)
    {
      return new IntRangeValue(
        IntStat.Compute(operations, range => range.Low),
        IntStat.Compute(operations, range => range.High));
    }

    protected override IntRangeValue ParseStatValue(string value) => ParseIntRange(value);

    public static IntRangeValue ParseIntRange(string value)
    {
      var split = value.Split('-');
      try
      {
        return new IntRangeValue(int.Parse(split[0]), int.Parse(split[1]));
      }
      catch (FormatException)
      {
        Debug.LogError($"Error parsing range: {value}");
        throw;
      }
    }
  }
}
