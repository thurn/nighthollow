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
using Nighthollow.Data;
using Nighthollow.Utils;

namespace Nighthollow.Stats
{
  public readonly struct TaggedStatsId<TKey, TStat> : IStatId<TaggedStats<TKey, TStat>>
    where TKey : Enum where TStat : IStat<TStat>, IAdditiveStat, new()
  {
    readonly int _value;

    public TaggedStatsId(int value)
    {
      _value = value;
    }

    public int Value => _value;

    public IStat NotFoundValue() => new TaggedStats<TKey, TStat>();
  }

  public interface ITaggedStats
  {
    void AddAddedModifier(IModifier modifier);

    void AddIncreaseModifier(IModifier modifier);
  }

  public sealed class TaggedStats<TKey, TStat> : IStat<TaggedStats<TKey, TStat>>, ITaggedStats
    where TKey : Enum where TStat : IStat<TStat>, IAdditiveStat, new()
  {
    readonly Dictionary<TKey, TStat> _stats;

    public IEnumerable<KeyValuePair<TKey, TStat>> AllEntries => _stats;

    public TaggedStats()
    {
      _stats = new Dictionary<TKey, TStat>();
    }

    TaggedStats(Dictionary<TKey, TStat> stats)
    {
      _stats = stats.ToDictionary(e => e.Key, e => e.Value.Clone());
    }

    public void Add(TKey key, TStat value)
    {
      Errors.CheckArgument(!key.Equals(default(TKey)), $"Cannot add default value {key} as a key");
      Errors.CheckState(!_stats.ContainsKey(key), $"Key {key} already added");
      _stats[key] = value;
    }

    public void AddValue<TValue>(IStatValue list) where TValue : IStatValue
    {
      var input = (TaggedStatListValue<TKey, TValue, TStat>) list;
      foreach (var pair in input.Values)
      {
        Get(pair.Tag).AddValue(pair.Value);
      }
    }

    public TaggedStats<TKey, TStat> Clone() => new TaggedStats<TKey, TStat>(_stats);

    public TStat Get(TKey key)
    {
      if (!_stats.ContainsKey(key))
      {
        _stats[key] = new TStat();
      }

      return _stats[key];
    }

    public void AddAddedModifier(IModifier modifier)
    {
      var taggedValue = (ITaggedStatValue) modifier.GetArgument();
      ModifierUtil.AddAddedModifierUnchecked(
        Get((TKey) taggedValue.GetTag()),
        modifier.WithValue(taggedValue.GetValue()));
    }

    public void AddIncreaseModifier(IModifier modifier)
    {
      var taggedValue = (ITaggedStatValue) modifier.GetArgument();
      ModifierUtil.AddIncreaseModifierUnchecked(
        Get((TKey) taggedValue.GetTag()),
        modifier.WithValue(taggedValue.GetValue()));
    }
  }
}