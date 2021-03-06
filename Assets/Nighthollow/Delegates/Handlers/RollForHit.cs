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
  public interface IRollForHit : IHandler
  {
    public sealed class Data : QueryData<IRollForHit, bool>
    {
      public Data(CreatureId self, SkillData skill, CreatureId target)
      {
        Self = self;
        Skill = skill;
        Target = target;
      }

      public override bool Invoke(IGameContext c, IRollForHit handler) =>
        handler.RollForHit(c, this);

      public CreatureId Self { get; }
      public SkillData Skill { get; }
      public CreatureId Target { get; }
    }

    /// <summary>
    /// Should roll a random number to determine if the current skill will be treated as a hit.
    /// </summary>
    bool RollForHit(IGameContext context, Data data);
  }
}