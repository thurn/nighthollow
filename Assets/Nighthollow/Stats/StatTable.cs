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

using System.Collections.Generic;
using System.Linq;
using Nighthollow.Utils;

namespace Nighthollow.Stats
{
  public sealed class StatTable
  {
    readonly Dictionary<int, IStat> _stats;

    public StatTable(Builder builder)
    {
      _stats = builder.Stats;
    }

    public StatTable(StatTable other)
    {
      _stats = other._stats.ToDictionary(e => e.Key, e => e.Value);
    }

    public T Get<T>(IStatId<T> statId) where T : IStat
    {
      if (!_stats.ContainsKey(statId.Value))
      {
        _stats[statId.Value] = statId.NotFoundValue();
      }

      return (T) _stats[statId.Value];
    }

    public StatTable Clone() => new StatTable(this);

    public sealed class Builder
    {
      readonly Dictionary<int, IStat> _stats = new Dictionary<int, IStat>();

      public Dictionary<int, IStat> Stats => _stats;

      public void AddStat<T>(IStatId<T> statId, T value) where T : IStat
      {
        Errors.CheckState(!_stats.ContainsKey(statId.Value), $"Stat {statId.Value} already added!");
        _stats[statId.Value] = value;
      }
    }
  }
}