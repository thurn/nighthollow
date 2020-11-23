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

#nullable enable

using Nighthollow.Delegates.Core;
using Nighthollow.Generated;
using Nighthollow.Stats;
using Nighthollow.Utils;
using UnityEngine;

namespace Nighthollow.Delegates.Implementations
{
  public sealed class ProjectileArcDelegate : AbstractProjectileOffsetDelegate
  {
    public override string Describe(StatEntity entity) =>
      $"Fires an Arc of {entity.GetInt(Stat.ProjectileArcCount)} Projectiles";

    protected override int GetProjectileCount(DelegateContext c) => c.GetInt(Stat.ProjectileArcCount);

    protected override Vector2 GetOrigin(DelegateContext c, int projectileNumber) =>
      c.Self.ProjectileSource.position;

    protected override Vector2 GetDirection(DelegateContext c, int projectileNumber) =>
      Constants.ForwardDirectionForPlayer(c.Self.Owner) +
      projectileNumber * new Vector2(0, c.GetInt(Stat.ProjectileArcRotationOffset) / 1000f);
  }
}
