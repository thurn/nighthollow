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

using Nighthollow.Components;
using Nighthollow.Data;
using Nighthollow.Services;

#nullable enable

namespace Nighthollow.Delegates.Handlers
{
  public interface IShouldSkipProjectileImpact : IHandler
  {
    public sealed class Data : QueryData<IShouldSkipProjectileImpact, bool>
    {
      public Data(CreatureState self, SkillData skill, Projectile? projectile)
      {
        Self = self;
        Skill = skill;
        Projectile = projectile;
      }

      public override bool Invoke(GameContext c, int delegateIndex, IShouldSkipProjectileImpact handler) =>
        handler.ShouldSkipProjectileImpact(c, delegateIndex, this);

      public CreatureState Self { get; }
      public SkillData Skill { get; }
      public Projectile? Projectile { get; }
    }

    /// <summary>
    /// Should check if the current projectile should *not* trigger an impact in its current position. Will be
    /// true if any delegate returns a true value.
    /// </summary>
    bool ShouldSkipProjectileImpact(GameContext context, int delegateIndex, Data data);
  }
}