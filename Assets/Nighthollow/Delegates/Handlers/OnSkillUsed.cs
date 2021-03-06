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
using Nighthollow.Services;

#nullable enable

namespace Nighthollow.Delegates.Handlers
{
  public interface IOnSkillUsed : IHandler
  {
    public sealed class Data : CreatureEventData<IOnSkillUsed>
    {
      public Data(CreatureId self, SkillData skill) : base(self)
      {
        Skill = skill;
      }

      public override IEnumerable<Effect> Invoke(IGameContext c, IOnSkillUsed handler) =>
        handler.OnSkillUsed(c, this);

      public SkillData Skill { get; }
    }

    /// <summary>
    /// Called when a skill's initial animation reaches its impact frame. The default implementation handles firing a
    /// projectile for projectile skills. For non-projectile skills, this will be called immediately before the
    /// <see cref="IOnSkillImpact" /> event.
    /// </summary>
    IEnumerable<Effect> OnSkillUsed(IGameContext context, Data data);
  }
}