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
  public sealed class CreatureService
  {
    readonly int _nextCreatureId;
    readonly ImmutableDictionary<CreatureId, Creature> _components;

    public CreatureService() : this(
      1,
      ImmutableDictionary<CreatureId, Creature>.Empty,
      ImmutableDictionary<CreatureId, CreatureState>.Empty,
      ImmutableHashSet<CreatureId>.Empty,
      ImmutableDictionary<(RankValue, FileValue), CreatureId>.Empty)
    {
    }

    CreatureService(
      int nextCreatureId,
      ImmutableDictionary<CreatureId, Creature> components,
      ImmutableDictionary<CreatureId, CreatureState> creatures,
      ImmutableHashSet<CreatureId> movingCreatures,
      ImmutableDictionary<(RankValue, FileValue), CreatureId> placedCreatures)
    {
      _nextCreatureId = nextCreatureId;
      _components = components;
      Creatures = creatures;
      MovingCreatures = movingCreatures;
      PlacedCreatures = placedCreatures;
    }

    public ImmutableDictionary<CreatureId, CreatureState> Creatures { get; }

    public CreatureState this[CreatureId index] => Creatures[index];

    public ImmutableHashSet<CreatureId> MovingCreatures { get; }

    public ImmutableDictionary<(RankValue, FileValue), CreatureId> PlacedCreatures { get; }

    public Collider2D GetCollider(CreatureId creatureId) => _components[creatureId].Collider;

    public Vector2 GetPosition(CreatureId creatureId) => _components[creatureId].transform.position;

    public Vector2 GetProjectileSourcePosition(CreatureId creatureId) =>
      _components[creatureId].ProjectileSource.position;

    public static void OnUpdate(GameServiceRegistry registry)
    {
      registry.MutateCreatures(self => new CreatureService(
        self._nextCreatureId,
        self._components,
        self.Creatures.ToImmutableDictionary(
          pair => pair.Key,
          pair => pair.Value.WithData(pair.Value.Data.OnTick(registry))),
        self.MovingCreatures,
        self.PlacedCreatures));

      foreach (var component in registry.Creatures._components.Values)
      {
        component.OnUpdate();
      }
    }

    public static CreatureId CreateUserCreature(
      GameServiceRegistry registry,
      CreatureData creatureData,
      Card? addPositionSelector = null)
    {
      var result = registry.AssetService.InstantiatePrefab<Creature>(creatureData.BaseType.PrefabAddress);
      var creatureId = new CreatureId(registry.Creatures._nextCreatureId);
      registry.MutateCreatures(self => new CreatureService(
        self._nextCreatureId + 1,
        self._components.SetItem(creatureId, result),
        self.Creatures.SetItem(creatureId, new CreatureState(creatureId, creatureData, creatureData.BaseType.Owner)),
        self.MovingCreatures,
        self.PlacedCreatures));

      result.Initialize(registry, creatureId, creatureData.BaseType.Name, creatureData.BaseType.Owner);
      if (addPositionSelector)
      {
        result.gameObject.AddComponent<CreaturePositionSelector>()
          .Initialize(registry, creatureId, addPositionSelector);
      }

      return creatureId;
    }

    public static void CreateMovingCreature(
      GameServiceRegistry registry,
      CreatureData creatureData,
      FileValue file,
      float startingX)
    {
      var result = registry.AssetService.InstantiatePrefab<Creature>(creatureData.BaseType.PrefabAddress);
      var creatureId = new CreatureId(registry.Creatures._nextCreatureId);
      var creatureState = new CreatureState(
        creatureId,
        creatureData,
        creatureData.BaseType.Owner,
        filePosition: file);

      registry.MutateCreatures(self => new CreatureService(
        self._nextCreatureId + 1,
        self._components.SetItem(creatureId, result),
        self.Creatures.SetItem(creatureId, creatureState),
        self.MovingCreatures.Add(creatureId),
        self.PlacedCreatures));

      result.Initialize(registry, creatureId, creatureData.BaseType.Name, creatureData.BaseType.Owner);
      result.ActivateCreature(startingX: startingX);
    }

    public static void AddUserCreatureAtPosition(
      GameServiceRegistry registry,
      CreatureId creatureId,
      RankValue rank,
      FileValue file)
    {
      registry.MutateCreatures(self => new CreatureService(
        self._nextCreatureId,
        self._components,
        self.Creatures.SetItem(creatureId, self.Creatures[creatureId].WithRankPosition(rank).WithFilePosition(file)),
        self.MovingCreatures,
        self.PlacedCreatures.SetItem((rank, file), creatureId)));
      registry.Creatures._components[creatureId].ActivateCreature();
    }

    public static void Mutate(
      GameServiceRegistry registry,
      CreatureId creatureId,
      Func<CreatureState, CreatureState> mutation)
    {
      registry.MutateCreatures(self => new CreatureService(
        self._nextCreatureId,
        self._components,
        self.Creatures.SetItem(creatureId, mutation(self[creatureId])),
        self.MovingCreatures,
        self.PlacedCreatures));
    }

    public static void AddDamage(GameServiceRegistry registry, CreatureId appliedById, CreatureId targetId, int damage)
    {
      Errors.CheckArgument(damage >= 0, "Damage must be non-negative");
      var health = registry.Creatures[targetId].GetInt(Stat.Health);
      Mutate(
        registry,
        targetId,
        s => s.WithDamageTaken(Mathf.Clamp(value: 0, s.DamageTaken + damage, health)));
      if (registry.Creatures[targetId].DamageTaken >= health)
      {
        registry.Invoke(appliedById, new IOnKilledEnemy.Data(appliedById));
        registry.Invoke(targetId, new IOnCreatureDeath.Data(targetId));
        registry.Creatures._components[targetId].Kill();
        OnDeath(registry, targetId);
      }
    }

    public static void Heal(GameServiceRegistry registry, CreatureId creatureId, int healing)
    {
      var state = registry.Creatures[creatureId];
      Errors.CheckArgument(healing >= 0, "Healing must be non-negative");
      var health = state.GetInt(Stat.Health);
      Mutate(registry, creatureId, s => s.WithDamageTaken(Mathf.Clamp(value: 0, state.DamageTaken - healing, health)));
    }

    public static void ApplyKnockback(
      GameServiceRegistry registry, CreatureId target, float distance, float durationSeconds)
    {
      var t = registry.Creatures._components[target].transform;
      t.DOMove(
        (Vector2) t.position +
        distance *
        Constants.ForwardDirectionForPlayer(registry.Creatures[target].Owner.GetOpponent()),
        durationSeconds);
    }

    public static void ApplyStun(GameServiceRegistry registry, CreatureId target, float durationSeconds)
    {
      registry.Creatures._components[target].Stun(durationSeconds);
    }

    public static void SetAnimationPaused(GameServiceRegistry registry, CreatureId target, bool animationPaused)
    {
      registry.Creatures._components[target].SetAnimationPaused(animationPaused);
    }

    public static void DespawnCreature(GameServiceRegistry registry, CreatureId target)
    {
      if (!registry.Creatures._components.ContainsKey(target))
      {
        return;
      }

      registry.Creatures._components[target].Despawn();

      registry.MutateCreatures(self => new CreatureService(
        self._nextCreatureId,
        self._components.Remove(target),
        self.Creatures.Remove(target),
        self.MovingCreatures,
        self.PlacedCreatures));
    }

    static void OnDeath(GameServiceRegistry registry, CreatureId creatureId)
    {
      if (!registry.Creatures._components.ContainsKey(creatureId))
      {
        return;
      }

      var state = registry.Creatures[creatureId];

      if (state.RankPosition.HasValue && state.FilePosition.HasValue)
      {
        registry.MutateCreatures(self => new CreatureService(
          self._nextCreatureId,
          self._components,
          self.Creatures,
          self.MovingCreatures,
          self.PlacedCreatures.Remove((state.RankPosition.Value, state.FilePosition.Value))));
      }
      else if (state.FilePosition.HasValue)
      {
        registry.MutateCreatures(self => new CreatureService(
          self._nextCreatureId,
          self._components,
          self.Creatures,
          self.MovingCreatures.Remove(creatureId),
          self.PlacedCreatures));
      }

      Mutate(registry, creatureId, s => s.WithIsAlive(false));
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

    public DelegateList GetDelegateList(IGameContext c)
    {
      var state = c.Creatures[this];
      return state.CurrentSkill != null ? state.CurrentSkill.DelegateList : state.Data.DelegateList;
    }
  }
}