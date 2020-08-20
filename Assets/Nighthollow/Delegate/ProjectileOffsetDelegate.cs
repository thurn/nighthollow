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
using Nighthollow.Utils;
using UnityEngine;

namespace Nighthollow.Delegate
{
  [CreateAssetMenu(menuName = "Delegate/ProjectileOffsetDelegate")]
  public sealed class ProjectileOffsetDelegate : CreatureDelegate
  {
    [SerializeField] Vector2 _positionOffset;
    [SerializeField] Vector2 _rotationOffset;
    [SerializeField] int _offsetsPerSide;

    public override void OnFireProjectile(
      Creature self,
      ProjectileData projectileData,
      Vector2 firingPoint,
      Vector2? inputDirection = null)
    {
      var direction = inputDirection == null ?
        Constants.ForwardDirectionForPlayer(self.Owner) :
        inputDirection.Value;

      for (var i = 1; i <= _offsetsPerSide; ++i)
      {
        base.OnFireProjectile(
          self,
          projectileData,
          firingPoint + (i * _positionOffset),
          direction + (i * _rotationOffset));
        base.OnFireProjectile(
          self,
          projectileData,
          firingPoint - (i * _positionOffset),
          direction - (i * _rotationOffset));
      }

      base.OnFireProjectile(self, projectileData, firingPoint, direction);
    }
  }
}
