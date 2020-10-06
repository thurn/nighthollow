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

namespace Nighthollow.Stats
{
  public readonly struct DurationStatId : IStatId<DurationStat>
  {
    readonly uint _value;

    public DurationStatId(uint value)
    {
      _value = value;
    }

    public uint Value => _value;

    public DurationStat NotFoundValue() => new DurationStat(new IntStat(0));

    public DurationStat Deserialize(string value) => new DurationStat(new IntStat(int.Parse(value)));
  }

  public sealed class DurationStat : IStat<DurationStat>
  {
    readonly IntStat _stat;

    public float ValueSeconds => _stat.Value / 1000f;

    public DurationStat(IntStat initialValue)
    {
      _stat = initialValue;
    }

    public DurationStat Clone() => new DurationStat(_stat.Clone());
  }
}