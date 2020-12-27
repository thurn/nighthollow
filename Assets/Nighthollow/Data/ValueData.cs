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
using MessagePack;
using Nighthollow.Stats;

#nullable enable

namespace Nighthollow.Data
{
  [Union(0, typeof(IntValueData))]
  [Union(1, typeof(BoolValueData))]
  [Union(2, typeof(DurationValue))]
  [Union(3, typeof(PercentageValue))]
  [Union(4, typeof(IntRangeValue))]
  public interface IValueData
  {
    T Switch<T>(
      Func<int, T> onInt,
      Func<bool, T> onBool,
      Func<DurationValue, T> onDuration,
      Func<PercentageValue, T> onPercentage,
      Func<IntRangeValue, T> onIntRange);
  }

  [MessagePackObject]
  public readonly struct IntValueData : IValueData
  {
    public IntValueData(int i)
    {
      Int = i;
    }

    [Key(0)] public int Int { get; }

    public T Switch<T>(
      Func<int, T> onInt,
      Func<bool, T> onBool,
      Func<DurationValue, T> onDuration,
      Func<PercentageValue, T> onPercentage,
      Func<IntRangeValue, T> onIntRange) => onInt(Int);

    public override string ToString()
    {
      return Int.ToString();
    }
  }

  [MessagePackObject]
  public readonly struct BoolValueData : IValueData
  {
    public BoolValueData(bool b)
    {
      Bool = b;
    }

    [Key(0)] public bool Bool { get; }

    public T Switch<T>(
      Func<int, T> onInt,
      Func<bool, T> onBool,
      Func<DurationValue, T> onDuration,
      Func<PercentageValue, T> onPercentage,
      Func<IntRangeValue, T> onIntRange) => onBool(Bool);

    public override string ToString()
    {
      return Bool.ToString();
    }
  }
}