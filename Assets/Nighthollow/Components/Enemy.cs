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
using Nighthollow.Services;
using Nighthollow.Utils;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Nighthollow.Components
{
  public sealed class Enemy : MonoBehaviour
  {
    [SerializeField] EnemiesData _data;
    [SerializeField] int _spawnCount;
    [SerializeField] int _deathCount;

    void Awake()
    {
      _data = Instantiate(_data);
    }

    public void StartSpawningEnemies()
    {
      StartCoroutine(SpawnAsync());
    }

    /// <summary>Called when an enemy is killed or reaches the victory line.</summary>
    public void OnEnemyCreatureKilled()
    {
      _deathCount++;
      if (_deathCount >= _data.EnemiesToSpawn)
      {
        Root.Instance.Prefabs.ShowDialog("Victory!", () => { SceneManager.LoadScene("Main", LoadSceneMode.Single); });
      }
    }

    IEnumerator<YieldInstruction> SpawnAsync()
    {
      yield return new WaitForSeconds(_data.InitialSpawnDelayMs / 1000f);
      Root.Instance.CreatureService.CreateMovingCreature(
        RandomEnemy(),
        RandomFile(),
        Constants.EnemyCreatureStartingX,
        Constants.EnemyCreatureYOffset);
      _spawnCount = 1;

      while (_spawnCount < _data.EnemiesToSpawn)
      {
        yield return new WaitForSeconds(_data.SpawnDelayMs / 1000f);
        Root.Instance.CreatureService.CreateMovingCreature(
          RandomEnemy(),
          RandomFile(),
          Constants.EnemyCreatureStartingX,
          Constants.EnemyCreatureYOffset);
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

      return Instantiate(selector.SelectRandomItem());
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