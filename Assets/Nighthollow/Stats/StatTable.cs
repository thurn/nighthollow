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
using Nighthollow.Data;
using Nighthollow.Utils;

#nullable enable

namespace Nighthollow.Stats
{
  public sealed class StatTable
  {
    readonly StatTable? _parent;
    readonly ImmutableDictionary<StatId, ImmutableList<IStatModifier>> _modifiers;
    readonly ImmutableDictionary<StatId, ImmutableList<IStatModifier>> _dynamicModifiers;
    readonly ImmutableDictionary<int, ImmutableList<StatusEffectData>> _statusEffects;

    public StatTable(
      StatTable? parent,
      ImmutableDictionary<StatId, ImmutableList<IStatModifier>>? modifiers = null,
      ImmutableDictionary<StatId, ImmutableList<IStatModifier>>? dynamicModifiers = null,
      ImmutableDictionary<int, ImmutableList<StatusEffectData>>? statusEffects = null)
    {
      _parent = parent;
      _modifiers = modifiers ?? ImmutableDictionary<StatId, ImmutableList<IStatModifier>>.Empty;
      _dynamicModifiers = dynamicModifiers ?? ImmutableDictionary<StatId, ImmutableList<IStatModifier>>.Empty;
      _statusEffects = statusEffects ?? ImmutableDictionary<int, ImmutableList<StatusEffectData>>.Empty;
    }

    [MustUseReturnValue]
    public StatTable InsertStatusEffect(StatusEffectData statusEffect)
    {
      if (_statusEffects[statusEffect.StatusEffectTypeId].Count >= statusEffect.BaseType.MaxStacks)
      {
        // Limit has been exceeded for this status effect, so it is ignored.
        return this;
      }

      return statusEffect.Lifetime == null
        ? new StatTable(
          _parent,
          InsertModifierList(_modifiers, statusEffect.Modifiers),
          _dynamicModifiers,
          _statusEffects.AppendOrCreateList(statusEffect.StatusEffectTypeId, statusEffect))
        : new StatTable(
          _parent,
          _modifiers,
          InsertModifierList(_dynamicModifiers, statusEffect.Modifiers),
          _statusEffects.AppendOrCreateList(statusEffect.StatusEffectTypeId, statusEffect));
    }

    [MustUseReturnValue]
    public StatTable InsertModifier(IStatModifier modifier)
    {
      return modifier.Lifetime == null
        ? new StatTable(
          _parent,
          _modifiers.AppendOrCreateList(modifier.StatId, modifier),
          _dynamicModifiers,
          _statusEffects)
        : new StatTable(
          _parent,
          _modifiers,
          _dynamicModifiers.AppendOrCreateList(modifier.StatId, modifier),
          _statusEffects);
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
    public StatTable OnTick() =>
      _dynamicModifiers.IsEmpty
        ? this
        : new StatTable(_parent,
          _modifiers,
          _dynamicModifiers.ToImmutableDictionary(
            pair => pair.Key,
            pair => pair.Value.RemoveAll(m => m.Lifetime?.IsValid() == false)),
          _statusEffects.ToImmutableDictionary(
            pair => pair.Key,
            pair => pair.Value.RemoveAll(m => m.Lifetime?.IsValid() == false)));

    IEnumerable<IStatModifier> ModifiersForStat(StatId statId)
    {
      return _modifiers
        .GetValueOrDefault(statId, ImmutableList<IStatModifier>.Empty)
        .Concat(_dynamicModifiers.GetValueOrDefault(statId, ImmutableList<IStatModifier>.Empty))
        .Concat(_parent == null ? Enumerable.Empty<IStatModifier>() : _parent.ModifiersForStat(statId));
    }

    ImmutableDictionary<StatId, ImmutableList<IStatModifier>> InsertModifierList(
      ImmutableDictionary<StatId, ImmutableList<IStatModifier>> modifiers,
      ImmutableList<IStatModifier> input)
    {
      var builder = modifiers.ToBuilder();
      foreach (var modifier in input)
      {
        if (!builder.ContainsKey(modifier.StatId))
        {
          builder[modifier.StatId] = ImmutableList<IStatModifier>.Empty;
        }

        builder[modifier.StatId] = builder[modifier.StatId].Add(modifier);
      }

      return builder.ToImmutable();
    }
  }
}