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
using System.Collections.Immutable;
using System.Reflection;
using MessagePack;
using MessagePack.Resolvers;
using UnityEngine;

#nullable enable

namespace Nighthollow.Data
{
  public sealed class Database
  {
    GameData _gameData;
    readonly Dictionary<(TableId, int), List<IListener>> _listeners;
    readonly List<IMutation> _mutations;
    bool _writePending;

    public Database(GameData gameData)
    {
      _gameData = gameData;
      _listeners = new Dictionary<(TableId, int), List<IListener>>();
      _mutations = new List<IMutation>();
    }

    /// <summary>
    /// Retrieves a snapshot of the database state. There are no guarantees about how up-to-date this value
    /// will be at any given time, mutations may take an arbitrarily long time to be applied to it.
    /// </summary>
    public GameData Snapshot() => _gameData;

    public void Subscribe<T>(TableId<T> tableId, int entityId, Action<T> action) where T : class
    {
      if (_gameData.ContainsKey(tableId) && _gameData.Get(tableId).ContainsKey(entityId))
      {
        action(_gameData.Get(tableId)[entityId]);
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
      _mutations.Add(new Writer<T>(tableId, entityId, value));
    }

    public void Mutate<T>(TableId<T> tableId, int entityId, Func<T?, T> mutation) where T : class
    {
      _mutations.Add(new Mutation<T>(tableId, entityId, mutation));
    }

    public async void PerformWrites()
    {
      if (_mutations.Count == 0 || _writePending)
      {
        return;
      }

      _writePending = true;

      var listenerIds = new HashSet<(TableId, int)>();
      var tableIds = new HashSet<TableId>();
      GameData gameData = _gameData;
      foreach (var mutation in _mutations)
      {
        gameData = mutation.Apply(gameData);
        listenerIds.Add((mutation.TableId, mutation.EntityId));
        tableIds.Add(mutation.TableId);
      }

      _mutations.Clear();

      foreach (var key in tableIds)
      {
        await key.SerializeAsync(gameData);
      }

      _gameData = gameData;

      foreach (var listenerId in listenerIds)
      {
        if (_listeners.ContainsKey(listenerId))
        {
          foreach (var listener in _listeners[listenerId])
          {
            listener.Invoke(gameData);
          }
        }
      }

      _writePending = false;
    }

    public interface IListener
    {
      void Invoke(GameData gameData);

      TableId TableId { get; }

      int EntityId { get; }
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

      public TableId TableId => _tableId;
      public int EntityId { get; }

      public void Invoke(GameData gameData)
      {
        if (gameData.ContainsKey(_tableId) && gameData.Get(_tableId).ContainsKey(EntityId))
        {
          _listener(gameData.Get(_tableId)[EntityId]);
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

      TableId TableId { get; }

      int EntityId { get; }
    }

    sealed class Writer<T> : IMutation where T : class
    {
      readonly T _value;
      readonly TableId<T> _tableId;

      public Writer(TableId<T> tableId, int entityId, T value)
      {
        _tableId = tableId;
        EntityId = entityId;
        _value = value;
      }

      public TableId TableId => _tableId;
      public int EntityId { get; }

      public GameData Apply(GameData gameData) =>
        gameData.SetItem(_tableId,
          gameData.ContainsKey(_tableId)
            ? gameData.Get(_tableId).SetItem(EntityId, _value)
            : ImmutableDictionary<int, T>.Empty.SetItem(EntityId, _value));
    }

    sealed class Mutation<T> : IMutation where T : class
    {
      readonly TableId<T> _tableId;
      readonly Func<T?, T> _mutation;

      public Mutation(TableId<T> tableId, int entityId, Func<T?, T> mutation)
      {
        _tableId = tableId;
        EntityId = entityId;
        _mutation = mutation;
      }

      public TableId TableId => _tableId;
      public int EntityId { get; }

      public GameData Apply(GameData gameData)
      {
        if (gameData.ContainsKey(_tableId))
        {
          var table = gameData.Get(_tableId);
          return gameData.SetItem(_tableId,
            table.SetItem(EntityId, _mutation(table.ContainsKey(EntityId) ? table[EntityId] : null)));
        }
        else
        {
          return gameData.SetItem(_tableId, ImmutableDictionary<int, T>.Empty.SetItem(EntityId, _mutation(null)));
        }
      }
    }
  }

  public interface IOnDatabaseReadyListener
  {
    void OnDatabaseReady(Database database);
  }

  public sealed class DataService : MonoBehaviour
  {
    Database? _database;
    readonly List<IOnDatabaseReadyListener> _listeners = new List<IOnDatabaseReadyListener>();

    void Start()
    {
      MessagePackInitializer.Initialize();
      Initialize();
    }

    async void Initialize()
    {
      var gameData = new GameData();

      foreach (var field in typeof(TableId).GetFields(BindingFlags.Public | BindingFlags.Static))
      {
        var key = (TableId) field.GetValue(typeof(TableId));
        var result = await key.DeserializeAsync(gameData);
        if (result != null)
        {
          gameData = result;
        }
      }

      _database = new Database(gameData);
      InvokeListeners(_database);
    }

    void Update()
    {
      _database?.PerformWrites();
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

  public static class MessagePackInitializer
  {
    static bool _serializerRegistered;

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    public static void Initialize()
    {
      if (!_serializerRegistered)
      {
        Debug.Log("Registering MessagePack Serializers");
        StaticCompositeResolver.Instance.Register(
          GeneratedResolver.Instance,
          StandardResolver.Instance
        );

        var option = MessagePackSerializerOptions.Standard.WithResolver(StaticCompositeResolver.Instance);
        MessagePackSerializer.DefaultOptions = option;
        _serializerRegistered = true;
      }
    }

#if UNITY_EDITOR
    [UnityEditor.InitializeOnLoadMethod]
    static void EditorInitialize()
    {
      Initialize();
    }
#endif
  }
}
