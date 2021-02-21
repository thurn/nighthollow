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
using Nighthollow.Data;
using Nighthollow.Delegates.Effects;

#nullable enable

namespace Nighthollow.Delegates.Handlers
{
  public interface IOnApplySkillToTarget : IHandler
  {
    public sealed class Data : EventData<IOnApplySkillToTarget>
    {
      public Data(CreatureState self, SkillData skill)
      {
        Self = self;
        Skill = skill;
      }

      public override IEnumerable<Effect> Invoke(DelegateContext c, IOnApplySkillToTarget handler) =>
        handler.OnApplySkillToTarget(c, this);

      public CreatureState Self { get; }
      public SkillData Skill { get; }
    }

    /// <summary>
    ///   Adds the effects for this skill as a result of a hit.
    /// </summary>
    ///
    /// Normally this is invoked by the default skill delegate's <see cref="IOnSkillImpact" /> implementation for each target
    /// returned from <see cref="FindTargets" /> for a skill. The default implementation implements the standard
    /// algorithm for applying the skill's BaseDamage, including things like checking for hit, checking for critical
    /// hit, applying damage, applying health drain, and applying stun.
    IEnumerable<Effect> OnApplySkillToTarget(DelegateContext c, Data d);
  }
}