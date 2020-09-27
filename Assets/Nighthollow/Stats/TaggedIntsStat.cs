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

using System;
using System.Collections.Generic;
using System.Linq;
using Nighthollow.Utils;

namespace Nighthollow.Stats
{
  public readonly struct TaggedIntsStatId<T> : IStatId<TaggedIntsStat<T>> where T : Enum
  {
    readonly uint _value;

    public TaggedIntsStatId(uint value)
    {
      _value = value;
    }

    public uint Value => _value;

    public TaggedIntsStat<T> NotFoundValue() => new TaggedIntsStat<T>();

    public TaggedIntsStat<T> Deserialize(string value)
    {
      throw new NotImplementedException();
    }
  }

  public sealed class TaggedIntsStat<T> : IStat<TaggedIntsStat<T>> where T : Enum
  {
    readonly Dictionary<T, IntStat> _stats;

    public IEnumerable<KeyValuePair<T, IntStat>> AllEntries => _stats;

    public TaggedIntsStat()
    {
      _stats = new Dictionary<T, IntStat>();
    }

    TaggedIntsStat(Dictionary<T, IntStat> stats)
    {
      _stats = stats.ToDictionary(e => e.Key, e => e.Value.Clone());
    }

    public void Add(T key, int value)
    {
      Errors.CheckArgument(!key.Equals(default(T)), $"Cannot add default value {key} as a key");
      Errors.CheckState(!_stats.ContainsKey(key), $"Key {key} already added");
      _stats[key] = new IntStat(value);
    }

    public TaggedIntsStat<T> Clone() => new TaggedIntsStat<T>(_stats);

    public bool LessThanOrEqualTo(TaggedIntsStat<T> other) =>
      _stats.All(pair => pair.Value.Value <= other._stats[pair.Key].Value);

    public IntStat Get(T key)
    {
      if (!_stats.ContainsKey(key))
      {
        _stats[key] = new IntStat(0);
      }

      return _stats[key];
    }
  }
}