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

using System.Collections.Immutable;
using Nighthollow.Data;
using Nighthollow.Services;

#nullable enable

namespace Nighthollow.Delegates.Handlers
{
  public interface IApplyDamageReduction : IHandler
  {
    public sealed class Data : QueryData<IApplyDamageReduction, ImmutableDictionary<DamageType, int>>
    {
      public Data(CreatureState self, SkillData skill, CreatureState target, ImmutableDictionary<DamageType, int> damage)
      {
        Self = self;
        Skill = skill;
        Target = target;
        Damage = damage;
      }

      public override ImmutableDictionary<DamageType, int> Invoke(
        GameContext c, int delegateIndex, IApplyDamageReduction handler) =>
        handler.ApplyDamageReduction(c, delegateIndex, this);

      public CreatureState Self { get; }
      public SkillData Skill { get; }
      public CreatureState Target { get; }
      public ImmutableDictionary<DamageType, int> Damage { get; }
    }

    /// <summary>
    /// Should apply damage reduction for this skill, reducing the damage value based on the target's reduction.
    /// Typically called by <see cref="IComputeFinalDamage" />.
    /// </summary>
    ImmutableDictionary<DamageType, int> ApplyDamageReduction(GameContext context, int delegateIndex, Data data);
  }
}