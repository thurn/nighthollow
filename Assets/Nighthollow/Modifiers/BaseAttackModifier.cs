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
    public override void Activate(Creature self)
    {
      var modifier = Modifier.Create(self.Data.Parameters.Operator, self.Data.Parameters.Value);
      self.Data.BaseAttack.Get(self.Data.Parameters.DamageType).AddModifier(modifier);
    }
  }
}
