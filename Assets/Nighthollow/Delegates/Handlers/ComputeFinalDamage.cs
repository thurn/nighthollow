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

#nullable enable

namespace Nighthollow.Delegates.Handlers
{
  public interface IComputeFinalDamage : IHandler
  {
    public sealed class Data : QueryData<IComputeFinalDamage, int>
    {
      public Data(
        CreatureState self,
        SkillData skill,
        CreatureState target,
        ImmutableDictionary<DamageType, int> damage,
        bool isCriticalHit)
      {
        Self = self;
        Skill = skill;
        Target = target;
        Damage = damage;
        IsCriticalHit = isCriticalHit;
      }

      public override int Invoke(DelegateContext c, int delegateIndex, IComputeFinalDamage handler) =>
        handler.ComputeFinalDamage(c, delegateIndex, this);

      public CreatureState Self { get; }
      public SkillData Skill { get; }
      public CreatureState Target { get; }
      public ImmutableDictionary<DamageType, int> Damage { get; }
      public bool IsCriticalHit { get; }
    }

    /// <summary>
    /// Should compute the final damage value for this skill based on the value adjusted by damage resistance and
    /// reduction. Should apply the critical hit multiplier if IsCriticalHit is true.
    /// </summary>
    int ComputeFinalDamage(DelegateContext context, int delegateIndex, Data data);
  }
}