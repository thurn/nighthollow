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
using System.Linq;
using Nighthollow.Components;
using Nighthollow.Data;
using UnityEngine;

#nullable enable

namespace Nighthollow.Services
{
  public sealed class CreatureService : MonoBehaviour
  {
    readonly HashSet<Creature2> _movingCreatures = new HashSet<Creature2>();

    readonly Dictionary<(RankValue, FileValue), Creature2> _userCreatures =
      new Dictionary<(RankValue, FileValue), Creature2>();

    GameServiceRegistry? _registry;

    public void OnServicesReady(GameServiceRegistry registry)
    {
      _registry = registry;
    }

    public IEnumerable<Creature2> EnemyCreatures()
    {
      return _movingCreatures.Where(c => c.Owner == PlayerName.Enemy);
    }

    public Creature2 CreateUserCreature(CreatureData creatureData)
    {
      var result = _registry!.AssetService.InstantiatePrefab<Creature2>(creatureData.BaseType.PrefabAddress);
      result.Initialize(_registry!, creatureData);
      return result;
    }

    public Creature2 CreateMovingCreature(
      CreatureData creatureData,
      FileValue file,
      float startingX)
    {
      var result = _registry!.AssetService.InstantiatePrefab<Creature2>(creatureData.BaseType.PrefabAddress);

      result.Initialize(_registry!, creatureData);
      result.ActivateCreature(rankValue: null, file, startingX);
      _movingCreatures.Add(result);

      return result;
    }

    public void AddUserCreatureAtPosition(Creature2 creature, RankValue rank, FileValue file)
    {
      creature.ActivateCreature(rank, file);
      _userCreatures[(rank, file)] = creature;
    }

    public void RemoveCreature(Creature2 creature)
    {
      if (creature.IsMoving)
      {
        _movingCreatures.Remove(creature);
      }
      else if (creature.RankPosition.HasValue)
      {
        _userCreatures.Remove((creature.RankPosition.Value, creature.FilePosition));
      }
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
    ///   Returns all User creatures in the 9 squares around the given (rank, file) position (including the creature at
    ///   that position, if any).
    /// </summary>
    public IEnumerable<Creature2> GetAdjacentUserCreatures(RankValue inputRank, FileValue inputFile) =>
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
