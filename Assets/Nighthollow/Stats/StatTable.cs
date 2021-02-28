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
using Nighthollow.Services;
using Nighthollow.Utils;

#nullable enable

namespace Nighthollow.Stats
{
  // Wrap modifiers in container object which implements IStatModifier
  // Parent class for all modifiers which handles tag & lifetimes wrapping
  // Use this wrapper pattern for lifetime too
  // Tag them with their associated status effect ID if any
  // On max stacks reached, for each modifier in the effect, remove the oldest modifier from the list
  // Remove the first status effect from the effect list

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

    public ImmutableList<(StatusEffectTypeData, int)> StatusEffects =>
      _statusEffects.Values.Where(v => !v.IsEmpty).Select(v => (v.First().BaseType, v.Count)).ToImmutableList();

    [MustUseReturnValue]
    public StatTable InsertStatusEffect(StatusEffectData statusEffect)
    {
      int? removeFirstMatchingId = null;
      // If the "Max Stacks" value would be exceeded for this status effect, we remove the oldest instance of the effect
      // before inserting a new instance.
      var removeOldest = _statusEffects.TryGetValue(statusEffect.StatusEffectTypeId, out var effects) &&
                         effects.Count >= statusEffect.BaseType.MaxStacks;

      var statusEffects = _statusEffects;
      if (removeOldest && statusEffects.ContainsKey(statusEffect.StatusEffectTypeId))
      {
        var list = statusEffects[statusEffect.StatusEffectTypeId];
        removeFirstMatchingId = statusEffect.StatusEffectTypeId;
        statusEffects = list.IsEmpty
          ? statusEffects
          : statusEffects.SetItem(statusEffect.StatusEffectTypeId, list.RemoveAt(0));
      }

      statusEffects = statusEffects.AppendOrCreateList(statusEffect.StatusEffectTypeId, statusEffect);

      return statusEffect.Lifetime == null
        ? new StatTable(
          _parent,
          UpdateModifierList(_modifiers, statusEffect.Modifiers, removeFirstMatchingId),
          _dynamicModifiers,
          statusEffects)
        : new StatTable(
          _parent,
          _modifiers,
          UpdateModifierList(_dynamicModifiers, statusEffect.Modifiers, removeFirstMatchingId),
          statusEffects);
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
          .Select(modifier => (TModifier) modifier.Unwrap())
          .GroupBy(modifier => modifier.Type)
          .ToDictionary(g => g.Key, g => g.Select(m => m)));

    [MustUseReturnValue]
    public StatTable OnTick(GameContext c) =>
      _dynamicModifiers.IsEmpty
        ? this
        : new StatTable(_parent,
          _modifiers,
          _dynamicModifiers.ToImmutableDictionary(
            pair => pair.Key,
            pair => pair.Value.RemoveAll(m => m.Lifetime?.IsValid(c) == false)),
          _statusEffects.ToImmutableDictionary(
            pair => pair.Key,
            pair => pair.Value.RemoveAll(m => m.Lifetime?.IsValid(c) == false)));

    IEnumerable<IStatModifier> ModifiersForStat(StatId statId)
    {
      return _modifiers
        .GetValueOrDefault(statId, ImmutableList<IStatModifier>.Empty)
        .Concat(_dynamicModifiers.GetValueOrDefault(statId, ImmutableList<IStatModifier>.Empty))
        .Concat(_parent == null ? Enumerable.Empty<IStatModifier>() : _parent.ModifiersForStat(statId));
    }

    ImmutableDictionary<StatId, ImmutableList<IStatModifier>> UpdateModifierList(
      ImmutableDictionary<StatId, ImmutableList<IStatModifier>> modifiers,
      ImmutableList<IStatModifier> input,
      int? removeStatusEffectId)
    {
      var builder = modifiers.ToBuilder();
      foreach (var modifier in input)
      {
        if (!builder.ContainsKey(modifier.StatId))
        {
          builder[modifier.StatId] = ImmutableList<IStatModifier>.Empty;
        }

        if (removeStatusEffectId != null)
        {
          var index = builder[modifier.StatId].FindIndex(m => m.StatusEffectTypeId == removeStatusEffectId.Value);
          if (index != -1)
          {
            builder[modifier.StatId] = builder[modifier.StatId].RemoveAt(index);
          }
        }

        builder[modifier.StatId] = builder[modifier.StatId].Add(modifier);
      }

      return builder.ToImmutable();
    }
  }
}