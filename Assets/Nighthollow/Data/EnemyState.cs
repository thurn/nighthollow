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
using System.Linq;
using Nighthollow.Delegates;
using Nighthollow.Services;
using Nighthollow.State;
using Nighthollow.Stats;

#nullable enable

namespace Nighthollow.Data
{
  public sealed partial class EnemyState : StatEntity, IPlayerState
  {
    public EnemyState(
      StatTable stats,
      DelegateList delegateList,
      KeyValueStore keyValueStore,
      int spawnCount = 0,
      int deathCount = 0)
    {
      Stats = stats;
      DelegateList = delegateList;
      KeyValueStore = keyValueStore;
      SpawnCount = spawnCount;
      DeathCount = deathCount;
    }

    [Field] public override StatTable Stats { get; }
    [Field] public DelegateList DelegateList { get; }
    [Field] public KeyValueStore KeyValueStore { get; }
    [Field] public int SpawnCount { get; }
    [Field] public int DeathCount { get; }

    public EnemyState OnTick(IGameContext c) => WithStats(Stats.OnTick(c));

    public static EnemyState BuildEnemyState(GameData gameData)
    {
      var stats = gameData.BattleData.EnemyModifiers.Aggregate(
        new StatTable(StatData.BuildDefaultStatTable(gameData)),
        (current, modifier) => current.InsertNullableModifier(modifier.BuildStatModifier()));
      // TODO: Persist enemy delegate list
      var delegateList = new DelegateList(ImmutableList<IDelegate>.Empty, DelegateList.Root);
      return new EnemyState(stats, delegateList, new KeyValueStore());
    }
  }
}