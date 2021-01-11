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
using System.Collections.Immutable;
using System.Linq;
using DataStructures.RandomSelector;
using Nighthollow.Data;
using Nighthollow.Interface;
using Nighthollow.Services;
using Nighthollow.Stats;
using Nighthollow.Utils;
using UnityEngine;

#nullable enable

namespace Nighthollow.Components
{
  public sealed class Enemy : MonoBehaviour
  {
    [SerializeField] int _spawnCount;
    [SerializeField] int _deathCount;

    ImmutableList<CreatureData> _enemies = null!;

    public EnemyData Data { get; private set; }

    public void OnGameStarted(GameData gameData)
    {
      Data = gameData.GameState.CurrentEnemy.BuildEnemyData(gameData);
      _enemies = Data.Enemies.Select(item => item.BuildCreature(gameData, Data.Stats)).ToImmutableList();

      StartCoroutine(SpawnAsync());
    }

    /// <summary>Called when an enemy is killed (not despawned).</summary>
    public void OnEnemyCreatureKilled()
    {
      _deathCount++;
      if (_deathCount >= Data.Get(Stat.EnemiesToSpawn))
      {
        Root.Instance.ScreenController.Get(ScreenController.GameOverMessage)
          .Show(new GameOverMessage.Args("Victory!", "World"), animate: true);
      }
    }

    public void OnEnemyCreatureAtEndzone(Creature creature)
    {
      Root.Instance.ScreenController.Get(ScreenController.GameOverMessage)
        .Show(new GameOverMessage.Args("Game Over", "World"));
      Root.Instance.ScreenController.Get(ScreenController.BlackoutWindow).Show(argument: 0.5f);
      Time.timeScale = 0f;
    }

    IEnumerator<YieldInstruction> SpawnAsync()
    {
      yield return new WaitForSeconds(Data.Get(Stat.InitialEnemySpawnDelay).AsSeconds());

      while (_spawnCount < Data.Get(Stat.EnemiesToSpawn))
      {
        _spawnCount++;
        Root.Instance.CreatureService.CreateMovingCreature(
          RandomEnemy(),
          RandomFile(),
          Constants.EnemyCreatureStartingX);
        yield return new WaitForSeconds(Data.Get(Stat.EnemySpawnDelay).AsSeconds());
      }

      // ReSharper disable once IteratorNeverReturns
    }

    CreatureData RandomEnemy()
    {
      var selector = new DynamicRandomSelector<CreatureData>();
      foreach (var enemy in _enemies)
      {
        selector.Add(enemy, weight: 1.0f);
      }

      selector.Build();

      return selector.SelectRandomItem();
    }

    FileValue RandomFile()
    {
      var selector = new DynamicRandomSelector<FileValue>();
      foreach (var file in BoardPositions.AllFiles)
      {
        selector.Add(file, weight: 1.0f);
      }

      selector.Build();

      return selector.SelectRandomItem();
    }
  }
}
