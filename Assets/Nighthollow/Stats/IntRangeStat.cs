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

#nullable enable

namespace Nighthollow.Stats
{
  public readonly struct IntRangeStatId : IStatId<IntRangeStat>
  {
    readonly int _value;

    public IntRangeStatId(int value)
    {
      _value = value;
    }

    public int Value => _value;

    public IStat NotFoundValue() => new IntRangeStat(new IntStat(), new IntStat());
  }

  public sealed class IntRangeStat : IStat<IntRangeStat>, IAdditiveStat
  {
    readonly IntStat _low;
    readonly IntStat _high;

    public IntRangeStat()
    {
      _low = new IntStat();
      _high = new IntStat();
    }

    public IntRangeStat(IntStat low, IntStat high)
    {
      _low = low;
      _high = high;
    }

    public int LowValue => _low.Value;

    public int HighValue => _high.Value;

    public IntRangeStat Clone() => new IntRangeStat(_low.Clone(), _high.Clone());

    public void AddAddedModifier(IModifier modifier)
    {
      _low.AddAddedModifier(modifier.WithValue(((IntRangeValue) modifier.BaseModifier.Argument).Low));
      _high.AddAddedModifier(modifier.WithValue(((IntRangeValue) modifier.BaseModifier.Argument).High));
    }

    public void AddIncreaseModifier(IModifier modifier)
    {
      _low.AddIncreaseModifier(modifier);
      _high.AddIncreaseModifier(modifier);
    }

    public void AddValue(IStatValue value)
    {
      var range = (IntRangeValue) value;
      _low.Add(range.Low.Value);
      _high.Add(range.High.Value);
    }
  }
}