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
using Nighthollow.Utils;
using SimpleJSON;
using UnityEngine;

#nullable enable

namespace Nighthollow.Services
{
  public sealed class DataService
  {
    public DataService(
      GameDataService gameData,
      AssetService assets,
      UserDataService userData,
      GameConfig currentGameConfig)
    {
      GameData = gameData;
      Assets = assets;
      UserData = userData;
      CurrentGameConfig = currentGameConfig;
    }

    public GameDataService GameData { get; }
    public AssetService Assets { get; }
    public UserDataService UserData { get; }
    public GameConfig CurrentGameConfig { get; }
  }

  public sealed class Database : MonoBehaviour
  {
    const string UserDataKey = "userData";
    const string CurrentGameConfigKey = "currentGameConfig";

    static DataService? _instance;
    static readonly List<Action<DataService>> ReadyList = new List<Action<DataService>>();

    public static bool IsInitialized => _instance != null;

    public static DataService Instance => Errors.CheckNotNull(_instance);

    string DataPath => Path.Combine(Application.persistentDataPath, "data.json");

    void Start()
    {
      if (_instance == null)
      {
        GameDataService.Initialize(this, gameDataService =>
        {
          AssetService.Initialize(this, gameDataService, assetService =>
          {
            LoadDatabase(json =>
            {
              if (json == null)
              {
                _instance = new DataService(
                  gameDataService,
                  assetService,
                  new UserDataService(this, gameDataService),
                  new GameConfig(gameDataService));
                Save();
              }
              else
              {
                _instance = new DataService(
                  gameDataService,
                  assetService,
                  new UserDataService(this, gameDataService, json[UserDataKey]),
                  new GameConfig(gameDataService, json[CurrentGameConfigKey]));
              }

              foreach (var onReady in ReadyList)
              {
                onReady(_instance);
              }

              ReadyList.Clear();
            });
          });
        });
      }
    }

    public static void OnReady(Action<DataService> action)
    {
      if (_instance == null)
      {
        ReadyList.Add(action);
      }
      else
      {
        action(_instance);
      }
    }

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
    static void Init()
    {
      _instance = null;
    }

    void LoadDatabase(Action<JSONNode?> onComplete)
    {
      onComplete(File.Exists(DataPath) ? JSON.Parse(File.ReadAllText(DataPath)) : null);
    }

    public void Save()
    {
      Debug.Log($"Saving to {DataPath}");
      if (_instance == null)
      {
        throw new InvalidOperationException("Attempted to save database before initialization.");
      }

      var result = new JSONObject
      {
        [UserDataKey] = _instance.UserData.Serialize(),
        [CurrentGameConfigKey] = _instance.CurrentGameConfig.Serialize()
      };

      File.WriteAllText(DataPath, result.ToString());

#pragma warning disable 618
      Application.ExternalEval("_JS_FileSystem_Sync();");
#pragma warning restore 618
    }
  }
}
