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
using DG.Tweening;
using Nighthollow.Components;
using Nighthollow.Data;
using Nighthollow.Delegates;
using Nighthollow.Delegates.Handlers;
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

  public sealed class CreatureService : ICreatureService
  {
    int _nextCreatureId = 1;

    readonly Dictionary<CreatureId, Creature> _components = new Dictionary<CreatureId, Creature>();

    readonly Dictionary<CreatureId, CreatureState> _creatures = new Dictionary<CreatureId, CreatureState>();

    readonly HashSet<CreatureId> _movingCreatures = new HashSet<CreatureId>();

    readonly Dictionary<(RankValue, FileValue), CreatureId> _userCreatures =
      new Dictionary<(RankValue, FileValue), CreatureId>();

    #region ICreatureService

    public ImmutableDictionary<CreatureId, CreatureState> Creatures => _creatures.ToImmutableDictionary();

    public CreatureState this[CreatureId index] => _creatures[index];

    public ImmutableHashSet<CreatureId> MovingCreatures => _movingCreatures.ToImmutableHashSet();

    public ImmutableDictionary<(RankValue, FileValue), CreatureId> PlacedCreatures =>
      _userCreatures.ToImmutableDictionary();

    public Collider2D GetCollider(CreatureId creatureId) => _components[creatureId].Collider;

    public Vector2 GetPosition(CreatureId creatureId) => _components[creatureId].transform.position;

    public Vector2 GetProjectileSourcePosition(CreatureId creatureId) =>
      _components[creatureId].ProjectileSource.position;

    #endregion

    public void OnUpdate(GameContext c)
    {
      foreach (var pair in _components)
      {
        var state = _creatures[pair.Key];
        _creatures[pair.Key] = state.WithData(state.Data.OnTick(c));
        pair.Value.OnUpdate(_creatures[pair.Key]);
      }
    }

    public CreatureId CreateUserCreature(
      GameServiceRegistry registry,
      CreatureData creatureData,
      Action<CreatureId, CreaturePositionSelector>? addPositionSelector)
    {
      var result = registry.AssetService.InstantiatePrefab<Creature>(creatureData.BaseType.PrefabAddress);
      var creatureId = new CreatureId(_nextCreatureId++);
      _components[creatureId] = result;
      _creatures[creatureId] = new CreatureState(creatureId, creatureData, creatureData.BaseType.Owner);

      result.Initialize(registry, creatureId, creatureData.BaseType.Owner);
      addPositionSelector?.Invoke(creatureId, result.gameObject.AddComponent<CreaturePositionSelector>());
      return creatureId;
    }

    public void CreateMovingCreature(
      GameServiceRegistry registry,
      CreatureData creatureData,
      FileValue file,
      float startingX)
    {
      var result = registry.AssetService.InstantiatePrefab<Creature>(creatureData.BaseType.PrefabAddress);
      var creatureId = new CreatureId(_nextCreatureId++);
      _components[creatureId] = result;
      var creatureState = new CreatureState(
        creatureId,
        creatureData,
        creatureData.BaseType.Owner,
        filePosition: file);
      _creatures[creatureId] = creatureState;
      _movingCreatures.Add(creatureId);

      result.Initialize(registry, creatureId, creatureData.BaseType.Owner);
      result.ActivateCreature(
        creatureState,
        startingX: startingX);
    }

    public void AddUserCreatureAtPosition(CreatureId creatureId, RankValue rank, FileValue file)
    {
      _userCreatures[(rank, file)] = creatureId;
      _creatures[creatureId] = _creatures[creatureId].WithRankPosition(rank).WithFilePosition(file);
      _components[creatureId].ActivateCreature(_creatures[creatureId]);
    }

    public CreatureState Mutate(CreatureId creatureId, Func<CreatureState, CreatureState> mutation)
    {
      var result = mutation(this[creatureId]);
      _creatures[creatureId] = result;
      return result;
    }

    public void AddDamage(GameServiceRegistry registry, CreatureId appliedById, CreatureId targetId, int damage)
    {
      var targetState = this[targetId];
      Errors.CheckArgument(damage >= 0, "Damage must be non-negative");
      var health = targetState.GetInt(Stat.Health);
      var newState = Mutate(targetId, s => s.WithDamageTaken(Mathf.Clamp(value: 0, s.DamageTaken + damage, health)));
      if (newState.DamageTaken >= health)
      {
        var appliedByState = this[appliedById];
        registry.Invoke(appliedById, new IOnKilledEnemy.Data(appliedByState));
        registry.Invoke(targetId, new IOnCreatureDeath.Data(newState));
        _components[targetId].Kill();
        OnDeath(targetId);
      }
    }

    public void Heal(CreatureId creatureId, int healing)
    {
      var state = this[creatureId];
      Errors.CheckArgument(healing >= 0, "Healing must be non-negative");
      var health = state.GetInt(Stat.Health);
      Mutate(creatureId, s => s.WithDamageTaken(Mathf.Clamp(value: 0, state.DamageTaken - healing, health)));
    }

    public void ApplyKnockback(CreatureId target, float distance, float durationSeconds)
    {
      var t = _components[target].transform;
      t.DOMove(
        (Vector2) t.position +
        distance *
        Constants.ForwardDirectionForPlayer(this[target].Owner.GetOpponent()),
        durationSeconds);
    }

    public void ApplyStun(CreatureId target, float durationSeconds)
    {
      _components[target].Stun(durationSeconds);
    }

    public void SetAnimationPaused(CreatureId target, bool animationPaused)
    {
      _components[target].SetAnimationPaused(animationPaused);
    }

    public void DespawnCreature(CreatureId target)
    {
      if (!_components.ContainsKey(target))
      {
        return;
      }

      _components[target].Despawn();

      _creatures.Remove(target);
      _components.Remove(target);
    }

    void OnDeath(CreatureId creatureId)
    {
      if (!_components.ContainsKey(creatureId))
      {
        return;
      }

      var state = _creatures[creatureId];

      if (state.RankPosition.HasValue && state.FilePosition.HasValue)
      {
        _userCreatures.Remove((state.RankPosition.Value, state.FilePosition.Value));
      }
      else if (state.FilePosition.HasValue)
      {
        _movingCreatures.Remove(creatureId);
      }

      Mutate(creatureId, s => s.WithIsAlive(false));
    }
  }

  public readonly struct CreatureId : IDelegateLocator
  {
    public readonly int Value;

    public CreatureId(int value)
    {
      Value = value;
    }

    public bool Equals(CreatureId other) => Value == other.Value;

    public override bool Equals(object? obj) => obj is CreatureId other && Equals(other);

    public override int GetHashCode() => Value;

    public static bool operator ==(CreatureId left, CreatureId right) => left.Equals(right);

    public static bool operator !=(CreatureId left, CreatureId right) => !left.Equals(right);

    public override string ToString() => Value.ToString();

    public DelegateList GetDelegateList(GameContext c)
    {
      var state = c.CreatureService[this];
      return state.CurrentSkill != null ? state.CurrentSkill.DelegateList : state.Data.DelegateList;
    }
  }
}
