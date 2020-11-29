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
using Nighthollow.Generated;
using UnityEngine;

#nullable enable

namespace Nighthollow.Stats
{
  public readonly struct PercentageValue
  {
    const float BasisPoints = 10_000f;
    readonly int _basisPoints;

    public PercentageValue(int basisPoints)
    {
      _basisPoints = basisPoints;
    }

    public override string ToString()
    {
      return $"{_basisPoints / 100f}%";
    }

    public int AsBasisPoints()
    {
      return _basisPoints;
    }

    public float AsMultiplier()
    {
      return _basisPoints / BasisPoints;
    }

    public int CalculateFraction(int input)
    {
      return Mathf.RoundToInt(input * _basisPoints / BasisPoints);
    }

    public bool IsReduction()
    {
      return _basisPoints < BasisPoints;
    }

    public bool Equals(PercentageValue other)
    {
      return _basisPoints == other._basisPoints;
    }

    public override bool Equals(object obj)
    {
      return obj is PercentageValue other && Equals(other);
    }

    public override int GetHashCode()
    {
      return _basisPoints;
    }

    public static bool operator ==(PercentageValue left, PercentageValue right)
    {
      return left.Equals(right);
    }

    public static bool operator !=(PercentageValue left, PercentageValue right)
    {
      return !left.Equals(right);
    }
  }

  public sealed class PercentageStat : NumericStat<PercentageValue>
  {
    public PercentageStat(StatId id) : base(id)
    {
    }

    public override PercentageValue ComputeValue(IReadOnlyList<NumericOperation<PercentageValue>> operations)
    {
      return Compute(operations);
    }

    public static PercentageValue Compute(IReadOnlyList<NumericOperation<PercentageValue>> operations)
    {
      return new PercentageValue(IntStat.Compute(operations, duration => duration.AsBasisPoints()));
    }

    protected override PercentageValue ParseStatValue(string value)
    {
      return ParsePercentage(value);
    }

    public static PercentageValue ParsePercentage(string value)
    {
      return new PercentageValue(Mathf.RoundToInt(float.Parse(value.Replace("%", "")) * 100f));
    }
  }
}
