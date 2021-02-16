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
using Nighthollow.Interface;
using Nighthollow.Stats;
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
      ObjectPoolService objectPoolService,
      Prefabs prefabs)
    {
      Database = database;
      AssetService = assetService;
      ScreenController = screenController;
      ObjectPoolService = objectPoolService;
      Prefabs = prefabs;
    }

    public Database Database { get; }
    public AssetService AssetService { get; }
    public ScreenController ScreenController { get; }
    public ObjectPoolService ObjectPoolService { get; }
    public Prefabs Prefabs { get; }
  }

  public class GameServiceRegistry : ServiceRegistry
  {
    public GameServiceRegistry(
      Database database,
      AssetService assetService,
      ScreenController screenController,
      ObjectPoolService objectPoolService,
      Prefabs prefabs,
      RectTransform mainCanvas,
      CreatureService creatureService,
      User user,
      Enemy enemy,
      DamageTextService damageTextService,
      HelperTextService helperTextService) :
      base(
        database,
        assetService,
        screenController,
        objectPoolService,
        prefabs)
    {
      MainCanvas = mainCanvas;
      CreatureService = creatureService;
      User = user;
      Enemy = enemy;
      DamageTextService = damageTextService;
      HelperTextService = helperTextService;
    }

    public RectTransform MainCanvas { get; }
    public CreatureService CreatureService { get; }
    public User User { get; }
    public Enemy Enemy { get; }
    public DamageTextService DamageTextService { get; }
    public HelperTextService HelperTextService { get; }

    public StatTable StatsForPlayer(PlayerName player)
    {
      return player switch
      {
        PlayerName.User => User.Data.Stats,
        PlayerName.Enemy => Enemy.Data.Stats,
        _ => throw Errors.UnknownEnumValue(player)
      };
    }
  }
}