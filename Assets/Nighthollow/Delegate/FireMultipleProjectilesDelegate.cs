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
using System.Collections.Generic;
using UnityEngine;

namespace Nighthollow.Delegate
{
  [CreateAssetMenu(menuName = "Delegate/FireMultipleProjectilesDelegate")]
  public sealed class FireMultipleProjectilesDelegate : CreatureDelegate
  {
    [SerializeField] int _projectileCount;
    [SerializeField] int _projectileDelayMs;

    public override void OnFireProjectile(Creature self, ProjectileData projectileData)
    {
      self.StartCoroutine(FireAsync(self, projectileData));
    }

    IEnumerator<YieldInstruction> FireAsync(Creature self, ProjectileData projectileData)
    {
      for (var i = 0; i < _projectileCount; ++i)
      {
        base.OnFireProjectile(self, projectileData);
        yield return new WaitForSeconds(_projectileDelayMs / 1000f);
      }
    }
  }
}
