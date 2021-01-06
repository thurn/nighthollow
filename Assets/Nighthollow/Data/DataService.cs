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

using System.Collections.Generic;
using System.IO;
using MessagePack;
using MessagePack.Resolvers;
using UnityEngine;

#nullable enable

namespace Nighthollow.Data
{
  public interface IOnDatabaseReadyListener
  {
    void OnDatabaseReady(Database database);
  }

  public sealed class DataService : MonoBehaviour
  {
    Database? _database;
    readonly List<IOnDatabaseReadyListener> _listeners = new List<IOnDatabaseReadyListener>();
    [SerializeField] bool _disablePersistence;

    void Start()
    {
      StartCoroutine(Initialize());
    }

    static string ResourceAddress(ITableId tableId) => $"Data/{tableId.TableName}";

    public static string PersistentFilePath(ITableId tableId) =>
      Path.Combine(Application.persistentDataPath, $"{tableId.TableName}.bytes");

    public static string EditorFilePath(ITableId tableId) =>
      Path.Combine(Application.dataPath, "Resources", "Data", $"{tableId.TableName}.bytes");

    IEnumerator<YieldInstruction> Initialize()
    {
      var serializationOptions = MessagePackSerializerOptions.Standard
        .WithResolver(CompositeResolver.Create(
          GeneratedResolver.Instance,
          StandardResolverAllowPrivate.Instance));

      GameData gameData = new GameData();
      File.Delete(Path.Combine(Application.persistentDataPath, $"GameData.bytes"));

      foreach (var tableId in TableId.AllTableIds.Reverse())
      {
        var persistentFilePath = PersistentFilePath(tableId);
        File.Delete(persistentFilePath);

        if (File.Exists(persistentFilePath) && !_disablePersistence)
        {
          Debug.Log($"Reading game data from {persistentFilePath}");
          using var file = File.OpenRead(persistentFilePath!);
          gameData = tableId.Deserialize(gameData, file, serializationOptions);
        }
        else
        {
          Debug.Log($"Reading game data from {ResourceAddress(tableId)}");
          var fetch = Resources.LoadAsync<TextAsset>(ResourceAddress(tableId!)!);
          yield return fetch;

          var asset = fetch.asset as TextAsset;
          if (asset && asset != null)
          {
            gameData = tableId.Deserialize(gameData, asset.bytes, serializationOptions);
          }
        }
      }

      _database = new Database(serializationOptions, gameData);
      InvokeListeners(_database);
    }

    void Update()
    {
      _database?.PerformWritesInternal(_disablePersistence);
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
