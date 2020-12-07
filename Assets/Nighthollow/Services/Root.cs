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
using Nighthollow.Components;
using Nighthollow.Generated;
using Nighthollow.Interface;
using Nighthollow.Stats;
using Nighthollow.Utils;
using UnityEngine;

#nullable enable

namespace Nighthollow.Services
{
  public sealed class Root : MonoBehaviour
  {
    static Root _instance = null!;

    [SerializeField] Camera _mainCamera = null!;

    [SerializeField] RectTransform _mainCanvas = null!;

    [SerializeField] Prefabs _prefabs = null!;

    [SerializeField] ObjectPoolService _objectPoolService = null!;

    [SerializeField] CreatureService _creatureService = null!;

    [SerializeField] ScreenController _screenController = null!;

    [SerializeField] User _user = null!;

    [SerializeField] Enemy _enemy = null!;

    [SerializeField] DamageTextService _damageTextService = null!;

    [SerializeField] HelperTextService _helperTextService = null!;
    public Camera MainCamera => _mainCamera;
    public RectTransform MainCanvas => _mainCanvas;
    public Prefabs Prefabs => _prefabs;
    public ObjectPoolService ObjectPoolService => _objectPoolService;
    public CreatureService CreatureService => _creatureService;
    public ScreenController ScreenController => _screenController;
    public User User => _user;
    public Enemy Enemy => _enemy;
    public DamageTextService DamageTextService => _damageTextService;
    public HelperTextService HelperTextService => _helperTextService;

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
      Errors.CheckNotNull(_mainCamera);
      Errors.CheckNotNull(_mainCanvas);
      Errors.CheckNotNull(_prefabs);
      Errors.CheckNotNull(_objectPoolService);
      Errors.CheckNotNull(_creatureService);
      Errors.CheckNotNull(_screenController);
      Errors.CheckNotNull(_user);
      Errors.CheckNotNull(_enemy);
      Errors.CheckNotNull(_damageTextService);
      Errors.CheckNotNull(_helperTextService);

      _instance = this;
    }

    public StatTable StatsForPlayer(PlayerName player)
    {
      return player switch
      {
        PlayerName.User => User.Data.Stats,
        PlayerName.Enemy => Enemy.Stats,
        _ => throw Errors.UnknownEnumValue(player)
      };
    }
  }
}
