// Copyright The Magewatch Project

// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at

//    https://www.apache.org/licenses/LICENSE-2.0

// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.

using Magewatch.Utils;
using UnityEngine;

namespace Magewatch.Components
{
  public sealed class Shooter : MonoBehaviour
  {
    [SerializeField] Transform _firingPoint;
    [SerializeField] Collider2D _target;
    [SerializeField] Projectile _prefab;
    [SerializeField] Projectile _prefab2;
    [SerializeField] Projectile _prefab3;
    [SerializeField] int _count;

    void Update()
    {
      if (Input.GetMouseButtonDown(0))
      {
        var projectile = _prefab3;
        switch (_count++ % 3)
        {
          case 0:
            projectile = _prefab;
            break;
          case 1:
            projectile = _prefab2;
            break;
        }
        ComponentUtils.Instantiate(projectile).Initialize(_firingPoint, _target, null);
      }
    }
  }
}