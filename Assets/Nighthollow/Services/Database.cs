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

#nullable enable

using System;
using System.Collections.Generic;
using Nighthollow.Utils;
using UnityEngine;

namespace Nighthollow.Services
{
  public sealed class DataService
  {
    public GameDataService GameData { get; }
    public AssetService Assets { get; }
    public UserDataService UserData { get; }

    public DataService(GameDataService gameData, AssetService assets, UserDataService userData)
    {
      GameData = gameData;
      Assets = assets;
      UserData = userData;
    }
  }

  public sealed class Database : MonoBehaviour
  {
    static DataService? _instance;
    static readonly List<Action<DataService>> ReadyList = new List<Action<DataService>>();

    public static bool IsInitialized => _instance != null;

    public static DataService Instance => Errors.CheckNotNull(_instance);

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

    void Start()
    {
      if (_instance == null)
      {
        GameDataService.Initialize(this, gameDataService =>
        {
          AssetService.Initialize(this, gameDataService, assetService =>
          {
            UserDataService.Initialize(this, gameDataService, userDataService =>
            {
              _instance = new DataService(gameDataService, assetService, userDataService);

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
  }
}
