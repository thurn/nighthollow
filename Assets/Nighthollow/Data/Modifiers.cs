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
using Nighthollow.Generated;
using Nighthollow.Stats;
using Nighthollow.Utils;

#nullable enable

namespace Nighthollow.Data
{
  public static class Modifiers
  {
    public static IStatValue? ParseArgument(ModifierTypeData modifierData, string? value)
    {
      if (value == null)
      {
        return null;
      }

      var statId = Errors.CheckNotNull(modifierData.StatId);
      switch (modifierData.Operator)
      {
        case Operator.Add:
          switch (Stat.GetType(statId))
          {
            case StatType.Int:
              return new IntValue(int.Parse(value));
            case StatType.SchoolInts:
              return new TaggedStatValue<School, IntValue>(
                Errors.CheckNotNull(modifierData.School),
                new IntValue(int.Parse(value)));
            case StatType.DamageTypeInts:
              return new TaggedStatValue<DamageType, IntValue>(
                Errors.CheckNotNull(modifierData.DamageType),
                new IntValue(int.Parse(value)));
            case StatType.Percentage:
              return new PercentageValue(double.Parse(value));
            case StatType.Duration:
              return new DurationValue(double.Parse(value));
            case StatType.IntRange:
              var split1 = value.Split(',');
              return new IntRangeValue(int.Parse(split1[0]), int.Parse(split1[1]));
            case StatType.DamageTypeIntRanges:
              var split2 = value.Split(',');
              return new TaggedStatValue<DamageType, IntRangeValue>(
                Errors.CheckNotNull(modifierData.DamageType),
                new IntRangeValue(int.Parse(split2[0]), int.Parse(split2[1])));
            case StatType.Bool:
              throw new InvalidOperationException("Cannot add boolean stats");
            case StatType.Unknown:
            default:
              throw new ArgumentOutOfRangeException();
          }
        case Operator.Increase:
          if (modifierData.DamageType.HasValue)
          {
            return new TaggedStatValue<DamageType, PercentageValue>(
              modifierData.DamageType.Value,
              new PercentageValue(double.Parse(value)));
          }
          else if (modifierData.School.HasValue)
          {
            return new TaggedStatValue<School, PercentageValue>(
              modifierData.School.Value,
              new PercentageValue(double.Parse(value)));
          }
          else
          {
            return new PercentageValue(double.Parse(value));
          }
        case Operator.SetFalse:
        case Operator.SetTrue:
          throw new InvalidOperationException("Boolean set operator should not have an associated value.");
        case Operator.Unknown:
        case null:
        default:
          throw new ArgumentOutOfRangeException(nameof(modifierData.Operator),
            "Modifiers with a value must have an operator");
      }
    }

    public static void ApplyModifierUnchecked(Operator @operator, IStat stat, IModifier modifier)
    {
      switch (@operator)
      {
        case Operator.Add:
          AddAddedModifierUnchecked(stat, modifier);
          break;
        case Operator.Increase:
          AddIncreaseModifierUnchecked(stat, modifier);
          break;
        case Operator.SetFalse:
          ((BoolStat) stat).AddSetFalseModifier((IModifier<NoValue>) modifier);
          break;
        case Operator.SetTrue:
          ((BoolStat) stat).AddSetTrueModifier((IModifier<NoValue>) modifier);
          break;
        case Operator.Unknown:
        default:
          throw new ArgumentOutOfRangeException(nameof(@operator), @operator, null);
      }
    }

    public static void AddAddedModifierUnchecked(IStat stat, IModifier modifier)
    {
      switch (stat)
      {
        case IntStat intStat:
          intStat.AddAddedModifier((IModifier<IntValue>) modifier);
          break;
        case DurationStat durationStat:
          durationStat.AddAddedModifier((IModifier<DurationValue>) modifier);
          break;
        case PercentageStat percentageStat:
          percentageStat.AddAddedModifier((IModifier<PercentageValue>) modifier);
          break;
        case IntRangeStat intRangeStat:
          intRangeStat.AddAddedModifier((IModifier<IntRangeValue>) modifier);
          break;
        case ITaggedStats taggedStats:
          taggedStats.AddAddedModifierUnchecked(modifier);
          break;
        default:
          throw new InvalidOperationException($"Add operator is not supported for {stat}");
      }
    }

    public static void AddIncreaseModifierUnchecked(IStat stat, IModifier modifier)
    {
      switch (stat)
      {
        case IntStat intStat:
          intStat.AddIncreaseModifier((IModifier<PercentageValue>) modifier);
          break;
        case DurationStat durationStat:
          durationStat.AddIncreaseModifier((IModifier<PercentageValue>) modifier);
          break;
        case IntRangeStat intRangeStat:
          intRangeStat.AddIncreaseModifier((IModifier<PercentageValue>) modifier);
          break;
        case ITaggedStats taggedStats:
          taggedStats.AddIncreaseModifierUnchecked(modifier);
          break;
        default:
          throw new InvalidOperationException($"Increase operator is not supported for {stat}");
      }
    }
  }
}