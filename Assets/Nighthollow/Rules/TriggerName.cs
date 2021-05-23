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
using Nighthollow.Rules.Triggers;
using Nighthollow.Utils;

#nullable enable

namespace Nighthollow.Rules
{
  public enum TriggerName
  {
    Unknown = 0,
    ApplyDamageReduction = 1,
    ApplyDamageResistance = 2,
    ComputeFinalDamage = 3,
    ComputeHealthDrain = 4,
    FilterTargets = 5,
    FindTargets = 6,
    GetCollider = 7,
    MeleeSkillCouldHit = 8,
    OnApplySkillToTarget = 9,
    OnBattleSceneLoaded = 10,
    OnCreatureActivated = 11,
    OnCreatureDeath = 12,
    OnCreatureOutOfBounds = 13,
    OnEnemyCreatureAtEndzone = 14,
    OnFiredProjectile = 15,
    OnHitTarget = 16,
    OnKilledEnemy = 17,
    OnSkillImpact = 18,
    OnSkillStarted = 19,
    OnSkillUsed = 20,
    OnStartBattle = 21,
    ProjectileSkillCouldHit = 22,
    RollForBaseDamage = 23,
    RollForCrit = 24,
    RollForHit = 25,
    RollForStun = 26,
    SelectSkill = 27,
    ShouldSkipProjectileImpact = 28,
    TransformDamage = 29
  }

  public static class TriggerNameUtil
  {
    public static Type GetHandlerType(this TriggerName name) => name switch
    {
      TriggerName.RollForBaseDamage => typeof(IRollForBaseDamage),
      _ => throw Errors.UnknownEnumValue(name)
    };
  }
}