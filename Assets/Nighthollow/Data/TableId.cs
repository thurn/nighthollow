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

  public abstract class TableId<T> : ITableId where T : class
  {
    protected TableId(int id, string tableName)
    {
      Id = id;
      TableName = tableName;
    }

    public int Id { get; }

    public string TableName { get; }

    public abstract ImmutableDictionary<int, T> GetIn(GameData gameData);

    public abstract GameData Write(GameData gameData, ImmutableDictionary<int, T> newValue);

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

  public abstract class SingletonTableId<T> : TableId<T> where T : class
  {
    protected SingletonTableId(int id, string tableName) : base(id, tableName)
    {
    }

    protected abstract T GetSingleton(GameData gameData);

    protected abstract GameData WriteSingleton(GameData gameData, T newValue);

    public sealed override ImmutableDictionary<int, T> GetIn(GameData gameData) =>
      ImmutableDictionary<int, T>.Empty.SetItem(1, GetSingleton(gameData));

    public sealed override GameData Write(GameData gameData, ImmutableDictionary<int, T> newValue) =>
      WriteSingleton(gameData, newValue[1]);
  }

  public static class TableId
  {
    public static readonly TableId<TableMetadata> TableMetadata =
      new TableMetadataTableId(0, "TableMetadata");

    public static readonly TableId<CreatureTypeData> CreatureTypes =
      new CreatureTypesTableId(1, "CreatureTypes");

    public static readonly TableId<AffixTypeData> AffixTypes =
      new AffixTypesTableId(2, "AffixTypes");

    public static readonly TableId<SkillTypeData> SkillTypes =
      new SkillTypesTableId(3, "SkillTypes");

    public static readonly TableId<StatData> Stats =
      new StatDataTableId(4, "Stats");

    public static readonly TableId<StaticItemListData> ItemLists =
      new ItemListsTableId(5, "ItemLists");

    public static readonly TableId<ModifierData> UserModifiers =
      new UserModifiersTableId(6, "UserModifiers");

    public static readonly TableId<CreatureItemData> Collection =
      new CollectionTableId(7, "Collection");

    public static readonly TableId<CreatureItemData> Deck =
      new DeckTableId(8, "Deck");

    public static readonly TableId<StatusEffectTypeData> StatusEffectTypes =
      new StatusEffectsTypesTableId(9, "StatusEffectTypes");

    public static readonly TableId<BattleData> BattleData =
      new BattleDataTableId(10, "BattleData");

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
      BattleData
    );

    sealed class TableMetadataTableId : TableId<TableMetadata>
    {
      public TableMetadataTableId(int id, string tableName) : base(id, tableName)
      {
      }

      public override ImmutableDictionary<int, TableMetadata> GetIn(GameData gameData) =>
        gameData.TableMetadata;

      public override GameData Write(GameData gameData, ImmutableDictionary<int, TableMetadata> newValue) =>
        gameData.WithTableMetadata(newValue);
    }

    sealed class CreatureTypesTableId : TableId<CreatureTypeData>
    {
      public CreatureTypesTableId(int id, string tableName) : base(id, tableName)
      {
      }

      public override ImmutableDictionary<int, CreatureTypeData> GetIn(GameData gameData) =>
        gameData.CreatureTypes;

      public override GameData Write(GameData gameData, ImmutableDictionary<int, CreatureTypeData> newValue) =>
        gameData.WithCreatureTypes(newValue);
    }

    sealed class AffixTypesTableId : TableId<AffixTypeData>
    {
      public AffixTypesTableId(int id, string tableName) : base(id, tableName)
      {
      }

      public override ImmutableDictionary<int, AffixTypeData> GetIn(GameData gameData) =>
        gameData.AffixTypes;

      public override GameData Write(GameData gameData, ImmutableDictionary<int, AffixTypeData> newValue) =>
        gameData.WithAffixTypes(newValue);
    }

    sealed class SkillTypesTableId : TableId<SkillTypeData>
    {
      public SkillTypesTableId(int id, string tableName) : base(id, tableName)
      {
      }

      public override ImmutableDictionary<int, SkillTypeData> GetIn(GameData gameData) =>
        gameData.SkillTypes;

      public override GameData Write(GameData gameData, ImmutableDictionary<int, SkillTypeData> newValue) =>
        gameData.WithSkillTypes(newValue);
    }

    sealed class StatDataTableId : TableId<StatData>
    {
      public StatDataTableId(int id, string tableName) : base(id, tableName)
      {
      }

      public override ImmutableDictionary<int, StatData> GetIn(GameData gameData) =>
        gameData.StatData;

      public override GameData Write(GameData gameData, ImmutableDictionary<int, StatData> newValue) =>
        gameData.WithStatData(newValue);
    }

    sealed class ItemListsTableId : TableId<StaticItemListData>
    {
      public ItemListsTableId(int id, string tableName) : base(id, tableName)
      {
      }

      public override ImmutableDictionary<int, StaticItemListData> GetIn(GameData gameData) =>
        gameData.ItemLists;

      public override GameData Write(GameData gameData, ImmutableDictionary<int, StaticItemListData> newValue) =>
        gameData.WithItemLists(newValue);
    }

    sealed class UserModifiersTableId : TableId<ModifierData>
    {
      public UserModifiersTableId(int id, string tableName) : base(id, tableName)
      {
      }

      public override ImmutableDictionary<int, ModifierData> GetIn(GameData gameData) =>
        gameData.UserModifiers;

      public override GameData Write(GameData gameData, ImmutableDictionary<int, ModifierData> newValue) =>
        gameData.WithUserModifiers(newValue);
    }

    sealed class CollectionTableId : TableId<CreatureItemData>
    {
      public CollectionTableId(int id, string tableName) : base(id, tableName)
      {
      }

      public override ImmutableDictionary<int, CreatureItemData> GetIn(GameData gameData) =>
        gameData.Collection;

      public override GameData Write(GameData gameData, ImmutableDictionary<int, CreatureItemData> newValue) =>
        gameData.WithCollection(newValue);
    }

    sealed class DeckTableId : TableId<CreatureItemData>
    {
      public DeckTableId(int id, string tableName) : base(id, tableName)
      {
      }

      public override ImmutableDictionary<int, CreatureItemData> GetIn(GameData gameData) =>
        gameData.Deck;

      public override GameData Write(GameData gameData, ImmutableDictionary<int, CreatureItemData> newValue) =>
        gameData.WithDeck(newValue);
    }

    sealed class StatusEffectsTypesTableId : TableId<StatusEffectTypeData>
    {
      public StatusEffectsTypesTableId(int id, string tableName) : base(id, tableName)
      {
      }

      public override ImmutableDictionary<int, StatusEffectTypeData> GetIn(GameData gameData) =>
        gameData.StatusEffects;

      public override GameData Write(GameData gameData, ImmutableDictionary<int, StatusEffectTypeData> newValue) =>
        gameData.WithStatusEffects(newValue);
    }

    sealed class BattleDataTableId : SingletonTableId<BattleData>
    {
      public BattleDataTableId(int id, string tableName) : base(id, tableName)
      {
      }

      protected override BattleData GetSingleton(GameData gameData) =>
        gameData.BattleData;

      protected override GameData WriteSingleton(GameData gameData, BattleData newValue) =>
        gameData.WithBattleData(newValue);
    }
  }
}