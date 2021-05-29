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
using System.Linq;
using MessagePack;
using Nighthollow.Utils;
using UnityEngine;

#nullable enable

namespace Nighthollow.Data
{
  public sealed class Database
  {
    readonly MessagePackSerializerOptions _serializationOptions;

    ImmutableDictionary<ITableId, ImmutableList<Action>> _tableUpdatedListeners;
    ImmutableDictionary<(ITableId, int), ImmutableList<IListener>> _updatedListeners;
    ImmutableDictionary<ITableId, ImmutableList<IListener>> _addedListeners;
    ImmutableDictionary<ITableId, ImmutableList<IListener>> _removedListeners;
    ImmutableList<EntityEvent> _events;
    GameData _gameData;
    bool _writeRequired;

    public Database(MessagePackSerializerOptions serializationOptions, GameData gameData)
    {
      _serializationOptions = serializationOptions;
      _gameData = gameData;
      _tableUpdatedListeners = ImmutableDictionary<ITableId, ImmutableList<Action>>.Empty;
      _updatedListeners = ImmutableDictionary<(ITableId, int), ImmutableList<IListener>>.Empty;
      _addedListeners = ImmutableDictionary<ITableId, ImmutableList<IListener>>.Empty;
      _removedListeners = ImmutableDictionary<ITableId, ImmutableList<IListener>>.Empty;
      _events = ImmutableList<EntityEvent>.Empty;
    }

    /// <summary>
    /// Retrieves a snapshot of the database state. There are no guarantees about how up-to-date this value
    /// will be at any given time, mutations may take an arbitrarily long time to be applied to it.
    /// </summary>
    public GameData Snapshot() => _gameData;

    public void OnTableUpdated<T>(TableId<T> tableId, Action action) where T : class
    {
      if (!_tableUpdatedListeners.ContainsKey(tableId))
      {
        _tableUpdatedListeners = _tableUpdatedListeners.Add(tableId, ImmutableList<Action>.Empty);
      }

      _tableUpdatedListeners = _tableUpdatedListeners.SetItem(
        tableId,
        _tableUpdatedListeners[tableId].Add(action));
    }

    public void OnEntityUpdated<T>(TableId<T> tableId, int entityId, Action<T> action) where T : class
    {
      if (tableId.GetIn(_gameData).ContainsKey(entityId))
      {
        action(tableId.GetIn(_gameData)[entityId]);
      }

      var key = (tableId, entityId);
      if (!_updatedListeners.ContainsKey(key))
      {
        _updatedListeners = _updatedListeners.Add(key, ImmutableList<IListener>.Empty);
      }

      _updatedListeners = _updatedListeners.SetItem(
        key,
        _updatedListeners[key].Add(new EntityUpdatedListener<T>(tableId, action)));
    }

    public void OnEntityAdded<T>(TableId<T> tableId, Action<int, T> action) where T : class
    {
      if (!_addedListeners.ContainsKey(tableId))
      {
        _addedListeners = _addedListeners.Add(tableId, ImmutableList<IListener>.Empty);
      }

      _addedListeners = _addedListeners.SetItem(
        tableId,
        _addedListeners[tableId].Add(new EntityAddedListener<T>(tableId, action)));
    }

    public void OnEntityRemoved<T>(TableId<T> tableId, Action<int> action) where T : class
    {
      if (!_removedListeners.ContainsKey(tableId))
      {
        _removedListeners = _removedListeners.Add(tableId, ImmutableList<IListener>.Empty);
      }

      _removedListeners = _removedListeners.SetItem(
        tableId,
        _removedListeners[tableId].Add(new EntityRemovedListener(action)));
    }

    /// <summary>
    /// Inserts a new entity into the provided table, assigning a new ID to it.
    /// </summary>
    public void Insert<T>(TableId<T> tableId, T value) where T : class
    {
      var metadata = _gameData.TableMetadata.GetOrReturnDefault(tableId.Id, new TableMetadata());
      var newId = metadata.NextId;
      _events = _events.Add(new EntityEvent(EntityEventType.Added, tableId, newId));
      _events = _events.Add(new EntityEvent(EntityEventType.Updated, TableId.TableMetadata, tableId.Id));
      _gameData = tableId.Write(
        _gameData.WithTable(
          TableId.TableMetadata,
          _gameData.TableMetadata.SetItem(tableId.Id, metadata.WithNextId(newId + 1))),
        tableId.GetIn(_gameData).SetItem(newId, value));
      _writeRequired = true;
    }

    public void InsertRange<T>(TableId<T> tableId, IEnumerable<T> values) where T : class
    {
      var events = ImmutableList.CreateBuilder<EntityEvent>();
      foreach (var value in values)
      {
        var metadata = _gameData.TableMetadata.GetOrReturnDefault(tableId.Id, new TableMetadata());
        var newId = metadata.NextId;
        events.Add(new EntityEvent(EntityEventType.Added, tableId, newId));
        events.Add(new EntityEvent(EntityEventType.Updated, TableId.TableMetadata, tableId.Id));
        _gameData = tableId.Write(
          _gameData.WithTable(
            TableId.TableMetadata,
            _gameData.TableMetadata.SetItem(tableId.Id, metadata.WithNextId(newId + 1))),
          tableId.GetIn(_gameData).SetItem(newId, value));
      }

      _events = _events.AddRange(events);
      _writeRequired = true;
    }

    /// <summary>
    /// Mutates an entity within a table by applying the provided mutation function to it. If no entity with the
    /// provided ID exists, it will be silently ignored.
    /// </summary>
    public void Update<T>(TableId<T> tableId, int entityId, Func<T, T> mutation) where T : class
    {
      UpdateInternal(tableId, entityId, null, mutation);
    }

    /// <summary>
    /// Equivalent to <see cref="Update{T}"/>, except that if no entity with the provided ID is found, the mutation
    /// function will be applied to <paramref name="defaultValue"/> instead.
    /// </summary>
    public void Upsert<T>(TableId<T> tableId, int entityId, T defaultValue, Func<T, T> mutation) where T : class
    {
      UpdateInternal(tableId, entityId, defaultValue, mutation);
    }

    void UpdateInternal<T>(TableId<T> tableId, int entityId, T? defaultValue, Func<T, T> update) where T : class
    {
      var table = tableId.GetIn(_gameData);
      T result;
      if (table.ContainsKey(entityId))
      {
        result = update(table[entityId]);
      }
      else if (defaultValue != null)
      {
        result = update(defaultValue);
      }
      else
      {
        return;
      }

      _events = _events.Add(new EntityEvent(EntityEventType.Updated, tableId, entityId));
      _gameData = tableId.Write(_gameData, table.SetItem(entityId, Errors.CheckNotNull(result)));
      _writeRequired = true;
    }

    /// <summary>
    /// Removes an entity from a table. If no entity with the provided ID exists, it will be silently ignored.
    /// </summary>
    public void Delete<T>(TableId<T> tableId, int entityId) where T : class
    {
      _events = _events.Add(new EntityEvent(EntityEventType.Removed, tableId, entityId));
      _gameData = tableId.Write(_gameData, tableId.GetIn(_gameData).Remove(entityId));
      _writeRequired = true;
    }

    public void ClearListeners()
    {
      _addedListeners = ImmutableDictionary<ITableId, ImmutableList<IListener>>.Empty;
      _updatedListeners = ImmutableDictionary<(ITableId, int), ImmutableList<IListener>>.Empty;
      _removedListeners = ImmutableDictionary<ITableId, ImmutableList<IListener>>.Empty;
    }

    /// <summary>Should only be invoked from <see cref="DataService"/>.</summary>
    public void PerformWritesInternal(bool disablePersistence)
    {
      if (!_writeRequired)
      {
        return;
      }

      var tableIds = _events.Select(e => e.TableId).ToImmutableHashSet();

      if (!disablePersistence)
      {
        foreach (var tableId in tableIds)
        {
          using var stream = File.OpenWrite(DataService.PersistentFilePath(tableId));
          tableId.Serialize(_gameData, stream, _serializationOptions);
          Debug.Log($"Wrote game data to {DataService.PersistentFilePath(tableId)}");
        }

        if (!Application.isEditor)
        {
#pragma warning disable 618
          // See https://forum.unity.com/threads/how-does-saving-work-in-webgl.390385/
          Application.ExternalEval("_JS_FileSystem_Sync();");
#pragma warning restore 618
        }
      }

#if UNITY_EDITOR
      // In the editor we write directly to assets too
      foreach (var tableId in tableIds)
      {
        using var editorStream = File.OpenWrite(DataService.EditorFilePath(tableId));
        tableId.Serialize(_gameData, editorStream, _serializationOptions);
        Debug.Log($"Wrote game data to {DataService.EditorFilePath(tableId)}");
      }
#endif

      var events = _events;
      _events = ImmutableList<EntityEvent>.Empty;
      FireEvents(events, tableIds, _gameData);
      _writeRequired = false;
    }

    void FireEvents(ImmutableList<EntityEvent> events, ImmutableHashSet<ITableId> tableIds, GameData gameData)
    {
      foreach (var entityEvent in events)
      {
        switch (entityEvent.EventType)
        {
          case EntityEventType.Added:
            if (_addedListeners.ContainsKey(entityEvent.TableId))
            {
              foreach (var listener in _addedListeners[entityEvent.TableId])
              {
                listener.Invoke(gameData, entityEvent.EntityId);
              }
            }

            break;
          case EntityEventType.Updated:
            var key = (entityEvent.TableId, entityEvent.EntityId);
            if (_updatedListeners.ContainsKey(key))
            {
              foreach (var listener in _updatedListeners[key])
              {
                listener.Invoke(gameData, entityEvent.EntityId);
              }
            }

            break;
          case EntityEventType.Removed:
            if (_removedListeners.ContainsKey(entityEvent.TableId))
            {
              foreach (var listener in _removedListeners[entityEvent.TableId])
              {
                listener.Invoke(gameData, entityEvent.EntityId);
              }
            }

            break;
          default:
            throw new ArgumentOutOfRangeException();
        }
      }

      foreach (var action in tableIds
        .Where(tableId => _tableUpdatedListeners.ContainsKey(tableId))
        .SelectMany(tableId => _tableUpdatedListeners[tableId]))
      {
        action();
      }
    }

    enum EntityEventType
    {
      Added,
      Updated,
      Removed
    }

    sealed class EntityEvent
    {
      public EntityEvent(EntityEventType eventType, ITableId tableId, int entityId)
      {
        EventType = eventType;
        TableId = tableId;
        EntityId = entityId;
      }

      public EntityEventType EventType { get; }
      public ITableId TableId { get; }
      public int EntityId { get; }
    }

    interface IListener
    {
      void Invoke(GameData gameData, int entityId);
    }

    sealed class EntityUpdatedListener<T> : IListener where T : class
    {
      readonly TableId<T> _tableId;
      readonly Action<T> _listener;

      public EntityUpdatedListener(TableId<T> tableId, Action<T> listener)
      {
        _tableId = tableId;
        _listener = listener;
      }

      public void Invoke(GameData gameData, int entityId)
      {
        if (_tableId.GetIn(gameData).ContainsKey(entityId))
        {
          _listener(_tableId.GetIn(gameData)[entityId]);
        }
        else
        {
          throw new InvalidOperationException(
            $"Error: value does not exist for key {_tableId} and entity ID {entityId}");
        }
      }
    }

    sealed class EntityAddedListener<T> : IListener where T : class
    {
      readonly TableId<T> _tableId;
      readonly Action<int, T> _listener;

      public EntityAddedListener(TableId<T> tableId, Action<int, T> listener)
      {
        _tableId = tableId;
        _listener = listener;
      }

      public void Invoke(GameData gameData, int entityId)
      {
        if (_tableId.GetIn(gameData).ContainsKey(entityId))
        {
          _listener(entityId, _tableId.GetIn(gameData)[entityId]);
        }
        else
        {
          throw new InvalidOperationException(
            $"Error: value does not exist for key {_tableId} and entity ID {entityId}");
        }
      }
    }

    sealed class EntityRemovedListener : IListener
    {
      readonly Action<int> _listener;

      public EntityRemovedListener(Action<int> listener)
      {
        _listener = listener;
      }

      public void Invoke(GameData gameData, int entityId)
      {
        _listener(entityId);
      }
    }
  }
}