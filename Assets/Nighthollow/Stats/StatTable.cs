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
using System.Collections.Immutable;
using System.Linq;
using JetBrains.Annotations;

#nullable enable

namespace Nighthollow.Stats
{
  public sealed class StatTable
  {
    readonly StatTable? _parent;
    readonly IDictionary<StatId, List<IStatModifier>> _modifiers;

    public StatTable(
      StatTable? parent,
      IDictionary<StatId, List<IStatModifier>>? modifiers = null)
    {
      _parent = parent;
      _modifiers = modifiers ?? ImmutableDictionary<StatId, List<IStatModifier>>.Empty;
    }

    [MustUseReturnValue]
    public StatTable InsertModifier(IStatModifier modifier)
    {
      var modifiers = _modifiers.ToDictionary(p => p.Key, p => p.Value.ToList());
      if (!modifiers.ContainsKey(modifier.StatId))
      {
        modifiers[modifier.StatId] = new List<IStatModifier>();
      }

      modifiers[modifier.StatId].Add(modifier);
      return new StatTable(_parent, modifiers);
    }

    [MustUseReturnValue]
    public StatTable InsertNullableModifier(IStatModifier? modifier) =>
      modifier == null ? this : InsertModifier(modifier);

    public TValue Get<TModifier, TValue>(AbstractStat<TModifier, TValue> stat)
      where TModifier : IStatModifier where TValue : notnull =>
      stat.ComputeValue(
        ModifiersForStat(stat.StatId)
          .Select(operation => (TModifier) operation)
          .GroupBy(operation => operation.Type)
          .ToDictionary(g => g.Key, g => g.Select(m => m)));

    [MustUseReturnValue]
    public StatTable OnTick()
    {
      return new StatTable(_parent,
        _modifiers.ToDictionary(
          pair => pair.Key,
          pair => pair.Value.Where(m => m.Lifetime == null || m.Lifetime.IsValid()).ToList()));
    }

    IEnumerable<IStatModifier> ModifiersForStat(StatId statId)
    {
      if (_modifiers.TryGetValue(statId, out var list))
      {
        return _parent == null ? list : _parent.ModifiersForStat(statId).Concat(list);
      }
      else
      {
        return _parent == null ? new List<IStatModifier>() : _parent.ModifiersForStat(statId);
      }
    }
  }
}
