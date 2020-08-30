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
using UnityEngine;

namespace Nighthollow.Delegate
{
  [CreateAssetMenu(menuName = "Delegate/ProjectileChainDelegate")]
  public sealed class ProjectileChainDelegate : AbstractCreatureDelegate
  {
    [SerializeField] int _projectilesFired;
    [SerializeField] int _maxChains;
    [SerializeField] int _chainActivationDelayMs;

    public override void OnProjectileImpact(Creature self, Projectile projectile)
    {
      Parent.OnProjectileImpact(self, projectile);

      if (projectile.Data.ChainCount < _maxChains)
      {
        for (var i = 0; i < _projectilesFired; ++i)
        {
          var direction = Quaternion.AngleAxis(Mathf.Lerp(
            0f, 360f, i / (float)_projectilesFired), Vector3.forward) *
            Vector2.left;
          var data = projectile.Data.Child(_chainActivationDelayMs,
            projectile.Data.ChainCount + 1);
          Parent.OnFireProjectile(self, data, projectile.transform.position, direction);
        }
      }
    }
  }
}
