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

using Nighthollow.Stats;
using System;

// Generated Code - Do Not Modify!
namespace Nighthollow.Generated
{
  public static class Stat
  {
    public static readonly IntStat Health = new IntStat(1);
    public static readonly DamageTypeIntRangesStat BaseDamage = new DamageTypeIntRangesStat(2);
    public static readonly IntStat CreatureSpeed = new IntStat(3);
    public static readonly PercentageStat CritChance = new PercentageStat(4);
    public static readonly PercentageStat CritMultiplier = new PercentageStat(5);
    public static readonly IntStat Accuracy = new IntStat(6);
    public static readonly IntStat Evasion = new IntStat(7);
    public static readonly DamageTypeIntsStat DamageResistance = new DamageTypeIntsStat(8);
    public static readonly DamageTypeIntsStat DamageReduction = new DamageTypeIntsStat(9);
    public static readonly PercentageStat MeleeHealthDrainPercent = new PercentageStat(10);
    public static readonly IntStat HealthGainOnMeleeHit = new IntStat(11);
    public static readonly PercentageStat SkillSpeedMultiplier = new PercentageStat(12);
    public static readonly DurationStat StunDurationOnEnemies = new DurationStat(13);
    public static readonly PercentageStat AddedStunChance = new PercentageStat(15);
    public static readonly IntStat MeleeReflect = new IntStat(17);
    public static readonly IntStat HealthRegenerationPerSecond = new IntStat(18);
    public static readonly IntStat ManaCost = new IntStat(19);
    public static readonly SchoolIntsStat InfluenceCost = new SchoolIntsStat(20);
    public static readonly IntStat ProjectileSpeed = new IntStat(21);
    public static readonly IntStat HitboxRadius = new IntStat(22);
    public static readonly IntStat StartingLife = new IntStat(23);
    public static readonly IntStat StartingMana = new IntStat(24);
    public static readonly SchoolIntsStat Influence = new SchoolIntsStat(25);
    public static readonly IntStat StartingHandSize = new IntStat(26);
    public static readonly IntStat ManaGain = new IntStat(27);
    public static readonly DurationStat CardDrawInterval = new DurationStat(28);
    public static readonly DurationStat EnemySpawnDelay = new DurationStat(29);
    public static readonly IntStat TotalEnemiesToSpawn = new IntStat(30);
    public static readonly BoolStat IsManaCreature = new BoolStat(32);
    public static readonly DamageTypeIntsStat GainedDamageOnKill = new DamageTypeIntsStat(33);
    public static readonly IntStat MaxMeleeAreaTargets = new IntStat(34);
    public static readonly PercentageStat ProjectileDamageMultiplier = new PercentageStat(35);
    public static readonly PercentageStat ChainProjectileDamageMultiplier = new PercentageStat(36);
    public static readonly DurationStat SkillCooldownRecovery = new DurationStat(37);
    public static readonly DurationStat MeleeKnockbackDuration = new DurationStat(38);
    public static readonly DurationStat ManaGainInterval = new DurationStat(39);
    public static readonly BoolStat UsesAccuracy = new BoolStat(40);
    public static readonly BoolStat CanCrit = new BoolStat(41);
    public static readonly BoolStat CanStun = new BoolStat(42);
    public static readonly BoolStat IgnoresDamageResistance = new BoolStat(43);
    public static readonly BoolStat IgnoresDamageReduction = new BoolStat(44);
    public static readonly PercentageStat MaximumDamageReduction = new PercentageStat(45);
    public static readonly PercentageStat MaximumDamageResistance = new PercentageStat(46);
    public static readonly PercentageStat MaximumStunChance = new PercentageStat(47);
    public static readonly DurationStat Cooldown = new DurationStat(48);
    public static readonly IntStat AddedManaGain = new IntStat(49);
    public static readonly PercentageStat MeleeDamageMultiplier = new PercentageStat(50);
    public static readonly IntStat ProjectileSequenceCount = new IntStat(51);
    public static readonly DurationStat ProjectileSequenceDelay = new DurationStat(52);
    public static readonly IntStat ProjectileArcCount = new IntStat(53);
    public static readonly IntStat ProjectileArcRotationOffset = new IntStat(54);
    public static readonly IntStat ProjectileAdjacentsCount = new IntStat(55);
    public static readonly IntStat ProjectileAdjacentsOffset = new IntStat(56);

    public static IStat GetStat(int statId)
    {
      switch (statId)
      {
        case 1: return Health;
        case 2: return BaseDamage;
        case 3: return CreatureSpeed;
        case 4: return CritChance;
        case 5: return CritMultiplier;
        case 6: return Accuracy;
        case 7: return Evasion;
        case 8: return DamageResistance;
        case 9: return DamageReduction;
        case 10: return MeleeHealthDrainPercent;
        case 11: return HealthGainOnMeleeHit;
        case 12: return SkillSpeedMultiplier;
        case 13: return StunDurationOnEnemies;
        case 15: return AddedStunChance;
        case 17: return MeleeReflect;
        case 18: return HealthRegenerationPerSecond;
        case 19: return ManaCost;
        case 20: return InfluenceCost;
        case 21: return ProjectileSpeed;
        case 22: return HitboxRadius;
        case 23: return StartingLife;
        case 24: return StartingMana;
        case 25: return Influence;
        case 26: return StartingHandSize;
        case 27: return ManaGain;
        case 28: return CardDrawInterval;
        case 29: return EnemySpawnDelay;
        case 30: return TotalEnemiesToSpawn;
        case 32: return IsManaCreature;
        case 33: return GainedDamageOnKill;
        case 34: return MaxMeleeAreaTargets;
        case 35: return ProjectileDamageMultiplier;
        case 36: return ChainProjectileDamageMultiplier;
        case 37: return SkillCooldownRecovery;
        case 38: return MeleeKnockbackDuration;
        case 39: return ManaGainInterval;
        case 40: return UsesAccuracy;
        case 41: return CanCrit;
        case 42: return CanStun;
        case 43: return IgnoresDamageResistance;
        case 44: return IgnoresDamageReduction;
        case 45: return MaximumDamageReduction;
        case 46: return MaximumDamageResistance;
        case 47: return MaximumStunChance;
        case 48: return Cooldown;
        case 49: return AddedManaGain;
        case 50: return MeleeDamageMultiplier;
        case 51: return ProjectileSequenceCount;
        case 52: return ProjectileSequenceDelay;
        case 53: return ProjectileArcCount;
        case 54: return ProjectileArcRotationOffset;
        case 55: return ProjectileAdjacentsCount;
        case 56: return ProjectileAdjacentsOffset;
        default: throw new ArgumentOutOfRangeException(statId.ToString());
      }
    }
  }
}
