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

using System;
using UnityEngine;

namespace Nighthollow.Data
{
  [Serializable]
  public sealed class SkillData
  {
    [SerializeField] int _energyCost;
    public int EnergyCost => _energyCost;
    
    [SerializeField] SkillAnimationNumber _animation;
    public SkillAnimationNumber Animation => _animation;

    [SerializeField] SkillType _skillType;
    public SkillType SkillType => _skillType;

    [SerializeField] ProjectileData _projectile;
    public ProjectileData Projectile => _projectile;

    public SkillData Clone()
    {
      return new SkillData
      {
        _energyCost = _energyCost,
        _animation = _animation,
        _skillType = _skillType,
        _projectile = _projectile ? _projectile.Clone() : null
      };
    }
  }
}
