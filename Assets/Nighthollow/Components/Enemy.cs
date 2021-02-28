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
  public sealed class Enemy : MonoBehaviour, IStatOwner
  {
    [SerializeField] int _spawnCount;
    [SerializeField] int _deathCount;
    ImmutableList<CreatureData> _enemies = null!;
    GameServiceRegistry? _registry;

    public EnemyData Data { get; private set; } = null!;

    public void OnGameStarted(GameServiceRegistry registry)
    {
      _registry = registry;
      var gameData = registry.Database.Snapshot();
      Data = gameData.BattleData.BuildEnemyData(gameData);
      Errors.CheckState(Data.Enemies.Count > 0, "No enemies found");
      _enemies = Data.Enemies.Select(item => item.BuildCreature(registry)).ToImmutableList();

      StartCoroutine(SpawnAsync());
    }

    public void InsertModifier(IStatModifier modifier)
    {
      Data = Data.WithStats(Data.Stats.InsertModifier(modifier));
    }

    public void InsertStatusEffect(StatusEffectData statusEffectData)
    {
      Data = Data.WithStats(Data.Stats.InsertStatusEffect(statusEffectData));
    }

    void Update()
    {
      if (_registry != null)
      {
        Data = Data.OnTick(_registry.Context);
      }
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
      var registry = Errors.CheckNotNull(_registry);

      while (_spawnCount < Data.Get(Stat.EnemiesToSpawn))
      {
        _spawnCount++;
        registry.CreatureService.CreateMovingCreature(
          registry,
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
      return FileValue.File3;
      // var selector = new DynamicRandomSelector<FileValue>();
      // foreach (var file in BoardPositions.AllFiles)
      // {
      //   selector.Add(file, weight: 1.0f);
      // }
      //
      // selector.Build();
      //
      // return selector.SelectRandomItem();
    }
  }
}
