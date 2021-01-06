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
using Nighthollow.Stats;

// Generated Code - Do Not Modify!
#nullable enable

namespace Nighthollow.Generated
{
  public enum OldStatId
  {
    Health = 1,
    BaseDamage = 2,
    CreatureSpeed = 3,
    CritChance = 4,
    CritMultiplier = 5,
    Accuracy = 6,
    Evasion = 7,
    DamageResistance = 8,
    DamageReduction = 9,
    MeleeHealthDrainPercent = 10,
    HealthGainOnMeleeHit = 11,
    SkillSpeedMultiplier = 12,
    StunDurationOnEnemies = 13,
    AddedStunChance = 15,
    MeleeReflect = 17,
    HealthRegenerationPerSecond = 18,
    ManaCost = 19,
    InfluenceCost = 20,
    ProjectileSpeed = 21,
    HitboxRadius = 22,
    StartingMana = 24,
    Influence = 25,
    StartingHandSize = 26,
    ManaGain = 27,
    CardDrawInterval = 28,
    EnemySpawnDelay = 29,
    InitialEnemySpawnDelay = 30,
    EnemiesToSpawn = 31,
    IsManaCreature = 32,
    GainedDamageOnKill = 33,
    MaxMeleeAreaTargets = 34,
    ProjectileDamageMultiplier = 35,
    ChainProjectileDamageMultiplier = 36,
    SkillCooldownRecovery = 37,
    KnockbackDuration = 38,
    ManaGainInterval = 39,
    UsesAccuracy = 40,
    CanCrit = 41,
    CanStun = 42,
    IgnoresDamageResistance = 43,
    IgnoresDamageReduction = 44,
    MaximumDamageReduction = 45,
    MaximumDamageResistance = 46,
    MaximumStunChance = 47,
    Cooldown = 48,
    AddedManaGain = 49,
    MeleeDamageMultiplier = 50,
    ProjectileSequenceCount = 51,
    ProjectileSequenceDelay = 52,
    ProjectileArcCount = 53,
    ProjectileArcRotationOffset = 54,
    ProjectileAdjacentsCount = 55,
    ProjectileAdjacentsOffset = 56,
    KnockbackDistanceMultiplier = 57,
    ProjectileChainCount = 58,
    MaxProjectileTimesChained = 59,
    Untargeted = 60,
    SameTargetAddedDamage = 61,
    CurseDuration = 62,
    GlobalDamageMultiplier = 63,
    ShockChance = 64,
    ShockDuration = 65,
    IsShocked = 66,
    ReceiveCritsChance = 67,
    ShockAddedReceiveCritsChance = 68,
    BuffDuration = 69
  }

  public static class OldStat
  {
    public static readonly IntStat Health = new IntStat(OldStatId.Health);
    public static readonly DamageTypeIntRangesStat BaseDamage = new DamageTypeIntRangesStat(OldStatId.BaseDamage);
    public static readonly IntStat CreatureSpeed = new IntStat(OldStatId.CreatureSpeed);
    public static readonly PercentageStat CritChance = new PercentageStat(OldStatId.CritChance);
    public static readonly PercentageStat CritMultiplier = new PercentageStat(OldStatId.CritMultiplier);
    public static readonly IntStat Accuracy = new IntStat(OldStatId.Accuracy);
    public static readonly IntStat Evasion = new IntStat(OldStatId.Evasion);
    public static readonly DamageTypeIntsStat DamageResistance = new DamageTypeIntsStat(OldStatId.DamageResistance);
    public static readonly DamageTypeIntsStat DamageReduction = new DamageTypeIntsStat(OldStatId.DamageReduction);
    public static readonly PercentageStat MeleeHealthDrainPercent = new PercentageStat(OldStatId.MeleeHealthDrainPercent);
    public static readonly IntStat HealthGainOnMeleeHit = new IntStat(OldStatId.HealthGainOnMeleeHit);
    public static readonly PercentageStat SkillSpeedMultiplier = new PercentageStat(OldStatId.SkillSpeedMultiplier);
    public static readonly DurationStat StunDurationOnEnemies = new DurationStat(OldStatId.StunDurationOnEnemies);
    public static readonly PercentageStat AddedStunChance = new PercentageStat(OldStatId.AddedStunChance);
    public static readonly IntStat MeleeReflect = new IntStat(OldStatId.MeleeReflect);
    public static readonly IntStat HealthRegenerationPerSecond = new IntStat(OldStatId.HealthRegenerationPerSecond);
    public static readonly IntStat ManaCost = new IntStat(OldStatId.ManaCost);
    public static readonly SchoolIntsStat InfluenceCost = new SchoolIntsStat(OldStatId.InfluenceCost);
    public static readonly IntStat ProjectileSpeed = new IntStat(OldStatId.ProjectileSpeed);
    public static readonly IntStat HitboxRadius = new IntStat(OldStatId.HitboxRadius);
    public static readonly IntStat StartingMana = new IntStat(OldStatId.StartingMana);
    public static readonly SchoolIntsStat Influence = new SchoolIntsStat(OldStatId.Influence);
    public static readonly IntStat StartingHandSize = new IntStat(OldStatId.StartingHandSize);
    public static readonly IntStat ManaGain = new IntStat(OldStatId.ManaGain);
    public static readonly DurationStat CardDrawInterval = new DurationStat(OldStatId.CardDrawInterval);
    public static readonly DurationStat EnemySpawnDelay = new DurationStat(OldStatId.EnemySpawnDelay);
    public static readonly DurationStat InitialEnemySpawnDelay = new DurationStat(OldStatId.InitialEnemySpawnDelay);
    public static readonly IntStat EnemiesToSpawn = new IntStat(OldStatId.EnemiesToSpawn);
    public static readonly BoolStat IsManaCreature = new BoolStat(OldStatId.IsManaCreature);
    public static readonly DamageTypeIntsStat GainedDamageOnKill = new DamageTypeIntsStat(OldStatId.GainedDamageOnKill);
    public static readonly IntStat MaxMeleeAreaTargets = new IntStat(OldStatId.MaxMeleeAreaTargets);

    public static readonly PercentageStat ProjectileDamageMultiplier =
      new PercentageStat(OldStatId.ProjectileDamageMultiplier);

    public static readonly PercentageStat ChainProjectileDamageMultiplier =
      new PercentageStat(OldStatId.ChainProjectileDamageMultiplier);

    public static readonly DurationStat SkillCooldownRecovery = new DurationStat(OldStatId.SkillCooldownRecovery);
    public static readonly DurationStat KnockbackDuration = new DurationStat(OldStatId.KnockbackDuration);
    public static readonly DurationStat ManaGainInterval = new DurationStat(OldStatId.ManaGainInterval);
    public static readonly BoolStat UsesAccuracy = new BoolStat(OldStatId.UsesAccuracy);
    public static readonly BoolStat CanCrit = new BoolStat(OldStatId.CanCrit);
    public static readonly BoolStat CanStun = new BoolStat(OldStatId.CanStun);
    public static readonly BoolStat IgnoresDamageResistance = new BoolStat(OldStatId.IgnoresDamageResistance);
    public static readonly BoolStat IgnoresDamageReduction = new BoolStat(OldStatId.IgnoresDamageReduction);
    public static readonly PercentageStat MaximumDamageReduction = new PercentageStat(OldStatId.MaximumDamageReduction);
    public static readonly PercentageStat MaximumDamageResistance = new PercentageStat(OldStatId.MaximumDamageResistance);
    public static readonly PercentageStat MaximumStunChance = new PercentageStat(OldStatId.MaximumStunChance);
    public static readonly DurationStat Cooldown = new DurationStat(OldStatId.Cooldown);
    public static readonly IntStat AddedManaGain = new IntStat(OldStatId.AddedManaGain);
    public static readonly PercentageStat MeleeDamageMultiplier = new PercentageStat(OldStatId.MeleeDamageMultiplier);
    public static readonly IntStat ProjectileSequenceCount = new IntStat(OldStatId.ProjectileSequenceCount);
    public static readonly DurationStat ProjectileSequenceDelay = new DurationStat(OldStatId.ProjectileSequenceDelay);
    public static readonly IntStat ProjectileArcCount = new IntStat(OldStatId.ProjectileArcCount);
    public static readonly IntStat ProjectileArcRotationOffset = new IntStat(OldStatId.ProjectileArcRotationOffset);
    public static readonly IntStat ProjectileAdjacentsCount = new IntStat(OldStatId.ProjectileAdjacentsCount);
    public static readonly IntStat ProjectileAdjacentsOffset = new IntStat(OldStatId.ProjectileAdjacentsOffset);

    public static readonly PercentageStat KnockbackDistanceMultiplier =
      new PercentageStat(OldStatId.KnockbackDistanceMultiplier);

    public static readonly IntStat ProjectileChainCount = new IntStat(OldStatId.ProjectileChainCount);
    public static readonly IntStat MaxProjectileTimesChained = new IntStat(OldStatId.MaxProjectileTimesChained);
    public static readonly BoolStat Untargeted = new BoolStat(OldStatId.Untargeted);

    public static readonly DamageTypeIntRangesStat SameTargetAddedDamage =
      new DamageTypeIntRangesStat(OldStatId.SameTargetAddedDamage);

    public static readonly DurationStat CurseDuration = new DurationStat(OldStatId.CurseDuration);
    public static readonly PercentageStat GlobalDamageMultiplier = new PercentageStat(OldStatId.GlobalDamageMultiplier);
    public static readonly PercentageStat ShockChance = new PercentageStat(OldStatId.ShockChance);
    public static readonly DurationStat ShockDuration = new DurationStat(OldStatId.ShockDuration);
    public static readonly BoolStat IsShocked = new BoolStat(OldStatId.IsShocked);
    public static readonly PercentageStat ReceiveCritsChance = new PercentageStat(OldStatId.ReceiveCritsChance);

    public static readonly PercentageStat ShockAddedReceiveCritsChance =
      new PercentageStat(OldStatId.ShockAddedReceiveCritsChance);

    public static readonly DurationStat BuffDuration = new DurationStat(OldStatId.BuffDuration);

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
        case 24: return StartingMana;
        case 25: return Influence;
        case 26: return StartingHandSize;
        case 27: return ManaGain;
        case 28: return CardDrawInterval;
        case 29: return EnemySpawnDelay;
        case 30: return InitialEnemySpawnDelay;
        case 31: return EnemiesToSpawn;
        case 32: return IsManaCreature;
        case 33: return GainedDamageOnKill;
        case 34: return MaxMeleeAreaTargets;
        case 35: return ProjectileDamageMultiplier;
        case 36: return ChainProjectileDamageMultiplier;
        case 37: return SkillCooldownRecovery;
        case 38: return KnockbackDuration;
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
        case 57: return KnockbackDistanceMultiplier;
        case 58: return ProjectileChainCount;
        case 59: return MaxProjectileTimesChained;
        case 60: return Untargeted;
        case 61: return SameTargetAddedDamage;
        case 62: return CurseDuration;
        case 63: return GlobalDamageMultiplier;
        case 64: return ShockChance;
        case 65: return ShockDuration;
        case 66: return IsShocked;
        case 67: return ReceiveCritsChance;
        case 68: return ShockAddedReceiveCritsChance;
        case 69: return BuffDuration;
        default: throw new ArgumentOutOfRangeException(statId.ToString());
      }
    }

    public static string? GetDescription(OldStatId statId)
    {
      switch (statId)
      {
        case OldStatId.Health: return "Health";
        case OldStatId.BaseDamage: return "Damage";
        case OldStatId.CreatureSpeed: return "Speed";
        case OldStatId.CritChance: return "Critical Hit Chance";
        case OldStatId.CritMultiplier: return "Critical Hit Multiplier";
        case OldStatId.Accuracy: return "Accuracy";
        case OldStatId.Evasion: return "Evasion";
        case OldStatId.DamageResistance: return "Damage Resistance";
        case OldStatId.DamageReduction: return "Damage Reduction";
        case OldStatId.MeleeHealthDrainPercent: return "Melee Health Drain";
        case OldStatId.HealthGainOnMeleeHit: return "Health Gained on Melee Hit";
        case OldStatId.SkillSpeedMultiplier: return "Skill Speed Multiplier";
        case OldStatId.StunDurationOnEnemies: return "Stun Duration";
        case OldStatId.AddedStunChance: return "Added Stun Chance";
        case OldStatId.MeleeReflect: return "Melee Damage Reflect";
        case OldStatId.HealthRegenerationPerSecond: return "Health Regeneration Per Second";
        case OldStatId.ManaCost: return "Mana Cost";
        case OldStatId.InfluenceCost: return "Influence Cost";
        case OldStatId.ProjectileSpeed: return "Projectile Speed";
        case OldStatId.StartingMana: return "Starting Mana";
        case OldStatId.Influence: return "Influence";
        case OldStatId.StartingHandSize: return "Starting Hand Size";
        case OldStatId.ManaGain: return "Mana Generation";
        case OldStatId.CardDrawInterval: return "Card Draw Interval";
        case OldStatId.MaxMeleeAreaTargets: return "Melee Attacks Hit up to # Targets";
        case OldStatId.ProjectileDamageMultiplier: return "Projectile Damage";
        case OldStatId.ChainProjectileDamageMultiplier: return "Chain Projectile Damage";
        case OldStatId.SkillCooldownRecovery: return "Skill Cooldown Recovery";
        case OldStatId.KnockbackDuration: return "Knockback Duration";
        case OldStatId.ManaGainInterval: return "Mana Generation Interval";
        case OldStatId.CanCrit: return "Can Critically Hit / Cannot Critically Hit";
        case OldStatId.CanStun: return "Can Stun / Cannot Stun";
        case OldStatId.IgnoresDamageResistance: return "Ignores Damage Resistance / Respects Damage Resistance";
        case OldStatId.IgnoresDamageReduction: return "Ignores Damage Reduction / Respects Damage Reduction";
        case OldStatId.MaximumDamageReduction: return "Maximum Damage Reduction";
        case OldStatId.MaximumDamageResistance: return "Maximum Damage Resistance";
        case OldStatId.MaximumStunChance: return "Maximum Stun Chance";
        case OldStatId.Cooldown: return "Cooldown";
        case OldStatId.AddedManaGain: return "Mana Generation";
        case OldStatId.MeleeDamageMultiplier: return "Melee Damage";
        default: return null;
      }
    }
  }
}
