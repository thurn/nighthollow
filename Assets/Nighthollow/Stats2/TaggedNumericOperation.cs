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

#nullable enable

namespace Nighthollow.Stats2
{
  [MessagePackObject]
  public sealed class TaggedNumericOperation<TTag, TValue> : IOperation where TTag : Enum where TValue : struct
  {
#pragma warning disable 618 // Using Obsolete
    public static TaggedNumericOperation<TTag, TValue> Add(ImmutableDictionary<TTag, TValue> value) =>
      new TaggedNumericOperation<TTag, TValue>(OperationType.Add, value, null, null);

    public static TaggedNumericOperation<TTag, TValue> Increase(ImmutableDictionary<TTag, PercentageValue> value) =>
      new TaggedNumericOperation<TTag, TValue>(OperationType.Increase, null, value, null);

    public static TaggedNumericOperation<TTag, TValue> Overwrite(ImmutableDictionary<TTag, TValue> value) =>
      new TaggedNumericOperation<TTag, TValue>(OperationType.Set, null, null, value);
#pragma warning restore 618

    [Obsolete("This constructor is visible only for use by the serialization system.")]
    public TaggedNumericOperation(
      OperationType type,
      ImmutableDictionary<TTag, TValue>? addTo,
      ImmutableDictionary<TTag, PercentageValue>? increaseBy,
      ImmutableDictionary<TTag, TValue>? overwriteWith)
    {
      Type = type;
      AddTo = addTo;
      IncreaseBy = increaseBy;
      OverwriteWith = overwriteWith;
      AllTags = ImmutableHashSet<TTag>.Empty
        .Union(addTo?.Keys.ToImmutableHashSet() ?? ImmutableHashSet<TTag>.Empty)
        .Union(increaseBy?.Keys.ToImmutableHashSet() ?? ImmutableHashSet<TTag>.Empty)
        .Union(overwriteWith?.Keys.ToImmutableHashSet() ?? ImmutableHashSet<TTag>.Empty);
    }

    [Key(0)] public OperationType Type { get; }
    [Key(1)] public ImmutableDictionary<TTag, TValue>? AddTo { get; }
    [Key(2)] public ImmutableDictionary<TTag, PercentageValue>? IncreaseBy { get; }
    [Key(3)] public ImmutableDictionary<TTag, TValue>? OverwriteWith { get; }
    [IgnoreMember] public ImmutableHashSet<TTag> AllTags { get; }

    public NumericOperation<TValue>? AsNumericOperation(TTag tag)
    {
      if (!AllTags.Contains(tag))
      {
        return null;
      }

      return Type switch
      {
        OperationType.Add => NumericOperation<TValue>.Add(AddTo![tag]),
        OperationType.Increase => NumericOperation<TValue>.Increase(IncreaseBy![tag]),
        OperationType.Set => NumericOperation<TValue>.Overwrite(OverwriteWith![tag]),
        _ => throw new ArgumentOutOfRangeException()
      };
    }
  }
}