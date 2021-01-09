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
using Nighthollow.Utils;

#nullable enable

namespace Nighthollow.Stats2
{
  public sealed class StatTable
  {
    public static readonly StatTable Defaults =
      new StatTable(null, ImmutableDictionary<StatId, ImmutableList<IStatModifier>>.Empty);

    readonly StatTable? _parent;
    readonly ImmutableDictionary<StatId, ImmutableList<IStatModifier>> _modifiers;

    public StatTable(StatTable? parent = null) : this(parent,
      ImmutableDictionary<StatId, ImmutableList<IStatModifier>>.Empty)
    {
    }

    StatTable(StatTable? parent, ImmutableDictionary<StatId, ImmutableList<IStatModifier>> modifiers)
    {
      _parent = parent;
      _modifiers = modifiers;
    }

    public StatTable InsertModifier(IStatModifier modifier) =>
      new StatTable(_parent, _modifiers.AppendOrCreateList(modifier.StatId, modifier));

    public StatTable InsertNullableModifier(IStatModifier? modifier) =>
      modifier == null ? this : InsertModifier(modifier);

    public TValue Get<TModifier, TValue>(AbstractStat<TModifier, TValue> stat)
      where TModifier : IStatModifier where TValue : notnull =>
      stat.ComputeValue(
        ModifiersForStat(stat.StatId)
          .Select(operation => (TModifier) operation)
          .GroupBy(operation => operation.Type)
          .ToDictionary(g => g.Key, g => g.Select(m => m)));

    IEnumerable<IStatModifier> ModifiersForStat(StatId statId)
    {
      if (_modifiers.TryGetValue(statId, out var list))
      {
        return _parent == null ? list : _parent.ModifiersForStat(statId).Concat(list);
      }
      else
      {
        return _parent == null ? ImmutableList<IStatModifier>.Empty : _parent.ModifiersForStat(statId);
      }
    }
  }
}
