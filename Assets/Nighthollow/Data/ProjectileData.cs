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

namespace Nighthollow.Data
{
  [CreateAssetMenu(menuName = "Data/Projectile")]
  public class ProjectileData : ScriptableObject
  {
    [SerializeField] Projectile _prefab;
    public Projectile Prefab => _prefab;

    [SerializeField] int _speed;
    public int Speed => _speed;

    [SerializeField] int _hitboxSize;
    public int HitboxSize => _hitboxSize;

    [SerializeField] int _energyCost;
    public int EnergyCost => _energyCost;

    // Delay before the projectile's collider will be activated
    [SerializeField] int _activationDelayMs;
    public int ActivationDelayMs => _activationDelayMs;

    // How many times this projectile has chained
    [SerializeField] int _chainCount;
    public int ChainCount => _chainCount;

    public ProjectileData Clone()
    {
      return Instantiate(this);
    }

    public ProjectileData Child(int activationDelayMs, int chainCount)
    {
      var result = Clone();
      result._activationDelayMs = activationDelayMs;
      result._chainCount = chainCount;
      return result;
    }
  }
}
