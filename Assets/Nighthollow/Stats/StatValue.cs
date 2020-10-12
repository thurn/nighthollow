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
    IModifier AsStaticModifier();

    IntValue AsIntValue();
  }

  public readonly struct NoValue : IStatValue
  {
    public IModifier AsStaticModifier() => new StaticModifier(this);

    public IntValue AsIntValue() => throw new NotImplementedException();
  }

  public readonly struct IntValue : IStatValue
  {
    public readonly int Value;

    public IntValue(int value)
    {
      Value = value;
    }

    public IModifier AsStaticModifier() => new StaticModifier(this);

    public IntValue AsIntValue() => this;
  }

  public readonly struct DurationValue : IStatValue
  {
    public readonly IntValue Value;

    public DurationValue(double valueSeconds)
    {
      Value = new IntValue((int) Math.Round(valueSeconds * 1000.0));
    }

    public IModifier AsStaticModifier() => new StaticModifier(this);

    public IntValue AsIntValue() => Value;
  }

  public readonly struct PercentageValue : IStatValue
  {
    public readonly IntValue Value;

    public PercentageValue(double value)
    {
      Value = new IntValue((int) Math.Round(value * 100.0));
    }

    public IModifier AsStaticModifier() => new StaticModifier(this);

    public IntValue AsIntValue() => Value;
  }

  public readonly struct IntRangeValue : IStatValue
  {
    public readonly IntValue Low;
    public readonly IntValue High;

    public IntRangeValue(int low, int high)
    {
      Low = new IntValue(low);
      High = new IntValue(high);
    }

    public IModifier AsStaticModifier() => new StaticModifier(this);

    public IntValue AsIntValue() => throw new NotImplementedException();
  }

  public interface ITaggedStatValue
  {
    Enum GetTag();
    IStatValue GetValue();
  }

  public readonly struct TaggedStatValue<TTag, TValue> : IStatValue, ITaggedStatValue
    where TTag : Enum where TValue : IStatValue
  {
    public TTag Tag { get; }
    public TValue Value { get; }

    public TaggedStatValue(TTag tag, TValue value)
    {
      Tag = tag;
      Value = value;
    }

    public IModifier AsStaticModifier() => new StaticModifier(this);

    public IntValue AsIntValue() => throw new NotImplementedException();

    public Enum GetTag() => Tag;

    public IStatValue GetValue() => Value;
  }
}