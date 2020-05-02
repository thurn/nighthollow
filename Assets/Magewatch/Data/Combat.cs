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

using System.Collections.Generic;

namespace Magewatch.Data
{
  public enum Skill
  {
    Skill1 = 1,
    Skill2 = 2,
    Skill3 = 3,
    Skill4 = 4,
    Skill5 = 5
  }

  public enum Reaction
  {
    Hit1 = 1,
    Hit2 = 2,
    Death = 3
  }

  public sealed class RunCombatCommand
  {
    public List<CombatStep> Steps;
  }

  /// <summary>
  /// Combat takes places over a series of finite rounds called "combat steps". All actions in an
  /// earlier combat step are completed before the next combat step.
  /// </summary>
  public sealed class CombatStep
  {
    public List<CombatAction> Actions;
  }

  public sealed class CombatAction
  {
    public MeleeEngage MeleeEngage;

    public Attack Attack;
  }

  public sealed class MeleeEngage
  {
    /// <summary>The creature which is moving to attack</summary>
    public int CreatureId;

    /// <summary>The creature should move to within melee range of this enemy creature</summary>
    public int TargetCreatureId;
  }

  public sealed  class Attack
  {
    /// <summary>The creature performing the attack</summary>
    public int CreatureId;

    /// <summary>The creature being attacked</summary>
    public int TargetCreatureId;

    /// <summary>Which skill animation should be used</summary>
    public Skill SkillNumber;

    /// <summary>How many times this skill is expected to raise the 'AttackStart' event</summary>
    public int HitCount;

    /// <summary>How the target creature should react to being hit</summary>
    public Reaction HitReaction;

    /// <summary>What percentage should be removed from the target's health bar for *each* attack hit</summary>
    public int DamagePercent;
  }
}