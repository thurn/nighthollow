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
    EnemyData _data;
    public EnemyData Data => _data;

    [SerializeField] int _spawnCount;
    [SerializeField] int _deathCount;

    public void OnStartGame(EnemyData data)
    {
      _data = data.Clone(StatTable.Defaults);
      StartCoroutine(SpawnAsync());
    }

    /// <summary>Called when an enemy is killed or reaches the victory line.</summary>
    public void OnEnemyCreatureRemoved()
    {
      _deathCount++;
      if (_deathCount >= _data.GetInt(Stat.TotalEnemiesToSpawn))
      {
        Debug.Log("Victory!");
      }
    }

    IEnumerator<YieldInstruction> SpawnAsync()
    {
      var spawnDelay = _data.GetDurationSeconds(Stat.EnemySpawnDelay);
      Errors.CheckArgument(spawnDelay > 0.1f, "Spawn delay cannot be 0");

      Root.Instance.CreatureService.CreateMovingCreature(
        RandomEnemy(),
        RandomFile(),
        Constants.EnemyCreatureStartingX);
      _spawnCount = 1;

      while (_spawnCount < _data.GetInt(Stat.TotalEnemiesToSpawn))
      {
        yield return new WaitForSeconds(spawnDelay);
        Root.Instance.CreatureService.CreateMovingCreature(
          RandomEnemy(),
          RandomFile(),
          Constants.EnemyCreatureStartingX);
        _spawnCount++;
      }
    }

    CreatureData RandomEnemy()
    {
      var selector = new DynamicRandomSelector<CreatureData>();
      foreach (var enemy in _data.Enemies)
      {
        selector.Add(enemy, 1.0f);
      }

      selector.Build();

      return selector.SelectRandomItem().Clone(Data.Stats);
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