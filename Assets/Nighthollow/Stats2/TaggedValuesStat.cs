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
using System.Collections.Immutable;
using System.Linq;
using Nighthollow.Generated;
using Nighthollow.Utils;

#nullable enable

namespace Nighthollow.Stats2
{
  public sealed class TaggedValues<TTag, TValue> where TTag : Enum where TValue : struct
  {
    public TaggedValues(ImmutableDictionary<TTag, TValue> values)
    {
      Values = values;
    }

    public ImmutableDictionary<TTag, TValue> Values { get; }

    public TValue Get(TTag tag, TValue notFound) => Values.GetOrReturnDefault(tag, notFound);

    public override string ToString() =>
      string.Join(",", Values.Select(pair => $"{pair.Value} {pair.Key}").ToArray());
  }

  public class TaggedValuesStat<TTag, TValue> :
    AbstractStat<TaggedNumericOperation<TTag, TValue>, TaggedValues<TTag, TValue>>
    where TTag : Enum where TValue : struct
  {
    readonly AbstractStat<NumericOperation<TValue>, TValue> _childStat;

    protected TaggedValuesStat(StatId statId,
      AbstractStat<NumericOperation<TValue>, TValue> childStat) : base(statId)
    {
      _childStat = childStat;
    }

    public override TaggedValues<TTag, TValue> ComputeValue(
      IReadOnlyDictionary<OperationType, IEnumerable<TaggedNumericOperation<TTag, TValue>>> groups)
    {
      var result = ImmutableDictionary<TTag, TValue>.Empty;
      var overwrites = groups[OperationType.Overwrite].ToList();
      if (overwrites.Any())
      {
        result = overwrites.Last().OverwriteWith!.Values;
      }

      var allTags = groups.Values.SelectMany(x => x)
        .Aggregate(ImmutableHashSet<TTag>.Empty, (acc, op) => acc.Union(op.AllTags));

      foreach (var tag in allTags)
      {
        result = result.SetItem(
          tag,
          _childStat.ComputeValue(groups.ToDictionary(
            p => p.Key,
            p => p.Value.Select(o => o.AsNumericOperation(tag)).WhereNotNull())));
      }

      return new TaggedValues<TTag, TValue>(result);
    }
  }
}