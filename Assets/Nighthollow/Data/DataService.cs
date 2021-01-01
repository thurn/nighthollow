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
using System.Collections.Generic;
using System.IO;
using System.Linq;
using MessagePack;
using MessagePack.Resolvers;
using UnityEngine;

#nullable enable

namespace Nighthollow.Data
{
  public sealed class Database
  {
    readonly MessagePackSerializerOptions _serializationOptions;
    GameData _gameData;
    readonly Dictionary<(ITableId, int), List<IListener>> _listeners;
    readonly List<IMutation> _mutations;
    bool _writePending;

    public Database(MessagePackSerializerOptions serializationOptions, GameData gameData)
    {
      _serializationOptions = serializationOptions;
      _gameData = gameData;
      _listeners = new Dictionary<(ITableId, int), List<IListener>>();
      _mutations = new List<IMutation>();
    }

    /// <summary>
    /// Retrieves a snapshot of the database state. There are no guarantees about how up-to-date this value
    /// will be at any given time, mutations may take an arbitrarily long time to be applied to it.
    /// </summary>
    public GameData Snapshot() => _gameData;

    public void Subscribe<T>(TableId<T> tableId, int entityId, Action<T> action) where T : class
    {
      if (tableId.GetIn(_gameData).ContainsKey(entityId))
      {
        action(tableId.GetIn(_gameData)[entityId]);
      }

      var listenerKey = (tableId, entityId);
      if (!_listeners.ContainsKey(listenerKey))
      {
        _listeners[listenerKey] = new List<IListener>();
      }

      _listeners[listenerKey].Add(new Listener<T>(tableId, entityId, action));
    }

    public void Insert<T>(TableId<T> tableId, int entityId, T value) where T : class
    {
      _mutations.Add(new InsertMutation<T>(tableId, entityId, value));
    }

    public void Update<T>(TableId<T> tableId, int entityId, Func<T?, T> mutation) where T : class
    {
      _mutations.Add(new UpdateMutation<T>(tableId, entityId, mutation));
    }

    public void Delete<T>(TableId<T> tableId, int entityId) where T : class
    {
      _mutations.Add(new DeleteMutation<T>(tableId, entityId));
    }

    public void PerformWrites(string persistentFilePath, string editorFilePath)
    {
      if (_mutations.Count == 0 || _writePending)
      {
        return;
      }

      _writePending = true;

      var listenerIds = new HashSet<(ITableId, int)>();
      GameData gameData = _gameData;
      foreach (var mutation in _mutations)
      {
        gameData = mutation.Apply(gameData);
        listenerIds.Add((mutation.TableId, mutation.EntityId));
      }

      _mutations.Clear();
      using var stream = File.OpenWrite(persistentFilePath);
      MessagePackSerializer.Serialize(stream, gameData, _serializationOptions);
      Debug.Log($"Wrote game data to {persistentFilePath}");

#if UNITY_WEBGL
#pragma warning disable 618
      // See https://forum.unity.com/threads/how-does-saving-work-in-webgl.390385/
      Application.ExternalEval("_JS_FileSystem_Sync();");
#pragma warning restore 618
#endif

#if UNITY_EDITOR
      // In the editor we write directly to assets as well for convenience
      using var editorStream = File.OpenWrite(editorFilePath);
      MessagePackSerializer.Serialize(editorStream, gameData, _serializationOptions);
      Debug.Log($"Wrote game data to {editorFilePath}");
#endif

      _gameData = gameData;

      foreach (var listener in listenerIds
        .Where(listenerId => _listeners.ContainsKey(listenerId))
        .SelectMany(listenerId => _listeners[listenerId]))
      {
        listener.Invoke(gameData);
      }

      _writePending = false;
    }

    public interface IListener
    {
      void Invoke(GameData gameData);

      ITableId TableId { get; }
    }

    sealed class Listener<T> : IListener where T : class
    {
      readonly TableId<T> _tableId;
      readonly Action<T> _listener;

      public Listener(TableId<T> tableId, int entityId, Action<T> listener)
      {
        _tableId = tableId;
        EntityId = entityId;
        _listener = listener;
      }

      public ITableId TableId => _tableId;
      int EntityId { get; }

      public void Invoke(GameData gameData)
      {
        if (_tableId.GetIn(gameData).ContainsKey(EntityId))
        {
          _listener(_tableId.GetIn(gameData)[EntityId]);
        }
        else
        {
          throw new InvalidOperationException(
            $"Error: value does not exist for key {_tableId} and entity ID {EntityId}");
        }
      }
    }

    public interface IMutation
    {
      GameData Apply(GameData gameData);

      ITableId TableId { get; }

      int EntityId { get; }
    }

    abstract class Mutation<T> : IMutation where T : class
    {
      protected Mutation(TableId<T> tableId, int entityId)
      {
        InternalTableId = tableId;
        EntityId = entityId;
      }

      public ITableId TableId => InternalTableId;

      public int EntityId { get; }

      public abstract GameData Apply(GameData gameData);

      protected TableId<T> InternalTableId { get; }
    }

    sealed class InsertMutation<T> : Mutation<T> where T : class
    {
      readonly T _value;

      public InsertMutation(TableId<T> tableId, int entityId, T value) : base(tableId, entityId)
      {
        _value = value;
      }

      public override GameData Apply(GameData gameData) =>
        InternalTableId.Write(gameData, InternalTableId.GetIn(gameData).SetItem(EntityId, _value));
    }

    sealed class UpdateMutation<T> : Mutation<T> where T : class
    {
      readonly Func<T?, T> _update;

      public UpdateMutation(TableId<T> tableId, int entityId, Func<T?, T> update) : base(tableId, entityId)
      {
        _update = update;
      }

      public override GameData Apply(GameData gameData)
      {
        var table = InternalTableId.GetIn(gameData);
        return InternalTableId.Write(gameData,
          table.SetItem(EntityId, _update(table.ContainsKey(EntityId) ? table[EntityId] : null)));
      }
    }

    sealed class DeleteMutation<T> : Mutation<T> where T : class
    {
      public DeleteMutation(TableId<T> tableId, int entityId) : base(tableId, entityId)
      {
      }

      public override GameData Apply(GameData gameData) =>
        InternalTableId.Write(gameData, InternalTableId.GetIn(gameData).Remove(EntityId));
    }
  }

  public interface IOnDatabaseReadyListener
  {
    void OnDatabaseReady(Database database);
  }

  public sealed class DataService : MonoBehaviour
  {
    string? _resourceAddress;
    string? _persistentFilePath;
    string? _editorFilePath;

    Database? _database;
    readonly List<IOnDatabaseReadyListener> _listeners = new List<IOnDatabaseReadyListener>();

    void Start()
    {
      _resourceAddress = Path.Combine("Data", "GameData");
      _persistentFilePath = Path.Combine(Application.persistentDataPath, "GameData.bytes");
      _editorFilePath = Path.Combine(Application.dataPath, "Resources", "Data", "GameData.bytes");

      StartCoroutine(Initialize());
    }

    IEnumerator<YieldInstruction> Initialize()
    {
      var serializationOptions = MessagePackSerializerOptions.Standard
        .WithResolver(CompositeResolver.Create(
          GeneratedResolver.Instance,
          StandardResolver.Instance));

      // This seems like it should not be needed, but MessagePack is ignoring the provided options for dictionaries...
      // MessagePackSerializer.DefaultOptions = serializationOptions;

      GameData gameData;
      if (File.Exists(_persistentFilePath))
      {
        Debug.Log($"Reading game data from {_persistentFilePath}");
        using var file = File.OpenRead(_persistentFilePath!);
        gameData = MessagePackSerializer.Deserialize<GameData>(file, serializationOptions);
      }
      else
      {
        Debug.Log($"Reading game data from {_resourceAddress}");
        var fetch = Resources.LoadAsync<TextAsset>(_resourceAddress!);
        yield return fetch;

        var asset = fetch.asset as TextAsset;
        gameData = asset
          ? MessagePackSerializer.Deserialize<GameData>(asset!.bytes, serializationOptions)
          : new GameData();
      }

      _database = new Database(serializationOptions, gameData);
      InvokeListeners(_database);
    }

    void Update()
    {
      _database?.PerformWrites(_persistentFilePath!, _editorFilePath!);
    }

    public void OnReady(IOnDatabaseReadyListener listener)
    {
      if (_database != null)
      {
        listener.OnDatabaseReady(_database);
      }
      else
      {
        _listeners.Add(listener);
      }
    }

    void InvokeListeners(Database database)
    {
      foreach (var listener in _listeners)
      {
        listener.OnDatabaseReady(database);
      }

      _listeners.Clear();
    }
  }
}