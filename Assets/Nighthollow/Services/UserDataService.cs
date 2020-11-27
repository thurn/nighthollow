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
using System.Linq;
using Nighthollow.Data;
using Nighthollow.Generated;
using Nighthollow.Stats;
using Nighthollow.Utils;
using SimpleJSON;

namespace Nighthollow.Services
{
  public sealed class UserDataService : StatEntity
  {
    readonly Database _database;
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
        _database.Save();
      }
    }

    public enum Tutorial
    {
      Unknown = 0,
      Starting = 1,
      InitialWorldScreen = 2,
      GameOne = 3,
      AfterFirstAttack = 4,
      Completed = 5
    }

    public UserDataService(Database database, GameDataService gameDataService, JSONNode? json = null)
    {
      _database = database;
      if (json == null)
      {
        Deck = gameDataService.GetStaticCardList(StaticCardList.StartingDeck).ToList();
        Collection = new List<CreatureItemData>();
        Stats = new StatTable(StatTable.Defaults);
        _tutorialState = Tutorial.Starting;
      }
      else
      {
        Deck = json["deck"].FromJsonArray()
          .Select(c => CreatureItemData.Deserialize(gameDataService, c)).ToList();
        Collection = json["collection"].FromJsonArray()
          .Select(c => CreatureItemData.Deserialize(gameDataService, c)).ToList();
        Stats = StatTable.Deserialize(json["user"], StatTable.Defaults);
        _tutorialState = (Tutorial) json["tutorial"].AsInt;
      }
    }

    public JSONNode Serialize() =>
      new JSONObject
      {
        ["deck"] = Deck.Select(c => c.Serialize()).AsJsonArray(),
        ["collection"] = Collection.Select(c => c.Serialize()).AsJsonArray(),
        ["user"] = Stats.Serialize(),
        ["tutorial"] = new JSONNumber((int) TutorialState)
      };
  }
}
