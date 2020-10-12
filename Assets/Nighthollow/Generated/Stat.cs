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
    public static readonly IntStatId Health = new IntStatId(1);
    public static readonly TaggedStatsId<DamageType, IntRangeStat> BaseDamage = new TaggedStatsId<DamageType, IntRangeStat>(2);
    public static readonly IntStatId Speed = new IntStatId(3);
    public static readonly PercentageStatId CritChance = new PercentageStatId(4);
    public static readonly PercentageStatId CritMultiplier = new PercentageStatId(5);
    public static readonly IntStatId Accuracy = new IntStatId(6);
    public static readonly IntStatId Evasion = new IntStatId(7);
    public static readonly TaggedStatsId<DamageType, IntStat> DamageResistance = new TaggedStatsId<DamageType, IntStat>(8);
    public static readonly TaggedStatsId<DamageType, IntStat> DamageReduction = new TaggedStatsId<DamageType, IntStat>(9);
    public static readonly PercentageStatId MeleeHealthDrainPercent = new PercentageStatId(10);
    public static readonly IntStatId HealthGainOnMeleeHit = new IntStatId(11);
    public static readonly PercentageStatId SkillSpeedMultiplier = new PercentageStatId(12);
    public static readonly DurationStatId StunDurationOnEnemies = new DurationStatId(13);
    public static readonly DurationStatId StunRecoveryTime = new DurationStatId(14);
    public static readonly PercentageStatId StunChance = new PercentageStatId(15);
    public static readonly PercentageStatId StunAvoidanceChance = new PercentageStatId(16);
    public static readonly IntStatId MeleeReflect = new IntStatId(17);
    public static readonly IntStatId HealthRegenerationPerSecond = new IntStatId(18);
    public static readonly IntStatId ManaCost = new IntStatId(19);
    public static readonly TaggedStatsId<School, IntStat> InfluenceCost = new TaggedStatsId<School, IntStat>(20);
    public static readonly IntStatId ProjectileSpeed = new IntStatId(21);
    public static readonly IntStatId HitboxRadius = new IntStatId(22);
    public static readonly IntStatId StartingLife = new IntStatId(23);
    public static readonly IntStatId StartingMana = new IntStatId(24);
    public static readonly TaggedStatsId<School, IntStat> Influence = new TaggedStatsId<School, IntStat>(25);
    public static readonly IntStatId StartingHandSize = new IntStatId(26);
    public static readonly IntStatId ManaGain = new IntStatId(27);
    public static readonly DurationStatId CardDrawInterval = new DurationStatId(28);
    public static readonly DurationStatId InitialEnemySpawnDelay = new DurationStatId(29);
    public static readonly DurationStatId EnemySpawnDelay = new DurationStatId(30);
    public static readonly IntStatId TotalEnemiesToSpawn = new IntStatId(31);
    public static readonly BoolStatId CanCrit = new BoolStatId(32);
    public static readonly BoolStatId CanStun = new BoolStatId(33);
    public static readonly BoolStatId MeleeAreaDamage = new BoolStatId(34);
    public static readonly BoolStatId IsManaCreature = new BoolStatId(35);
    public static readonly TaggedStatsId<DamageType, IntStat> GainedDamageOnKill = new TaggedStatsId<DamageType, IntStat>(36);
    public static readonly IntStatId MeleeSplashMaxTargets = new IntStatId(37);
    public static readonly PercentageStatId ProjectileDamageMultiplier = new PercentageStatId(38);
    public static readonly PercentageStatId ChainProjectileDamageMultiplier = new PercentageStatId(39);
    public static readonly DurationStatId SkillCooldownRecovery = new DurationStatId(40);
    public static readonly DurationStatId MeleeKnockbackDuration = new DurationStatId(41);
    public static readonly DurationStatId ManaGainInterval = new DurationStatId(42);

    public static StatType GetType(int statId)
    {
      switch (statId)
      {
        case 1: return StatType.Int;
        case 2: return StatType.DamageTypeIntRanges;
        case 3: return StatType.Int;
        case 4: return StatType.Percentage;
        case 5: return StatType.Percentage;
        case 6: return StatType.Int;
        case 7: return StatType.Int;
        case 8: return StatType.DamageTypeInts;
        case 9: return StatType.DamageTypeInts;
        case 10: return StatType.Percentage;
        case 11: return StatType.Int;
        case 12: return StatType.Percentage;
        case 13: return StatType.Duration;
        case 14: return StatType.Duration;
        case 15: return StatType.Percentage;
        case 16: return StatType.Percentage;
        case 17: return StatType.Int;
        case 18: return StatType.Int;
        case 19: return StatType.Int;
        case 20: return StatType.SchoolInts;
        case 21: return StatType.Int;
        case 22: return StatType.Int;
        case 23: return StatType.Int;
        case 24: return StatType.Int;
        case 25: return StatType.SchoolInts;
        case 26: return StatType.Int;
        case 27: return StatType.Int;
        case 28: return StatType.Duration;
        case 29: return StatType.Duration;
        case 30: return StatType.Duration;
        case 31: return StatType.Int;
        case 32: return StatType.Bool;
        case 33: return StatType.Bool;
        case 34: return StatType.Bool;
        case 35: return StatType.Bool;
        case 36: return StatType.DamageTypeInts;
        case 37: return StatType.Int;
        case 38: return StatType.Percentage;
        case 39: return StatType.Percentage;
        case 40: return StatType.Duration;
        case 41: return StatType.Duration;
        case 42: return StatType.Duration;
        default: throw new ArgumentOutOfRangeException();
      }
    }

    public static IStatId GetStat(int statId)
    {
      switch (statId)
      {
        case 1: return Health;
        case 2: return BaseDamage;
        case 3: return Speed;
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
        case 14: return StunRecoveryTime;
        case 15: return StunChance;
        case 16: return StunAvoidanceChance;
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
        case 29: return InitialEnemySpawnDelay;
        case 30: return EnemySpawnDelay;
        case 31: return TotalEnemiesToSpawn;
        case 32: return CanCrit;
        case 33: return CanStun;
        case 34: return MeleeAreaDamage;
        case 35: return IsManaCreature;
        case 36: return GainedDamageOnKill;
        case 37: return MeleeSplashMaxTargets;
        case 38: return ProjectileDamageMultiplier;
        case 39: return ChainProjectileDamageMultiplier;
        case 40: return SkillCooldownRecovery;
        case 41: return MeleeKnockbackDuration;
        case 42: return ManaGainInterval;
        default: throw new ArgumentOutOfRangeException();
      }
    }
  }
}
