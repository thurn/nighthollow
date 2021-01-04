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
  public class TaggedValuesStat<TTag, TValue> :
    AbstractStat<TaggedNumericOperation<TTag, TValue>, ImmutableDictionary<TTag, TValue>>
    where TTag : Enum where TValue : struct
  {
    readonly AbstractStat<NumericOperation<TValue>, TValue> _childStat;

    protected TaggedValuesStat(StatId statId, AbstractStat<NumericOperation<TValue>, TValue> childStat) : base(statId)
    {
      _childStat = childStat;
    }

    public override ImmutableDictionary<TTag, TValue> ComputeValue(
      IReadOnlyDictionary<OperationType, IEnumerable<TaggedNumericOperation<TTag, TValue>>> groups)
    {
      var result = ImmutableDictionary<TTag, TValue>.Empty;
      var overwrites = groups[OperationType.Set].ToList();
      if (overwrites.Any())
      {
        result = overwrites.Last().OverwriteWith!;
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

      return result;
    }
  }
}