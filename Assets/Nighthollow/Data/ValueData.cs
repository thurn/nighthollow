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

using System.Collections.Immutable;
using System.Linq;
using MessagePack;
using Nighthollow.Generated;
using Nighthollow.Stats2;

#nullable enable

namespace Nighthollow.Data
{
  [Union(0, typeof(IntValueData))]
  [Union(1, typeof(DurationValue))]
  [Union(2, typeof(PercentageValue))]
  [Union(3, typeof(IntRangeValue))]
  [Union(4, typeof(BoolValueData))]
  [Union(5, typeof(ImmutableDictionaryValue<DamageType, int>))]
  [Union(6, typeof(ImmutableDictionaryValue<DamageType, PercentageValue>))]
  [Union(7, typeof(ImmutableDictionaryValue<DamageType, IntRangeValue>))]
  [Union(8, typeof(ImmutableDictionaryValue<School, int>))]
  public interface IValueData
  {
    object Get();
  }

  public static class ValueDataUtil
  {
    public static bool TryParse(string input, out IValueData value)
    {
      if (int.TryParse(input, out var i))
      {
        value = new IntValueData(i);
        return true;
      }

      if (bool.TryParse(input, out var b))
      {
        value = new BoolValueData(b);
        return true;
      }

      if (DurationValue.TryParse(input, out var d))
      {
        value = d;
        return true;
      }

      if (PercentageValue.TryParse(input, out var p))
      {
        value = p;
        return true;
      }

      if (IntRangeValue.TryParse(input, out var ir))
      {
        value = ir;
        return true;
      }

      value = null!;
      return false;
    }
  }

  [MessagePackObject]
  public readonly struct IntValueData : IValueData
  {
    public IntValueData(int i)
    {
      Int = i;
    }

    [Key(0)] public int Int { get; }

    public object Get() => Int;

    public override string ToString() => Int.ToString();
  }

  [MessagePackObject]
  public readonly struct BoolValueData : IValueData
  {
    public BoolValueData(bool b)
    {
      Bool = b;
    }

    [Key(0)] public bool Bool { get; }

    public object Get() => Bool;

    public override string ToString() => Bool.ToString();
  }

  [MessagePackObject]
  public sealed class ImmutableDictionaryValue<TTag, TValue> : IValueData where TTag : notnull
  {
    public ImmutableDictionaryValue(ImmutableDictionary<TTag, TValue> dictionary)
    {
      Dictionary = dictionary;
    }

    [Key(0)] public ImmutableDictionary<TTag, TValue> Dictionary { get; }

    public object Get() => Dictionary;

    public override string ToString() => string.Join(",", Dictionary.Select(pair => $"{pair.Key}: {pair.Value}"));
  }
}