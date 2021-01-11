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
using Nighthollow.Data;

// Generated Code - Do Not Modify!
namespace Nighthollow.Stats
{
  public enum StatId
  {
    Unknown = 0,
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
    AddedStunChance = 14,
    MeleeReflect = 15,
    HealthRegenerationPerSecond = 16,
    ManaCost = 17,
    InfluenceCost = 18,
    ProjectileSpeed = 19,
    HitboxRadius = 20,
    StartingMana = 21,
    Influence = 22,
    StartingHandSize = 23,
    ManaGain = 24,
    CardDrawInterval = 25,
    EnemySpawnDelay = 26,
    IsManaCreature = 27,
    GainedDamageOnKill = 28,
    MaxMeleeAreaTargets = 29,
    ProjectileDamageMultiplier = 30,
    ChainProjectileDamageMultiplier = 31,
    SkillCooldownRecovery = 32,
    KnockbackDuration = 33,
    ManaGainInterval = 34,
    UsesAccuracy = 35,
    CanCrit = 36,
    CanStun = 37,
    IgnoresDamageResistance = 38,
    IgnoresDamageReduction = 39,
    InitialEnemySpawnDelay = 40,
    EnemiesToSpawn = 41,
    MaximumDamageReduction = 42,
    MaximumDamageResistance = 43,
    MaximumStunChance = 44,
    Cooldown = 45,
    AddedManaGain = 47,
    MeleeDamageMultiplier = 48,
    ProjectileSequenceCount = 49,
    ProjectileSequenceDelay = 50,
    ProjectileArcCount = 51,
    ProjectileArcRotationOffset = 52,
    ProjectileAdjacentsCount = 53,
    ProjectileAdjacentsOffset = 54,
    KnockbackDistanceMultiplier = 55,
    ProjectileChainCount = 56,
    MaxProjectileTimesChained = 57,
    SameTargetAddedDamage = 59,
    CurseDuration = 60,
    GlobalDamageMultiplier = 61,
    ShockChance = 62,
    ShockDuration = 63,
    ReceiveCritsChance = 65,
    ShockAddedReceiveCritsChance = 66,
    BuffDuration = 67,
  }

  public static class Stat
  {
    public static readonly IntStat Health =
        new IntStat(StatId.Health);
    public static readonly TaggedValuesStat<DamageType, IntRangeValue> BaseDamage =
        new TaggedValuesStat<DamageType, IntRangeValue>(StatId.BaseDamage, new IntRangeStat(StatId.BaseDamage));
    public static readonly IntStat CreatureSpeed =
        new IntStat(StatId.CreatureSpeed);
    public static readonly PercentageStat CritChance =
        new PercentageStat(StatId.CritChance);
    public static readonly PercentageStat CritMultiplier =
        new PercentageStat(StatId.CritMultiplier);
    public static readonly IntStat Accuracy =
        new IntStat(StatId.Accuracy);
    public static readonly IntStat Evasion =
        new IntStat(StatId.Evasion);
    public static readonly TaggedValuesStat<DamageType, int> DamageResistance =
        new TaggedValuesStat<DamageType, int>(StatId.DamageResistance, new IntStat(StatId.DamageResistance));
    public static readonly TaggedValuesStat<DamageType, int> DamageReduction =
        new TaggedValuesStat<DamageType, int>(StatId.DamageReduction, new IntStat(StatId.DamageReduction));
    public static readonly PercentageStat MeleeHealthDrainPercent =
        new PercentageStat(StatId.MeleeHealthDrainPercent);
    public static readonly IntStat HealthGainOnMeleeHit =
        new IntStat(StatId.HealthGainOnMeleeHit);
    public static readonly PercentageStat SkillSpeedMultiplier =
        new PercentageStat(StatId.SkillSpeedMultiplier);
    public static readonly DurationStat StunDurationOnEnemies =
        new DurationStat(StatId.StunDurationOnEnemies);
    public static readonly PercentageStat AddedStunChance =
        new PercentageStat(StatId.AddedStunChance);
    public static readonly IntStat MeleeReflect =
        new IntStat(StatId.MeleeReflect);
    public static readonly IntStat HealthRegenerationPerSecond =
        new IntStat(StatId.HealthRegenerationPerSecond);
    public static readonly IntStat ManaCost =
        new IntStat(StatId.ManaCost);
    public static readonly TaggedValuesStat<School, int> InfluenceCost =
        new TaggedValuesStat<School, int>(StatId.InfluenceCost, new IntStat(StatId.InfluenceCost));
    public static readonly IntStat ProjectileSpeed =
        new IntStat(StatId.ProjectileSpeed);
    public static readonly IntStat HitboxRadius =
        new IntStat(StatId.HitboxRadius);
    public static readonly IntStat StartingMana =
        new IntStat(StatId.StartingMana);
    public static readonly TaggedValuesStat<School, int> Influence =
        new TaggedValuesStat<School, int>(StatId.Influence, new IntStat(StatId.Influence));
    public static readonly IntStat StartingHandSize =
        new IntStat(StatId.StartingHandSize);
    public static readonly IntStat ManaGain =
        new IntStat(StatId.ManaGain);
    public static readonly DurationStat CardDrawInterval =
        new DurationStat(StatId.CardDrawInterval);
    public static readonly DurationStat EnemySpawnDelay =
        new DurationStat(StatId.EnemySpawnDelay);
    public static readonly BoolStat IsManaCreature =
        new BoolStat(StatId.IsManaCreature);
    public static readonly TaggedValuesStat<DamageType, int> GainedDamageOnKill =
        new TaggedValuesStat<DamageType, int>(StatId.GainedDamageOnKill, new IntStat(StatId.GainedDamageOnKill));
    public static readonly IntStat MaxMeleeAreaTargets =
        new IntStat(StatId.MaxMeleeAreaTargets);
    public static readonly PercentageStat ProjectileDamageMultiplier =
        new PercentageStat(StatId.ProjectileDamageMultiplier);
    public static readonly PercentageStat ChainProjectileDamageMultiplier =
        new PercentageStat(StatId.ChainProjectileDamageMultiplier);
    public static readonly DurationStat SkillCooldownRecovery =
        new DurationStat(StatId.SkillCooldownRecovery);
    public static readonly DurationStat KnockbackDuration =
        new DurationStat(StatId.KnockbackDuration);
    public static readonly DurationStat ManaGainInterval =
        new DurationStat(StatId.ManaGainInterval);
    public static readonly BoolStat UsesAccuracy =
        new BoolStat(StatId.UsesAccuracy);
    public static readonly BoolStat CanCrit =
        new BoolStat(StatId.CanCrit);
    public static readonly BoolStat CanStun =
        new BoolStat(StatId.CanStun);
    public static readonly BoolStat IgnoresDamageResistance =
        new BoolStat(StatId.IgnoresDamageResistance);
    public static readonly BoolStat IgnoresDamageReduction =
        new BoolStat(StatId.IgnoresDamageReduction);
    public static readonly DurationStat InitialEnemySpawnDelay =
        new DurationStat(StatId.InitialEnemySpawnDelay);
    public static readonly IntStat EnemiesToSpawn =
        new IntStat(StatId.EnemiesToSpawn);
    public static readonly PercentageStat MaximumDamageReduction =
        new PercentageStat(StatId.MaximumDamageReduction);
    public static readonly PercentageStat MaximumDamageResistance =
        new PercentageStat(StatId.MaximumDamageResistance);
    public static readonly PercentageStat MaximumStunChance =
        new PercentageStat(StatId.MaximumStunChance);
    public static readonly DurationStat Cooldown =
        new DurationStat(StatId.Cooldown);
    public static readonly IntStat AddedManaGain =
        new IntStat(StatId.AddedManaGain);
    public static readonly PercentageStat MeleeDamageMultiplier =
        new PercentageStat(StatId.MeleeDamageMultiplier);
    public static readonly IntStat ProjectileSequenceCount =
        new IntStat(StatId.ProjectileSequenceCount);
    public static readonly DurationStat ProjectileSequenceDelay =
        new DurationStat(StatId.ProjectileSequenceDelay);
    public static readonly IntStat ProjectileArcCount =
        new IntStat(StatId.ProjectileArcCount);
    public static readonly IntStat ProjectileArcRotationOffset =
        new IntStat(StatId.ProjectileArcRotationOffset);
    public static readonly IntStat ProjectileAdjacentsCount =
        new IntStat(StatId.ProjectileAdjacentsCount);
    public static readonly IntStat ProjectileAdjacentsOffset =
        new IntStat(StatId.ProjectileAdjacentsOffset);
    public static readonly PercentageStat KnockbackDistanceMultiplier =
        new PercentageStat(StatId.KnockbackDistanceMultiplier);
    public static readonly IntStat ProjectileChainCount =
        new IntStat(StatId.ProjectileChainCount);
    public static readonly IntStat MaxProjectileTimesChained =
        new IntStat(StatId.MaxProjectileTimesChained);
    public static readonly TaggedValuesStat<DamageType, IntRangeValue> SameTargetAddedDamage =
        new TaggedValuesStat<DamageType, IntRangeValue>(StatId.SameTargetAddedDamage, new IntRangeStat(StatId.SameTargetAddedDamage));
    public static readonly DurationStat CurseDuration =
        new DurationStat(StatId.CurseDuration);
    public static readonly PercentageStat GlobalDamageMultiplier =
        new PercentageStat(StatId.GlobalDamageMultiplier);
    public static readonly PercentageStat ShockChance =
        new PercentageStat(StatId.ShockChance);
    public static readonly DurationStat ShockDuration =
        new DurationStat(StatId.ShockDuration);
    public static readonly PercentageStat ReceiveCritsChance =
        new PercentageStat(StatId.ReceiveCritsChance);
    public static readonly PercentageStat ShockAddedReceiveCritsChance =
        new PercentageStat(StatId.ShockAddedReceiveCritsChance);
    public static readonly DurationStat BuffDuration =
        new DurationStat(StatId.BuffDuration);

    public static IStat GetStat(StatId statId)
    {
      switch (statId)
      {
        case StatId.Health: return Health;
        case StatId.BaseDamage: return BaseDamage;
        case StatId.CreatureSpeed: return CreatureSpeed;
        case StatId.CritChance: return CritChance;
        case StatId.CritMultiplier: return CritMultiplier;
        case StatId.Accuracy: return Accuracy;
        case StatId.Evasion: return Evasion;
        case StatId.DamageResistance: return DamageResistance;
        case StatId.DamageReduction: return DamageReduction;
        case StatId.MeleeHealthDrainPercent: return MeleeHealthDrainPercent;
        case StatId.HealthGainOnMeleeHit: return HealthGainOnMeleeHit;
        case StatId.SkillSpeedMultiplier: return SkillSpeedMultiplier;
        case StatId.StunDurationOnEnemies: return StunDurationOnEnemies;
        case StatId.AddedStunChance: return AddedStunChance;
        case StatId.MeleeReflect: return MeleeReflect;
        case StatId.HealthRegenerationPerSecond: return HealthRegenerationPerSecond;
        case StatId.ManaCost: return ManaCost;
        case StatId.InfluenceCost: return InfluenceCost;
        case StatId.ProjectileSpeed: return ProjectileSpeed;
        case StatId.HitboxRadius: return HitboxRadius;
        case StatId.StartingMana: return StartingMana;
        case StatId.Influence: return Influence;
        case StatId.StartingHandSize: return StartingHandSize;
        case StatId.ManaGain: return ManaGain;
        case StatId.CardDrawInterval: return CardDrawInterval;
        case StatId.EnemySpawnDelay: return EnemySpawnDelay;
        case StatId.IsManaCreature: return IsManaCreature;
        case StatId.GainedDamageOnKill: return GainedDamageOnKill;
        case StatId.MaxMeleeAreaTargets: return MaxMeleeAreaTargets;
        case StatId.ProjectileDamageMultiplier: return ProjectileDamageMultiplier;
        case StatId.ChainProjectileDamageMultiplier: return ChainProjectileDamageMultiplier;
        case StatId.SkillCooldownRecovery: return SkillCooldownRecovery;
        case StatId.KnockbackDuration: return KnockbackDuration;
        case StatId.ManaGainInterval: return ManaGainInterval;
        case StatId.UsesAccuracy: return UsesAccuracy;
        case StatId.CanCrit: return CanCrit;
        case StatId.CanStun: return CanStun;
        case StatId.IgnoresDamageResistance: return IgnoresDamageResistance;
        case StatId.IgnoresDamageReduction: return IgnoresDamageReduction;
        case StatId.InitialEnemySpawnDelay: return InitialEnemySpawnDelay;
        case StatId.EnemiesToSpawn: return EnemiesToSpawn;
        case StatId.MaximumDamageReduction: return MaximumDamageReduction;
        case StatId.MaximumDamageResistance: return MaximumDamageResistance;
        case StatId.MaximumStunChance: return MaximumStunChance;
        case StatId.Cooldown: return Cooldown;
        case StatId.AddedManaGain: return AddedManaGain;
        case StatId.MeleeDamageMultiplier: return MeleeDamageMultiplier;
        case StatId.ProjectileSequenceCount: return ProjectileSequenceCount;
        case StatId.ProjectileSequenceDelay: return ProjectileSequenceDelay;
        case StatId.ProjectileArcCount: return ProjectileArcCount;
        case StatId.ProjectileArcRotationOffset: return ProjectileArcRotationOffset;
        case StatId.ProjectileAdjacentsCount: return ProjectileAdjacentsCount;
        case StatId.ProjectileAdjacentsOffset: return ProjectileAdjacentsOffset;
        case StatId.KnockbackDistanceMultiplier: return KnockbackDistanceMultiplier;
        case StatId.ProjectileChainCount: return ProjectileChainCount;
        case StatId.MaxProjectileTimesChained: return MaxProjectileTimesChained;
        case StatId.SameTargetAddedDamage: return SameTargetAddedDamage;
        case StatId.CurseDuration: return CurseDuration;
        case StatId.GlobalDamageMultiplier: return GlobalDamageMultiplier;
        case StatId.ShockChance: return ShockChance;
        case StatId.ShockDuration: return ShockDuration;
        case StatId.ReceiveCritsChance: return ReceiveCritsChance;
        case StatId.ShockAddedReceiveCritsChance: return ShockAddedReceiveCritsChance;
        case StatId.BuffDuration: return BuffDuration;
        default: throw new ArgumentOutOfRangeException(statId.ToString());
      }
    }
  }
}
