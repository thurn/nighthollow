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
  public interface IOnSkillImpact : IHandler
  {
    public sealed class Data : EventData<IOnSkillImpact>
    {
      public Data(CreatureState self, SkillData skill, Projectile? projectile)
      {
        Self = self;
        Skill = skill;
        Projectile = projectile;
      }

      public override IEnumerable<Effect> Invoke(IGameContext c, int delegateIndex, IOnSkillImpact handler) =>
        handler.OnSkillImpact(c, delegateIndex, this);

      public CreatureState Self { get; }
      public SkillData Skill { get; }
      public Projectile? Projectile { get; }
    }

    /// <summary>
    /// Called to apply the effect of a skill on a melee hit or projectile impact.
    /// </summary>
    IEnumerable<Effect> OnSkillImpact(IGameContext context, int delegateIndex, Data data);
  }
}