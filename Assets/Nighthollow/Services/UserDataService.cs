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
  public sealed class UserDataService : StatEntity
  {
    public IReadOnlyList<CreatureItemData> Deck { get; }
    public IReadOnlyList<CreatureItemData> Collection { get; }
    public override StatTable Stats { get; }

    Tutorial _tutorialState;

    public Tutorial TutorialState
    {
      get => _tutorialState;
      set
      {
        _tutorialState = value;
        Save();
      }
    }

    public enum Tutorial
    {
      Starting,
      InitialWorldScreen,
      GameOne,
      AfterFirstAttack,
      Completed
    }

    public static void Initialize(MonoBehaviour _, GameDataService gameDataService, Action<UserDataService> action)
    {
      action(new UserDataService(gameDataService));
    }

    string DataPath => Path.Combine(Application.persistentDataPath, "data.json");

    UserDataService(GameDataService gameDataService)
    {
      if (File.Exists(DataPath))
      {
        Debug.Log($"Loading saved data from {DataPath}");
        var parsed = JSON.Parse(File.ReadAllText(DataPath));
        Deck = parsed["deck"].FromJsonArray()
          .Select(c => CreatureItemData.Deserialize(gameDataService, c)).ToList();
        Collection = parsed["collection"].FromJsonArray()
          .Select(c => CreatureItemData.Deserialize(gameDataService, c)).ToList();
        Stats = StatTable.Deserialize(parsed["user"], StatTable.Defaults);
        TutorialState = (Tutorial) parsed["tutorial"].AsInt;
      }
      else
      {
        Deck = gameDataService.GetStaticCardList(StaticCardList.StartingDeck).ToList();
        Collection = new List<CreatureItemData>();
        Stats = new StatTable(StatTable.Defaults);
        TutorialState = Tutorial.Starting;
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
        ["user"] = Stats.Serialize(),
        ["tutorial"] = new JSONNumber((int) TutorialState)
      };

      File.WriteAllText(DataPath, result.ToString());
    }
  }
}
