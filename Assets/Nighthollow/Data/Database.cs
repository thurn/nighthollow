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
using Nighthollow.Utils;
using UnityEngine;

#nullable enable

namespace Nighthollow.Data
{
  public sealed class Database
  {
    readonly MessagePackSerializerOptions _serializationOptions;
    readonly Dictionary<(ITableId, int), List<IListener>> _updatedListeners;
    readonly Dictionary<ITableId, List<IListener>> _addedListeners;
    readonly Dictionary<ITableId, List<IListener>> _removedListeners;
    readonly List<IMutation> _mutations;

    GameData _gameData;
    bool _writePending;

    public Database(MessagePackSerializerOptions serializationOptions, GameData gameData)
    {
      _serializationOptions = serializationOptions;
      _gameData = gameData;
      _updatedListeners = new Dictionary<(ITableId, int), List<IListener>>();
      _addedListeners = new Dictionary<ITableId, List<IListener>>();
      _removedListeners = new Dictionary<ITableId, List<IListener>>();
      _mutations = new List<IMutation>();
    }

    /// <summary>
    /// Retrieves a snapshot of the database state. There are no guarantees about how up-to-date this value
    /// will be at any given time, mutations may take an arbitrarily long time to be applied to it.
    /// </summary>
    public GameData Snapshot() => _gameData;

    public void OnEntityUpdated<T>(TableId<T> tableId, int entityId, Action<T> action) where T : class
    {
      if (tableId.GetIn(_gameData).ContainsKey(entityId))
      {
        action(tableId.GetIn(_gameData)[entityId]);
      }

      _updatedListeners.GetOrInsertDefault((tableId, entityId), new List<IListener>())
        .Add(new EntityUpdatedListener<T>(tableId, action));
    }

    public void OnEntityAdded<T>(TableId<T> tableId, Action<int, T> action) where T : class
    {
      _addedListeners.GetOrInsertDefault(tableId, new List<IListener>())
        .Add(new EntityAddedListener<T>(tableId, action));
    }

    public void OnEntityRemoved<T>(TableId<T> tableId, Action<int> action) where T : class
    {
      _removedListeners.GetOrInsertDefault(tableId, new List<IListener>())
        .Add(new EntityRemovedListener(action));
    }

    public void Insert<T>(TableId<T> tableId, T value) where T : class
    {
      _mutations.Add(new InsertMutation<T>(tableId, value));
    }

    public void Update<T>(TableId<T> tableId, int entityId, Func<T?, T> mutation) where T : class
    {
      _mutations.Add(new UpdateMutation<T>(tableId, entityId, mutation));
    }

    public void Delete<T>(TableId<T> tableId, int entityId) where T : class
    {
      _mutations.Add(new DeleteMutation<T>(tableId, entityId));
    }

    /// <summary>Should only be invoked from <see cref="DataService"/>.</summary>
    public void PerformWritesInternal(string persistentFilePath, string editorFilePath)
    {
      if (_mutations.Count == 0 || _writePending)
      {
        return;
      }

      _writePending = true;

      var events = new List<EntityEvent>();
      GameData gameData = _gameData;
      foreach (var mutation in _mutations)
      {
        gameData = mutation.Apply(gameData, events);
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
      FireEvents(events, gameData);
      _writePending = false;
    }

    void FireEvents(IEnumerable<EntityEvent> events, GameData gameData)
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

    interface IMutation
    {
      GameData Apply(GameData gameData, List<EntityEvent> events);
    }

    sealed class InsertMutation<T> : IMutation where T : class
    {
      readonly TableId<T> _tableId;
      readonly T _value;

      public InsertMutation(TableId<T> tableId, T value)
      {
        _tableId = tableId;
        _value = value;
      }

      public GameData Apply(GameData gameData, List<EntityEvent> events)
      {
        var metadata = gameData.TableMetadata.GetOrReturnDefault(_tableId.Id, new TableMetadata());
        var newId = metadata.NextId;
        events.Add(new EntityEvent(EntityEventType.Added, _tableId, newId));
        return _tableId.Write(
          gameData.WithTableMetadata(gameData.TableMetadata.SetItem(_tableId.Id, metadata.WithNextId(newId + 1))),
          _tableId.GetIn(gameData).SetItem(newId, _value));
      }
    }

    sealed class UpdateMutation<T> : IMutation where T : class
    {
      readonly TableId<T> _tableId;
      readonly int _entityId;
      readonly Func<T?, T> _update;

      public UpdateMutation(TableId<T> tableId, int entityId, Func<T?, T> update)
      {
        _tableId = tableId;
        _entityId = entityId;
        _update = update;
      }

      public GameData Apply(GameData gameData, List<EntityEvent> events)
      {
        var table = _tableId.GetIn(gameData);
        events.Add(new EntityEvent(EntityEventType.Updated, _tableId, _entityId));
        return _tableId.Write(gameData,
          table.SetItem(_entityId, _update(table.ContainsKey(_entityId) ? table[_entityId] : null)));
      }
    }

    sealed class DeleteMutation<T> : IMutation where T : class
    {
      readonly TableId<T> _tableId;
      readonly int _entityId;

      public DeleteMutation(TableId<T> tableId, int entityId)
      {
        _tableId = tableId;
        _entityId = entityId;
      }

      public GameData Apply(GameData gameData, List<EntityEvent> events)
      {
        events.Add(new EntityEvent(EntityEventType.Removed, _tableId, _entityId));
        return _tableId.Write(gameData, _tableId.GetIn(gameData).Remove(_entityId));
      }
    }
  }
}