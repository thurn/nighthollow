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
    readonly uint _value;

    public IntRangeStatId(uint value)
    {
      _value = value;
    }

    public uint Value => _value;

    public IntRangeStat NotFoundValue() => new IntRangeStat(new IntStat(0), new IntStat(0));

    public IntRangeStat Deserialize(string value)
    {
      var split = value.Split(',');
      return new IntRangeStat(new IntStat(int.Parse(split[0])), new IntStat(int.Parse(split[1])));
    }
  }

  public sealed class IntRangeStat : IStat<IntRangeStat>
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

    public IntRangeStat Clone() => new IntRangeStat(_low.Clone(), _high.Clone());
  }
}