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
using System.Linq;
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

    public sealed class Controller : ICreatureCallbacks
    {
      readonly GameServiceRegistry _registry;
      readonly GameServiceRegistry.ICreatureServiceMutator _mutator;

      public Controller(GameServiceRegistry registry, GameServiceRegistry.ICreatureServiceMutator mutator)
      {
        _registry = registry;
        _mutator = mutator;

        _registry.CoroutineRunner.StartCoroutine(UpdateCreaturesCoroutine());
      }

      public void OnUpdate()
      {
        _mutator.MutateCreatures(self => new CreatureService(
          self._nextCreatureId,
          self._components,
          self.Creatures.ToImmutableDictionary(
            pair => pair.Key,
            pair => pair.Value.WithData(pair.Value.Data.OnTick(_registry))),
          self.MovingCreatures,
          self.PlacedCreatures));

        foreach (var component in _registry.Creatures._components.Values)
        {
          component.OnUpdate();
        }
      }

      IEnumerator<YieldInstruction> UpdateCreaturesCoroutine()
      {
        while (true)
        {
          yield return new WaitForSeconds(seconds: 1);
          foreach (var pair in _registry.Creatures.Creatures.Where(pair => pair.Value.IsAlive))
          {
            Heal(pair.Key, pair.Value.GetInt(Stat.HealthRegenerationPerSecond));
          }
        }

        // ReSharper disable once IteratorNeverReturns
      }

      public CreatureId CreateUserCreature(
        CreatureData creatureData,
        Card? addPositionSelector = null)
      {
        var result = _registry.AssetService.InstantiatePrefab<Creature>(creatureData.BaseType.PrefabAddress);
        var creatureId = new CreatureId(_registry.Creatures._nextCreatureId);
        _mutator.MutateCreatures(self => new CreatureService(
          self._nextCreatureId + 1,
          self._components.SetItem(creatureId, result),
          self.Creatures.SetItem(creatureId, new CreatureState(creatureId, creatureData, creatureData.BaseType.Owner)),
          self.MovingCreatures,
          self.PlacedCreatures));

        result.Initialize(_registry, this, creatureId, creatureData.BaseType.Name, creatureData.BaseType.Owner);
        if (addPositionSelector)
        {
          result.gameObject.AddComponent<CreaturePositionSelector>()
            .Initialize(_registry, creatureId, addPositionSelector);
        }

        return creatureId;
      }

      public void CreateMovingCreature(
        CreatureData creatureData,
        FileValue file,
        float startingX)
      {
        var result = _registry.AssetService.InstantiatePrefab<Creature>(creatureData.BaseType.PrefabAddress);
        var creatureId = new CreatureId(_registry.Creatures._nextCreatureId);
        var creatureState = new CreatureState(
          creatureId,
          creatureData,
          creatureData.BaseType.Owner,
          filePosition: file);

        _mutator.MutateCreatures(self => new CreatureService(
          self._nextCreatureId + 1,
          self._components.SetItem(creatureId, result),
          self.Creatures.SetItem(creatureId, creatureState),
          self.MovingCreatures.Add(creatureId),
          self.PlacedCreatures));

        result.Initialize(_registry, this, creatureId, creatureData.BaseType.Name, creatureData.BaseType.Owner);
        result.ActivateCreature(startingX: startingX);
      }

      public void AddUserCreatureAtPosition(CreatureId creatureId, RankValue rank, FileValue file)
      {
        _mutator.MutateCreatures(self => new CreatureService(
          self._nextCreatureId,
          self._components,
          self.Creatures.SetItem(creatureId, self.Creatures[creatureId].WithRankPosition(rank).WithFilePosition(file)),
          self.MovingCreatures,
          self.PlacedCreatures.SetItem((rank, file), creatureId)));
        _registry.Creatures._components[creatureId].ActivateCreature();
      }

      public void Mutate(CreatureId creatureId, Func<CreatureState, CreatureState> mutation)
      {
        _mutator.MutateCreatures(self => new CreatureService(
          self._nextCreatureId,
          self._components,
          self.Creatures.SetItem(creatureId, mutation(self[creatureId])),
          self.MovingCreatures,
          self.PlacedCreatures));
      }

      public void AddDamage(CreatureId appliedById, CreatureId targetId, int damage)
      {
        Errors.CheckArgument(damage >= 0, "Damage must be non-negative");
        var health = _registry.Creatures[targetId].GetInt(Stat.Health);
        Mutate(
          targetId,
          s => s.WithDamageTaken(Mathf.Clamp(value: 0, s.DamageTaken + damage, health)));
        if (_registry.Creatures[targetId].DamageTaken >= health)
        {
          _registry.Invoke(new IOnKilledEnemy.Data(appliedById));
          _registry.Invoke(new IOnCreatureDeath.Data(targetId));
          _registry.Creatures._components[targetId].Kill();
          OnDeath(targetId);
        }
      }

      public void Heal(CreatureId creatureId, int healing)
      {
        var state = _registry.Creatures[creatureId];
        Errors.CheckArgument(healing >= 0, "Healing must be non-negative");
        var health = state.GetInt(Stat.Health);
        Mutate(creatureId, s => s.WithDamageTaken(Mathf.Clamp(value: 0, state.DamageTaken - healing, health)));
      }

      public void ApplyKnockback(CreatureId target, float distance, float durationSeconds)
      {
        var t = _registry.Creatures._components[target].transform;
        t.DOMove(
          (Vector2) t.position +
          distance *
          Constants.ForwardDirectionForPlayer(_registry.Creatures[target].Owner.GetOpponent()),
          durationSeconds);
      }

      public void ApplyStun(CreatureId target, float durationSeconds)
      {
        _registry.CoroutineRunner.StartCoroutine(StunAsync(target, durationSeconds));
      }

      IEnumerator<YieldInstruction> StunAsync(CreatureId creatureId, float durationSeconds)
      {
        var creature = _registry.Creatures._components[creatureId];
        creature.StartStunAnimation();
        yield return new WaitForSeconds(durationSeconds);
        creature.ToDefaultAnimation(_registry.Creatures[creatureId]);
      }

      public void SetAnimationPaused(CreatureId target, bool animationPaused)
      {
        _registry.Creatures._components[target].SetAnimationPaused(animationPaused);
      }

      public void DespawnCreature(CreatureId target)
      {
        if (!_registry.Creatures._components.ContainsKey(target))
        {
          return;
        }

        _registry.Creatures._components[target].Despawn();

        _mutator.MutateCreatures(self => new CreatureService(
          self._nextCreatureId,
          self._components.Remove(target),
          self.Creatures.Remove(target),
          self.MovingCreatures,
          self.PlacedCreatures));
      }

      public void OnAttackStart(CreatureId creatureId)
      {
        var state = _registry.Creatures[creatureId];
        if (state.CurrentSkill == null || !state.IsAlive || state.IsStunned)
        {
          return;
        }

        _registry.Invoke(new IOnSkillUsed.Data(creatureId, state.CurrentSkill));
        state = _registry.Creatures[creatureId];

        if (state.CurrentSkill != null && state.CurrentSkill.IsMelee())
        {
          _registry.Invoke(new IOnSkillImpact.Data(creatureId, state.CurrentSkill, projectile: null));
        }
      }

      public void OnActionAnimationCompleted(CreatureId creatureId)
      {
        var state = _registry.Creatures[creatureId];
        if (!state.IsAlive || state.IsStunned)
        {
          // Ignore exit states from skills that ended early due to stun
          return;
        }

        _registry.Creatures._components[creatureId].ToDefaultAnimation(state);
      }

      public void OnDeathAnimationCompleted(CreatureId creatureId)
      {
        DespawnCreature(creatureId);
      }

      void OnDeath(CreatureId creatureId)
      {
        if (!_registry.Creatures._components.ContainsKey(creatureId))
        {
          return;
        }

        var state = _registry.Creatures[creatureId];

        if (state.RankPosition.HasValue && state.FilePosition.HasValue)
        {
          _mutator.MutateCreatures(self => new CreatureService(
            self._nextCreatureId,
            self._components,
            self.Creatures.SetItem(creatureId, self[creatureId].WithIsAlive(false)),
            self.MovingCreatures,
            self.PlacedCreatures.Remove((state.RankPosition.Value, state.FilePosition.Value))));
        }
        else
        {
          _mutator.MutateCreatures(self => new CreatureService(
            self._nextCreatureId,
            self._components,
            self.Creatures.SetItem(creatureId, self[creatureId].WithIsAlive(false)),
            self.MovingCreatures.Remove(creatureId),
            self.PlacedCreatures));
        }
      }
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