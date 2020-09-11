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
using System.Collections.Generic;
using System.Linq;
using Nighthollow.Utils;

namespace Nighthollow.Data
{
  public static class StatNames
  {
    public static readonly IEnumerable<StatName> AllStats =
      Enum.GetValues(typeof(StatName))
        .OfType<StatName>()
        .Where(statName => statName != StatName.Unknown);

    public static string DisplayName(StatName statName)
    {
      switch (statName)
      {
        case StatName.Health:
          return "Health";
        case StatName.BaseAttackRadiantDamage:
          return "Radiant Damage";
        case StatName.BaseAttackLightningDamage:
          return "Lightning Damage";
        case StatName.BaseAttackFireDamage:
          return "Fire Damage";
        case StatName.BaseAttackColdDamage:
          return "Cold Damage";
        case StatName.BaseAttackPhysicalDamage:
          return "Physical Damage";
        case StatName.BaseAttackNecroticDamage:
          return "Necrotic Damage";
        case StatName.DamageRange:
          return "Damage Range";
        case StatName.Speed:
          return "Speed";
        case StatName.StartingEnergy:
          return "Starting Energy";
        case StatName.MaximumEnergy:
          return "Maximum Energy";
        case StatName.EnergyGain:
          return "Energy Gain";
        case StatName.RegenerationInterval:
          return "Regeneration Interval";
        case StatName.CritChance:
          return "Critical Hit Chance";
        case StatName.CritMultiplier:
          return "Critical Hit Multiplier";
        case StatName.Accuracy:
          return "Accuracy";
        case StatName.Evasion:
          return "Evasion";
        case StatName.RadiantDamageResistance:
          return "Radiant Damage Resistance";
        case StatName.LightningDamageResistance:
          return "Lighting Damage Resistance";
        case StatName.FireDamageResistance:
          return "Fire Damage Resistance";
        case StatName.ColdDamageResistance:
          return "Cold Damage Resistance";
        case StatName.PhysicalDamageResistance:
          return "Physical Damage Resistance";
        case StatName.NecroticDamageResistance:
          return "Necrotic Damage Resistance";
        case StatName.RadiantDamageReduction:
          return "Radiant Damage Reduction";
        case StatName.LightningDamageReduction:
          return "Lightning Damage Reduction";
        case StatName.FireDamageReduction:
          return "Fire Damage Reduction";
        case StatName.ColdDamageReduction:
          return "Cold Damage Reduction";
        case StatName.PhysicalDamageReduction:
          return "Armor";
        case StatName.NecroticDamageReduction:
          return "Necrotic Damage Reduction";
        case StatName.MeleeLifeDrain:
          return "Melee Life Drain";
        case StatName.SkillSpeedMultiplier:
          return "Skill Speed";
        case StatName.StunDuration:
          return "Stun Duration";
        case StatName.AddedStunChance:
          return "Stun Chance";
        case StatName.MeleeReflect:
          return "Melee Damage Reflection";
        case StatName.HealthRegeneration:
          return "Health Regeneration";
        default:
          throw new ArgumentOutOfRangeException(nameof(statName), statName, null);
      }
    }

    public static string FormatStatValue(StatName statName, int value)
    {
      switch (statName)
      {
        case StatName.CritChance:
        case StatName.CritMultiplier:
        case StatName.MeleeLifeDrain:
        case StatName.SkillSpeedMultiplier:
        case StatName.AddedStunChance:
        case StatName.MeleeReflect:
          return Constants.PercentageStringBasisPoints(value);
        case StatName.StunDuration:
          return $"{value / 1000f:0.##}s";
        default:
          return value.ToString();
      }
    }
  }
}