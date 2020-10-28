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

namespace Nighthollow.Statz
{
  public readonly struct TaggedValues<TTag, TValue> : IStatValue
    where TTag : struct, Enum where TValue : struct, IStatValue
  {
    public IReadOnlyDictionary<TTag, TValue> Values { get; }

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

    public override TaggedValues<TTag, TValue> DefaultValue() =>
      new TaggedValues<TTag, TValue>(new Dictionary<TTag, TValue>());

    public override TaggedValues<TTag, TValue> ComputeValue(
      IReadOnlyList<TaggedNumericOperation<TTag, TValue>> operations)
    {
      var result = new Dictionary<TTag, TValue>();

      foreach (var group in operations.GroupBy(op => op.Tag))
      {
        result[group.Key] = Compute(group.ToList());
      }

      return new TaggedValues<TTag, TValue>(result);
    }

    protected abstract TValue Compute(IReadOnlyList<TaggedNumericOperation<TTag, TValue>> operations);

    protected override TaggedValues<TTag, TValue> ParseStatValue(string value)
    {
      var result = new Dictionary<TTag, TValue>();
      foreach (var instance in value.Split(','))
      {
        result[ParseTag(instance.Last())] = ParseInstance(instance.Remove(instance.Length - 1));
      }

      return new TaggedValues<TTag, TValue>(result);
    }

    protected abstract TTag ParseTag(char tagCharacter);

    protected abstract TValue ParseInstance(string value);
  }

  public abstract class TaggedIntValuesStat<TTag> : TaggedValuesStat<TTag, IntValue> where TTag : struct, Enum
  {
    protected TaggedIntValuesStat(int id) : base(id)
    {
    }

    protected override IntValue Compute(
      IReadOnlyList<TaggedNumericOperation<TTag, IntValue>> operations) =>
      IntStat.Compute(operations, op => op);

    protected override IntValue ParseInstance(string value) => new IntValue(int.Parse(value));
  }

  public abstract class TaggedIntRangesStat<TTag> : TaggedValuesStat<TTag, IntRangeValue> where TTag : struct, Enum
  {
    protected TaggedIntRangesStat(int id) : base(id)
    {
    }

    protected override IntRangeValue Compute(
      IReadOnlyList<TaggedNumericOperation<TTag, IntRangeValue>> operations) =>
      new IntRangeValue(
        IntStat.Compute(operations, op => new IntValue(op.Low)).Int,
        IntStat.Compute(operations, op => new IntValue(op.High)).Int);

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