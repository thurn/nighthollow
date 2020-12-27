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
using MessagePack;
using Nighthollow.Data;
using Nighthollow.Generated;
using UnityEngine;

#nullable enable

namespace Nighthollow.Stats
{
  [MessagePackObject]
  public readonly struct DurationValue : IValueData
  {
    public DurationValue(int timeMilliseconds)
    {
      TimeMilliseconds = timeMilliseconds;
    }

    [Key(0)] public int TimeMilliseconds { get; }

    public T Switch<T>(
      Func<int, T> onInt,
      Func<bool, T> onBool,
      Func<DurationValue, T> onDuration,
      Func<PercentageValue, T> onPercentage,
      Func<IntRangeValue, T> onIntRange) => onDuration(this);

    public override string ToString() => $"{TimeMilliseconds / 1000f}s";

    public float AsSeconds() => TimeMilliseconds / 1000f;

    public int AsMilliseconds() => TimeMilliseconds;

    public bool Equals(DurationValue other) => TimeMilliseconds == other.TimeMilliseconds;

    public override bool Equals(object? obj) => obj is DurationValue other && Equals(other);

    public override int GetHashCode() => TimeMilliseconds;

    public static bool operator ==(DurationValue left, DurationValue right) => left.Equals(right);

    public static bool operator !=(DurationValue left, DurationValue right) => !left.Equals(right);
  }

  public sealed class DurationStat : NumericStat<DurationValue>
  {
    public DurationStat(StatId id) : base(id)
    {
    }

    public override DurationValue ComputeValue(IReadOnlyList<NumericOperation<DurationValue>> operations)
    {
      return new DurationValue(IntStat.Compute(operations, duration => duration.AsMilliseconds()));
    }

    protected override DurationValue ParseStatValue(string value) => ParseDuration(value);

    public static DurationValue ParseDuration(string value) =>
      new DurationValue(Mathf.RoundToInt(float.Parse(value.Replace("s", "")) * 1000f));
  }
}
