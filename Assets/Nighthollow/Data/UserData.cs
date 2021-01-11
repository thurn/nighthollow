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

using System.Linq;
using MessagePack;
using Nighthollow.Stats;

#nullable enable

namespace Nighthollow.Data
{
  [MessagePackObject]
  public sealed partial class GameState
  {
    public GameState(TutorialState? tutorialState = null, CurrentEnemyState? currentEnemy = null)
    {
      TutorialState = tutorialState ?? TutorialState.NewPlayer;
      CurrentEnemy = currentEnemy ?? new CurrentEnemyState();
    }

    [Key(0)] public TutorialState TutorialState { get; }
    [Key(1)] public CurrentEnemyState CurrentEnemy { get; }

    public UserData BuildUserData(GameData gameData) =>
      new UserData(this,
        gameData.UserModifiers.Values.Aggregate(
          new StatTable(StatData.BuildDefaultStatTable(gameData)),
          (current, modifier) => current.InsertNullableModifier(modifier.BuildStatModifier())));
  }

  public sealed partial class UserData : StatEntity
  {
    public UserData(
      GameState gameState,
      StatTable stats)
    {
      State = gameState;
      Stats = stats;
    }

    [Field] public GameState State { get; }
    [Field] public override StatTable Stats { get; }
  }
}
