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
    public static readonly IntStatId ManaGainPerSecond = new IntStatId(27);
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
  }
}
