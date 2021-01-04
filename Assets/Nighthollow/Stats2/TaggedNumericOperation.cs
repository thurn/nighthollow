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

#nullable enable

namespace Nighthollow.Stats2
{
  public sealed class TaggedNumericOperation<TTag, TValue> : IOperation where TTag: Enum where TValue: struct
  {
    public static TaggedNumericOperation<TTag, TValue> Add(TaggedValues<TTag, TValue> value) =>
      new TaggedNumericOperation<TTag, TValue>(OperationType.Add, value, null, null);

    public static TaggedNumericOperation<TTag, TValue> Increase(TaggedValues<TTag, PercentageValue> value) =>
      new TaggedNumericOperation<TTag, TValue>(OperationType.Increase, null, value, null);

    public static TaggedNumericOperation<TTag, TValue> Overwrite(TaggedValues<TTag, TValue> value) =>
      new TaggedNumericOperation<TTag, TValue>(OperationType.Overwrite, null, null, value);

    TaggedNumericOperation(
      OperationType type,
      TaggedValues<TTag, TValue>? addTo,
      TaggedValues<TTag, PercentageValue>? increaseBy,
      TaggedValues<TTag, TValue>? overwriteWith)
    {
      Type = type;
      AddTo = addTo;
      IncreaseBy = increaseBy;
      OverwriteWith = overwriteWith;
      AllTags = ImmutableHashSet<TTag>.Empty
        .Union(addTo?.Values.Keys.ToImmutableHashSet() ?? ImmutableHashSet<TTag>.Empty)
        .Union(increaseBy?.Values.Keys.ToImmutableHashSet() ?? ImmutableHashSet<TTag>.Empty)
        .Union(overwriteWith?.Values.Keys.ToImmutableHashSet() ?? ImmutableHashSet<TTag>.Empty);
    }

    public OperationType Type { get; }
    public ImmutableHashSet<TTag> AllTags { get; }
    public TaggedValues<TTag, TValue>? AddTo { get; }
    public TaggedValues<TTag, PercentageValue>? IncreaseBy { get; }
    public TaggedValues<TTag, TValue>? OverwriteWith { get; }

    public NumericOperation<TValue>? AsNumericOperation(TTag tag)
    {
      if (!AllTags.Contains(tag))
      {
        return null;
      }

      return Type switch
      {
        OperationType.Add => NumericOperation<TValue>.Add(AddTo!.Values[tag]),
        OperationType.Increase => NumericOperation<TValue>.Increase(IncreaseBy!.Values[tag]),
        OperationType.Overwrite => NumericOperation<TValue>.Overwrite(OverwriteWith!.Values[tag]),
        _ => throw new ArgumentOutOfRangeException()
      };
    }
  }
}