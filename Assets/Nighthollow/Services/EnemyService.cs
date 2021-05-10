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
using System.Collections.Immutable;
using System.Linq;
using Nighthollow.Data;
using Nighthollow.Interface;
using Nighthollow.Stats;
using Nighthollow.Utils;
using UnityEngine;

#nullable enable

namespace Nighthollow.Services
{
  public sealed class EnemyService : StatEntity
  {
    public EnemyService(GameData gameData)
    {
      EnemyState = EnemyState.BuildEnemyState(gameData);
      Deck = BuildStartingDeck(gameData, EnemyState);
    }

    EnemyService(EnemyState enemyState, DeckData deck)
    {
      EnemyState = enemyState;
      Deck = deck;
    }

    public EnemyState EnemyState { get; }

    public override StatTable Stats => EnemyState.Stats;

    public DeckData Deck { get; }

    static DeckData BuildStartingDeck(GameData gameData, EnemyState state)
    {
      var cards =
        gameData.BattleData.EnemyListOverride.HasValue
          ? gameData.ItemLists[gameData.BattleData.EnemyListOverride.Value].Creatures
          : gameData.BattleData.Enemies;
      return new DeckData(
        cards.Select(card => card.BuildCreature(gameData, state)).ToImmutableList(),
        orderedDraws: gameData.BattleData.UserDeckOverride.HasValue);
    }

    public sealed class Controller
    {
      readonly GameServiceRegistry _registry;
      readonly GameServiceRegistry.IEnemyServiceMutator _mutator;

      public Controller(GameServiceRegistry registry, GameServiceRegistry.IEnemyServiceMutator mutator)
      {
        _registry = registry;
        _mutator = mutator;
      }

      public void OnUpdate()
      {
        _mutator.SetEnemyService(
          new EnemyService(_registry.EnemyService.EnemyState.OnTick(_registry), _registry.EnemyService.Deck));
      }

      public void StartBattle()
      {
        _registry.CoroutineRunner.StartCoroutine(SpawnAsync());
      }

      public void InsertStatModifier(IStatModifier modifier)
      {
        var state = _registry.EnemyService.EnemyState;
        _mutator.SetEnemyService(
          new EnemyService(state.WithStats(state.Stats.InsertModifier(modifier)),
            _registry.EnemyService.Deck));
      }

      /// <summary>Called when an enemy is killed (not despawned).</summary>
      public void OnEnemyCreatureKilled()
      {
        _mutator.SetEnemyService(new EnemyService(
          _registry.EnemyService.EnemyState.WithDeathCount(_registry.EnemyService.EnemyState.DeathCount + 1),
          _registry.EnemyService.Deck));
        if (_registry.EnemyService.EnemyState.DeathCount >= _registry.EnemyService.Get(Stat.EnemiesToSpawn))
        {
          _registry.ScreenController.Get(ScreenController.GameOverMessage)
            .Show(new GameOverMessage.Args("Victory!", "World"), animate: true);
        }
      }

      /// <summary>Called when an enemy reaches the end of the battlefield.</summary>
      public void OnEnemyCreatureAtEndzone()
      {
        _registry.ScreenController.Get(ScreenController.GameOverMessage)
          .Show(new GameOverMessage.Args("Game Over", "World"));
        _registry.ScreenController.Get(ScreenController.BlackoutWindow).Show(argument: 0.5f);
        Time.timeScale = 0f;
      }

      IEnumerator<YieldInstruction> SpawnAsync()
      {
        yield return new WaitForSeconds(_registry.EnemyService.EnemyState.Get(Stat.InitialEnemySpawnDelay).AsSeconds());

        while (_registry.EnemyService.EnemyState.SpawnCount < _registry.EnemyService.Get(Stat.EnemiesToSpawn))
        {
          var deck = _registry.EnemyService.Deck.DrawCard(out var card);
          _mutator.SetEnemyService(new EnemyService(
            _registry.EnemyService.EnemyState.WithSpawnCount(_registry.EnemyService.EnemyState.SpawnCount + 1),
            deck));
          _registry.CreatureController.CreateMovingCreature(card, RandomFile(), Constants.EnemyCreatureStartingX);

          yield return new WaitForSeconds(_registry.EnemyService.Get(Stat.EnemySpawnDelay).AsSeconds());
        }

        // ReSharper disable once IteratorNeverReturns
      }

      FileValue RandomFile()
      {
        return FileValue.File3;
      }
    }
  }
}