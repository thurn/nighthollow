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
using System.Linq;
using Nighthollow.Data;
using Nighthollow.Generated;
using Nighthollow.Model;
using Nighthollow.Stats;
using Nighthollow.Utils;
using SimpleJSON;
using CreatureItemData = Nighthollow.Model.CreatureItemData;

#nullable enable

namespace Nighthollow.Services
{
  public sealed class GameConfig
  {
    public GameConfig(GameDataService gameDataService, JSONNode? json = null)
    {
      if (json == null)
      {
        Enemies = gameDataService.GetStaticCardList(StaticCardList.TutorialEnemy).ToList();
        EnemyStats = new StatTable(StatTable.Defaults);
      }
      else
      {
        Enemies = json["enemies"].FromJsonArray()
          .Select(c => CreatureItemData.Deserialize(gameDataService, c)).ToList();
        EnemyStats = StatTable.Deserialize(json["enemyStats"], StatTable.Defaults);
      }
    }

    public IReadOnlyList<CreatureItemData> Enemies { get; }
    public StatTable EnemyStats { get; }

    public JSONNode Serialize()
    {
      return new JSONObject
      {
        ["enemies"] = Enemies.Select(c => c.Serialize()).AsJsonArray(),
        ["enemyStats"] = EnemyStats.Serialize()
      };
    }
  }
}
