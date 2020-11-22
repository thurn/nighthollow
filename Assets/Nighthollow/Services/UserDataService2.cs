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
  public sealed class UserDataService2 : MonoBehaviour
  {
    [SerializeField] GameDataService _gameDataService = null!;

    List<CreatureItemData> _deck = null!;
    public IReadOnlyList<CreatureItemData> Deck => _deck;

    List<CreatureItemData> _collection = null!;
    public IReadOnlyList<CreatureItemData> Collection => _collection;

    StatTable _userStats = null!;
    public StatTable UserStats => _userStats;

    string DataPath => Path.Combine(Application.persistentDataPath, "data.json");

    void Start()
    {
      _gameDataService.FetchData(OnFetchCompleted);
    }

    void OnFetchCompleted()
    {
      if (File.Exists(DataPath))
      {
        Debug.Log($"Loading saved data from {DataPath}");
        var parsed = JSON.Parse(File.ReadAllText(DataPath));
        _deck = parsed["deck"].FromJsonArray()
          .Select(c => CreatureItemData.Deserialize(_gameDataService, c)).ToList();
        _collection = parsed["collection"].FromJsonArray()
          .Select(c => CreatureItemData.Deserialize(_gameDataService, c)).ToList();
        _userStats = StatTable.Deserialize(parsed["user"], StatTable.Defaults);
      }
      else
      {
        Debug.Log("Save file not found, loading new player tutorial data.");
        _deck = _gameDataService.GetStaticCardList(StaticCardList.StartingDeck).ToList();
        _collection = new List<CreatureItemData>();
        _userStats = new StatTable(StatTable.Defaults);
        Save();
      }
    }

    void Save()
    {
      var result = new JSONObject
      {
        ["deck"] = _deck.Select(c => c.Serialize()).AsJsonArray(),
        ["collection"] = _collection.Select(c => c.Serialize()).AsJsonArray(),
        ["user"] = _userStats.Serialize()
      };

      File.WriteAllText(DataPath, result.ToString());
    }
  }
}
