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
using Nighthollow.Generated;
using Nighthollow.Utils;

#nullable enable

namespace Nighthollow.Stats
{
  public sealed class TaggedValues<TTag, TValue> where TTag : struct, Enum where TValue : struct
  {
    public TaggedValues(IReadOnlyDictionary<TTag, TValue> values)
    {
      Values = values;
    }

    public IReadOnlyDictionary<TTag, TValue> Values { get; }

    public TValue Get(TTag tag, TValue notFound) => Values.GetOrReturnDefault(tag, notFound);

    public override string ToString()
    {
      return string.Join(",", Values.Select(pair => $"{pair.Value} {pair.Key}").ToArray());
    }
  }

  public abstract class TaggedValuesStat<TTag, TValue> :
    AbstractStat<TaggedNumericOperation<TTag, TValue>, TaggedValues<TTag, TValue>>
    where TTag : struct, Enum where TValue : struct
  {
    protected TaggedValuesStat(OldStatId id) : base(id)
    {
    }

    public override TaggedValues<TTag, TValue> ComputeValue(
      IReadOnlyList<TaggedNumericOperation<TTag, TValue>> operations)
    {
      var result = new Dictionary<TTag, TValue>();

      var overwrite = operations.Select(op => op.Overwrite).LastOrDefault(o => o != null);
      if (overwrite != null)
      {
        result = overwrite.Values.ToDictionary(k => k.Key, v => v.Value);
      }

      foreach (var tag in AllTags().Where(tag => operations.Any(o => o.Tags.Contains(tag))))
      {
        result[tag] = ComputeTagged(operations.Select(op => op.ToNumericOperation(tag)).WhereNotNull().ToList());
      }

      return new TaggedValues<TTag, TValue>(result);
    }

    protected abstract TValue ComputeTagged(IReadOnlyList<NumericOperation<TValue>> operations);

    protected abstract IEnumerable<TTag> AllTags();

    protected override TaggedValues<TTag, TValue> ParseStatValue(string value)
    {
      var result = new Dictionary<TTag, TValue>();
      foreach (var instance in value.Split(','))
      {
        var tagged = instance.Split(' ');
        result[ParseTag(tagged[1])] = ParseTaggedValue(tagged[0]);
      }

      return new TaggedValues<TTag, TValue>(result);
    }

    protected TaggedValues<TTag, PercentageValue> ParseAsPercentages(string value)
    {
      var result = new Dictionary<TTag, PercentageValue>();
      foreach (var instance in value.Split(','))
      {
        var tagged = instance.Split(' ');
        result[ParseTag(tagged[1])] = PercentageValue.ParsePercentage(tagged[0]);
      }

      return new TaggedValues<TTag, PercentageValue>(result);
    }

    protected abstract TTag ParseTag(string tag);

    protected abstract TValue ParseTaggedValue(string value);

    public override IStatModifier ParseModifier(string value, Operator op)
    {
      return op switch
      {
        Operator.Add => StaticModifier(
          TaggedNumericOperation.Add(ParseStatValue(value))),
        Operator.Increase => StaticModifier(
          TaggedNumericOperation.Increase<TTag, TValue>(ParseAsPercentages(value))),
        Operator.Overwrite => StaticModifier(
          TaggedNumericOperation.Overwrite(ParseStatValue(value))),
        _ => throw new ArgumentException($"Unsupported modifier type: {op}")
      };
    }

    public override IStatModifier? StaticModifierForOperator(Operator op) => null;
  }

  public abstract class TaggedIntValuesStat<TTag> : TaggedValuesStat<TTag, int> where TTag : struct, Enum
  {
    protected TaggedIntValuesStat(OldStatId id) : base(id)
    {
    }

    protected override int ComputeTagged(IReadOnlyList<NumericOperation<int>> operations)
    {
      return IntStat.Compute(operations, op => op);
    }

    protected override int ParseTaggedValue(string value) => IntStat.ParseInt(value);
  }

  public abstract class TaggedPercentageValuesStat<TTag> : TaggedValuesStat<TTag, PercentageValue>
    where TTag : struct, Enum
  {
    protected TaggedPercentageValuesStat(OldStatId id) : base(id)
    {
    }

    protected override PercentageValue ComputeTagged(IReadOnlyList<NumericOperation<PercentageValue>> operations) =>
      PercentageStat.Compute(operations);

    protected override PercentageValue ParseTaggedValue(string value) => PercentageValue.ParsePercentage(value);
  }

  public abstract class TaggedIntRangesStat<TTag> : TaggedValuesStat<TTag, IntRangeValue> where TTag : struct, Enum
  {
    protected TaggedIntRangesStat(OldStatId id) : base(id)
    {
    }

    protected override IntRangeValue ComputeTagged(
      IReadOnlyList<NumericOperation<IntRangeValue>> operations) =>
      IntRangeStat.Compute(operations);

    protected override IntRangeValue ParseTaggedValue(string value) => IntRangeValue.ParseIntRange(value);
  }

  public sealed class DamageTypeIntsStat : TaggedIntValuesStat<DamageType>
  {
    public DamageTypeIntsStat(OldStatId id) : base(id)
    {
    }

    protected override IEnumerable<DamageType> AllTags() =>
      CollectionUtils.AllNonDefaultEnumValues<DamageType>(typeof(DamageType));

    protected override DamageType ParseTag(string tag) => ParseDamageTypeTag(tag);

    public static DamageType ParseDamageTypeTag(string tag)
    {
      return tag switch
      {
        "Untyped" => DamageType.Untyped,
        "Radiant" => DamageType.Radiant,
        "Lightning" => DamageType.Lightning,
        "Fire" => DamageType.Fire,
        "Cold" => DamageType.Cold,
        "Physical" => DamageType.Physical,
        "Necrotic" => DamageType.Necrotic,
        _ => throw new ArgumentException($"Unknown damage type tag: {tag}")
      };
    }
  }

  public sealed class DamageTypeIntRangesStat : TaggedIntRangesStat<DamageType>
  {
    public DamageTypeIntRangesStat(OldStatId id) : base(id)
    {
    }

    protected override DamageType ParseTag(string tag) => DamageTypeIntsStat.ParseDamageTypeTag(tag);

    protected override IEnumerable<DamageType> AllTags() =>
      CollectionUtils.AllNonDefaultEnumValues<DamageType>(typeof(DamageType));
  }

  public sealed class SchoolIntsStat : TaggedIntValuesStat<School>
  {
    public SchoolIntsStat(OldStatId id) : base(id)
    {
    }

    protected override School ParseTag(string tag)
    {
      return tag switch
      {
        "Light" => School.Light,
        "Sky" => School.Sky,
        "Flame" => School.Flame,
        "Ice" => School.Ice,
        "Earth" => School.Earth,
        "Shadow" => School.Shadow,
        _ => throw new ArgumentException($"Unknown school tag: {tag}")
      };
    }

    protected override IEnumerable<School> AllTags() => CollectionUtils.AllNonDefaultEnumValues<School>(typeof(School));
  }
}
