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

using Nighthollow.Components;
using Nighthollow.Model;
using UnityEngine;

namespace Nighthollow.Delegates.CreatureDelegates
{
  public sealed class ApplyDamageEffect : CreatureEffect
  {
    public Creature Target { get; }
    public int Amount { get; }

    public ApplyDamageEffect(Creature target, int amount)
    {
      Target = target;
      Amount = amount;
    }

    public override void Execute(Creature self)
    {
      Target.AddDamage(self, Amount);
    }
  }

  public sealed class HealEffect : CreatureEffect
  {
    public Creature Target { get; }
    public int Amount { get; }

    public HealEffect(Creature target, int amount)
    {
      Target = target;
      Amount = amount;
    }

    public override void Execute(Creature self)
    {
      Target.Heal(Amount);
    }
  }

  public sealed class StunEffect : CreatureEffect
  {
    public Creature Target { get; }
    public int DurationMs { get; }

    public StunEffect(Creature target, int durationMs)
    {
      Target = target;
      DurationMs = durationMs;
    }

    public override void Execute(Creature self)
    {
      Target.Stun(DurationMs);
    }
  }

  public sealed class FireProjectileEffect : CreatureEffect
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

    public override void Execute(Creature self)
    {
      throw new System.NotImplementedException();
    }
  }

  public sealed class ApplyMeleeHitEffect : CreatureEffect
  {
    public SkillData SkillData { get; }

    public ApplyMeleeHitEffect(SkillData skillData)
    {
      SkillData = skillData;
    }

    public override void Execute(Creature self)
    {
      throw new System.NotImplementedException();
    }
  }
}