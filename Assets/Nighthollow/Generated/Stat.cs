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

using Nighthollow.Stats;
using System;

// Generated Code - Do Not Modify!
namespace Nighthollow.Generated
{
  public enum StatId
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
    StartingLife = 23,
    StartingMana = 24,
    Influence = 25,
    StartingHandSize = 26,
    ManaGain = 27,
    CardDrawInterval = 28,
    EnemySpawnDelay = 29,
    TotalEnemiesToSpawn = 30,
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
    BuffDuration = 69,
  }

  public static class Stat
  {
    public static readonly IntStat Health = new IntStat(StatId.Health);
    public static readonly DamageTypeIntRangesStat BaseDamage = new DamageTypeIntRangesStat(StatId.BaseDamage);
    public static readonly IntStat CreatureSpeed = new IntStat(StatId.CreatureSpeed);
    public static readonly PercentageStat CritChance = new PercentageStat(StatId.CritChance);
    public static readonly PercentageStat CritMultiplier = new PercentageStat(StatId.CritMultiplier);
    public static readonly IntStat Accuracy = new IntStat(StatId.Accuracy);
    public static readonly IntStat Evasion = new IntStat(StatId.Evasion);
    public static readonly DamageTypeIntsStat DamageResistance = new DamageTypeIntsStat(StatId.DamageResistance);
    public static readonly DamageTypeIntsStat DamageReduction = new DamageTypeIntsStat(StatId.DamageReduction);
    public static readonly PercentageStat MeleeHealthDrainPercent = new PercentageStat(StatId.MeleeHealthDrainPercent);
    public static readonly IntStat HealthGainOnMeleeHit = new IntStat(StatId.HealthGainOnMeleeHit);
    public static readonly PercentageStat SkillSpeedMultiplier = new PercentageStat(StatId.SkillSpeedMultiplier);
    public static readonly DurationStat StunDurationOnEnemies = new DurationStat(StatId.StunDurationOnEnemies);
    public static readonly PercentageStat AddedStunChance = new PercentageStat(StatId.AddedStunChance);
    public static readonly IntStat MeleeReflect = new IntStat(StatId.MeleeReflect);
    public static readonly IntStat HealthRegenerationPerSecond = new IntStat(StatId.HealthRegenerationPerSecond);
    public static readonly IntStat ManaCost = new IntStat(StatId.ManaCost);
    public static readonly SchoolIntsStat InfluenceCost = new SchoolIntsStat(StatId.InfluenceCost);
    public static readonly IntStat ProjectileSpeed = new IntStat(StatId.ProjectileSpeed);
    public static readonly IntStat HitboxRadius = new IntStat(StatId.HitboxRadius);
    public static readonly IntStat StartingLife = new IntStat(StatId.StartingLife);
    public static readonly IntStat StartingMana = new IntStat(StatId.StartingMana);
    public static readonly SchoolIntsStat Influence = new SchoolIntsStat(StatId.Influence);
    public static readonly IntStat StartingHandSize = new IntStat(StatId.StartingHandSize);
    public static readonly IntStat ManaGain = new IntStat(StatId.ManaGain);
    public static readonly DurationStat CardDrawInterval = new DurationStat(StatId.CardDrawInterval);
    public static readonly DurationStat EnemySpawnDelay = new DurationStat(StatId.EnemySpawnDelay);
    public static readonly IntStat TotalEnemiesToSpawn = new IntStat(StatId.TotalEnemiesToSpawn);
    public static readonly BoolStat IsManaCreature = new BoolStat(StatId.IsManaCreature);
    public static readonly DamageTypeIntsStat GainedDamageOnKill = new DamageTypeIntsStat(StatId.GainedDamageOnKill);
    public static readonly IntStat MaxMeleeAreaTargets = new IntStat(StatId.MaxMeleeAreaTargets);
    public static readonly PercentageStat ProjectileDamageMultiplier = new PercentageStat(StatId.ProjectileDamageMultiplier);
    public static readonly PercentageStat ChainProjectileDamageMultiplier = new PercentageStat(StatId.ChainProjectileDamageMultiplier);
    public static readonly DurationStat SkillCooldownRecovery = new DurationStat(StatId.SkillCooldownRecovery);
    public static readonly DurationStat KnockbackDuration = new DurationStat(StatId.KnockbackDuration);
    public static readonly DurationStat ManaGainInterval = new DurationStat(StatId.ManaGainInterval);
    public static readonly BoolStat UsesAccuracy = new BoolStat(StatId.UsesAccuracy);
    public static readonly BoolStat CanCrit = new BoolStat(StatId.CanCrit);
    public static readonly BoolStat CanStun = new BoolStat(StatId.CanStun);
    public static readonly BoolStat IgnoresDamageResistance = new BoolStat(StatId.IgnoresDamageResistance);
    public static readonly BoolStat IgnoresDamageReduction = new BoolStat(StatId.IgnoresDamageReduction);
    public static readonly PercentageStat MaximumDamageReduction = new PercentageStat(StatId.MaximumDamageReduction);
    public static readonly PercentageStat MaximumDamageResistance = new PercentageStat(StatId.MaximumDamageResistance);
    public static readonly PercentageStat MaximumStunChance = new PercentageStat(StatId.MaximumStunChance);
    public static readonly DurationStat Cooldown = new DurationStat(StatId.Cooldown);
    public static readonly IntStat AddedManaGain = new IntStat(StatId.AddedManaGain);
    public static readonly PercentageStat MeleeDamageMultiplier = new PercentageStat(StatId.MeleeDamageMultiplier);
    public static readonly IntStat ProjectileSequenceCount = new IntStat(StatId.ProjectileSequenceCount);
    public static readonly DurationStat ProjectileSequenceDelay = new DurationStat(StatId.ProjectileSequenceDelay);
    public static readonly IntStat ProjectileArcCount = new IntStat(StatId.ProjectileArcCount);
    public static readonly IntStat ProjectileArcRotationOffset = new IntStat(StatId.ProjectileArcRotationOffset);
    public static readonly IntStat ProjectileAdjacentsCount = new IntStat(StatId.ProjectileAdjacentsCount);
    public static readonly IntStat ProjectileAdjacentsOffset = new IntStat(StatId.ProjectileAdjacentsOffset);
    public static readonly PercentageStat KnockbackDistanceMultiplier = new PercentageStat(StatId.KnockbackDistanceMultiplier);
    public static readonly IntStat ProjectileChainCount = new IntStat(StatId.ProjectileChainCount);
    public static readonly IntStat MaxProjectileTimesChained = new IntStat(StatId.MaxProjectileTimesChained);
    public static readonly BoolStat Untargeted = new BoolStat(StatId.Untargeted);
    public static readonly DamageTypeIntRangesStat SameTargetAddedDamage = new DamageTypeIntRangesStat(StatId.SameTargetAddedDamage);
    public static readonly DurationStat CurseDuration = new DurationStat(StatId.CurseDuration);
    public static readonly PercentageStat GlobalDamageMultiplier = new PercentageStat(StatId.GlobalDamageMultiplier);
    public static readonly PercentageStat ShockChance = new PercentageStat(StatId.ShockChance);
    public static readonly DurationStat ShockDuration = new DurationStat(StatId.ShockDuration);
    public static readonly BoolStat IsShocked = new BoolStat(StatId.IsShocked);
    public static readonly PercentageStat ReceiveCritsChance = new PercentageStat(StatId.ReceiveCritsChance);
    public static readonly PercentageStat ShockAddedReceiveCritsChance = new PercentageStat(StatId.ShockAddedReceiveCritsChance);
    public static readonly DurationStat BuffDuration = new DurationStat(StatId.BuffDuration);

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

    public static string? GetDescription(StatId statId)
    {
      switch (statId)
      {
        case StatId.Health: return "Health";
        case StatId.BaseDamage: return "Damage";
        case StatId.CreatureSpeed: return "Speed";
        case StatId.CritChance: return "Critical Hit Chance";
        case StatId.CritMultiplier: return "Critical Hit Multiplier";
        case StatId.Accuracy: return "Accuracy";
        case StatId.Evasion: return "Evasion";
        case StatId.DamageResistance: return "Damage Resistance";
        case StatId.DamageReduction: return "Damage Reduction";
        case StatId.MeleeHealthDrainPercent: return "Melee Health Drain";
        case StatId.HealthGainOnMeleeHit: return "Health Gained on Melee Hit";
        case StatId.SkillSpeedMultiplier: return "Skill Speed Multiplier";
        case StatId.StunDurationOnEnemies: return "Stun Duration";
        case StatId.AddedStunChance: return "Added Stun Chance";
        case StatId.MeleeReflect: return "Melee Damage Reflect";
        case StatId.HealthRegenerationPerSecond: return "Health Regeneration Per Second";
        case StatId.ManaCost: return "Mana Cost";
        case StatId.InfluenceCost: return "Influence Cost";
        case StatId.ProjectileSpeed: return "Projectile Speed";
        case StatId.StartingLife: return "Starting Life";
        case StatId.StartingMana: return "Starting Mana";
        case StatId.Influence: return "Influence";
        case StatId.StartingHandSize: return "Starting Hand Size";
        case StatId.ManaGain: return "Mana Generation";
        case StatId.CardDrawInterval: return "Card Draw Interval";
        case StatId.MaxMeleeAreaTargets: return "Melee Attacks Hit up to # Targets";
        case StatId.ProjectileDamageMultiplier: return "Projectile Damage";
        case StatId.ChainProjectileDamageMultiplier: return "Chain Projectile Damage";
        case StatId.SkillCooldownRecovery: return "Skill Cooldown Recovery";
        case StatId.KnockbackDuration: return "Knockback Duration";
        case StatId.ManaGainInterval: return "Mana Generation Interval";
        case StatId.CanCrit: return "Can Critically Hit / Cannot Critically Hit";
        case StatId.CanStun: return "Can Stun / Cannot Stun";
        case StatId.IgnoresDamageResistance: return "Ignores Damage Resistance / Respects Damage Resistance";
        case StatId.IgnoresDamageReduction: return "Ignores Damage Reduction / Respects Damage Reduction";
        case StatId.MaximumDamageReduction: return "Maximum Damage Reduction";
        case StatId.MaximumDamageResistance: return "Maximum Damage Resistance";
        case StatId.MaximumStunChance: return "Maximum Stun Chance";
        case StatId.Cooldown: return "Cooldown";
        case StatId.AddedManaGain: return "Mana Generation";
        case StatId.MeleeDamageMultiplier: return "Melee Damage";
        default: return null;
      }
    }
  }
}
