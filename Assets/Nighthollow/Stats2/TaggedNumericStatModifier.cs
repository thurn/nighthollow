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
using System.Collections.Immutable;
using MessagePack;
using Nighthollow.Generated;

#nullable enable

namespace Nighthollow.Stats2
{
  [MessagePackObject]
  public sealed class TaggedNumericStatModifier<TTag, TValue> : IStatModifier where TTag : Enum where TValue : struct
  {
#pragma warning disable 618 // Using Obsolete
    public static TaggedNumericStatModifier<TTag, TValue> Add(
      AbstractStat<TaggedNumericStatModifier<TTag, TValue>, ImmutableDictionary<TTag, TValue>> stat,
      ImmutableDictionary<TTag, TValue> value) =>
      new TaggedNumericStatModifier<TTag, TValue>(stat.StatId, ModifierType.Add, value, null, null);

    public static TaggedNumericStatModifier<TTag, TValue> Increase(
      AbstractStat<TaggedNumericStatModifier<TTag, TValue>, ImmutableDictionary<TTag, TValue>> stat,
      ImmutableDictionary<TTag, PercentageValue> value) =>
      new TaggedNumericStatModifier<TTag, TValue>(stat.StatId, ModifierType.Increase, null, value, null);

    public static TaggedNumericStatModifier<TTag, TValue> Set(
      AbstractStat<TaggedNumericStatModifier<TTag, TValue>, ImmutableDictionary<TTag, TValue>> stat,
      ImmutableDictionary<TTag, TValue> value) =>
      new TaggedNumericStatModifier<TTag, TValue>(stat.StatId, ModifierType.Set, null, null, value);
#pragma warning restore 618

    [Obsolete("This constructor is visible only for use by the serialization system.")]
    public TaggedNumericStatModifier(
      StatId statId,
      ModifierType type,
      ImmutableDictionary<TTag, TValue>? addTo,
      ImmutableDictionary<TTag, PercentageValue>? increaseBy,
      ImmutableDictionary<TTag, TValue>? setTo)
    {
      StatId = statId;
      Type = type;
      AddTo = addTo;
      IncreaseBy = increaseBy;
      SetTo = setTo;
      AllTags = ImmutableHashSet<TTag>.Empty
        .Union(addTo?.Keys.ToImmutableHashSet() ?? ImmutableHashSet<TTag>.Empty)
        .Union(increaseBy?.Keys.ToImmutableHashSet() ?? ImmutableHashSet<TTag>.Empty)
        .Union(setTo?.Keys.ToImmutableHashSet() ?? ImmutableHashSet<TTag>.Empty);
    }

    [Key(0)] public StatId StatId { get; }
    [Key(1)] public ModifierType Type { get; }
    [Key(2)] public ImmutableDictionary<TTag, TValue>? AddTo { get; }
    [Key(3)] public ImmutableDictionary<TTag, PercentageValue>? IncreaseBy { get; }
    [Key(4)] public ImmutableDictionary<TTag, TValue>? SetTo { get; }
    [IgnoreMember] public ImmutableHashSet<TTag> AllTags { get; }

    public NumericStatModifier<TValue>? AsNumericOperation(
      AbstractStat<NumericStatModifier<TValue>, TValue> stat, TTag tag)
    {
      if (!AllTags.Contains(tag))
      {
        return null;
      }

      return Type switch
      {
        ModifierType.Add => NumericStatModifier<TValue>.Add(stat, AddTo![tag]),
        ModifierType.Increase => NumericStatModifier<TValue>.Increase(stat, IncreaseBy![tag]),
        ModifierType.Set => NumericStatModifier<TValue>.Set(stat, SetTo![tag]),
        _ => throw new ArgumentOutOfRangeException()
      };
    }
  }
}