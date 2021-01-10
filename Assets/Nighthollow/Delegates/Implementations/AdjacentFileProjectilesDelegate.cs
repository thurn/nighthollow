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


using Nighthollow.Delegates.Core;
using Nighthollow.Generated;
using Nighthollow.Stats2;
using Nighthollow.Utils;
using UnityEngine;
using StatEntity = Nighthollow.Stats.StatEntity;

#nullable enable

namespace Nighthollow.Delegates.Implementations
{
  public sealed class AdjacentFileProjectilesDelegate : AbstractProjectileOffsetDelegate
  {
    public override string DescribeOld(StatEntity entity) =>
      $"Fires {entity.GetInt(OldStat.ProjectileAdjacentsCount)} Projectiles in Adjacent Files";

    public override string Describe(IStatDescriptionProvider provider) =>
      $"Fires {provider.Get(Stat.ProjectileAdjacentsCount)} Projectiles in Adjacent Files";

    protected override int GetProjectileCount(DelegateContext c) => c.GetInt(OldStat.ProjectileAdjacentsCount);

    protected override Vector2 GetOrigin(DelegateContext c, int projectileNumber) =>
      (Vector2) c.Self.ProjectileSource.position +
      projectileNumber * new Vector2(x: 0, c.GetInt(OldStat.ProjectileAdjacentsOffset) / 1000f);

    protected override Vector2 GetDirection(DelegateContext c, int projectileNumber) =>
      Constants.ForwardDirectionForPlayer(c.Self.Owner);
  }
}
