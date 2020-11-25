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

using Nighthollow.Components;
using Nighthollow.Utils;
using System;
using Nighthollow.Generated;
using Nighthollow.Interface;
using Nighthollow.Stats;
using UnityEngine;

namespace Nighthollow.Services
{
  public sealed class Root : MonoBehaviour
  {
    static Root _instance;

#pragma warning disable 0649
    [SerializeField] Camera _mainCamera;
    public Camera MainCamera => _mainCamera;

    [SerializeField] RectTransform _mainCanvas;
    public RectTransform MainCanvas => _mainCanvas;

    [SerializeField] Prefabs _prefabs;
    public Prefabs Prefabs => _prefabs;

    [SerializeField] ObjectPoolService _objectPoolService;
    public ObjectPoolService ObjectPoolService => _objectPoolService;

    [SerializeField] CreatureService _creatureService;
    public CreatureService CreatureService => _creatureService;

    [SerializeField] ScreenController _screenController;
    public ScreenController ScreenController => _screenController;

    [SerializeField] User _user;
    public User User => _user;

    [SerializeField] Enemy _enemy;
    public Enemy Enemy => _enemy;

    [SerializeField] DamageTextService _damageTextService;
    public DamageTextService DamageTextService => _damageTextService;

    [SerializeField] EnemyDataService _enemyDataService;
    public EnemyDataService EnemyDataService => _enemyDataService;
#pragma warning restore 0649

    public StatTable StatsForPlayer(PlayerName player) => player switch
    {
      PlayerName.User => User.Data.Stats,
      PlayerName.Enemy => Enemy.Data.Stats,
      _ => throw Errors.UnknownEnumValue(player)
    };

    public static Root Instance
    {
      get
      {
        if (!_instance)
        {
          throw new NullReferenceException("Attempted to access Root before OnEnable!");
        }

        return _instance;
      }
    }

    void OnEnable()
    {
      DontDestroyOnLoad(this);
      Errors.CheckNotNull(_mainCamera);
      Errors.CheckNotNull(_mainCanvas);
      Errors.CheckNotNull(_prefabs);
      Errors.CheckNotNull(_objectPoolService);
      Errors.CheckNotNull(_creatureService);
      Errors.CheckNotNull(_screenController);
      Errors.CheckNotNull(_user);
      Errors.CheckNotNull(_enemy);
      Errors.CheckNotNull(_damageTextService);
      Errors.CheckNotNull(_enemyDataService);

      _instance = this;
    }
  }
}
