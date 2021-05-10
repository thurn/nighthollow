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
using Nighthollow.Utils;
using UnityEngine;

#nullable enable

namespace Nighthollow.Services
{
  public class ServiceRegistry
  {
    public ServiceRegistry(
      Database database,
      AssetService assetService,
      ScreenController screenController,
      Camera mainCamera,
      ObjectPoolService objectPoolService,
      Prefabs prefabs)
    {
      Database = database;
      AssetService = assetService;
      ScreenController = screenController;
      MainCamera = mainCamera;
      ObjectPoolService = objectPoolService;
      Prefabs = prefabs;
    }

    public Database Database { get; }
    public AssetService AssetService { get; }
    public ScreenController ScreenController { get; }
    public Camera MainCamera { get; }
    public ObjectPoolService ObjectPoolService { get; }
    public Prefabs Prefabs { get; }
  }

  public sealed class GameServiceRegistry : ServiceRegistry, IGameContext
  {
    public GameServiceRegistry(
      IStartCoroutine coroutineRunner,
      Database database,
      AssetService assetService,
      ScreenController screenController,
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
        screenController,
        mainCamera,
        objectPoolService,
        prefabs)
    {
      CoroutineRunner = coroutineRunner;
      Creatures = new CreatureService();
      var gameData = database.Snapshot();
      UserService = new UserService(hand, gameData);
      EnemyService = new EnemyService(gameData);

      MainCanvas = mainCanvas;
      DamageTextService = damageTextService;
      HelperTextService = helperTextService;
    }

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

    public void OnUpdate()
    {
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
      readonly GameServiceRegistry _registry;

      public CreatureServiceMutator(GameServiceRegistry registry)
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
      readonly GameServiceRegistry _registry;

      public UserServiceMutator(GameServiceRegistry registry)
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
      readonly GameServiceRegistry _registry;

      public EnemyServiceMutator(GameServiceRegistry registry)
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