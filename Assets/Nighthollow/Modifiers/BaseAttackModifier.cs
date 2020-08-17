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
using UnityEngine;

namespace Nighthollow.Modifiers
{
  [CreateAssetMenu(menuName = "Modifiers/BaseAttackModifier")]
  public sealed class BaseAttackModifier : CreatureModifier
  {
    [SerializeField] DamageType _damageType;
    public DamageType DamageType => _damageType;

    [SerializeField] Operator _operator;
    public Operator Operator => _operator;

    [SerializeField] int _value;
    public int Value => _value;

    public override void Activate(Creature self)
    {
      var modifier = Modifier.Create(_operator, _value);
      self.Data.BaseAttack.Get(_damageType).AddModifier(modifier);
    }
  }
}
