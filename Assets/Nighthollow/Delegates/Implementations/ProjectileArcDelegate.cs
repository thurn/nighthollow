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
using Nighthollow.Stats;
using Nighthollow.Utils;
using UnityEngine;

#nullable enable

namespace Nighthollow.Delegates.Implementations
{
  public sealed class ProjectileArcDelegate : AbstractProjectileOffsetDelegate
  {
    public override string Describe(IStatDescriptionProvider provider) =>
      $"Fires an Arc of {provider.Get(Stat.ProjectileArcCount)} Projectiles";

    protected override int GetProjectileCount(IGameContext c, CreatureState creature, SkillData skill) =>
      skill.GetInt(Stat.ProjectileArcCount);

    protected override Vector2 GetOrigin(IGameContext c, CreatureState self, SkillData skill, int projectileNumber) =>
      c.Creatures.GetProjectileSourcePosition(self.CreatureId);

    protected override Vector2 GetDirection(
      IGameContext c, CreatureState self, SkillData skill, int projectileNumber) =>
      Constants.ForwardDirectionForPlayer(self.Owner) +
      projectileNumber * new Vector2(x: 0, skill.GetInt(Stat.ProjectileArcRotationOffset) / 1000f);
  }
}
