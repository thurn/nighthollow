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

using Nighthollow.Data;
using Nighthollow.Services;

#nullable enable

namespace Nighthollow.Delegates.Handlers
{
  public interface IMeleeSkillCouldHit : IHandler
  {
    public sealed class Data : QueryData<IMeleeSkillCouldHit, bool>
    {
      public Data(CreatureId self, SkillData skill)
      {
        Self = self;
        Skill = skill;
      }

      public override bool Invoke(IGameContext c, IMeleeSkillCouldHit handler) =>
        handler.MeleeSkillCouldHit(c, this);

      public CreatureId Self { get; }
      public SkillData Skill { get; }
    }

    /// <summary>
    /// Should check if the creature could currently hit with a melee skill. Will be true if any delegate returns
    /// a true value.
    /// </summary>
    bool MeleeSkillCouldHit(IGameContext context, Data data);
  }
}