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
using Nighthollow.Rules.Events;
using Nighthollow.Services;
using Nighthollow.World.Data;
using UnityEngine;
using UnityEngine.UIElements;

#nullable enable

namespace Nighthollow.World
{
  public sealed class WorldInitializer : AbstractInitializer
  {
    [SerializeField] Camera _mainCamera = null!;
    [SerializeField] DataService _dataService = null!;
    [SerializeField] UIDocument _document = null!;
    [SerializeField] WorldStaticAssets _staticAssets = null!;
    [SerializeField] CameraMover _cameraMover = null!;
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
        this,
        _staticAssets);

      _registry.ScreenController.Show(ScreenController.AdvisorBar);
      _registry.RulesEngine.Invoke(new WorldSceneReadyEvent());
      _cameraMover.Initialize(_registry);
    }
  }
}