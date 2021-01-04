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
    string? _resourceAddress;
    string? _persistentFilePath;
    string? _editorFilePath;
    Database? _database;
    readonly List<IOnDatabaseReadyListener> _listeners = new List<IOnDatabaseReadyListener>();
    [SerializeField] bool _disablePersistence;

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
          StandardResolverAllowPrivate.Instance));

      File.Delete(_persistentFilePath!);

      GameData gameData;
      if (File.Exists(_persistentFilePath) && !_disablePersistence)
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
      _database?.PerformWritesInternal(_disablePersistence, _persistentFilePath!, _editorFilePath!);
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