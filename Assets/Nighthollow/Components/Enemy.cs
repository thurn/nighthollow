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

using System;
using System.Collections.Generic;
using DataStructures.RandomSelector;
using Nighthollow.Data;
using Nighthollow.Services;
using UnityEngine;

namespace Nighthollow.Components
{
  public class Enemy : MonoBehaviour
  {
    [SerializeField] EnemiesData _data;

    void Awake()
    {
      _data = Instantiate(_data);
    }

    public void StartSpawningEnemies()
    {
      StartCoroutine(SpawnAsync());
    }

    IEnumerator<YieldInstruction> SpawnAsync()
    {
      yield return new WaitForSeconds(_data.InitialSpawnDelayMs / 1000f);
      Root.Instance.CreatureService.CreateEnemyCreature(RandomEnemy(), RandomFile());

      while (true)
      {
        yield return new WaitForSeconds(_data.SpawnDelayMs / 1000f);
        Root.Instance.CreatureService.CreateEnemyCreature(RandomEnemy(), RandomFile());
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
      foreach (FileValue file in Enum.GetValues(typeof(FileValue)))
      {
        if (file != FileValue.Unknown)
        {
          selector.Add(file, 1.0f);
        }
      }
      selector.Build();

      return selector.SelectRandomItem();
    }
  }
}