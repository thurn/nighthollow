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

using System.Collections.Immutable;
using Nighthollow.Data;

#nullable enable

namespace Nighthollow.Services
{
  public sealed class RewardsService
  {
    readonly Database _database;

    public RewardsService(Database database)
    {
      _database = database;
    }

    public RewardChoiceData CreateRewardsForCurrentBattle()
    {
      var gameData = _database.Snapshot();

      var choices = ImmutableList<IItemData>.Empty;
      if (gameData.BattleData.RewardChoicesOverride is { } choicesId)
      {
        choices = gameData.ItemLists[choicesId].AsItems();
      }

      var fixedRewards = ImmutableList<IItemData>.Empty;
      if (gameData.BattleData.FixedRewardsOverride is { } fixedRewardsId)
      {
        choices = gameData.ItemLists[fixedRewardsId].AsItems();
      }

      return new RewardChoiceData(choices, fixedRewards);
    }
  }
}