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

using DG.Tweening;
using Nighthollow.Components;
using Nighthollow.Data;
using Nighthollow.Utils;
using UnityEngine;

namespace Nighthollow.Delegate
{
  [CreateAssetMenu(menuName = "Delegate/KnockbackOnMeleeHitDelegate")]
  public sealed class KnockbackOnMeleeHitDelegate : CreatureDelegate
  {
    [SerializeField] Vector2 _knockback;
    [SerializeField] float _durationMs;
    [SerializeField] SkillAnimationNumber _forSkill;

    public override void ExecuteAttack(
      Creature self,
      Creature target,
      Damage damage,
      SkillType skillType)
    {
      if (skillType == SkillType.Melee && self.CurrentSkill.Animation == _forSkill)
      {
        target.transform.DOMove(
            (Vector2)target.transform.position +
            (_knockback *
            Constants.ForwardDirectionForPlayer(target.Owner.GetOpponent())),
          _durationMs / 1000f);
      }

      base.ExecuteAttack(self, target, damage, skillType);
    }
  }
}