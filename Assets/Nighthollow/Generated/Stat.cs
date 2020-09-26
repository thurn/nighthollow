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

namespace Nighthollow.Generated
{
  public static class Stat
  {
    public static readonly IntStatId Health = new IntStatId(1);
    public static readonly TaggedIntsStatId<DamageType> BaseDamage = new TaggedIntsStatId<DamageType>(2);
    public static readonly IntStatId DamageRange = new IntStatId(3);
    public static readonly IntStatId Speed = new IntStatId(4);
    public static readonly IntStatId RegenerationInterval = new IntStatId(5);
    public static readonly IntStatId CritChance = new IntStatId(6);
    public static readonly IntStatId CritMultiplier = new IntStatId(7);
    public static readonly IntStatId Accuracy = new IntStatId(8);
    public static readonly IntStatId Evasion = new IntStatId(9);
    public static readonly TaggedIntsStatId<DamageType> DamageResistance = new TaggedIntsStatId<DamageType>(10);
    public static readonly TaggedIntsStatId<DamageType> DamageReduction = new TaggedIntsStatId<DamageType>(11);
    public static readonly IntStatId MeleeLifeDrainPercent = new IntStatId(12);
    public static readonly IntStatId SkillSpeedMultiplier = new IntStatId(13);
    public static readonly IntStatId StunDuration = new IntStatId(14);
    public static readonly IntStatId AddedStunChance = new IntStatId(15);
    public static readonly IntStatId MeleeReflect = new IntStatId(16);
    public static readonly IntStatId HealthRegeneration = new IntStatId(17);
    public static readonly IntStatId ManaCost = new IntStatId(18);
    public static readonly TaggedIntsStatId<School> InfluenceCost = new TaggedIntsStatId<School>(19);
    public static readonly IntStatId ProjectileSpeed = new IntStatId(20);
    public static readonly IntStatId HitboxRadius = new IntStatId(21);
    public static readonly IntStatId StartingLife = new IntStatId(22);
    public static readonly IntStatId StartingMana = new IntStatId(23);
    public static readonly TaggedIntsStatId<School> Influence = new TaggedIntsStatId<School>(24);
    public static readonly IntStatId StartingHandSize = new IntStatId(25);
    public static readonly IntStatId ManaGain = new IntStatId(26);
    public static readonly IntStatId ManaGainInterval = new IntStatId(27);
    public static readonly IntStatId CardDrawInterval = new IntStatId(28);
    public static readonly IntStatId InitialEnemySpawnDelay = new IntStatId(29);
    public static readonly IntStatId EnemySpawnDelay = new IntStatId(30);
    public static readonly IntStatId TotalEnemiesToSpawn = new IntStatId(31);
    public static readonly BoolStatId CanCrit = new BoolStatId(32);
    public static readonly BoolStatId CanStun = new BoolStatId(33);
    public static readonly BoolStatId MeleeAreaDamage = new BoolStatId(34);
  }
}