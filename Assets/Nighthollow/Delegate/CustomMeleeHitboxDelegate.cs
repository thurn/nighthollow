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
using System.Linq;
using UnityEngine;

namespace Nighthollow.Delegate
{
  [CreateAssetMenu(menuName = "Delegate/CustomMeleeHitboxDelegate")]
  public sealed class CustomMeleeHitboxDelegate : CreatureDelegate
  { 
    [Header("State")]
    [SerializeField] CustomTriggerCollider _collider;

    public override void OnActivate(Creature self)
    {
      var size = self.Data.Parameters.MeleeHitSize;
      _collider = CustomTriggerCollider.Add(self,
        new Vector2(size.x / 2000f, 0),
        new Vector2(size.x / 1000f, size.y / 1000f));
    }

    public override bool ShouldUseMeleeSkill(Creature self)
    {
      return self.HasMelee() && GetCollidingCreatures(self.Owner, _collider.Collider).Any();
    }

    public override void OnMeleeHit(Creature self)
    {
      foreach (var creature in GetCollidingCreatures(self.Owner, _collider.Collider))
      {
        creature.ApplyDamage(self.Data.BaseAttack.Total());
      }
    }
  }
}