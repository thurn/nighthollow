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
  public interface IFindTargets : IHandler
  {
    public sealed class Data : QueryData<IFindTargets, IEnumerable<CreatureId>?>
    {
      public Data(CreatureId self, SkillData skill, Projectile? projectile)
      {
        Self = self;
        Skill = skill;
        Projectile = projectile;
      }

      public override IEnumerable<CreatureId>? Invoke(IGameContext c, IFindTargets handler) =>
        handler.FindTargets(c, this);

      public CreatureId Self { get; }
      public SkillData Skill { get; }
      public Projectile? Projectile { get; }
    }

    /// <summary>
    ///   Returns the creatures to target for this skill. Normally this is invoked by the default skill delegate's
    ///   <see cref="IOnSkillImpact" /> implementation. Default implementation uses @<see cref="IGetCollider" /> to find all
    ///   creatures in the impact area and then adds targets returned by <see cref="IFilterTargets" />.
    /// </summary>
    IEnumerable<CreatureId>? FindTargets(IGameContext context, Data data);
  }
}