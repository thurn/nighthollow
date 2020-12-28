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
using MessagePack;
using UnityEngine;

#nullable enable

namespace Nighthollow.Data
{
  public sealed class Database
  {
    GameData _gameData;
    readonly Dictionary<QueryKey, List<IQueryListener>> _listeners;
    readonly List<IQueryWriter> _writers;
    bool _writePending;

    public Database(GameData gameData)
    {
      _gameData = gameData;
      _listeners = new Dictionary<QueryKey, List<IQueryListener>>();
      _writers = new List<IQueryWriter>();
    }

    /// <summary>
    /// Retrieves a snapshot of the current database state. There are no guarantees about how 'up to date' this value
    /// will be at any given time, use <see cref="Subscribe{T}"/> instead to receive updates over time in sequence.
    /// </summary>
    public GameData Data => _gameData;

    /// <summary>
    /// Registers a listener to be invoked whenever the value for 'query' is written to. The listener will be invoked
    /// once immediately with the current value of the query, and then will be invoked again each time the value is
    /// updated via a corresponding call to <see cref="Write{T}"/>
    /// </summary>
    public void Subscribe<T>(IQuery<T> query, Action<T> listener) => Subscribe(new QueryListener<T>(query, listener));

    /// <summary>Non-generic version of <see cref="Subscribe{T}"/></summary>
    public void Subscribe(IQueryListener listener)
    {
      listener.Invoke(_gameData);

      if (!_listeners.ContainsKey(listener.QueryKey))
      {
        _listeners[listener.QueryKey] = new List<IQueryListener>();
      }

      _listeners[listener.QueryKey].Add(listener);
    }

    /// <summary>
    /// Writes a new value to the database asynchronously. This involves the following three steps:
    ///
    /// 1) The saved file on disk is updated asynchronously with the new content
    /// 2) The value of <see cref="Data"/> is updated
    /// 3) Any registered listeners for this query are invoked with the written value.
    ///
    /// Note that because writes are asynchronous, code should not assume that a write has completed at any particular
    /// time, and should instead rely on <see cref="Subscribe{T}"/> to be notified about changes.
    /// </summary>
    public void Write<T>(IQuery<T> query, T value) => Write(new QueryWriter<T>(query, value));

    /// <summary>Non-generic version of <see cref="Write{T}"/></summary>
    public void Write(IQueryWriter writer)
    {
      _writers.Add(writer);
    }

    /// <summary>
    /// Empties the write queue, performing all pending write operations. Should only be invoked by
    /// <see cref="DataService"/>.
    /// </summary>
    public async void PerformWrites()
    {
      if (_writers.Count == 0 || _writePending)
      {
        return;
      }

      _writePending = true;

      var queryKeys = new HashSet<QueryKey>();
      GameData gameData = _gameData;
      foreach (var writer in _writers)
      {
        gameData = writer.Write(gameData);
        queryKeys.Add(writer.QueryKey);
      }
      _writers.Clear();

      await MessagePackSerializer.SerializeAsync(File.OpenWrite(DataService.GameDataPath), gameData);
      _gameData = gameData;

      foreach (var queryKey in queryKeys)
      {
        if (_listeners.ContainsKey(queryKey))
        {
          foreach (var listener in _listeners[queryKey])
          {
            listener.Invoke(gameData);
          }
        }
      }

      _writePending = false;
    }
  }

  public interface IOnDatabaseReadyListener
  {
    void OnDatabaseReady(Database database);
  }

  public sealed class DataService : MonoBehaviour
  {
    Database? _database;
    public static string GameDataPath => Path.Combine(Application.persistentDataPath, "data.bin");
    readonly List<IOnDatabaseReadyListener> _listeners = new List<IOnDatabaseReadyListener>();

    void Start()
    {
      Initialize();
    }

    async void Initialize()
    {
      if (File.Exists(GameDataPath))
      {
        _database = new Database(await MessagePackSerializer.DeserializeAsync<GameData>(File.OpenRead(GameDataPath)));
        InvokeListeners(_database);
      }
      else
      {
        var gameData = new GameData();
        await MessagePackSerializer.SerializeAsync(File.OpenWrite(GameDataPath), gameData);
        _database = new Database(gameData);
        InvokeListeners(_database);
      }
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
