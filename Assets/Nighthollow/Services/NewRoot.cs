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

using System.Collections.Generic;
using Nighthollow.Data;
using Nighthollow.Delegates;
using Nighthollow.Interface;
using Nighthollow.Utils;
using UnityEngine;

#nullable enable

namespace Nighthollow.Services
{
  public sealed class GameContext
  {
    public GameContext(
      GameData gameData,
      AssetService assetService,
      ObjectPoolService objectPoolService,
      Prefabs prefabs,
      NewCreatureService creatureService)
    {
      GameData = gameData;
      AssetService = assetService;
      ObjectPoolService = objectPoolService;
      Prefabs = prefabs;
      CreatureService = creatureService;
    }

    public GameData GameData { get; }
    public AssetService AssetService { get; }
    public ObjectPoolService ObjectPoolService { get; }
    public Prefabs Prefabs { get; }
    public NewCreatureService CreatureService { get; }

    public GameContext WithGameData(GameData gameData) =>
      ReferenceEquals(gameData, GameData)
        ? this
        : new GameContext(gameData, AssetService, ObjectPoolService, Prefabs, CreatureService);

    public GameContext WithCreatureService(NewCreatureService creatureService) =>
      ReferenceEquals(creatureService, CreatureService)
        ? this
        : new GameContext(GameData, AssetService, ObjectPoolService, Prefabs, CreatureService);
  }

  public sealed class NewRoot : MonoBehaviour
  {
    [SerializeField] ScreenController? _screenController;
    [SerializeField] DataService? _dataService;
    [SerializeField] ObjectPoolService? _objectPoolService;
    [SerializeField] Prefabs? _prefabs;

    Database? _database;
    AssetService? _assetService;
    GameContext? _context;

    void Start()
    {
      Errors.CheckNotNull(_screenController).Initialize();
      Errors.CheckNotNull(_dataService).OnReady(OnDataFetched);
    }

    void OnDataFetched(FetchResult result)
    {
      _database = result.Database;
      _assetService = result.AssetService;
      _context = new GameContext(
        _database.Snapshot(),
        _assetService,
        Errors.CheckNotNull(_objectPoolService),
        Errors.CheckNotNull(_prefabs),
        new NewCreatureService(null!));
    }

    void Update()
    {
      if (_database != null && _context != null)
      {
        _context = _context.WithGameData(_database.Snapshot());
        ProcessEffects(_context, _context.CreatureService.OnUpdate(_context));
      }
    }

    void ProcessEffects(GameContext c, IEnumerable<Effect> effects)
    {
      var queue = new Queue<Effect>(effects);
      while (queue.Count > 0)
      {
        var effect = queue.Dequeue();
        effect.Execute(c);
        foreach (var e in effect.RaiseTriggeredEvents(c))
        {
          queue.Enqueue(e);
        }
      }
    }
  }
}