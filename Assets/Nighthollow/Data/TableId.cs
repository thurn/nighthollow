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

#nullable enable

namespace Nighthollow.Data
{
  public interface ITableId
  {
    Type GetUnderlyingType();

    IDictionary GetInUnchecked(GameData gameData);
  }

  public abstract class TableId<T> : ITableId where T : class
  {
    protected TableId(int id)
    {
      Id = id;
    }

    public int Id { get; }

    public abstract ImmutableDictionary<int, T> GetIn(GameData gameData);

    public abstract GameData Write(GameData gameData, ImmutableDictionary<int, T> newValue);

    public Type GetUnderlyingType() => typeof(T);

    public IDictionary GetInUnchecked(GameData gameData) => GetIn(gameData);
  }

  public static class TableId
  {
    public static readonly TableId<TableMetadata> TableMetadata = new TableMetadataTableId(0);
    public static readonly TableId<CreatureTypeData> CreatureTypes = new CreatureTypesTableId(1);
    public static readonly TableId<AffixTypeData> AffixTypes = new AffixTypesTableId(2);
    public static readonly TableId<SkillTypeData> SkillTypes = new SkillTypesTableId(3);
    public static readonly TableId<StatData> StatData = new StatDataTableId(4);
    public static readonly TableId<StaticCreatureListData> CreatureLists = new CreatureListsTableId(5);
    public static readonly TableId<ModifierData> UserModifiers = new UserModifiersTableId(6);
    public static readonly TableId<CreatureItemData> Collection = new CollectionTableId(7);
    public static readonly TableId<CreatureItemData> Deck = new DeckTableId(8);

    sealed class TableMetadataTableId : TableId<TableMetadata>
    {
      public TableMetadataTableId(int id) : base(id)
      {
      }

      public override ImmutableDictionary<int, TableMetadata> GetIn(GameData gameData) =>
        gameData.TableMetadata;

      public override GameData Write(GameData gameData, ImmutableDictionary<int, TableMetadata> newValue) =>
        gameData.WithTableMetadata(newValue);
    }

    sealed class CreatureTypesTableId : TableId<CreatureTypeData>
    {
      public CreatureTypesTableId(int id) : base(id)
      {
      }

      public override ImmutableDictionary<int, CreatureTypeData> GetIn(GameData gameData) =>
        gameData.CreatureTypes;

      public override GameData Write(GameData gameData, ImmutableDictionary<int, CreatureTypeData> newValue) =>
        gameData.WithCreatureTypes(newValue);
    }

    sealed class AffixTypesTableId : TableId<AffixTypeData>
    {
      public AffixTypesTableId(int id) : base(id)
      {
      }

      public override ImmutableDictionary<int, AffixTypeData> GetIn(GameData gameData) =>
        gameData.AffixTypes;

      public override GameData Write(GameData gameData, ImmutableDictionary<int, AffixTypeData> newValue) =>
        gameData.WithAffixTypes(newValue);
    }

    sealed class SkillTypesTableId : TableId<SkillTypeData>
    {
      public SkillTypesTableId(int id) : base(id)
      {
      }

      public override ImmutableDictionary<int, SkillTypeData> GetIn(GameData gameData) =>
        gameData.SkillTypes;

      public override GameData Write(GameData gameData, ImmutableDictionary<int, SkillTypeData> newValue) =>
        gameData.WithSkillTypes(newValue);
    }

    sealed class StatDataTableId : TableId<StatData>
    {
      public StatDataTableId(int id) : base(id)
      {
      }

      public override ImmutableDictionary<int, StatData> GetIn(GameData gameData) =>
        gameData.StatData;

      public override GameData Write(GameData gameData, ImmutableDictionary<int, StatData> newValue) =>
        gameData.WithStatData(newValue);
    }

    sealed class CreatureListsTableId : TableId<StaticCreatureListData>
    {
      public CreatureListsTableId(int id) : base(id)
      {
      }

      public override ImmutableDictionary<int, StaticCreatureListData> GetIn(GameData gameData) =>
        gameData.CreatureLists;

      public override GameData Write(GameData gameData, ImmutableDictionary<int, StaticCreatureListData> newValue) =>
        gameData.WithCreatureLists(newValue);
    }

    sealed class UserModifiersTableId : TableId<ModifierData>
    {
      public UserModifiersTableId(int id) : base(id)
      {
      }

      public override ImmutableDictionary<int, ModifierData> GetIn(GameData gameData) =>
        gameData.UserModifiers;

      public override GameData Write(GameData gameData, ImmutableDictionary<int, ModifierData> newValue) =>
        gameData.WithUserModifiers(newValue);
    }

    sealed class CollectionTableId : TableId<CreatureItemData>
    {
      public CollectionTableId(int id) : base(id)
      {
      }

      public override ImmutableDictionary<int, CreatureItemData> GetIn(GameData gameData) =>
        gameData.Collection;

      public override GameData Write(GameData gameData, ImmutableDictionary<int, CreatureItemData> newValue) =>
        gameData.WithCollection(newValue);
    }

    sealed class DeckTableId : TableId<CreatureItemData>
    {
      public DeckTableId(int id) : base(id)
      {
      }

      public override ImmutableDictionary<int, CreatureItemData> GetIn(GameData gameData) =>
        gameData.Deck;

      public override GameData Write(GameData gameData, ImmutableDictionary<int, CreatureItemData> newValue) =>
        gameData.WithDeck(newValue);
    }
  }
}