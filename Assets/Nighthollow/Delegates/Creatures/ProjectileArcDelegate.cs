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
using Nighthollow.Utils;
using UnityEngine;

namespace Nighthollow.Delegates.Creatures
{
  public sealed class ProjectileArcDelegate : AbstractProjectileOffsetDelegate
  {
    protected override int GetProjectileCount(CreatureContext c) => c.GetInt(Stat.ProjectileArcCount);

    protected override Vector2 GetOrigin(CreatureContext c, int projectileNumber) =>
      c.Self.ProjectileSource.position;

    protected override Vector2 GetDirection(CreatureContext c, int projectileNumber) =>
      Constants.ForwardDirectionForPlayer(c.Self.Owner) +
      projectileNumber * new Vector2(0, c.GetInt(Stat.ProjectileArcRotationOffset) / 1000f);
  }
}