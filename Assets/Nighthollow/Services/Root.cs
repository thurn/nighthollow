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
using Nighthollow.Components;
using Nighthollow.Data;
using Nighthollow.Delegates.Handlers;
using Nighthollow.Interface;
using Nighthollow.Stats;
using Nighthollow.Utils;
using UnityEngine;

#nullable enable

namespace Nighthollow.Services
{
  public interface IStartCoroutine
  {
    Coroutine StartCoroutine(IEnumerator<YieldInstruction> routine);
  }

  public sealed class Root : MonoBehaviour, IStartCoroutine
  {
    static Root _instance = null!;

    [SerializeField] Camera _mainCamera = null!;

    [SerializeField] RectTransform _mainCanvas = null!;

    [SerializeField] Prefabs _prefabs = null!;

    [SerializeField] DataService _dataService = null!;

    [SerializeField] ObjectPoolService _objectPoolService = null!;

    [SerializeField] ScreenController _screenController = null!;

    [SerializeField] User _user = null!;

    [SerializeField] Enemy _enemy = null!;

    [SerializeField] DamageTextService _damageTextService = null!;

    [SerializeField] HelperTextService _helperTextService = null!;

    public Camera MainCamera => _mainCamera;
    public RectTransform MainCanvas => _mainCanvas;
    public Prefabs Prefabs => _prefabs;
    public ObjectPoolService ObjectPoolService => _objectPoolService;
    public ScreenController ScreenController => _screenController;
    public User User => _user;
    public Enemy Enemy => _enemy;
    public DamageTextService DamageTextService => _damageTextService;
    public HelperTextService HelperTextService => _helperTextService;
    GameServiceRegistry? _registry;

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

    public Coroutine StartCoroutine(IEnumerator<YieldInstruction> routine) => base.StartCoroutine(routine);

    void Start()
    {
      Errors.CheckNotNull(_mainCamera);
      Errors.CheckNotNull(_mainCanvas);
      Errors.CheckNotNull(_prefabs);
      Errors.CheckNotNull(_dataService);
      Errors.CheckNotNull(_objectPoolService);
      Errors.CheckNotNull(_screenController);
      Errors.CheckNotNull(_user);
      Errors.CheckNotNull(_enemy);
      Errors.CheckNotNull(_damageTextService);
      Errors.CheckNotNull(_helperTextService);

      _instance = this;

      _screenController.Initialize();
      _dataService.OnReady(OnDataFetched);
    }

    void Update()
    {
      _registry?.OnUpdate();
    }

    void OnDataFetched(FetchResult fetchResult)
    {
      _registry = new GameServiceRegistry(
        this,
        fetchResult.Database,
        fetchResult.AssetService,
        _screenController,
        _objectPoolService,
        _prefabs,
        _mainCanvas,
        _user,
        _enemy,
        _damageTextService,
        _helperTextService);

      _screenController.OnServicesReady(_registry);
      _helperTextService.OnServicesReady(_registry);
      _registry.Invoke(new IOnBattleSceneLoaded.Data());
      
      // _user.Hand.OnServicesReady(_registry);
      // _user.DrawOpeningHand(_registry);
    }

    public StatTable StatsForPlayer(PlayerName player)
    {
      return player switch
      {
        PlayerName.User => User.State.Stats,
        PlayerName.Enemy => Enemy.Data.Stats,
        _ => throw Errors.UnknownEnumValue(player)
      };
    }
  }
}
