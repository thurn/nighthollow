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

namespace Nighthollow.Stats2
{
  public abstract class StatEntity
  {
    public abstract StatTable Stats { get; }

    public TValue Get<TModifier, TValue>(AbstractStat<TModifier, TValue> stat) where TModifier : IStatModifier =>
      Stats.Get(stat);

    public int GetInt(IntStat statId) => Stats.Get(statId);

    public bool GetBool(BoolStat statId) => Stats.Get(statId);

    public int GetDurationMilliseconds(DurationStat statId) => Stats.Get(statId).AsMilliseconds();

    public float GetDurationSeconds(DurationStat statId) => Stats.Get(statId).AsSeconds();
  }

  public sealed class StatContainer : StatEntity
  {
    public StatContainer(StatTable? stats = null)
    {
      Stats = stats ?? new StatTable();
    }

    public override StatTable Stats { get; }

    public StatContainer Insert(IStatModifier? modifier) =>
      new StatContainer(Stats.InsertNullableModifier(modifier));
  }
}
