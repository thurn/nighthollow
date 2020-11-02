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
using UnityEngine;

namespace Nighthollow.Services
{
  public sealed class UserDataService : MonoBehaviour
  {
    readonly List<CreatureItemData> _deck = new List<CreatureItemData>();
    public IReadOnlyList<CreatureItemData> Deck => _deck;

    StatTable? _userStats;
    public StatTable UserStats => Errors.CheckNotNull(_userStats);

    public void OnNewGame(GameDataService gameDataService)
    {
      _userStats = new StatTable(StatTable.Defaults);
      _deck.Clear();
      _deck.AddRange(gameDataService.GetStaticCardList(StaticCardList.StartingDeck));
    }

    public void StartGame(bool isTutorial)
    {
      var cards = isTutorial ? Root.Instance.GameDataService.GetStaticCardList(StaticCardList.TutorialDraws) : Deck;
      var stats = UserStats.Clone(StatTable.Defaults);
      Root.Instance.User.OnStartGame(
        new UserData(
          cards.Select(c => CreatureUtil.Build(stats, c)).ToList(),
          stats,
          orderedDraws: isTutorial));
    }
  }
}