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
using Nighthollow.Interface.Components.Core;
using Nighthollow.Interface.Components.Windows;
using Nighthollow.Rules;
using Nighthollow.World;
using UnityEngine;
using UnityEngine.UIElements;

#nullable enable

namespace Nighthollow.Services
{
  public sealed class WorldServiceRegistry : ServiceRegistry
  {
    public WorldServiceRegistry(
      Database database,
      AssetService assetService,
      UIDocument document,
      Camera mainCamera,
      IStartCoroutine coroutineRunner,
      WorldStaticAssets staticAssets) : base(
      database,
      assetService,
      document.rootVisualElement,
      mainCamera,
      coroutineRunner)
    {
      StaticAssets = staticAssets;
      ComponentController = ComponentController<RootComponent>.Create(
        coroutineRunner, this, document.rootVisualElement.Q("ComponentRoot"), new RootComponent());
    }

    public override ServiceRegistryName Name => ServiceRegistryName.World;

    Scope? _scope;

    public new static ImmutableHashSet<IKey> Keys => ImmutableHashSet.Create<IKey>(
      Key.WorldMapRenderer,
      Key.WorldMapController,
      Key.ComponentController
    );

    public override Scope Scope => _scope ??= Scope.CreateBuilder(Keys, base.Scope)
      .AddBinding(Key.WorldMapRenderer, WorldMapRenderer)
      .AddBinding(Key.WorldMapController, WorldMapController)
      .AddBinding(Key.ComponentController, ComponentController)
      .Build();

    WorldMapRenderer? _worldMap;

    public WorldMapRenderer WorldMapRenderer =>
      _worldMap ??= new WorldMapRenderer(this);

    public WorldStaticAssets StaticAssets { get; }

    WorldMapController? _worldMapController;
    public WorldMapController WorldMapController => _worldMapController ??= new WorldMapController(this);

    public IComponentController<RootComponent> ComponentController { get; }

    public override void OnUpdate()
    {
      base.OnUpdate();
      WorldMapRenderer.OnUpdate();
      WorldMapController.OnUpdate();
      ComponentController.OnUpdate();
    }
  }
}