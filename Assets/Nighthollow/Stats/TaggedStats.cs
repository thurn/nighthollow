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

using System;
using System.Collections.Generic;
using System.Linq;
using Nighthollow.Utils;

namespace Nighthollow.Stats
{
  public readonly struct TaggedStatsId<TKey, TValue> : IStatId<TaggedStats<TKey, TValue>>
    where TKey : Enum where TValue : IStat<TValue>, new()
  {
    readonly uint _value;

    public TaggedStatsId(uint value)
    {
      _value = value;
    }

    public uint Value => _value;

    public TaggedStats<TKey, TValue> NotFoundValue() => new TaggedStats<TKey, TValue>();

    public TaggedStats<TKey, TValue> Deserialize(string value)
    {
      throw new NotImplementedException();
    }
  }

  public sealed class TaggedStats<TKey, TValue> : IStat<TaggedStats<TKey, TValue>>
    where TKey : Enum where TValue : IStat<TValue>, new()
  {
    readonly Dictionary<TKey, TValue> _stats;

    public IEnumerable<KeyValuePair<TKey, TValue>> AllEntries => _stats;

    public TaggedStats()
    {
      _stats = new Dictionary<TKey, TValue>();
    }

    TaggedStats(Dictionary<TKey, TValue> stats)
    {
      _stats = stats.ToDictionary(e => e.Key, e => e.Value.Clone());
    }

    public void Add(TKey key, TValue value)
    {
      Errors.CheckArgument(!key.Equals(default(TKey)), $"Cannot add default value {key} as a key");
      Errors.CheckState(!_stats.ContainsKey(key), $"Key {key} already added");
      _stats[key] = value;
    }

    public TaggedStats<TKey, TValue> Clone() => new TaggedStats<TKey, TValue>(_stats);

    public TValue Get(TKey key)
    {
      if (!_stats.ContainsKey(key))
      {
        _stats[key] = new TValue();
      }

      return _stats[key];
    }
  }
}