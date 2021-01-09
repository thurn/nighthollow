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
using UnityEngine;

#nullable enable

namespace Nighthollow.Stats2
{
  [MessagePackObject]
  public readonly struct DurationValue : IValueData, IIsNegative
  {
    public DurationValue(int timeMilliseconds)
    {
      TimeMilliseconds = timeMilliseconds;
    }

    [Key(0)] public int TimeMilliseconds { get; }

    public object Get() => this;

    public override string ToString() => $"{TimeMilliseconds / 1000f}s";

    public bool IsNegative() => false;

    public float AsSeconds() => TimeMilliseconds / 1000f;

    public int AsMilliseconds() => TimeMilliseconds;

    public bool Equals(DurationValue other) => TimeMilliseconds == other.TimeMilliseconds;

    public override bool Equals(object? obj) => obj is DurationValue other && Equals(other);

    public override int GetHashCode() => TimeMilliseconds;

    public static bool operator ==(DurationValue left, DurationValue right) => left.Equals(right);

    public static bool operator !=(DurationValue left, DurationValue right) => !left.Equals(right);

    public static bool TryParse(string value, out DurationValue result)
    {
      if (float.TryParse(value.Replace("s", ""), out var f))
      {
        result = new DurationValue(Mathf.RoundToInt(f * 1000f));
        return true;
      }

      result = default;
      return false;
    }
  }

  public sealed class DurationStat : NumericStat<DurationValue>
  {
    public DurationStat(StatId statId) : base(statId)
    {
    }

    public override DurationValue ComputeValue(
      IReadOnlyDictionary<ModifierType, IEnumerable<NumericStatModifier<DurationValue>>> groups) =>
      new DurationValue(IntStat.Compute(groups, duration => duration.AsMilliseconds()));

    protected override bool TryParseValue(string input, out IValueData result)
    {
      if (DurationValue.TryParse(input, out var duration))
      {
        result = duration;
        return true;
      }

      result = null!;
      return false;
    }
  }
}
