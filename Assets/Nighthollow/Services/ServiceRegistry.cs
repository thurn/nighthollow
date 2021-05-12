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
using Nighthollow.Delegates;
using Nighthollow.Interface;
using Nighthollow.Triggers;
using Nighthollow.Utils;
using Nighthollow.World;
using UnityEngine;
using UnityEngine.UIElements;

#nullable enable

namespace Nighthollow.Services
{
  public class ServiceRegistry
  {
    readonly UIDocument _document;

    public ServiceRegistry(
      Database database,
      AssetService assetService,
      UIDocument document,
      Camera mainCamera,
      ObjectPoolService objectPoolService)
    {
      Database = database;
      AssetService = assetService;
      _document = document;
      MainCamera = mainCamera;
      ObjectPoolService = objectPoolService;
      Globals = new GlobalsService(database);
    }

    public virtual void OnUpdate()
    {
      ScreenController.OnUpdate();
    }

    public Database Database { get; }
    public AssetService AssetService { get; }
    ScreenController? _screenController;
    public ScreenController ScreenController => _screenController ??= new ScreenController(_document, this);
    public Camera MainCamera { get; }
    public ObjectPoolService ObjectPoolService { get; }
    TriggerService? _triggerService;
    public TriggerService TriggerService => _triggerService ??= new TriggerService(this);
    public GlobalsService Globals { get; }
  }

  public sealed class WorldServiceRegistry : ServiceRegistry
  {
    public WorldServiceRegistry(
      Database database,
      AssetService assetService,
      UIDocument document,
      Camera mainCamera,
      ObjectPoolService objectPoolService,
      WorldMap worldMap,
      WorldStaticAssets staticAssets) : base(database, assetService, document, mainCamera, objectPoolService)
    {
      WorldMap = worldMap;
      StaticAssets = staticAssets;
    }

    public WorldMap WorldMap { get; }
    public WorldStaticAssets StaticAssets { get; }

    WorldTutorial? _worldTutorial;
    public WorldTutorial WorldTutorial => _worldTutorial ??= new WorldTutorial(this);
  }

  public sealed class BattleServiceRegistry : ServiceRegistry, IGameContext
  {
    public BattleServiceRegistry(
      IStartCoroutine coroutineRunner,
      Database database,
      AssetService assetService,
      UIDocument document,
      Camera mainCamera,
      ObjectPoolService objectPoolService,
      Prefabs prefabs,
      RectTransform mainCanvas,
      Hand hand,
      DamageTextService damageTextService,
      HelperTextService helperTextService) :
      base(
        database,
        assetService,
        document,
        mainCamera,
        objectPoolService)
    {
      Prefabs = prefabs;
      CoroutineRunner = coroutineRunner;
      Creatures = new CreatureService();
      var gameData = database.Snapshot();
      UserService = new UserService(hand, gameData);
      EnemyService = new EnemyService(gameData);
      MainCanvas = mainCanvas;
      DamageTextService = damageTextService;
      HelperTextService = helperTextService;
    }

    public Prefabs Prefabs { get; }
    public IStartCoroutine CoroutineRunner { get; }
    public RectTransform MainCanvas { get; }
    public DamageTextService DamageTextService { get; }
    public HelperTextService HelperTextService { get; }

    public UserService UserService { get; private set; }
    UserService.Controller? _userController;

    public UserService.Controller UserController =>
      _userController ??= new UserService.Controller(this, new UserServiceMutator(this));

    public EnemyService EnemyService { get; private set; }
    EnemyService.Controller? _enemyController;

    public EnemyService.Controller EnemyController =>
      _enemyController ??= new EnemyService.Controller(this, new EnemyServiceMutator(this));

    public CreatureState this[CreatureId creatureId] => Creatures[creatureId];
    public CreatureService Creatures { get; private set; }
    CreatureService.Controller? _creatureController;

    public CreatureService.Controller CreatureController =>
      _creatureController ??= new CreatureService.Controller(this, new CreatureServiceMutator(this));

    public override void OnUpdate()
    {
      base.OnUpdate();
      UserController.OnUpdate();
      CreatureController.OnUpdate();
    }

    public void Invoke(IEventData arg)
    {
      var eventQueue = new Queue<IEventData>();
      eventQueue.Enqueue(arg);

      while (true)
      {
        var effects = eventQueue.Dequeue().Raise(this);

        foreach (var effect in effects)
        {
          effect.Execute(this);
          foreach (var eventData in effect.Events(this))
          {
            eventQueue.Enqueue(eventData);
          }
        }

        if (eventQueue.Count == 0)
        {
          break;
        }
      }
    }

    public IPlayerState StateForPlayer(PlayerName player)
    {
      return player switch
      {
        PlayerName.User => UserService.UserState,
        PlayerName.Enemy => EnemyService.EnemyState,
        _ => throw Errors.UnknownEnumValue(player)
      };
    }

    public interface ICreatureServiceMutator
    {
      void SetCreatureService(CreatureService service);

      void MutateCreatureService(Func<CreatureService, CreatureService> function);
    }

    sealed class CreatureServiceMutator : ICreatureServiceMutator
    {
      readonly BattleServiceRegistry _registry;

      public CreatureServiceMutator(BattleServiceRegistry registry)
      {
        _registry = registry;
      }

      public void SetCreatureService(CreatureService service)
      {
        _registry.Creatures = service;
      }

      public void MutateCreatureService(Func<CreatureService, CreatureService> function)
      {
        _registry.Creatures = function(_registry.Creatures);
      }
    }

    public interface IUserServiceMutator
    {
      void SetUserService(UserService userService);
    }

    sealed class UserServiceMutator : IUserServiceMutator
    {
      readonly BattleServiceRegistry _registry;

      public UserServiceMutator(BattleServiceRegistry registry)
      {
        _registry = registry;
      }

      public void SetUserService(UserService userService)
      {
        _registry.UserService = userService;
      }
    }

    public interface IEnemyServiceMutator
    {
      void SetEnemyService(EnemyService enemyService);
    }

    sealed class EnemyServiceMutator : IEnemyServiceMutator
    {
      readonly BattleServiceRegistry _registry;

      public EnemyServiceMutator(BattleServiceRegistry registry)
      {
        _registry = registry;
      }

      public void SetEnemyService(EnemyService enemyService)
      {
        _registry.EnemyService = enemyService;
      }
    }
  }
}