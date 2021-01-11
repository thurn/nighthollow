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
using Nighthollow.Data;
using Nighthollow.Utils;

#nullable enable

namespace Nighthollow.Stats2
{
  public sealed class TaggedValuesStat<TTag, TValue> :
    AbstractStat<TaggedNumericStatModifier<TTag, TValue>, ImmutableDictionary<TTag, TValue>>
    where TTag : Enum where TValue : struct
  {
    readonly AbstractStat<NumericStatModifier<TValue>, TValue> _childStat;

    public TaggedValuesStat(StatId statId, AbstractStat<NumericStatModifier<TValue>, TValue> childStat) : base(statId)
    {
      _childStat = childStat;
    }

    public IStatModifier Add(ImmutableDictionary<TTag, TValue> value) =>
      TaggedNumericStatModifier<TTag, TValue>.Add(this, value);

    public IStatModifier Increase(ImmutableDictionary<TTag, PercentageValue> value) =>
      TaggedNumericStatModifier<TTag, TValue>.Increase(this, value);

    public IStatModifier Set(ImmutableDictionary<TTag, TValue> value) =>
      TaggedNumericStatModifier<TTag, TValue>.Set(this, value);

    public override ImmutableDictionary<TTag, TValue> ComputeValue(
      IReadOnlyDictionary<ModifierType, IEnumerable<TaggedNumericStatModifier<TTag, TValue>>> groups)
    {
      var result = ImmutableDictionary<TTag, TValue>.Empty;
      var overwrites = groups
        .GetOrReturnDefault(ModifierType.Set, Enumerable.Empty<TaggedNumericStatModifier<TTag, TValue>>()).ToList();
      if (overwrites.Any())
      {
        result = overwrites.Last().SetTo!;
      }

      var allTags = groups.Values.SelectMany(x => x)
        .Aggregate(ImmutableHashSet<TTag>.Empty, (acc, op) => acc.Union(op.AllTags));

      foreach (var tag in allTags)
      {
        result = result.SetItem(
          tag,
          _childStat.ComputeValue(groups.ToDictionary(
            p => p.Key,
            p => p.Value.Select(o => o.AsNumericModifier(_childStat, tag)).WhereNotNull())));
      }

      return result;
    }

    public override IStatModifier BuildModifier(ModifierType type, IValueData value) =>
      type switch
      {
        ModifierType.Add => Add((ImmutableDictionary<TTag, TValue>) value.Get()),
        ModifierType.Increase => Increase((ImmutableDictionary<TTag, PercentageValue>) value.Get()),
        ModifierType.Set => Set((ImmutableDictionary<TTag, TValue>) value.Get()),
        _ => throw new InvalidOperationException($"Unsupported modifier type: {type}")
      };

    protected override bool TryParseValue(string input, out IValueData result) =>
      TryParseInternal<TValue>(input, _childStat, out result);

    protected override bool TryParseIncrease(string input, out IValueData result) =>
      TryParseInternal<PercentageValue>(input, new PercentageStat(StatId), out result);

    static bool TryParseInternal<TType>(string input, IStat childStat, out IValueData result)
    {
      var builder = ImmutableDictionary.CreateBuilder<TTag, TType>();
      // Tag/value pairs should be separated by commas, each value should be written followed by a space and then the
      // full name of tag.
      foreach (var pairString in input.Split(','))
      {
        if (!TryParsePair(pairString, childStat, builder))
        {
          result = null!;
          return false;
        }
      }

      result = new ImmutableDictionaryValue<TTag, TType>(builder.ToImmutable());
      return true;
    }

    static bool TryParsePair<TType>
    (string pairString,
      IStat childStat,
      ImmutableDictionary<TTag, TType>.Builder builder)
    {
      var split = pairString.Split(' ');
      if (split.Length != 2)
      {
        return false;
      }

      var (valueString, tagString) = (split[0], split[1]);

      // Just using a fake modifier type here, probably doesn't matter...
      if (!childStat.TryParse(valueString, ModifierType.Set, out var result))
      {
        return false;
      }

      var tagNames = CollectionUtils
        .AllNonDefaultEnumValues<TTag>(typeof(TTag))
        .ToDictionary(v => v.ToString(), v => v);
      if (!tagNames.ContainsKey(tagString))
      {
        return false;
      }

      builder[tagNames[tagString]] = (TType) result.Get();
      return true;
    }
  }
}
