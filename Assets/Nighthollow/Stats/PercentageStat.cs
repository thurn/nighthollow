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

using UnityEngine;

namespace Nighthollow.Stats
{
  public readonly struct PercentageStatId : IStatId<PercentageStat>
  {
    readonly uint _value;

    public PercentageStatId(uint value)
    {
      _value = value;
    }

    public uint Value => _value;

    public PercentageStat NotFoundValue() => new PercentageStat(new IntStat(0));

    public PercentageStat Deserialize(string value) => new PercentageStat(new IntStat(int.Parse(value)));
  }

  public sealed class PercentageStat : IStat<PercentageStat>
  {
    const float BasisPoints = 10_000f;
    readonly IntStat _stat;

    public PercentageStat(IntStat initialValue)
    {
      _stat = initialValue;
    }

    public float AsMultplier() => _stat.Value / BasisPoints;

    public int CalculateFraction(int input) =>
      Mathf.RoundToInt((input * _stat.Value) / BasisPoints);

    public PercentageStat Clone() => new PercentageStat(_stat.Clone());
  }
}