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
using System.Collections.Immutable;
using Nighthollow.Data;
using Nighthollow.Interface;
using Nighthollow.Interface.Components.Core;
using Nighthollow.Interface.Components.Windows;
using Nighthollow.Rules;
using UnityEngine;
using UnityEngine.UIElements;

#nullable enable

namespace Nighthollow.Services
{
  public enum ServiceRegistryName
  {
    Unknown = 0,
    Battle = 1,
    World = 2,
    SchoolSelection = 3,
    Editor = 4
  }

  public static class ServiceRegistryBindings
  {
    public static ImmutableHashSet<IKey> Get(ServiceRegistryName name) => name switch
    {
      ServiceRegistryName.Battle => BattleServiceRegistry.Keys,
      ServiceRegistryName.World => WorldServiceRegistry.Keys,
      ServiceRegistryName.SchoolSelection => SchoolSelectionServiceRegistry.Keys,
      _ => throw new ArgumentOutOfRangeException(nameof(name), name, null)
    };
  }

  public abstract class ServiceRegistry
  {
    readonly VisualElement _rootVisualElement;

    protected ServiceRegistry(
      Database database,
      AssetService assetService,
      VisualElement rootVisualElement,
      Camera mainCamera,
      IStartCoroutine coroutineRunner)
    {
      Database = database;
      AssetService = assetService;
      _rootVisualElement = rootVisualElement;
      MainCamera = mainCamera;
      ObjectPoolService = new ObjectPoolService();
      Globals = new GlobalsService(database);
      CoroutineRunner = coroutineRunner;
      PlayerPrefs = new PlayerPrefsService();
      RewardsService = new RewardsService(Database);
    }

    public abstract ServiceRegistryName Name { get; }

    Scope? _scope;

    public static ImmutableHashSet<IKey> Keys => ImmutableHashSet.Create<IKey>(
      Key.Database,
      Key.AssetService,
      Key.ScreenController,
      Key.MainCamera,
      Key.RulesEngine,
      Key.PlayerPrefs,
      Key.RewardsService
    );

    public virtual Scope Scope => _scope ??= Scope.CreateBuilder(Keys)
      .AddBinding(Key.Database, Database)
      .AddBinding(Key.AssetService, AssetService)
      .AddBinding(Key.ScreenController, ScreenController)
      .AddBinding(Key.MainCamera, MainCamera)
      .AddBinding(Key.RulesEngine, RulesEngine)
      .AddBinding(Key.PlayerPrefs, PlayerPrefs)
      .AddBinding(Key.RewardsService, RewardsService)
      .Build();

    public virtual void OnUpdate()
    {
      ScreenController.OnUpdate();
    }

    public Database Database { get; }

    public AssetService AssetService { get; }

    ScreenController? _screenController;

    public ScreenController ScreenController =>
      _screenController ??= new ScreenController(_rootVisualElement, this);

    public Camera MainCamera { get; }

    public ObjectPoolService ObjectPoolService { get; }

    public GlobalsService Globals { get; }

    RulesEngine? _rulesEngine;
    public RulesEngine RulesEngine => _rulesEngine ??= new RulesEngine(this);

    public IStartCoroutine CoroutineRunner { get; }

    public PlayerPrefsService PlayerPrefs { get; }

    public RewardsService RewardsService { get; }
  }

  public sealed class EditorServiceRegistry : ServiceRegistry
  {
    public EditorServiceRegistry(
      Database database,
      AssetService assetService,
      VisualElement rootVisualElement,
      Camera mainCamera,
      IStartCoroutine coroutineRunner) : base(database, assetService, rootVisualElement, mainCamera, coroutineRunner)
    {
    }

    public override ServiceRegistryName Name => ServiceRegistryName.Editor;

    public new static ImmutableHashSet<IKey> Keys => ImmutableHashSet<IKey>.Empty;
  }

  public sealed class SchoolSelectionServiceRegistry : ServiceRegistry
  {
    public SchoolSelectionServiceRegistry(
      Database database,
      AssetService assetService,
      VisualElement rootVisualElement,
      Camera mainCamera,
      IStartCoroutine coroutineRunner) : base(database, assetService, rootVisualElement, mainCamera, coroutineRunner)
    {
    }

    public override ServiceRegistryName Name => ServiceRegistryName.SchoolSelection;

    public new static ImmutableHashSet<IKey> Keys => ImmutableHashSet.Create<IKey>(
      Key.ComponentController
    );

    Scope? _scope;

    public override Scope Scope => _scope ??= Scope.CreateBuilder(Keys, base.Scope)
      .AddBinding(Key.ComponentController, ComponentController)
      .Build();

    IComponentController<RootComponent>? _componentController;

    public IComponentController<RootComponent> ComponentController => _componentController ??=
      ComponentController<RootComponent>.Create(
        CoroutineRunner,
        this,
        ScreenController.Screen.Q("ComponentRoot"),
        new RootComponent());

    public override void OnUpdate()
    {
      base.OnUpdate();
      ComponentController.OnUpdate();
    }
  }
}