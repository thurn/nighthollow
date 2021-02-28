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
using Nighthollow.Components;
using Nighthollow.Data;
using Nighthollow.Services;

#nullable enable

namespace Nighthollow.Delegates.Handlers
{
  public interface IOnApplySkillToTarget : IHandler
  {
    public sealed class Data : EventData<IOnApplySkillToTarget>
    {
      public Data(CreatureState self, SkillData skill, CreatureState target, Projectile? projectile)
      {
        Self = self;
        Skill = skill;
        Target = target;
        Projectile = projectile;
      }

      public override IEnumerable<Effect> Invoke(IGameContext c, int delegateIndex, IOnApplySkillToTarget handler) =>
        handler.OnApplySkillToTarget(c, delegateIndex, this);

      public CreatureState Self { get; }
      public SkillData Skill { get; }
      public CreatureState Target { get; }
      public Projectile? Projectile { get; }
    }

    /// <summary>
    ///   Adds the effects for this skill as a result of a hit.
    /// </summary>
    ///
    /// Normally this is invoked by the default skill delegate's <see cref="IOnSkillImpact" /> implementation for each target
    /// returned from <see cref="IFindTargets" /> for a skill. The default implementation implements the standard
    /// algorithm for applying the skill's BaseDamage, including things like checking for hit, checking for critical
    /// hit, applying damage, applying health drain, and applying stun.
    IEnumerable<Effect> OnApplySkillToTarget(IGameContext context, int delegateIndex, Data data);
  }
}