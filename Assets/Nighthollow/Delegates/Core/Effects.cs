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

#nullable enable

using System;
using Nighthollow.Components;
using Nighthollow.Data;
using Nighthollow.Generated;
using Nighthollow.Services;
using Nighthollow.Stats;
using Nighthollow.Utils;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Nighthollow.Delegates.Core
{
  public sealed class EnemyRemovedEffect : Effect
  {
    public override void Execute()
    {
      Root.Instance.Enemy.OnEnemyCreatureRemoved();
    }
  }

  public sealed class ApplyModifierToOwner : Effect
  {
    public Creature Self { get; }
    public Operator Operator { get; }
    public IStatId StatId { get; }
    public IModifier Modifier { get; }

    public ApplyModifierToOwner(Creature self, Operator @operator, IStatId statId, IModifier modifier)
    {
      Self = self;
      Operator = @operator;
      Modifier = modifier;
      StatId = statId;
    }

    public override void Execute()
    {
      switch (Self.Owner)
      {
        case PlayerName.User:
          ModifierUtil.ApplyModifierUnchecked(Operator, Root.Instance.User.Data.Stats.UnsafeGet(StatId), Modifier);
          break;
        case PlayerName.Enemy:
          ModifierUtil.ApplyModifierUnchecked(Operator, Root.Instance.Enemy.Data.Stats.UnsafeGet(StatId), Modifier);
          break;
        case PlayerName.Unknown:
        default:
          throw new ArgumentOutOfRangeException();
      }
    }
  }

  public sealed class ApplyDamageEffect : Effect
  {
    public Creature Self { get; }
    public Creature Target { get; }
    public int Amount { get; }

    public ApplyDamageEffect(Creature self, Creature target, int amount)
    {
      Self = self;
      Target = target;
      Amount = amount;
    }

    public override void Execute()
    {
      Target.AddDamage(Self, Amount);
    }
  }

  public sealed class HealEffect : Effect
  {
    public Creature Target { get; }
    public int Amount { get; }

    public HealEffect(Creature target, int amount)
    {
      Target = target;
      Amount = amount;
    }

    public override void Execute()
    {
      Target.Heal(Amount);
    }
  }

  public sealed class StunEffect : Effect
  {
    public Creature Target { get; }
    public float DurationSeconds { get; }

    public StunEffect(Creature target, float durationSeconds)
    {
      Target = target;
      DurationSeconds = durationSeconds;
    }

    public override void Execute()
    {
      Target.Stun(DurationSeconds);
    }
  }

  public sealed class FireProjectileEffect : Effect
  {
    public SkillData SkillData { get; }
    public Vector2 FiringPoint { get; }
    public Vector2 FiringDirectionOffset { get; }

    public FireProjectileEffect(SkillData skillData, Vector2 firingPoint, Vector2 firingDirectionOffset)
    {
      SkillData = skillData;
      FiringPoint = firingPoint;
      FiringDirectionOffset = firingDirectionOffset;
    }

    public override void Execute()
    {
      throw new System.NotImplementedException();
    }
  }

  public sealed class ApplyMeleeHitEffect : Effect
  {
    public SkillData SkillData { get; }

    public ApplyMeleeHitEffect(SkillData skillData)
    {
      SkillData = skillData;
    }

    public override void Execute()
    {
      throw new System.NotImplementedException();
    }
  }

  public sealed class SkillEventEffect : Effect
  {
    public enum Event
    {
      Missed,
      Evade,
      Crit,
      Stun
    }

    public Event EventName { get; }
    public Creature Creature { get; }

    public SkillEventEffect(Event eventName, Creature creature)
    {
      EventName = eventName;
      Creature = creature;
    }

    public override void Execute()
    {
      switch (EventName)
      {
        case Event.Missed:
         Root.Instance.Prefabs.CreateMiss(RandomEffectPoint(Creature));
          break;
        case Event.Evade:
          Root.Instance.Prefabs.CreateEvade(RandomEffectPoint(Creature));
          break;
        case Event.Crit:
          Root.Instance.Prefabs.CreateCrit(RandomEffectPoint(Creature));
          break;
        case Event.Stun:
          Root.Instance.Prefabs.CreateStun(RandomEffectPoint(Creature));
          break;
        default:
          throw Errors.UnknownEnumValue(EventName);
      }
    }

    public static Vector3 RandomEffectPoint(Creature creature)
    {
      var bounds = creature.Collider.bounds;
      return bounds.center + new Vector3(
        (Random.value - 0.5f) * bounds.size.x,
        (Random.value - 0.5f) * bounds.size.y,
        0
      );
    }
  }

  public sealed class DamageTextEffect : Effect
  {
    public Creature Target { get; }
    public int DamageAmount { get; }

    public DamageTextEffect(Creature target, int damageAmount)
    {
      Target = target;
      DamageAmount = damageAmount;
    }

    public override void Execute()
    {
      Root.Instance.DamageTextService.ShowDamageText(Target, DamageAmount);
    }
  }
}