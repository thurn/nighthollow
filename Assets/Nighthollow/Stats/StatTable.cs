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

namespace Nighthollow.Stats
{
  public sealed class StatTable
  {
    readonly StatTable? _parent;
    readonly ImmutableDictionary<StatId, ImmutableList<IStatModifier>> _modifiers;
    readonly ImmutableDictionary<StatId, ImmutableList<IStatModifier>> _dynamicModifiers;

    public StatTable(
      StatTable? parent,
      ImmutableDictionary<StatId, ImmutableList<IStatModifier>>? modifiers = null,
      ImmutableDictionary<StatId, ImmutableList<IStatModifier>>? dynamicModifiers = null)
    {
      _parent = parent;
      _modifiers = modifiers ?? ImmutableDictionary<StatId, ImmutableList<IStatModifier>>.Empty;
      _dynamicModifiers = dynamicModifiers ?? ImmutableDictionary<StatId, ImmutableList<IStatModifier>>.Empty;
    }

    [MustUseReturnValue]
    public StatTable InsertModifier(IStatModifier modifier) =>
      modifier.Lifetime == null
        ? new StatTable(_parent, _modifiers.AppendOrCreateList(modifier.StatId, modifier), _dynamicModifiers)
        : new StatTable(_parent, _modifiers, _dynamicModifiers.AppendOrCreateList(modifier.StatId, modifier));

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
    public StatTable OnTick() => _dynamicModifiers.IsEmpty
      ? this
      : new StatTable(_parent,
        _modifiers,
        _dynamicModifiers.ToImmutableDictionary(
          pair => pair.Key,
          pair => pair.Value.RemoveAll(m => m.Lifetime?.IsValid() == false)));

    IEnumerable<IStatModifier> ModifiersForStat(StatId statId)
    {
      return _modifiers
        .GetValueOrDefault(statId, ImmutableList<IStatModifier>.Empty)
        .Concat(_dynamicModifiers.GetValueOrDefault(statId, ImmutableList<IStatModifier>.Empty))
        .Concat(_parent == null ? Enumerable.Empty<IStatModifier>() : _parent.ModifiersForStat(statId));
    }
  }
}