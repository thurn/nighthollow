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
  public interface IProjectileSkillCouldHit : IHandler
  {
    public sealed class Data : QueryData<IProjectileSkillCouldHit, bool>
    {
      public Data(CreatureState self, SkillData skill)
      {
        Self = self;
        Skill = skill;
      }

      public override bool Invoke(IGameContext c, int delegateIndex, IProjectileSkillCouldHit handler) =>
        handler.ProjectileSkillCouldHit(c, delegateIndex, this);

      public CreatureState Self { get; }
      public SkillData Skill { get; }
    }

    /// <summary>
    /// Called to check if a projectile fired by this creature would currently hit a target. Will be true if
    /// any delegate returns a true value.
    /// </summary>
    bool ProjectileSkillCouldHit(IGameContext context, int delegateIndex, Data data);
  }
}