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
using Nighthollow.Events;
using UnityEngine;

namespace Nighthollow.Data
{
  public class AttachmentData : ScriptableObject
  {
    [SerializeField] Sprite _image;
    public Sprite Image => _image;
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
    [SerializeField] Creature _prefab;
    public Creature Prefab => _prefab;

    [SerializeField] PlayerName _owner;
    public PlayerName Owner => _owner;

    [SerializeField] string _name;
    public string Name => _name;

    [SerializeField] CreatureHandlers _handlers;
    public CreatureHandlers Handlers => _handlers;

    [SerializeField] CreatureEvents _events;
    public CreatureEvents Events => _events;

    [SerializeField] SkillAnimationNumber _defaultMeleeSkill;
    public SkillAnimationNumber DefaultMeleeSkill => _defaultMeleeSkill;

    [SerializeField] SkillAnimationNumber _defaultCastSkill;
    public SkillAnimationNumber DefaultCastSkill => _defaultCastSkill;

    [SerializeField] Stat _health;
    public Stat Health => _health;

    [SerializeField] Damage _baseAttack;
    public Damage BaseAttack => _baseAttack;

    [SerializeField] Stat _speed;
    public Stat Speed => _speed;

    [SerializeField] Stat _startingEnergy;
    public Stat StartingEnergy => _startingEnergy;

    [SerializeField] Stat _maximumEnergy;
    public Stat MaximumEnergy => _maximumEnergy;

    [SerializeField] Stat _energyRegeneration;
    public Stat EnergyRegeneration => _energyRegeneration;

    [SerializeField] Stat _critChance;
    public Stat CritChance => _critChance;

    [SerializeField] Stat _critMultiplier;
    public Stat CritMultiplier => _critMultiplier;

    [SerializeField] Stat _accuracy;
    public Stat Accuracy => _accuracy;

    [SerializeField] Stat _evasion;
    public Stat Evasion => _evasion;

    [SerializeField] Damage _damageResistance;
    public Damage DamageResistance => _damageResistance;

    [SerializeField] Damage _damageReduction;
    public Damage DamageReduction => _damageReduction;

    [SerializeField] ProjectileData _projectile;
    public ProjectileData Projectile => _projectile;

    [SerializeField] Stat _manaGained;
    public Stat ManaGained => _manaGained;

    [SerializeField] Influence _influence;
    public Influence Influence => _influence;
  }
}