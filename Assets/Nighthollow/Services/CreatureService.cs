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
using Nighthollow.Utils;
using UnityEngine;

#nullable enable

namespace Nighthollow.Services
{
  public sealed class CreatureService : MonoBehaviour
  {
    int _nextCreatureId = 1;

    readonly Dictionary<CreatureId, Creature> _creatures = new Dictionary<CreatureId, Creature>();

    readonly HashSet<Creature> _movingCreatures = new HashSet<Creature>();

    readonly Dictionary<(RankValue, FileValue), Creature> _userCreatures =
      new Dictionary<(RankValue, FileValue), Creature>();

    GameServiceRegistry? _registry;

    public Creature GetCreature(CreatureId id)
    {
      Errors.CheckState(_creatures.ContainsKey(id), $"Creature with ID {id.Value} not found");
      return _creatures[id];
    }

    public void OnServicesReady(GameServiceRegistry registry)
    {
      _registry = registry;
    }

    public IEnumerable<CreatureId> EnemyCreatures()
    {
      return _movingCreatures.Where(c => c.Owner == PlayerName.Enemy).Select(c => c.CreatureId!.Value);
    }

    public Creature CreateUserCreature(CreatureData creatureData)
    {
      var result = _registry!.AssetService.InstantiatePrefab<Creature>(creatureData.BaseType.PrefabAddress);
      result.Initialize(_registry!, creatureData);
      return result;
    }

    public Creature CreateMovingCreature(
      CreatureData creatureData,
      FileValue file,
      float startingX)
    {
      var result = _registry!.AssetService.InstantiatePrefab<Creature>(creatureData.BaseType.PrefabAddress);
      var creatureId = new CreatureId(_nextCreatureId++);
      _creatures[creatureId] = result;
      _movingCreatures.Add(result);

      result.Initialize(_registry!, creatureData);
      result.ActivateCreature(
        creatureId: creatureId,
        rankValue: null,
        fileValue: file,
        startingX: startingX);

      return result;
    }

    public void AddUserCreatureAtPosition(Creature creature, RankValue rank, FileValue file)
    {
      var creatureId = new CreatureId(_nextCreatureId++);
      _userCreatures[(rank, file)] = creature;
      _creatures[creatureId] = creature;

      creature.ActivateCreature(creatureId, rank, file);
    }

    public void RemoveCreature(Creature creature)
    {
      if (creature.CreatureId == null || !_creatures.ContainsKey(creature.CreatureId.Value))
      {
        return;
      }

      _creatures.Remove(creature.CreatureId.Value);

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
    public IEnumerable<CreatureId> GetAdjacentUserCreatures(RankValue inputRank, FileValue inputFile) =>
      from rank in BoardPositions.AdjacentRanks(inputRank)
      from file in BoardPositions.AdjacentFiles(inputFile)
      where _userCreatures.ContainsKey((rank, file))
      select _userCreatures[(rank, file)].CreatureId.Value;

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

  public readonly struct CreatureId
  {
    public readonly int Value;

    public CreatureId(int value)
    {
      Value = value;
    }

    public bool Equals(CreatureId other)
    {
      return Value == other.Value;
    }

    public override bool Equals(object? obj)
    {
      return obj is CreatureId other && Equals(other);
    }

    public override int GetHashCode()
    {
      return Value;
    }

    public static bool operator ==(CreatureId left, CreatureId right)
    {
      return left.Equals(right);
    }

    public static bool operator !=(CreatureId left, CreatureId right)
    {
      return !left.Equals(right);
    }

    public override string ToString() => Value.ToString();
  }
}