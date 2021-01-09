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
  public readonly struct PercentageValue : IValueData, IIsNegative
  {
    public PercentageValue(int basisPoints)
    {
      BasisPoints = basisPoints;
    }

    const float BasisPointsPerUnit = 10_000f;

    [Key(0)] public int BasisPoints { get; }

    public object Get() => this;

    public override string ToString() => $"{BasisPoints / 100f}%";

    public int AsBasisPoints() => BasisPoints;

    public float AsMultiplier() => BasisPoints / BasisPointsPerUnit;

    public int CalculateFraction(int input) => Mathf.RoundToInt(input * BasisPoints / BasisPointsPerUnit);

    public bool IsNegative() => BasisPoints < 0;

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

    public static bool TryParseValue(string input, out IValueData result)
    {
      if (TryParse(input, out var percentage))
      {
        result = percentage;
        return true;
      }

      result = null!;
      return false;
    }
  }

  public sealed class PercentageStat : NumericStat<PercentageValue>
  {
    public PercentageStat(StatId statId) : base(statId)
    {
    }

    public override PercentageValue ComputeValue(
      IReadOnlyDictionary<ModifierType, IEnumerable<NumericStatModifier<PercentageValue>>> groups) =>
      new PercentageValue(IntStat.Compute(groups, duration => duration.AsBasisPoints()));

    protected override bool TryParseValue(string input, out IValueData result) =>
      PercentageValue.TryParseValue(input, out result);
  }
}
