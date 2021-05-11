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

using System.Collections.Immutable;
using Nighthollow.Data;
using Nighthollow.Interface;
using Nighthollow.Services;
using Nighthollow.Triggers;
using Nighthollow.Triggers.Conditions;
using Nighthollow.Triggers.Effects;
using Nighthollow.Triggers.Events;
using UnityEngine;
using UnityEngine.UIElements;

#nullable enable

namespace Nighthollow.World
{
  public sealed class WorldInitializer : MonoBehaviour
  {
    [SerializeField] Camera _mainCamera = null!;
    [SerializeField] DataService _dataService = null!;
    [SerializeField] ObjectPoolService _objectPoolService = null!;
    [SerializeField] UIDocument _document = null!;
    [SerializeField] WorldMap _worldMap = null!;
    [SerializeField] WorldStaticAssets _staticAssets = null!;
    WorldServiceRegistry? _registry;

    void Start()
    {
      _dataService.OnReady(OnDataFetched);
    }

    void Update()
    {
      _registry?.OnUpdate();
    }

    void OnDataFetched(FetchResult result)
    {
      _registry = new WorldServiceRegistry(
        result.Database,
        result.AssetService,
        _document,
        _mainCamera,
        _objectPoolService,
        _worldMap,
        _staticAssets);

      _worldMap.Initialize(_registry);
      _registry.ScreenController.Show(ScreenController.AdvisorBar);
      _registry.WorldTutorial.OnWorldSceneLoaded();
      _registry.TriggerService.Invoke(new SceneReadyEvent(SceneReadyEvent.Name.World));

      // var testTrigger = new TriggerData<SceneReadyEvent>(
      //   "Test",
      //   ImmutableList<ICondition<SceneReadyEvent>>.Empty.Add(new SceneNameCondition(SceneReadyEvent.Name.World)),
      //   ImmutableList<IEffect<SceneReadyEvent>>.Empty.Add(new DisplayHelpTextEffect(
      //     new DisplayHelpTextEffect.ArrowPosition(x: 250, y: 300),
      //     DisplayHelpTextEffect.Direction.Bottom,
      //     "Hello, world")));
      // _registry.Database.Insert(TableId.Triggers, testTrigger);
    }
  }
}