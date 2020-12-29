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
using System.IO;
using System.Reflection;
using UnityEngine;

#nullable enable

namespace Nighthollow.Data
{
  public sealed class Database
  {
    GameData _gameData;
    readonly Dictionary<ListenerId, List<IListener>> _listeners;
    readonly List<IMutation> _mutations;
    bool _writePending;

    public Database(GameData gameData)
    {
      _gameData = gameData;
      _listeners = new Dictionary<ListenerId, List<IListener>>();
      _mutations = new List<IMutation>();
    }

    /// <summary>
    /// Retrieves a snapshot of the database state. There are no guarantees about how up-to-date this value
    /// will be at any given time, mutations may take an arbitrarily long time to be applied to it.
    /// </summary>
    public GameData Snapshot() => _gameData;

    public void Subscribe<T>(Key<T> key, int entityId, Action<T> action) where T : class
    {
      if (_gameData.ContainsKey(key) && _gameData.Get(key).ContainsKey(entityId))
      {
        action(_gameData.Get(key)[entityId]);
      }

      var listenerKey = new ListenerId(key, entityId);
      if (!_listeners.ContainsKey(listenerKey))
      {
        _listeners[listenerKey] = new List<IListener>();
      }

      _listeners[listenerKey].Add(new Listener<T>(key, entityId, action));
    }

    public void Insert<T>(Key<T> key, int entityId, T value) where T : class
    {
      _mutations.Add(new Writer<T>(key, entityId, value));
    }

    public void Mutate<T>(Key<T> key, int entityId, Func<T?, T> mutation) where T : class
    {
      _mutations.Add(new Mutation<T>(key, entityId, mutation));
    }

    public async void PerformWrites()
    {
      if (_mutations.Count == 0 || _writePending)
      {
        return;
      }

      _writePending = true;

      var listenerIds = new HashSet<ListenerId>();
      var keys = new HashSet<Key>();
      GameData gameData = _gameData;
      foreach (var mutation in _mutations)
      {
        gameData = mutation.Apply(gameData);
        listenerIds.Add(mutation.ListenerId);
        keys.Add(mutation.Key);
      }

      _mutations.Clear();

      foreach (var key in keys)
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

    sealed class ListenerId
    {
      public ListenerId(Key key, int entityId)
      {
        Key = key;
        EntityId = entityId;
      }

      Key Key { get; }
      int EntityId { get; }

      bool Equals(ListenerId other) => Key.Equals(other.Key) && EntityId == other.EntityId;

      public override bool Equals(object? obj) =>
        ReferenceEquals(this, obj) || obj is ListenerId other && Equals(other);

      public override int GetHashCode()
      {
        unchecked
        {
          return (Key.GetHashCode() * 397) ^ EntityId;
        }
      }

      public static bool operator ==(ListenerId? left, ListenerId? right) => Equals(left, right);

      public static bool operator !=(ListenerId? left, ListenerId? right) => !Equals(left, right);
    }

    interface IListener
    {
      void Invoke(GameData gameData);
    }

    sealed class Listener<T> : IListener where T : class
    {
      readonly Key<T> _key;
      readonly int _entityId;
      readonly Action<T> _listener;

      public Listener(Key<T> key, int entityId, Action<T> listener)
      {
        _key = key;
        _entityId = entityId;
        _listener = listener;
      }

      public void Invoke(GameData gameData)
      {
        if (gameData.ContainsKey(_key) && gameData.Get(_key).ContainsKey(_entityId))
        {
          _listener(gameData.Get(_key)[_entityId]);
        }
        else
        {
          throw new InvalidOperationException($"Error: value does not exist for key {_key} and entity ID {_entityId}");
        }
      }
    }

    interface IMutation
    {
      GameData Apply(GameData gameData);

      ListenerId ListenerId { get; }

      Key Key { get; }
    }

    sealed class Writer<T> : IMutation where T : class
    {
      readonly Key<T> _key;
      readonly int _entityId;
      readonly T _value;

      public Writer(Key<T> key, int entityId, T value)
      {
        _key = key;
        _entityId = entityId;
        _value = value;
        ListenerId = new ListenerId(key, entityId);
      }

      public ListenerId ListenerId { get; }

      public Key Key => _key;

      public GameData Apply(GameData gameData) =>
        gameData.SetItem(_key,
          gameData.ContainsKey(_key)
            ? gameData.Get(_key).SetItem(_entityId, _value)
            : ImmutableDictionary<int, T>.Empty.SetItem(_entityId, _value));
    }

    sealed class Mutation<T> : IMutation where T : class
    {
      readonly Key<T> _key;
      readonly int _entityId;
      readonly Func<T?, T> _mutation;

      public Mutation(Key<T> key, int entityId, Func<T?, T> mutation)
      {
        _key = key;
        _entityId = entityId;
        _mutation = mutation;

        ListenerId = new ListenerId(key, entityId);
      }

      public ListenerId ListenerId { get; }
      public Key Key => _key;

      public GameData Apply(GameData gameData)
      {
        if (gameData.ContainsKey(_key))
        {
          var table = gameData.Get(_key);
          return gameData.SetItem(_key,
            table.SetItem(_entityId, _mutation(table.ContainsKey(_entityId) ? table[_entityId] : null)));
        }
        else
        {
          return gameData.SetItem(_key, ImmutableDictionary<int, T>.Empty.SetItem(_entityId, _mutation(null)));
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
      Initialize();
    }

    async void Initialize()
    {
      var gameData = new GameData();

      foreach (var field in typeof(Key).GetFields(BindingFlags.Public | BindingFlags.Static))
      {
        var key = (Key) field.GetValue(typeof(Key));
        if (File.Exists(key.SaveFilePath()))
        {
          gameData = await key.DeserializeAsync(gameData);
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
}
