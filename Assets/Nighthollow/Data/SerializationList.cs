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
using MessagePack;
using Nighthollow.Stats;

#nullable enable

namespace Nighthollow.Data
{
  /// <summary>
  /// List of types we need to be able to serialize for GameData. This class needs to exist so the MessagePack code
  /// generator can discover the types & register serializers for them.
  /// </summary>
  [MessagePackObject]
  public sealed class SerializationList
  {
    // From GameData
    [Key(0)] public ImmutableDictionary<int, TableMetadata> TableMetadata { get; } = null!;
    [Key(1)] public ImmutableDictionary<int, CreatureTypeData> CreatureTypes { get; } = null!;
    [Key(2)] public ImmutableDictionary<int, AffixTypeData> AffixTypes { get; } = null!;
    [Key(3)] public ImmutableDictionary<int, SkillTypeData> SkillTypes { get; } = null!;
    [Key(4)] public ImmutableDictionary<int, StatData> StatData { get; } = null!;
    [Key(5)] public ImmutableDictionary<int, StaticItemListData> ItemLists { get; } = null!;
    [Key(6)] public ImmutableDictionary<int, ModifierData> UserModifiers { get; } = null!;
    [Key(7)] public ImmutableDictionary<int, CreatureItemData> Collection { get; } = null!;
    [Key(8)] public ImmutableDictionary<int, CreatureItemData> Deck { get; } = null!;
    [Key(9)] public BattleData BattleData { get; } = null!;
    [Key(10)] public ImmutableDictionary<int, StatusEffectTypeData> StatusEffects { get; } = null!;

    // From IValueData
    [Key(100)] public IntValueData A { get; } = default;
    [Key(101)] public DurationValue B { get; } = default;
    [Key(102)] public PercentageValue C { get; } = default;
    [Key(104)] public IntRangeValue D { get; } = default;
    [Key(105)] public BoolValueData E { get; } = default;
    [Key(106)] public ImmutableDictionaryValue<DamageType, int> F { get; } = null!;
    [Key(107)] public ImmutableDictionaryValue<DamageType, PercentageValue> G { get; } = null!;
    [Key(108)] public ImmutableDictionaryValue<DamageType, IntRangeValue> H { get; } = null!;
    [Key(109)] public ImmutableDictionaryValue<School, int> I { get; } = null!;
  }
}
