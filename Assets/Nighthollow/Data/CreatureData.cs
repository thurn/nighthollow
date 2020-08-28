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
using Nighthollow.Delegate;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Nighthollow.Data
{
  public sealed class AttachmentData : ScriptableObject
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
    Unknown,
    Melee,
    Projectile,
    Untargeted
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

    [SerializeField] CreatureDelegate _delegate;
    public CreatureDelegate Delegate => _delegate;

    [SerializeField] List<SkillData> _skills;
    public IReadOnlyCollection<SkillData> Skills => _skills.AsReadOnly();

    [SerializeField] List<ProjectileData> _projectiles;
    public IReadOnlyCollection<ProjectileData> Projectiles => _projectiles.AsReadOnly();

    [SerializeField] Stat _health;
    public Stat Health => _health;

    [SerializeField] Damage _baseAttack;
    public Damage BaseAttack => _baseAttack;

    [SerializeField] Stat _damageRange;

    /// <summary>
    /// Represents the size of a creature's damage range as a percentage of the average. For example, if a creature
    /// has average damage of 50 and a range of 30, the range size is 50 * 30% = 15, and thus the damage range is 35-65.
    /// </summary>
    public Stat DamageRange => _damageRange;

    [SerializeField] Stat _speed;
    public Stat Speed => _speed;

    [SerializeField] Stat _startingEnergy;
    public Stat StartingEnergy => _startingEnergy;

    [SerializeField] Stat _maximumEnergy;
    public Stat MaximumEnergy => _maximumEnergy;

    [SerializeField] Stat _energyGain;
    public Stat EnergyGain => _energyGain;

    [SerializeField] Stat _energyGainIntervalMs;
    public Stat EnergyGainIntervalMs => _energyGainIntervalMs;

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

    public CreatureData Clone()
    {
      var result = Instantiate(this);

      if (_delegate)
      {
        result._delegate = _delegate.Clone();
      }

      if (_projectiles != null)
      {
        result._projectiles = _projectiles.Select(p => p.Clone()).ToList();
      }

      return result;
    }

    public void Merge(CreatureData other)
    {
      _health.Merge(other.Health);
      _baseAttack.Merge(other.BaseAttack);
      _damageRange.Merge(other.DamageRange);
      _speed.Merge(other.Speed);
      _startingEnergy.Merge(other.StartingEnergy);
      _maximumEnergy.Merge(other.MaximumEnergy);
      _energyGain.Merge(other.EnergyGain);
      _energyGainIntervalMs.Merge(other.EnergyGainIntervalMs);
      _critChance.Merge(other.CritChance);
      _critMultiplier.Merge(other.CritMultiplier);
      _accuracy.Merge(other.Accuracy);
      _evasion.Merge(other.Evasion);
      _damageResistance.Merge(other.DamageResistance);
      _damageReduction.Merge(other.DamageReduction);
    }

    void Reset()
    {
      _health = new Stat(100);
      _damageRange = new Stat(30);
      _energyGain = new Stat(5);
      _energyGainIntervalMs = new Stat(5000);
      _maximumEnergy = new Stat(100);
      _critChance = new Stat(50);
      _critMultiplier = new Stat(1000);
      _accuracy = new Stat(100);
      _evasion = new Stat(50);
    }
  }
}