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
using DataStructures.RandomSelector;
using Nighthollow.Data;
using Nighthollow.Generated;
using Nighthollow.Services;
using Nighthollow.Stats;
using Nighthollow.Utils;
using UnityEngine;

namespace Nighthollow.Components
{
  public sealed class Enemy : MonoBehaviour
  {
    StatTable _stats = null!;
    public StatTable Stats => _stats;

    List<CreatureData> _enemies = null!;

    [SerializeField] int _deathCount;

    public void OnGameStarted()
    {
      var config = Database.Instance.CurrentGameConfig;
      _stats = config.EnemyStats;
      _enemies = config.Enemies.Select(c => CreatureUtil.Build(_stats, c)).ToList();

      StartCoroutine(SpawnAsync());
    }

    /// <summary>Called when an enemy is killed (not despawned).</summary>
    public void OnEnemyCreatureKilled()
    {
      _deathCount++;
      if (_deathCount >= _stats.Get(Stat.KillsRequiredForVictory))
      {
        Debug.Log("Victory!");
      }
    }

    IEnumerator<YieldInstruction> SpawnAsync()
    {
      var spawnDelay = _stats.Get(Stat.EnemySpawnDelay).AsSeconds();
      Errors.CheckArgument(spawnDelay > 0.1f, "Spawn delay cannot be 0");

      while (true)
      {
        yield return new WaitForSeconds(spawnDelay);
        Root.Instance.CreatureService.CreateMovingCreature(
          RandomEnemy(),
          RandomFile(),
          Constants.EnemyCreatureStartingX);
      }
    }

    CreatureData RandomEnemy()
    {
      var selector = new DynamicRandomSelector<CreatureData>();
      foreach (var enemy in _enemies)
      {
        selector.Add(enemy, 1.0f);
      }

      selector.Build();

      return selector.SelectRandomItem().Clone(_stats);
    }

    FileValue RandomFile()
    {
      var selector = new DynamicRandomSelector<FileValue>();
      foreach (var file in BoardPositions.AllFiles)
      {
        selector.Add(file, 1.0f);
      }

      selector.Build();

      return selector.SelectRandomItem();
    }
  }
}
