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

#nullable enable

using System;
using System.Collections.Generic;
using System.Linq;
using Nighthollow.Generated;
using Nighthollow.Utils;

namespace Nighthollow.Stats
{
  public sealed class TaggedValues<TTag, TValue> : IStatValue
    where TTag : struct, Enum where TValue : struct, IStatValue
  {
    public IReadOnlyDictionary<TTag, TValue> Values { get; }

    public TValue Get(TTag tag, TValue notFound) => Values.GetValueOrDefault(tag, notFound);

    public TaggedValues(IReadOnlyDictionary<TTag, TValue> values)
    {
      Values = values;
    }
  }

  public abstract class TaggedValuesStat<TTag, TValue> :
    AbstractStat<TaggedNumericOperation<TTag, TValue>, TaggedValues<TTag, TValue>>
    where TTag : struct, Enum where TValue : struct, IStatValue
  {
    protected TaggedValuesStat(int id) : base(id)
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

      foreach (var group in operations
        .Where(o => o.Operations != null)
        .SelectMany(op => op.Operations)
        .GroupBy(pair => pair.Key))
      {
        var operationsForTag = group.Select(pair => pair.Value).ToList();
        if (overwrite != null)
        {
          operationsForTag.Add(NumericOperation.Overwrite(overwrite.Get(group.Key, default)));
        }

        result[group.Key] = Compute(operationsForTag);
      }

      return new TaggedValues<TTag, TValue>(result);
    }

    protected abstract TValue Compute(IReadOnlyList<NumericOperation<TValue>> operations);

    protected override TaggedValues<TTag, TValue> ParseStatValue(string value)
    {
      var result = new Dictionary<TTag, TValue>();
      foreach (var instance in value.Split(','))
      {
        var tag = instance.Last();
        result[ParseTag(tag)] = ParseInstance(instance.Replace(tag.ToString(), ""));
      }

      return new TaggedValues<TTag, TValue>(result);
    }

    protected TaggedValues<TTag, PercentageValue> ParseAsPercentages(string value)
    {
      var result = new Dictionary<TTag, PercentageValue>();
      foreach (var instance in value.Split(','))
      {
        var tag = instance.Last();
        result[ParseTag(tag)] = PercentageStat.ParsePercentage(instance.Replace(tag.ToString(), ""));
      }

      return new TaggedValues<TTag, PercentageValue>(result);
    }

    protected abstract TTag ParseTag(char tagCharacter);

    protected abstract TValue ParseInstance(string value);

    public override IStatModifier ParseModifier(string value, Operator op) =>
      op switch
      {
        Operator.Add => StaticModifier(new TaggedNumericOperation<TTag, TValue>(
          null,
          ParseStatValue(value).Values.ToDictionary(k => k.Key, v => NumericOperation.Add(v.Value)))),
        Operator.Overwrite => StaticModifier(TaggedNumericOperation.Overwrite(ParseStatValue(value))),
        Operator.Increase => StaticModifier(new TaggedNumericOperation<TTag, TValue>(
          null,
          ParseAsPercentages(value).Values.ToDictionary(k => k.Key, v => NumericOperation.Increase<TValue>(v.Value)))),
        _ => throw new ArgumentException($"Unsupported modifier type: {op}")
      };

    public override IStatModifier? StaticModifierForOperator(Operator op) => null;
  }

  public abstract class TaggedIntValuesStat<TTag> : TaggedValuesStat<TTag, IntValue> where TTag : struct, Enum
  {
    protected TaggedIntValuesStat(int id) : base(id)
    {
    }

    protected override IntValue Compute(IReadOnlyList<NumericOperation<IntValue>> operations) =>
      IntStat.Compute(operations, op => op);

    protected override IntValue ParseInstance(string value) => IntStat.ParseInt(value);
  }

  public abstract class TaggedPercentageValuesStat<TTag> : TaggedValuesStat<TTag, PercentageValue>
    where TTag : struct, Enum
  {
    protected TaggedPercentageValuesStat(int id) : base(id)
    {
    }

    protected override PercentageValue Compute(IReadOnlyList<NumericOperation<PercentageValue>> operations) =>
      PercentageStat.Compute(operations);

    protected override PercentageValue ParseInstance(string value) => PercentageStat.ParsePercentage(value);
  }

  public abstract class TaggedIntRangesStat<TTag> : TaggedValuesStat<TTag, IntRangeValue> where TTag : struct, Enum
  {
    protected TaggedIntRangesStat(int id) : base(id)
    {
    }

    protected override IntRangeValue Compute(
      IReadOnlyList<NumericOperation<IntRangeValue>> operations) => IntRangeStat.Compute(operations);

    protected override IntRangeValue ParseInstance(string value) => IntRangeStat.ParseIntRange(value);
  }

  public sealed class DamageTypeIntsStat : TaggedIntValuesStat<DamageType>
  {
    public DamageTypeIntsStat(int id) : base(id)
    {
    }

    protected override DamageType ParseTag(char tagCharacter) => ParseDamageTypeTag(tagCharacter);

    public static DamageType ParseDamageTypeTag(char tagCharacter) =>
      tagCharacter switch
      {
        'R' => DamageType.Radiant,
        'L' => DamageType.Lightning,
        'F' => DamageType.Fire,
        'C' => DamageType.Cold,
        'P' => DamageType.Physical,
        'N' => DamageType.Necrotic,
        _ => throw new ArgumentException($"Unknown damage type identifier: {tagCharacter}")
      };
  }

  public sealed class DamageTypeIntRangesStat : TaggedIntRangesStat<DamageType>
  {
    public DamageTypeIntRangesStat(int id) : base(id)
    {
    }

    protected override DamageType ParseTag(char tagCharacter) => DamageTypeIntsStat.ParseDamageTypeTag(tagCharacter);
  }

  public sealed class SchoolIntsStat : TaggedIntValuesStat<School>
  {
    public SchoolIntsStat(int id) : base(id)
    {
    }

    protected override School ParseTag(char tagCharacter) =>
      tagCharacter switch
      {
        'L' => School.Light,
        'S' => School.Sky,
        'F' => School.Flame,
        'I' => School.Ice,
        'E' => School.Earth,
        'N' => School.Night,
        _ => throw new ArgumentException($"Unknown school identifier: {tagCharacter}")
      };
  }
}