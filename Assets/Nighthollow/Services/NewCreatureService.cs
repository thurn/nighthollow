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
using System.Collections.Immutable;
using JetBrains.Annotations;
using System.Linq;
using System.Numerics;
using Nighthollow.Components;
using Nighthollow.Data;
using Nighthollow.Delegates;

#nullable enable

namespace Nighthollow.Services
{
  public sealed class NewCreatureService
  {
    readonly GameServiceRegistry _registry;
    readonly DelegateContext _delegateContext;
    readonly ImmutableDictionary<int, CreatureState> _creatureState;
    readonly ImmutableDictionary<int, Creature> _creatures;
    readonly ImmutableDictionary<(RankValue, FileValue), int> _userCreatures;
    readonly ImmutableList<int> _enemyCreatures;
    readonly int _nextCreatureId;

    public NewCreatureService(GameServiceRegistry registry) : this(
      registry,
      new DelegateContext(registry),
      ImmutableDictionary<int, CreatureState>.Empty,
      ImmutableDictionary<int, Creature>.Empty,
      ImmutableDictionary<(RankValue, FileValue), int>.Empty,
      ImmutableList<int>.Empty,
      nextCreatureId: 1)
    {
    }

    NewCreatureService(
      GameServiceRegistry registry,
      DelegateContext delegateContext,
      ImmutableDictionary<int, CreatureState> creatureState,
      ImmutableDictionary<int, Creature> creatures,
      ImmutableDictionary<(RankValue, FileValue), int> userCreatures,
      ImmutableList<int> enemyCreatures,
      int nextCreatureId)
    {
      _registry = registry;
      _delegateContext = delegateContext;
      _creatureState = creatureState;
      _creatures = creatures;
      _userCreatures = userCreatures;
      _enemyCreatures = enemyCreatures;
      _nextCreatureId = nextCreatureId;
    }

    [MustUseReturnValue]
    public NewCreatureService CreateUserCreature(CreatureData data, out Creature result)
    {
      result = _registry.AssetService.InstantiatePrefab<Creature>(data.BaseType.PrefabAddress);
      result.Initialize(_registry.Prefabs, data.BaseType.Owner);
      return new NewCreatureService(
        _registry,
        _delegateContext,
        _creatureState.SetItem(_nextCreatureId,
          new CreatureState(null! /* _nextCreatureId */,
            data,
            CreatureAnimation.Placing,
            owner: data.BaseType.Owner)),
        _creatures.SetItem(_nextCreatureId, result),
        _userCreatures,
        _enemyCreatures,
        _nextCreatureId + 1);
    }

    [MustUseReturnValue]
    public IEnumerable<Effect> OnUpdate(GameContext c) =>
      _creatures.SelectMany(pair => pair.Value.OnUpdate(_delegateContext, _creatureState[pair.Key]));

    [MustUseReturnValue]
    public NewCreatureService CreateMovingCreature(
      CreatureData data,
      FileValue file,
      float startingX,
      out Creature result,
      out IEnumerable<Effect> effects)
    {
      var owner = data.BaseType.Owner;
      result = _registry.AssetService.InstantiatePrefab<Creature>(data.BaseType.PrefabAddress);
      result.Initialize(_registry.Prefabs, owner);
      var state = new CreatureState(null! /* _nextCreatureId */,
        data,
        CreatureAnimation.Moving,
        owner: owner,
        filePosition: file,
        startingX: startingX);
      effects = result.OnUpdate(_delegateContext, state);

      return new NewCreatureService(
        _registry,
        _delegateContext,
        _creatureState.SetItem(_nextCreatureId, state),
        _creatures.SetItem(_nextCreatureId, result),
        _userCreatures,
        owner == PlayerName.Enemy ? _enemyCreatures.Add(_nextCreatureId) : _enemyCreatures,
        _nextCreatureId + 1);
    }

    [MustUseReturnValue]
    public NewCreatureService SetUserCreatureToPosition(
      int creatureId,
      RankValue rank,
      FileValue file,
      out IEnumerable<Effect> effects)
    {
      var state = _creatureState[creatureId]
        .WithRankPosition(rank)
        .WithFilePosition(file)
        .WithAnimation(CreatureAnimation.Idle);
      effects = _creatures[creatureId].OnUpdate(_delegateContext, state);

      return new NewCreatureService(
        _registry,
        _delegateContext,
        _creatureState.SetItem(creatureId, state),
        _creatures,
        _userCreatures.SetItem((rank, file), creatureId),
        _enemyCreatures,
        _nextCreatureId);
    }

    /// <summary>
    ///   Returns the first open rank position in front of this (rank, file) if one exists
    /// </summary>
    public RankValue? GetOpenForwardRank(RankValue rank, FileValue file)
    {
      while (true)
      {
        var result = rank.Increment();
        if (result == null)
        {
          return null;
        }

        if (!_userCreatures.ContainsKey((result.Value, file)))
        {
          return result.Value;
        }

        rank = result.Value;
      }
    }

    /// <summary>
    /// Returns all User creatures IDs in the 9 squares around the given (rank, file) position (including the
    /// creature at that position, if any).
    /// </summary>
    public IEnumerable<int> GetAdjacentUserCreatures(RankValue inputRank, FileValue inputFile) =>
      from rank in BoardPositions.AdjacentRanks(inputRank)
      from file in BoardPositions.AdjacentFiles(inputFile)
      where _userCreatures.ContainsKey((rank, file))
      select _userCreatures[(rank, file)];

    /// <summary>Gets the position closest file to 'filePosition' which is not full.</summary>
    public (RankValue, FileValue) GetClosestAvailablePosition(Vector2 position)
    {
      RankValue? closestRank = null;
      FileValue? closestFile = null;
      var closestDistance = float.MaxValue;

      foreach (var rank in BoardPositions.AllRanks)
      foreach (var file in BoardPositions.AllFiles)
      {
        if (rank == RankValue.Unknown ||
            file == FileValue.Unknown ||
            _userCreatures.ContainsKey((rank, file)))
        {
          continue;
        }

        var distance = Vector2.Distance(position,
          new Vector2(rank.ToXPosition(), file.ToYPosition()));
        if (distance < closestDistance)
        {
          closestDistance = distance;
          closestRank = rank;
          closestFile = file;
        }
      }

      if (closestRank == null || closestFile == null)
      {
        throw new InvalidOperationException("Board is full!");
      }

      return (closestRank.Value, closestFile.Value);
    }
  }
}