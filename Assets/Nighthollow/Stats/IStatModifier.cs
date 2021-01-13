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

using MessagePack;
using Nighthollow.Data;

#nullable enable

namespace Nighthollow.Stats
{
  [Union(0, typeof(NumericStatModifier<int>))]
  [Union(1, typeof(NumericStatModifier<DurationValue>))]
  [Union(2, typeof(NumericStatModifier<PercentageValue>))]
  [Union(3, typeof(NumericStatModifier<IntRangeValue>))]
  [Union(4, typeof(BooleanStatModifier))]
  [Union(5, typeof(TaggedNumericStatModifier<DamageType, int>))]
  [Union(6, typeof(TaggedNumericStatModifier<DamageType, PercentageValue>))]
  [Union(7, typeof(TaggedNumericStatModifier<DamageType, IntRangeValue>))]
  [Union(8, typeof(TaggedNumericStatModifier<School, int>))]
  public interface IStatModifier
  {
    public StatId StatId { get; }

    ModifierType Type { get; }

    /// <summary>
    /// A modifier may optionally be constructed with a lifetime, which dictates when this modifier should be removed
    /// from its containing StatTable.
    /// </summary>
    ILifetime? Lifetime { get; }

    IStatModifier WithLifetime(ILifetime lifetime);

    /// <summary>
    /// Produce a description of this modifier based on the provided template. Templates for numeric stats contain the
    /// character '#' which should be replaced with the modifier description. Templates for boolean stats contain two
    /// descriptions separated by the '/' character, which correspond to true / false respectively.
    ///
    /// If <paramref name="highValue"/> is provided, this description is for a range of numeric modifier values, and
    /// the # character should be replaced with a hyphen-separated range of currentValue - highValue.
    /// </summary>
    public string Describe(string template, IValueData? highValue);
  }

  // Hack: MessagePack can't find types that are only ever used in a union declaration, so we need to have a concrete
  // list of all types we expect to appear as well.
  [MessagePackObject]
  public sealed class SerializationHack
  {
    [Key(0)] public NumericStatModifier<int> Zero = null!;
    [Key(1)] public NumericStatModifier<DurationValue> One = null!;
    [Key(2)] public NumericStatModifier<PercentageValue> Two = null!;
    [Key(3)] public NumericStatModifier<IntRangeValue> Three = null!;
    [Key(4)] public BooleanStatModifier Four = null!;
    [Key(5)] public TaggedNumericStatModifier<DamageType, int> Five = null!;
    [Key(6)] public TaggedNumericStatModifier<DamageType, PercentageValue> Six = null!;
    [Key(7)] public TaggedNumericStatModifier<DamageType, IntRangeValue> Seven = null!;
    [Key(8)] public TaggedNumericStatModifier<School, int> Eight = null!;
  }
}