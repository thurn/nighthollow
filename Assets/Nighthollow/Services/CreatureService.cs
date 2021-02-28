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
using Nighthollow.Components;
using Nighthollow.Data;
using Nighthollow.State;
using Nighthollow.Stats;
using Nighthollow.Utils;
using UnityEngine;

#nullable enable

namespace Nighthollow.Services
{
  public interface ICreatureService
  {
    /// <summary>Map of all currently-known creatures, including creatures in the 'placing' or 'dying' states</summary>
    ImmutableDictionary<CreatureId, CreatureState> Creatures { get; }

    /// <summary>Allows the look up of creature states by creature ID.</summary>
    CreatureState this[CreatureId index] { get; }

    /// <summary>Set of both user and enemy creature IDs which are not anchored to a specific board position</summary>
    ImmutableHashSet<CreatureId> MovingCreatures { get; }

    /// <summary>Map of both user and enemy creature IDs which have a specific board position</summary>
    ImmutableDictionary<(RankValue, FileValue), CreatureId> PlacedCreatures { get; }

    /// <summary>Return the collider for a given creature.</summary>
    Collider2D GetCollider(CreatureId creatureId);

    /// <summary>Returns the current position of a creature.</summary>
    Vector2 GetPosition(CreatureId creatureId);

    /// <summary>Returns the position from which a creature should fire projectiles.</summary>
    Vector2 GetProjectileSourcePosition(CreatureId creatureId);
  }

  public sealed class CreatureService : MonoBehaviour, ICreatureService
  {
    int _nextCreatureId = 1;

    readonly Dictionary<CreatureId, Creature> _creatures = new Dictionary<CreatureId, Creature>();

    readonly Dictionary<CreatureId, CreatureState> _creatureState = new Dictionary<CreatureId, CreatureState>();

    readonly HashSet<CreatureId> _movingCreatures = new HashSet<CreatureId>();

    readonly Dictionary<(RankValue, FileValue), CreatureId> _userCreatures =
      new Dictionary<(RankValue, FileValue), CreatureId>();

    GameServiceRegistry? _registry;

#region ICreatureService

    public ImmutableDictionary<CreatureId, CreatureState> Creatures => _creatureState.ToImmutableDictionary();

    public CreatureState this[CreatureId index] => _creatureState[index];

    public ImmutableHashSet<CreatureId> MovingCreatures => _movingCreatures.ToImmutableHashSet();

    public ImmutableDictionary<(RankValue, FileValue), CreatureId> PlacedCreatures =>
      _userCreatures.ToImmutableDictionary();

    public Collider2D GetCollider(CreatureId creatureId) => _creatures[creatureId].Collider;

    public Vector2 GetPosition(CreatureId creatureId) => _creatures[creatureId].transform.position;

    public Vector2 GetProjectileSourcePosition(CreatureId creatureId) =>
      _creatures[creatureId].ProjectileSource.position;

#endregion

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

      _creatureState[creature.CreatureId] = state.WithIsAlive(false);
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

    public CreatureState SetDamageTaken(CreatureId creatureId, int damageTaken)
    {
      var state = _creatureState[creatureId].WithDamageTaken(damageTaken);
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

    public void MarkSkillUsed(CreatureId creatureId, int skillId)
    {
      var state = _creatureState[creatureId];
      _creatureState[creatureId] = state.WithSkillLastUsedTimes(state.SkillLastUsedTimes.SetItem(skillId, Time.time));
    }

    void Update()
    {
      foreach (var pair in _creatures)
      {
        var state = _creatureState[pair.Key];
        _creatureState[pair.Key] = state.WithData(state.Data.OnTick(_registry!.Context));
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