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
using Nighthollow.Editing.Scenarios;
using Nighthollow.Rules;
using Nighthollow.World.Data;

#nullable enable

namespace Nighthollow.Data
{
  public sealed class GameData
  {
    readonly ImmutableDictionary<int, object> _data;

    public GameData() : this(ImmutableDictionary<int, object>.Empty)
    {
    }

    GameData(ImmutableDictionary<int, object> data)
    {
      _data = data;
    }

    public ImmutableDictionary<int, T> GetTable<T>(TableId<T> tableId) where T : class =>
      _data.ContainsKey(tableId.Id)
        ? (ImmutableDictionary<int, T>) _data[tableId.Id]
        : ImmutableDictionary<int, T>.Empty;

    public GameData WithTable<T>(TableId<T> tableId, ImmutableDictionary<int, T> table) where T : class =>
      new GameData(_data.SetItem(tableId.Id, table));

    public BattleData BattleData => TableId.BattleData.GetSingleton(this);
    public ImmutableDictionary<int, TableMetadata> TableMetadata => GetTable(TableId.TableMetadata);
    public ImmutableDictionary<int, CreatureTypeData> CreatureTypes => GetTable(TableId.CreatureTypes);
    public ImmutableDictionary<int, AffixTypeData> AffixTypes => GetTable(TableId.AffixTypes);
    public ImmutableDictionary<int, SkillTypeData> SkillTypes => GetTable(TableId.SkillTypes);
    public ImmutableDictionary<int, StatData> StatData => GetTable(TableId.Stats);
    public ImmutableDictionary<int, StaticItemListData> ItemLists => GetTable(TableId.ItemLists);
    public ImmutableDictionary<int, ModifierData> UserModifiers => GetTable(TableId.UserModifiers);
    public ImmutableDictionary<int, CreatureItemData> Collection => GetTable(TableId.Collection);
    public ImmutableDictionary<int, CreatureItemData> Deck => GetTable(TableId.Deck);
    public ImmutableDictionary<int, StatusEffectTypeData> StatusEffects => GetTable(TableId.StatusEffectTypes);
    public ImmutableDictionary<int, Rule> Rules => GetTable(TableId.Rules);
    public ImmutableDictionary<int, GlobalData> Globals => GetTable(TableId.Globals);
    public ImmutableDictionary<int, HexData> Hexes => GetTable(TableId.Hexes);
    public ImmutableDictionary<int, KingdomData> Kingdoms => GetTable(TableId.Kingdoms);
    public ImmutableDictionary<int, ScenarioData> Scenarios => GetTable(TableId.Scenarios);
  }
}
