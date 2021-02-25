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
  public interface IRollForCrit : IHandler
  {
    public sealed class Data : QueryData<IRollForCrit, bool>
    {
      public Data(CreatureState self, SkillData skill, CreatureState target)
      {
        Self = self;
        Skill = skill;
        Target = target;
      }

      public override bool Invoke(GameContext c, int delegateIndex, IRollForCrit handler) =>
        handler.RollForCrit(c, delegateIndex, this);

      public CreatureState Self { get; }
      public SkillData Skill { get; }
      public CreatureState Target { get; }
    }

    /// <summary>
    /// Should roll a random number to determine if the current skill will be treated as a critical hit.
    /// </summary>
    bool RollForCrit(GameContext context, int delegateIndex, Data data);
  }
}