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
using System.Collections;
using System.Collections.Immutable;
using System.IO;
using MessagePack;
using Nighthollow.Rules;
using Nighthollow.World.Data;

#nullable enable

namespace Nighthollow.Data
{
  public interface ITableId
  {
    int Id { get; }

    string TableName { get; }

    Type GetUnderlyingType();

    IDictionary GetInUnchecked(GameData gameData);

    void Serialize(GameData gameData, FileStream fileStream, MessagePackSerializerOptions options);

    GameData Deserialize(GameData gameData, FileStream fileStream, MessagePackSerializerOptions options);

    GameData Deserialize(GameData gameData, ReadOnlyMemory<byte> bytes, MessagePackSerializerOptions options);
  }

  public class TableId<T> : ITableId where T : class
  {
    public TableId(int id, string tableName)
    {
      Id = id;
      TableName = tableName;
    }

    public int Id { get; }

    public string TableName { get; }

    public ImmutableDictionary<int, T> GetIn(GameData gameData) => gameData.GetTable(this);

    public GameData Write(GameData gameData, ImmutableDictionary<int, T> newValue) =>
      gameData.WithTable(this, newValue);

    public void Serialize(GameData gameData, FileStream fileStream, MessagePackSerializerOptions options)
    {
      MessagePackSerializer.Serialize(fileStream, GetIn(gameData), options);
    }

    public GameData Deserialize(GameData gameData, FileStream fileStream, MessagePackSerializerOptions options) =>
      Write(gameData, MessagePackSerializer.Deserialize<ImmutableDictionary<int, T>>(fileStream, options));

    public GameData Deserialize(GameData gameData, ReadOnlyMemory<byte> bytes, MessagePackSerializerOptions options) =>
      Write(gameData, MessagePackSerializer.Deserialize<ImmutableDictionary<int, T>>(bytes, options));

    public Type GetUnderlyingType() => typeof(T);

    public IDictionary GetInUnchecked(GameData gameData) => GetIn(gameData);
  }

  public sealed class SingletonTableId<T> : TableId<T> where T : class
  {
    const int SingletonEntityId = 1;

    public SingletonTableId(int id, string tableName) : base(id, tableName)
    {
    }

    public T GetSingleton(GameData gameData) => gameData.GetTable(this)[SingletonEntityId];

    public GameData WriteSingleton(GameData gameData, T newValue) =>
      gameData.WithTable(this, ImmutableDictionary<int, T>.Empty.Add(SingletonEntityId, newValue));
  }

  public static class TableId
  {
    public static readonly TableId<TableMetadata> TableMetadata =
      new TableId<TableMetadata>(0, "TableMetadata");

    public static readonly TableId<CreatureTypeData> CreatureTypes =
      new TableId<CreatureTypeData>(1, "CreatureTypes");

    public static readonly TableId<AffixTypeData> AffixTypes =
      new TableId<AffixTypeData>(2, "AffixTypes");

    public static readonly TableId<SkillTypeData> SkillTypes =
      new TableId<SkillTypeData>(3, "SkillTypes");

    public static readonly TableId<StatData> Stats =
      new TableId<StatData>(4, "Stats");

    public static readonly TableId<StaticItemListData> ItemLists =
      new TableId<StaticItemListData>(5, "ItemLists");

    public static readonly TableId<ModifierData> UserModifiers =
      new TableId<ModifierData>(6, "UserModifiers");

    public static readonly TableId<CreatureItemData> Collection =
      new TableId<CreatureItemData>(7, "Collection");

    public static readonly TableId<CreatureItemData> Deck =
      new TableId<CreatureItemData>(8, "Deck");

    public static readonly TableId<StatusEffectTypeData> StatusEffectTypes =
      new TableId<StatusEffectTypeData>(9, "StatusEffectTypes");

    public static readonly SingletonTableId<BattleData> BattleData =
      new SingletonTableId<BattleData>(10, "BattleData");

    public static readonly TableId<Rule> Rules =
      new TableId<Rule>(11, "Rules");

    public static readonly TableId<GlobalData> Globals =
      new TableId<GlobalData>(12, "Globals");

    public static readonly TableId<HexData> Hexes =
      new TableId<HexData>(13, "Hexes");

    public static readonly TableId<KingdomData> Kingdoms =
      new TableId<KingdomData>(14, "Kingdoms");

    public static readonly ImmutableList<ITableId> AllTableIds = ImmutableList.Create<ITableId>(
      TableMetadata,
      CreatureTypes,
      AffixTypes,
      SkillTypes,
      Stats,
      ItemLists,
      UserModifiers,
      Collection,
      Deck,
      StatusEffectTypes,
      BattleData,
      Rules,
      Globals,
      Hexes,
      Kingdoms
    );
  }
}
