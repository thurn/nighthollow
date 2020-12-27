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

#nullable enable

namespace Nighthollow.Data
{
  public enum PlayerName
  {
    Unknown = 0,
    User = 1,
    Enemy = 2
  }

  public enum School
  {
    Unknown = 0,
    Light = 1,
    Sky = 2,
    Flame = 3,
    Ice = 4,
    Earth = 5,
    Shadow = 6
  }

  public enum DamageType
  {
    Unknown = 0,
    Untyped = 1,
    Radiant = 2,
    Lightning = 3,
    Fire = 4,
    Cold = 5,
    Physical = 6,
    Necrotic = 7
  }

  public enum RankValue
  {
    Unknown = 0,
    Rank1 = 1,
    Rank2 = 2,
    Rank3 = 3,
    Rank4 = 4,
    Rank5 = 5,
    Rank6 = 6,
    Rank7 = 7,
    Rank8 = 8
  }

  public enum FileValue
  {
    Unknown = 0,
    File1 = 1,
    File2 = 2,
    File3 = 3,
    File4 = 4,
    File5 = 5
  }

  public enum StatType
  {
    Unknown = 0,
    Int = 1,
    Bool = 2,
    SchoolInts = 3,
    DamageTypeInts = 4,
    Percentage = 5,
    Duration = 6,
    IntRange = 7,
    DamageTypeIntRanges = 8
  }

  public enum SkillAnimationNumber
  {
    Unknown = 0,
    Skill1 = 1,
    Skill2 = 2,
    Skill3 = 3,
    Skill4 = 4,
    Skill5 = 5
  }

  public enum Operator
  {
    Unknown = 0,
    Add = 1,
    Increase = 2,
    Overwrite = 3
  }

  public enum StaticCardList
  {
    Unknown = 0,
    StartingDeck = 1,
    TutorialDraws = 2,
    TutorialEnemy = 3,
    TutorialReward1 = 4,
    TutorialReward2 = 5,
    TutorialReward3 = 6,
    Summons = 7
  }

  public enum AffixPool
  {
    Unknown = 0,
    CreatureImplicits = 1,
    SkillImplicits = 2
  }

  public enum SkillAnimationType
  {
    Unknown = 0,
    MeleeSkill = 1,
    CastSkill = 2,
    ImplicitSkill = 3,
    Unused = 4
  }

  public enum ModifierApplicationType
  {
    Unknown = 0,
    Self = 1,
    Targeted = 2
  }

  public enum SkillType
  {
    Unknown = 0,
    Custom = 1,
    Melee = 2,
    Projectile = 3,
    Area = 4
  }

  public enum Rarity
  {
    Unknown = 0,
    Common = 1
  }
}