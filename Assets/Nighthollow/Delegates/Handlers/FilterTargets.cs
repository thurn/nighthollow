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

#nullable enable

namespace Nighthollow.Delegates.Handlers
{
  public interface IFilterTargets
  {
    public sealed class Data : QueryData<IFilterTargets, IEnumerable<Creature>>
    {
      public Data(CreatureState self, SkillData skill, IEnumerable<Creature> hits)
      {
        Self = self;
        Skill = skill;
        Hits = hits;
      }

      public override IEnumerable<Creature> Invoke(DelegateContext c, IFilterTargets handler) =>
        handler.FilterTargets(c, this);

      public CreatureState Self { get; }
      public SkillData Skill { get; }
      public IEnumerable<Creature> Hits { get; }
    }

    /// <summary>
    /// Given a list of creatures hit by a skill, returns a list of the creatures which should have the skill effect
    /// applied by <see cref="IOnApplySkillToTarget" />.
    /// </summary>
    IEnumerable<Creature> FilterTargets(DelegateContext c, Data d);
  }
}