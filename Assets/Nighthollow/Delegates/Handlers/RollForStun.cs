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
  public interface IRollForStun : IHandler
  {
    public sealed class Data : QueryData<IRollForStun, bool>
    {
      public Data(CreatureId self, SkillData skill, CreatureId target, int damageAmount)
      {
        Self = self;
        Skill = skill;
        Target = target;
        DamageAmount = damageAmount;
      }

      public override bool Invoke(IGameContext c, IRollForStun handler) =>
        handler.RollForStun(c, this);

      public CreatureId Self { get; }
      public SkillData Skill { get; }
      public CreatureId Target { get; }
      public int DamageAmount { get; }
    }

    /// <summary>
    /// Should roll a random number to determine if a hit from this creature for DamageAmount damage should stun the
    /// target
    /// </summary>
    bool RollForStun(IGameContext context, Data data);
  }
}