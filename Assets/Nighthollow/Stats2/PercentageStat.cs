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
using System.Linq;
using MessagePack;
using Nighthollow.Data;
using Nighthollow.Generated;
using UnityEngine;

#nullable enable

namespace Nighthollow.Stats2
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

    public static bool TryParse(string value, out PercentageValue result)
    {
      if (float.TryParse(value.Replace("%", ""), out var f))
      {
        result = new PercentageValue(Mathf.RoundToInt(f * 100f));
        return true;
      }

      result = default;
      return false;
    }
  }

  public sealed class PercentageStat : AbstractStat<NumericOperation<PercentageValue>, PercentageValue>
  {
    public PercentageStat(StatId statId) : base(statId)
    {
    }

    public override PercentageValue ComputeValue(
      IReadOnlyDictionary<OperationType, IEnumerable<NumericOperation<PercentageValue>>> groups) =>
      new PercentageValue(IntStat.Compute(groups, duration => duration.AsBasisPoints()));
  }
}