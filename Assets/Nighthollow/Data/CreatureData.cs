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
using Nighthollow.Components;
using Nighthollow.Delegate;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Serialization;

namespace Nighthollow.Data
{
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
    Unknown,
    Melee,
    Projectile,
    Untargeted
  }

  public enum StatName
  {
    Unknown,
    Health,

    // Base Attack
    BaseAttackRadiantDamage,
    BaseAttackLightningDamage,
    BaseAttackFireDamage,
    BaseAttackColdDamage,
    BaseAttackPhysicalDamage,
    BaseAttackNecroticDamage,

    DamageRange,
    Speed,
    StartingEnergy,
    MaximumEnergy,
    EnergyGain,
    RegenerationInterval,
    CritChance,
    CritMultiplier,
    Accuracy,
    Evasion,

    // Damage Resistance
    RadiantDamageResistance,
    LightningDamageResistance,
    FireDamageResistance,
    ColdDamageResistance,
    PhysicalDamageResistance,
    NecroticDamageResistance,

    // Damage Reduction
    RadiantDamageReduction,
    LightningDamageReduction,
    FireDamageReduction,
    ColdDamageReduction,
    PhysicalDamageReduction,
    NecroticDamageReduction,

    MeleeLifeDrain,
    SkillSpeedMultiplier,
    StunDuration,
    AddedStunChance,
    MeleeReflect,
    HealthRegeneration
  }

  [CreateAssetMenu(menuName = "Data/Creature")]
  public sealed class CreatureData : ScriptableObject
  {
    [SerializeField] Creature _prefab;
    public Creature Prefab => _prefab;

    [SerializeField] PlayerName _owner;
    public PlayerName Owner => _owner;

    [SerializeField] string _name;
    public string Name => _name;

    [SerializeField] List<AbstractCreatureDelegate> _delegates;
    CreatureDelegateList _delegateList;

    public IReadOnlyCollection<AbstractCreatureDelegate> AllDelegates => _delegates.AsReadOnly();

    public CreatureDelegateList Delegate
    {
      get
      {
        if (!_delegateList)
        {
          _delegateList = CreateInstance<CreatureDelegateList>();
          _delegateList.Initialize(_delegates);
        }

        return _delegateList;
      }
    }

    [SerializeField] List<SkillData> _skills;
    public IReadOnlyCollection<SkillData> Skills => _skills.AsReadOnly();

    [SerializeField] List<ProjectileData> _projectiles;
    public IReadOnlyCollection<ProjectileData> Projectiles => _projectiles.AsReadOnly();

    [SerializeField] string _customAffixDescription;
    public string CustomAffixDescription => _customAffixDescription;

    [SerializeField] Stat _health;
    public Stat Health => _health;

    [SerializeField] Damage _baseAttack;
    public Damage BaseAttack => _baseAttack;

    /// <summary>
    /// Represents the size of a creature's damage range as a percentage of the average. For example, if a creature
    /// has average damage of 50 and a range of 30%, the range size is 50 * 30% = 15, and thus the damage range is
    /// 35-65. Expressed in units of basis points.
    /// </summary>
    [SerializeField] Stat _damageRange;

    public Stat DamageRange => _damageRange;

    [SerializeField] Stat _speed;
    public Stat Speed => _speed;

    [SerializeField] Stat _startingEnergy;
    public Stat StartingEnergy => _startingEnergy;

    [SerializeField] Stat _maximumEnergy;
    public Stat MaximumEnergy => _maximumEnergy;

    [SerializeField] Stat _energyGain;
    public Stat EnergyGain => _energyGain;

    [FormerlySerializedAs("_energyGainIntervalMs")] [SerializeField]
    Stat _regenerationIntervalMs;

    public Stat RegenerationIntervalMs => _regenerationIntervalMs;

    /// <summary>Chance of obtaining a critical hit, in basis points.</summary>
    [SerializeField] Stat _critChance;

    public Stat CritChance => _critChance;

    /// <summary>Damage multiplier for critical hits, in basis points.</summary>
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

    /// <summary>Gain a fraction of life per damage point dealt, in basis points.</summary>
    [FormerlySerializedAs("_meleeLifeDrainBp")] [SerializeField]
    Stat _meleeLifeDrain;

    public Stat MeleeLifeDrain => _meleeLifeDrain;

    /// <summary>Multiplier for skill animation speed, in basis points.</summary>
    [SerializeField] Stat _skillSpeedMultiplier;

    public Stat SkillSpeedMultiplier => _skillSpeedMultiplier;

    /// <summary>Duration for inflicted stuns, in milliseconds.</summary>
    [SerializeField] Stat _stunDuration;

    public Stat StunDuration => _stunDuration;

    /// <summary>
    /// Additional % chance to stun added to the base stun chance on melee hit, in units of
    /// basis points
    /// </summary>
    [SerializeField] Stat _addedStunChance;

    public Stat AddedStunChance => _addedStunChance;

    /// <summary>Health regenerated every regeneration interval.</summary>
    [SerializeField] Stat _healthRegeneration;

    public Stat HealthRegeneration => _healthRegeneration;

    /// <summary>Fraction of melee damage to reflect back on attackers.</summary>
    [SerializeField] Stat _meleeReflect;

    public Stat MeleeReflect => _meleeReflect;

    public CreatureData Clone()
    {
      var result = Instantiate(this);

      if (_delegates != null)
      {
        result._delegates = _delegates.Select(d => d.Clone()).ToList();
      }

      if (_projectiles != null)
      {
        result._projectiles = _projectiles.Select(p => p.Clone()).ToList();
      }

      return result;
    }

    public Stat Get(StatName statName)
    {
      switch (statName)
      {
        case StatName.Health:
          return Health;
        case StatName.BaseAttackRadiantDamage:
          return BaseAttack.Radiant;
        case StatName.BaseAttackLightningDamage:
          return BaseAttack.Lightning;
        case StatName.BaseAttackFireDamage:
          return BaseAttack.Fire;
        case StatName.BaseAttackColdDamage:
          return BaseAttack.Cold;
        case StatName.BaseAttackPhysicalDamage:
          return BaseAttack.Physical;
        case StatName.BaseAttackNecroticDamage:
          return BaseAttack.Necrotic;
        case StatName.DamageRange:
          return DamageRange;
        case StatName.Speed:
          return Speed;
        case StatName.StartingEnergy:
          return StartingEnergy;
        case StatName.MaximumEnergy:
          return MaximumEnergy;
        case StatName.EnergyGain:
          return EnergyGain;
        case StatName.RegenerationInterval:
          return RegenerationIntervalMs;
        case StatName.CritChance:
          return CritChance;
        case StatName.CritMultiplier:
          return CritMultiplier;
        case StatName.Accuracy:
          return Accuracy;
        case StatName.Evasion:
          return Evasion;
        case StatName.RadiantDamageResistance:
          return DamageResistance.Radiant;
        case StatName.LightningDamageResistance:
          return DamageResistance.Lightning;
        case StatName.FireDamageResistance:
          return DamageResistance.Fire;
        case StatName.ColdDamageResistance:
          return DamageResistance.Cold;
        case StatName.PhysicalDamageResistance:
          return DamageResistance.Physical;
        case StatName.NecroticDamageResistance:
          return DamageResistance.Necrotic;
        case StatName.RadiantDamageReduction:
          return DamageReduction.Radiant;
        case StatName.LightningDamageReduction:
          return DamageReduction.Lightning;
        case StatName.FireDamageReduction:
          return DamageReduction.Fire;
        case StatName.ColdDamageReduction:
          return DamageReduction.Cold;
        case StatName.PhysicalDamageReduction:
          return DamageReduction.Physical;
        case StatName.NecroticDamageReduction:
          return DamageReduction.Necrotic;
        case StatName.MeleeLifeDrain:
          return MeleeLifeDrain;
        case StatName.SkillSpeedMultiplier:
          return SkillSpeedMultiplier;
        case StatName.StunDuration:
          return StunDuration;
        case StatName.AddedStunChance:
          return AddedStunChance;
        case StatName.MeleeReflect:
          return MeleeReflect;
        case StatName.HealthRegeneration:
          return HealthRegeneration;
        default:
          throw new ArgumentOutOfRangeException(nameof(statName), statName, null);
      }
    }

    public void SetDefaults()
    {
      _health = new Stat(100);
      _damageRange = new Stat(3000);
      _energyGain = new Stat(5);
      _regenerationIntervalMs = new Stat(5000);
      _maximumEnergy = new Stat(100);
      _critChance = new Stat(500);
      _critMultiplier = new Stat(20_000);
      _accuracy = new Stat(100);
      _evasion = new Stat(50);
      _skillSpeedMultiplier = new Stat(10000);
      _stunDuration = new Stat(1000);
    }

    void Reset()
    {
      SetDefaults();
    }

    public CreatureData Roll()
    {
      var result = Clone();

      foreach (var stat in StatNames.AllStats)
      {
          result.Get(stat).Roll();
      }

      return result;
    }
  }
}