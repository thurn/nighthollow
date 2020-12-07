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
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Nighthollow.Generated;

#nullable enable

namespace Nighthollow.Stats
{
  public static class TaggedNumericOperation
  {
    public static TaggedNumericOperation<TTag, TValue> Add<TTag, TValue>(TTag tag, TValue value)
      where TTag : struct, Enum where TValue : struct =>
      new TaggedNumericOperation<TTag, TValue>(
        new TaggedValues<TTag, TValue>(new Dictionary<TTag, TValue> {{tag, value}}),
        increaseBy: null,
        overwrite: null);

    public static TaggedNumericOperation<TTag, TValue> Add<TTag, TValue>(TaggedValues<TTag, TValue> values)
      where TTag : struct, Enum where TValue : struct =>
      new TaggedNumericOperation<TTag, TValue>(values, increaseBy: null, overwrite: null);

    public static TaggedNumericOperation<TTag, TValue> Increase<TTag, TValue>(TTag tag, PercentageValue value)
      where TTag : struct, Enum where TValue : struct =>
      new TaggedNumericOperation<TTag, TValue>(
        addTo: null,
        new TaggedValues<TTag, PercentageValue>(new Dictionary<TTag, PercentageValue> {{tag, value}}),
        overwrite: null);

    public static TaggedNumericOperation<TTag, TValue> Increase<TTag, TValue>(
      TaggedValues<TTag, PercentageValue> values) where TTag : struct, Enum where TValue : struct =>
      new TaggedNumericOperation<TTag, TValue>(addTo: null, values, overwrite: null);

    public static TaggedNumericOperation<TTag, TValue> Overwrite<TTag, TValue>(TaggedValues<TTag, TValue> values)
      where TTag : struct, Enum where TValue : struct =>
      new TaggedNumericOperation<TTag, TValue>(addTo: null, increaseBy: null, values);
  }

  public sealed class TaggedNumericOperation<TTag, TValue> : IOperation
    where TTag : struct, Enum where TValue : struct
  {
    public TaggedNumericOperation(
      TaggedValues<TTag, TValue>? addTo,
      TaggedValues<TTag, PercentageValue>? increaseBy,
      TaggedValues<TTag, TValue>? overwrite)
    {
      AddTo = addTo;
      IncreaseBy = increaseBy;
      Overwrite = overwrite;
      var tags = new HashSet<TTag>();

      tags.UnionWith(addTo?.Values.Keys ?? Enumerable.Empty<TTag>());
      tags.UnionWith(increaseBy?.Values.Keys ?? Enumerable.Empty<TTag>());
      tags.UnionWith(overwrite?.Values.Keys ?? Enumerable.Empty<TTag>());

      Tags = tags;
    }

    public ISet<TTag> Tags { get; }
    public TaggedValues<TTag, TValue>? AddTo { get; }
    public TaggedValues<TTag, PercentageValue>? IncreaseBy { get; }
    public TaggedValues<TTag, TValue>? Overwrite { get; }

    public string Describe(string statDescription)
    {
      var result = new StringBuilder();
      var appended = false;
      foreach (var tag in Tags)
      {
        if (appended)
        {
          result.Append("\n");
        }

        var operation = ToNumericOperation(tag);
        if (operation != null)
        {
          result.Append(operation.Describe(statDescription, tag.ToString()));
          appended = true;
        }
      }

      return result.ToString();
    }

    public SerializedOperation Serialize()
    {
      if (AddTo != null)
      {
        return new SerializedOperation(AddTo.ToString(), Operator.Add);
      }
      else if (IncreaseBy != null)
      {
        return new SerializedOperation(IncreaseBy.ToString(), Operator.Increase);
      }
      else if (Overwrite != null)
      {
        return new SerializedOperation(Overwrite.ToString(), Operator.Overwrite);
      }
      else
      {
        throw new InvalidOperationException("Invalid TaggedNumericOperation");
      }
    }

    public NumericOperation<TValue>? ToNumericOperation(TTag tag)
    {
      if (AddTo != null)
      {
        return AddTo.Values.ContainsKey(tag) ? NumericOperation.Add(AddTo.Values[tag]) : null;
      }
      else if (IncreaseBy != null)
      {
        return IncreaseBy.Values.ContainsKey(tag) ? NumericOperation.Increase<TValue>(IncreaseBy.Values[tag]) : null;
      }
      else if (Overwrite != null)
      {
        return Overwrite.Values.ContainsKey(tag) ? NumericOperation.Overwrite(Overwrite.Values[tag]) : null;
      }
      else
      {
        throw new InvalidOperationException("Invalid TaggedNumericOperation");
      }
    }

    public override string ToString() => $"{Serialize().Operator} {Serialize().Value}";
  }
}
