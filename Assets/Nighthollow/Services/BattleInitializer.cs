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
using Nighthollow.Data;
using Nighthollow.Delegates.Handlers;
using Nighthollow.Editing.Scenarios;
using Nighthollow.Interface;
using Nighthollow.Rules.Events;
using Nighthollow.Utils;
using UnityEngine;
using UnityEngine.UIElements;

#nullable enable

namespace Nighthollow.Services
{
  public sealed class BattleInitializer : AbstractInitializer
  {
    [SerializeField] Camera _mainCamera = null!;
    [SerializeField] RectTransform _mainCanvas = null!;
    [SerializeField] Prefabs _prefabs = null!;
    [SerializeField] DataService _dataService = null!;
    [SerializeField] UIDocument _document = null!;
    [SerializeField] Hand _hand = null!;
    [SerializeField] DamageTextService _damageTextService = null!;
    BattleServiceRegistry? _registry;

    void Start()
    {
      Errors.CheckNotNull(_mainCamera);
      Errors.CheckNotNull(_mainCanvas);
      Errors.CheckNotNull(_prefabs);
      Errors.CheckNotNull(_dataService);
      Errors.CheckNotNull(_document);
      Errors.CheckNotNull(_hand);
      Errors.CheckNotNull(_damageTextService);

      _dataService.OnReady(OnDataFetched);
    }

    void Update()
    {
      _registry?.OnUpdate();
    }

    void OnDataFetched(FetchResult fetchResult)
    {
      _registry = new BattleServiceRegistry(
        this,
        fetchResult.Database,
        fetchResult.AssetService,
        _document,
        _mainCamera,
        _prefabs,
        _mainCanvas,
        _hand,
        _damageTextService);

      _registry.ScreenController.Get(ScreenController.VictoryWindow).Show();

      // Run a scenario if one has been requested -- takes precedence over normal load events.
      // if (!ScenarioData.Invoke(_registry))
      // {
      //   _registry.Invoke(new IOnBattleSceneLoaded.Data());
      //   _registry.RulesEngine.Invoke(new BattleSceneReadyEvent());
      // }
    }
  }
}