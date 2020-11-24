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
using System.IO;
using System.Linq;
using Nighthollow.Data;
using Nighthollow.Generated;
using Nighthollow.Stats;
using Nighthollow.Utils;
using SimpleJSON;
using UnityEngine;

namespace Nighthollow.Services
{
  public sealed class UserDataService2
  {
    public IReadOnlyList<CreatureItemData> Deck { get; }
    public IReadOnlyList<CreatureItemData> Collection { get; }
    public StatTable UserStats { get; }

    TutorialState _tutorial;

    public TutorialState Tutorial
    {
      get => _tutorial;
      set
      {
        _tutorial = value;
        Save();
      }
    }

    public enum TutorialState
    {
      Starting,
      InitialWorldScreen,
      AfterFirstAttack,
      Completed
    }

    public static void Initialize(MonoBehaviour _, GameDataService gameDataService, Action<UserDataService2> action)
    {
      action(new UserDataService2(gameDataService));
    }

    string DataPath => Path.Combine(Application.persistentDataPath, "data.json");

    UserDataService2(GameDataService gameDataService)
    {
      if (File.Exists(DataPath))
      {
        Debug.Log($"Loading saved data from {DataPath}");
        var parsed = JSON.Parse(File.ReadAllText(DataPath));
        Deck = parsed["deck"].FromJsonArray()
          .Select(c => CreatureItemData.Deserialize(gameDataService, c)).ToList();
        Collection = parsed["collection"].FromJsonArray()
          .Select(c => CreatureItemData.Deserialize(gameDataService, c)).ToList();
        UserStats = StatTable.Deserialize(parsed["user"], StatTable.Defaults);
        Tutorial = (TutorialState) parsed["tutorial"].AsInt;
      }
      else
      {
        Deck = gameDataService.GetStaticCardList(StaticCardList.StartingDeck).ToList();
        Collection = new List<CreatureItemData>();
        UserStats = new StatTable(StatTable.Defaults);
        Tutorial = TutorialState.Starting;
        Debug.Log($"Save file not found, saving new player tutorial data to {DataPath}");
        Save();

#pragma warning disable 618
        Application.ExternalEval("_JS_FileSystem_Sync();");
#pragma warning restore 618
      }
    }

    void Save()
    {
      var result = new JSONObject
      {
        ["deck"] = Deck.Select(c => c.Serialize()).AsJsonArray(),
        ["collection"] = Collection.Select(c => c.Serialize()).AsJsonArray(),
        ["user"] = UserStats.Serialize(),
        ["tutorial"] = new JSONNumber((int) Tutorial)
      };

      File.WriteAllText(DataPath, result.ToString());
    }
  }
}
