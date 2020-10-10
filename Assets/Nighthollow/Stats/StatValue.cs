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

#nullable enable

namespace Nighthollow.Stats
{
  public interface IStatValue
  {
  }

  public readonly struct IntValue : IStatValue
  {
    public readonly int Value;

    public IntValue(int value)
    {
      Value = value;
    }
  }

  public readonly struct BoolValue : IStatValue
  {
    public readonly bool Value;

    public BoolValue(bool value)
    {
      Value = value;
    }
  }

  public readonly struct DurationValue : IStatValue
  {
    public readonly int Value;

    public DurationValue(double valueSeconds)
    {
      Value = (int) Math.Round(valueSeconds * 1000.0);
    }
  }

  public readonly struct PercentageValue : IStatValue
  {
    public readonly int Value;

    public PercentageValue(double value)
    {
      Value = (int) Math.Round(value * 100.0);
    }
  }

  public readonly struct IntRangeValue : IStatValue
  {
    public readonly int Low;
    public readonly int High;

    public IntRangeValue(int low, int high)
    {
      Low = low;
      High = high;
    }
  }

  public readonly struct TaggedStatValue<TTag, TValue> : IStatValue where TTag : struct where TValue : IStatValue
  {
    public TTag Tag { get; }
    public TValue Value { get; }

    public TaggedStatValue(TTag tag, TValue value)
    {
      Tag = tag;
      Value = value;
    }
  }
}