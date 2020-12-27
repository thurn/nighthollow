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
  public readonly struct PercentageValue : IValueData
  {
    public PercentageValue(int basisPoints)
    {
      BasisPoints = basisPoints;
    }

    const float BasisPointsPerUnit = 10_000f;

    [Key(0)] public int BasisPoints { get; }

    public T Switch<T>(
      Func<int, T> onInt,
      Func<bool, T> onBool,
      Func<DurationValue, T> onDuration,
      Func<PercentageValue, T> onPercentage,
      Func<IntRangeValue, T> onIntRange) => onPercentage(this);

    public override string ToString() => $"{BasisPoints / 100f}%";

    public int AsBasisPoints() => BasisPoints;

    public float AsMultiplier() => BasisPoints / BasisPointsPerUnit;

    public int CalculateFraction(int input) => Mathf.RoundToInt(input * BasisPoints / BasisPointsPerUnit);

    public bool IsReduction() => BasisPoints < BasisPointsPerUnit;

    public bool Equals(PercentageValue other) => BasisPoints == other.BasisPoints;

    public override bool Equals(object obj) => obj is PercentageValue other && Equals(other);

    public override int GetHashCode() => BasisPoints;

    public static bool operator ==(PercentageValue left, PercentageValue right) => left.Equals(right);

    public static bool operator !=(PercentageValue left, PercentageValue right) => !left.Equals(right);
  }

  public sealed class PercentageStat : NumericStat<PercentageValue>
  {
    public PercentageStat(StatId id) : base(id)
    {
    }

    public override PercentageValue ComputeValue(IReadOnlyList<NumericOperation<PercentageValue>> operations) =>
      Compute(operations);

    public static PercentageValue Compute(IReadOnlyList<NumericOperation<PercentageValue>> operations)
    {
      return new PercentageValue(IntStat.Compute(operations, duration => duration.AsBasisPoints()));
    }

    protected override PercentageValue ParseStatValue(string value) => ParsePercentage(value);

    public static PercentageValue ParsePercentage(string value) =>
      new PercentageValue(Mathf.RoundToInt(float.Parse(value.Replace("%", "")) * 100f));
  }
}
