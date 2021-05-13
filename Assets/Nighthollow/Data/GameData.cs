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
using Nighthollow.Triggers;
using Nighthollow.World.Data;

#nullable enable

namespace Nighthollow.Data
{
  public sealed partial class GameData
  {
    public GameData(
      BattleData? battleData = null,
      ImmutableDictionary<int, TableMetadata>? tableMetadata = null,
      ImmutableDictionary<int, CreatureTypeData>? creatureTypes = null,
      ImmutableDictionary<int, AffixTypeData>? affixTypes = null,
      ImmutableDictionary<int, SkillTypeData>? skillTypes = null,
      ImmutableDictionary<int, StatData>? statData = null,
      ImmutableDictionary<int, StaticItemListData>? creatureLists = null,
      ImmutableDictionary<int, ModifierData>? userModifiers = null,
      ImmutableDictionary<int, CreatureItemData>? collection = null,
      ImmutableDictionary<int, CreatureItemData>? deck = null,
      ImmutableDictionary<int, StatusEffectTypeData>? statusEffects = null,
      ImmutableDictionary<int, ITrigger>? triggers = null,
      ImmutableDictionary<int, GlobalData>? globals = null,
      ImmutableDictionary<int, HexData>? hexes = null,
      ImmutableDictionary<int, KingdomData>? kingdoms = null)
    {
      BattleData = battleData ?? new BattleData();
      TableMetadata = tableMetadata ?? ImmutableDictionary<int, TableMetadata>.Empty;
      CreatureTypes = creatureTypes ?? ImmutableDictionary<int, CreatureTypeData>.Empty;
      AffixTypes = affixTypes ?? ImmutableDictionary<int, AffixTypeData>.Empty;
      SkillTypes = skillTypes ?? ImmutableDictionary<int, SkillTypeData>.Empty;
      StatData = statData ?? ImmutableDictionary<int, StatData>.Empty;
      ItemLists = creatureLists ?? ImmutableDictionary<int, StaticItemListData>.Empty;
      UserModifiers = userModifiers ?? ImmutableDictionary<int, ModifierData>.Empty;
      Collection = collection ?? ImmutableDictionary<int, CreatureItemData>.Empty;
      Deck = deck ?? ImmutableDictionary<int, CreatureItemData>.Empty;
      StatusEffects = statusEffects ?? ImmutableDictionary<int, StatusEffectTypeData>.Empty;
      Triggers = triggers ?? ImmutableDictionary<int, ITrigger>.Empty;
      Globals = globals ?? ImmutableDictionary<int, GlobalData>.Empty;
      Hexes = hexes ?? ImmutableDictionary<int, HexData>.Empty;
      Kingdoms = kingdoms ?? ImmutableDictionary<int, KingdomData>.Empty;
    }

    [Field] public BattleData BattleData { get; }
    [Field] public ImmutableDictionary<int, TableMetadata> TableMetadata { get; }
    [Field] public ImmutableDictionary<int, CreatureTypeData> CreatureTypes { get; }
    [Field] public ImmutableDictionary<int, AffixTypeData> AffixTypes { get; }
    [Field] public ImmutableDictionary<int, SkillTypeData> SkillTypes { get; }
    [Field] public ImmutableDictionary<int, StatData> StatData { get; }
    [Field] public ImmutableDictionary<int, StaticItemListData> ItemLists { get; }
    [Field] public ImmutableDictionary<int, ModifierData> UserModifiers { get; }
    [Field] public ImmutableDictionary<int, CreatureItemData> Collection { get; }
    [Field] public ImmutableDictionary<int, CreatureItemData> Deck { get; }
    [Field] public ImmutableDictionary<int, StatusEffectTypeData> StatusEffects { get; }
    [Field] public ImmutableDictionary<int, ITrigger> Triggers { get; }
    [Field] public ImmutableDictionary<int, GlobalData> Globals { get; }
    [Field] public ImmutableDictionary<int, HexData> Hexes { get; }
    [Field] public ImmutableDictionary<int, KingdomData> Kingdoms { get; }
  }
}
