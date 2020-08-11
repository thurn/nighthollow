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

using Bolt;
using Nighthollow.Components;
using Nighthollow.Events;
using UnityEngine;

namespace Nighthollow.Data
{
  public class AttachmentData : ScriptableObject
  {
    public Sprite Image;
  }

  public enum SkillAnimationNumber
  {
    Unknown,
    Skill1,
    Skill2,
    Skill3,
    Skill4,
    Skill5
  }

  public enum SkillType
  {
    Melee,
    Ranged
  }

  [CreateAssetMenu(menuName = "Data/Creature")]
  public class CreatureData : ScriptableObject
  {
    public Creature Prefab;
    public PlayerName Owner;
    public string Name;
    public CreatureEvents Events;

    public SkillAnimationNumber DefaultMeleeSkill;
    public SkillAnimationNumber DefaultCastSkill;
    public Stat Health;
    public Damage BaseAttack;
    public Stat Speed;
    public Stat StartingEnergy;
    public Stat MaximumEnergy;
    public Stat EnergyRegeneration;
    public Stat CritChance;
    public Stat CritMultiplier;
    public Stat Accuracy;
    public Stat Evasion;
    public Damage DamageResistance;
    public Damage DamageReduction;

    public ProjectileData Projectile;
    public Stat ManaGained;
    public Influence Influence;
  }
}