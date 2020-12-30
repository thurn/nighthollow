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
using System.Collections.Generic;
using System.Collections.Immutable;
using System.IO;
using System.Threading.Tasks;
using MessagePack;
using UnityEngine;

#nullable enable

namespace Nighthollow.Data
{
  public abstract class TableId
  {
    public static readonly TableId<CreatureTypeData> CreatureTypes = new TableId<CreatureTypeData>("CreatureTypes");

    protected TableId(string identifier)
    {
      Identifier = identifier;
    }

    public string Identifier { get; }

    public abstract Task<GameData?> DeserializeAsync(MessagePackSerializerOptions options, GameData gameData);

    public abstract Task SerializeAsync(MessagePackSerializerOptions options, GameData gameData);

    public abstract Type GetUnderlyingType();

    public abstract IDictionary LookUpIn(GameData gameData);
  }

  public sealed class TableId<T> : TableId where T : class
  {
    readonly string _writeFilePath;
    readonly string _readFilePath;

    public TableId(string identifier) : base(identifier)
    {
      _readFilePath = Path.Combine("Data", $"{Identifier}");
      _writeFilePath = Path.Combine(Application.dataPath, "Resources", "Data", $"{Identifier}.bytes");
    }

    public override async Task<GameData?> DeserializeAsync(MessagePackSerializerOptions options, GameData gameData)
    {
      Debug.Log($"Attempting to read: {_readFilePath}");
      var asset = Resources.Load<TextAsset>(_readFilePath);
      if (asset == null)
      {
        Debug.Log("Asset Not Found");
        return null;
      }

      var stream = new MemoryStream(asset.bytes);
      Debug.Log("Deserializing...");
      var result = await MessagePackSerializer.DeserializeAsync<ImmutableDictionary<int, T>>(stream, options);
      Debug.Log("Deserialized.");
      return gameData.SetItem(this, result.ToImmutableDictionary());
    }

    public override async Task SerializeAsync(MessagePackSerializerOptions options, GameData gameData)
    {
      Debug.Log($"Attempting to write: {_writeFilePath}");
      using var stream = File.OpenWrite(_writeFilePath);
      await MessagePackSerializer.SerializeAsync<ImmutableDictionary<int, T>>(stream, gameData.Get(this), options);
    }

    public override Type GetUnderlyingType() => typeof(T);

    public override IDictionary LookUpIn(GameData gameData) => gameData.Get(this);
  }

  public sealed class GameData
  {
    readonly ImmutableDictionary<TableId, IDictionary> _tables;

    public GameData()
    {
      _tables = ImmutableDictionary<TableId, IDictionary>.Empty;
    }

    GameData(ImmutableDictionary<TableId, IDictionary>? value = null)
    {
      _tables = value ?? ImmutableDictionary<TableId, IDictionary>.Empty;
    }

    public bool ContainsKey<T>(TableId<T> tableId) where T : class => _tables.ContainsKey(tableId);

    public ImmutableDictionary<int, T> Get<T>(TableId<T> tableId) where T : class =>
      (ImmutableDictionary<int, T>) _tables[tableId];

    public GameData SetItem<T>(TableId<T> tableId, ImmutableDictionary<int, T> value) where T : class =>
      new GameData(_tables.SetItem(tableId, value));

    public IEnumerable<TableId> Keys => _tables.Keys;
  }
}
