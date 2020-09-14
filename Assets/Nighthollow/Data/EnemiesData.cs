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
using System.Collections.ObjectModel;
using UnityEngine;

namespace Nighthollow.Data
{
  [CreateAssetMenu(menuName = "Data/Enemies")]
  public sealed class EnemiesData : ScriptableObject
  {
    [SerializeField] List<CreatureData> _enemies;
    public ReadOnlyCollection<CreatureData> Enemies => _enemies.AsReadOnly();

    [SerializeField] int _initialSpawnDelayMs;
    public int InitialSpawnDelayMs => _initialSpawnDelayMs;

    [SerializeField] int _spawnDelayMs;
    public int SpawnDelayMs => _spawnDelayMs;

    [SerializeField] int _enemiesToSpawn;
    public int EnemiesToSpawn => _enemiesToSpawn;
  }
}