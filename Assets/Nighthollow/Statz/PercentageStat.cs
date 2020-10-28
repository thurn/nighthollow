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
using UnityEngine;

namespace Nighthollow.Statz
{
  public readonly struct PercentageValue : IStatValue
  {
    const float BasisPoints = 10_000f;
    readonly int _basisPoints;

    public PercentageValue(int basisPoints)
    {
      _basisPoints = basisPoints;
    }

    public int AsBasisPoints() => _basisPoints;

    public float AsMultiplier() => _basisPoints / BasisPoints;

    public int CalculateFraction(int input) =>
      Mathf.RoundToInt((input * _basisPoints) / BasisPoints);
  }

  public sealed class PercentageStat : AbstractStat<NumericOperation<PercentageValue>, PercentageValue>
  {
    public PercentageStat(int id) : base(id)
    {
    }

    public override PercentageValue DefaultValue() => new PercentageValue(0);

    public override PercentageValue ComputeValue(IReadOnlyList<NumericOperation<PercentageValue>> operations) =>
      new PercentageValue(IntStat.Compute(operations, duration => new IntValue(duration.AsBasisPoints())).Int);

    protected override PercentageValue ParseStatValue(string value) =>
      new PercentageValue(Mathf.RoundToInt(float.Parse(value.Replace("%", "")) * 100f));
  }
}