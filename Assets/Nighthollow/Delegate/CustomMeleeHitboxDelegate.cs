﻿// Copyright © 2020-present Derek Thurn

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
using System.Linq;
using UnityEngine;

namespace Nighthollow.Delegate
{
  [CreateAssetMenu(menuName = "Delegate/CustomMeleeHitboxDelegate")]
  public sealed class CustomMeleeHitboxDelegate : CreatureDelegate
  {
    [Header("Config")]
    [SerializeField] Vector2 _meleeHitSize;

    [Header("State")]
    [SerializeField] CustomTriggerCollider _collider;

    public override void OnActivate(Creature self)
    {
      _collider = CustomTriggerCollider.Add(self,
        new Vector2(_meleeHitSize.x / 2000f, 0),
        new Vector2(_meleeHitSize.x / 1000f, _meleeHitSize.y / 1000f));
    }

    public override bool ShouldUseMeleeSkill(Creature self)
    {
      return self.HasMeleeSkill() && GetCollidingCreatures(self.Owner, _collider.Collider).Any();
    }

    public override void OnMeleeHit(Creature self)
    {
      foreach (var creature in GetCollidingCreatures(self.Owner, _collider.Collider))
      {
        ExecuteMeleeAttack(self, creature, self.Data.BaseAttack);
      }
    }
  }
}
