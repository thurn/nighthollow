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
using Nighthollow.State;
using Nighthollow.Stats;
using Nighthollow.Utils;
using UnityEngine;

#nullable enable

namespace Nighthollow.Services
{
  public sealed class CreatureService : MonoBehaviour
  {
    int _nextCreatureId = 1;

    readonly Dictionary<CreatureId, Creature> _creatures = new Dictionary<CreatureId, Creature>();

    readonly Dictionary<CreatureId, CreatureState> _creatureState = new Dictionary<CreatureId, CreatureState>();

    readonly HashSet<CreatureId> _movingCreatures = new HashSet<CreatureId>();

    readonly Dictionary<(RankValue, FileValue), CreatureId> _userCreatures =
      new Dictionary<(RankValue, FileValue), CreatureId>();

    GameServiceRegistry? _registry;

    public Creature GetCreature(CreatureId id)
    {
      Errors.CheckState(_creatures.ContainsKey(id), $"Creature with ID {id.Value} not found");
      return _creatures[id];
    }

    public CreatureState GetCreatureState(CreatureId id)
    {
      Errors.CheckState(_creatures.ContainsKey(id), $"Creature with ID {id.Value} not found");
      return _creatureState[id];
    }

    public void OnServicesReady(GameServiceRegistry registry)
    {
      _registry = registry;
    }

    public IEnumerable<CreatureId> EnemyCreatures()
    {
      // TODO: return opponent creatures instead
      return _movingCreatures.Where(c => _creatureState[c].Owner == PlayerName.Enemy);
    }

    public Creature CreateUserCreature(CreatureData creatureData)
    {
      var result = _registry!.AssetService.InstantiatePrefab<Creature>(creatureData.BaseType.PrefabAddress);
      var creatureId = new CreatureId(_nextCreatureId++);
      _creatures[creatureId] = result;
      _creatureState[creatureId] = new CreatureState(creatureId, creatureData, creatureData.BaseType.Owner);

      result.Initialize(_registry!, creatureId, creatureData.BaseType.Owner);
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
      var creatureState = new CreatureState(
        creatureId,
        creatureData,
        creatureData.BaseType.Owner,
        filePosition: file);
      _creatureState[creatureId] = creatureState;
      _movingCreatures.Add(creatureId);

      result.Initialize(_registry!, creatureId, creatureData.BaseType.Owner);
      result.ActivateCreature(
        creatureState,
        startingX: startingX);

      return result;
    }

    public void AddUserCreatureAtPosition(Creature creature, RankValue rank, FileValue file)
    {
      var id = creature.CreatureId;
      _userCreatures[(rank, file)] = id;
      _creatureState[id] = _creatureState[id].WithRankPosition(rank).WithFilePosition(file);
      creature.ActivateCreature(_creatureState[id]);
    }

    public void OnDeath(Creature creature)
    {
      if (!_creatures.ContainsKey(creature.CreatureId))
      {
        return;
      }

      var state = _creatureState[creature.CreatureId];

      if (state.RankPosition.HasValue && state.FilePosition.HasValue)
      {
        _userCreatures.Remove((state.RankPosition.Value, state.FilePosition.Value));
      }
      else if (state.FilePosition.HasValue)
      {
        _movingCreatures.Remove(creature.CreatureId);
      }
    }

    public void OnDestroyed(Creature creature)
    {
      if (!_creatures.ContainsKey(creature.CreatureId))
      {
        return;
      }

      _creatures.Remove(creature.CreatureId);
    }

    public CreatureState SetCurrentSkill(CreatureId creatureId, SkillData skillData)
    {
      var state = _creatureState[creatureId].WithCurrentSkill(skillData);
      _creatureState[creatureId] = state;
      return state;
    }

    public void InsertModifier(CreatureId creatureId, IStatModifier modifier)
    {
      var state = _creatureState[creatureId];
      _creatureState[creatureId] = state.WithData(state.Data.WithStats(state.Data.Stats.InsertModifier(modifier)));
    }

    public void InsertStatusEffect(CreatureId creatureId, StatusEffectData statusEffectData)
    {
      var state = _creatureState[creatureId];
      _creatureState[creatureId] =
        state.WithData(state.Data.WithStats(state.Data.Stats.InsertStatusEffect(statusEffectData)));
    }

    public void ExecuteMutatation(CreatureId creatureId, IMutation mutation)
    {
      var state = _creatureState[creatureId];
      _creatureState[creatureId] =
        state.WithData(state.Data.WithKeyValueStore(mutation.Mutate(state.Data.KeyValueStore)));
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

    void Update()
    {
      foreach (var pair in _creatures)
      {
        var state = _creatureState[pair.Key];
        _creatureState[pair.Key] = state.WithData(state.Data.OnTick());
        pair.Value.OnUpdate(_creatureState[pair.Key]);
      }
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