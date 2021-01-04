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
using Nighthollow.Generated;
using Nighthollow.Utils;

#nullable enable

namespace Nighthollow.Stats2
{
  public interface IStatTableBuilder
  {
    IStatTableBuilder InsertModifier(IStatModifier modifier);

    StatTable Build(StatTable parent);
  }

  public sealed class StatTable : IStatTableBuilder
  {
    readonly StatTable? _parent;
    readonly ImmutableDictionary<StatId, ImmutableList<IStatModifier>> _modifiers;

    StatTable(StatTable? parent, ImmutableDictionary<StatId, ImmutableList<IStatModifier>> modifiers)
    {
      _parent = parent;
      _modifiers = modifiers;
    }

    IStatTableBuilder IStatTableBuilder.InsertModifier(IStatModifier modifier) => InsertModifier(modifier);

    public StatTable InsertModifier(IStatModifier modifier) =>
      new StatTable(_parent, _modifiers.AppendOrCreateList(modifier.Stat.StatId, modifier));

    public TValue Get<TOperation, TValue>(AbstractStat<TOperation, TValue> stat) where TOperation : IOperation =>
      stat.ComputeValue(
        ModifiersForStat(stat.StatId)
          .Select(modifier => (TOperation) modifier.Operation)
          .GroupBy(op => op.Type)
          .ToDictionary(g => g.Key, g => g.Select(o => o)));

    public StatTable Build(StatTable parent) => new StatTable(parent, _modifiers);

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