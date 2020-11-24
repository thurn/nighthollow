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

using System.Linq;
using Nighthollow.Data;
using Nighthollow.Generated;
using Nighthollow.Stats;
using Nighthollow.Utils;

namespace Nighthollow.Interface
{
  public static class CreatureItemTooltip
  {
    public static TooltipBuilder Create(StatTable ownerStats, CreatureItemData data)
    {
      var builder = new TooltipBuilder(data.Name);
      var built = CreatureUtil.Build(ownerStats, data);
      builder.AppendText($"Health: {built.GetInt(Stat.Health)}");

      var baseDamage = built.Stats.Get(Stat.BaseDamage);
      foreach (var damageType in CollectionUtils.AllNonDefaultEnumValues<DamageType>(typeof(DamageType)))
      {
        var range = baseDamage.Get(damageType, IntRangeValue.Zero);
        if (range != IntRangeValue.Zero)
        {
          builder.AppendText($"Base Attack: {range} {damageType} Damage");
        }
      }

      if (built.GetInt(Stat.Accuracy) != ownerStats.Get(Stat.Accuracy))
      {
        builder.AppendText($"Accuracy: {built.GetInt(Stat.Accuracy)}");
      }

      if (built.GetInt(Stat.Evasion) != ownerStats.Get(Stat.Evasion))
      {
        builder.AppendText($"Evasion: {built.GetInt(Stat.Evasion)}");
      }

      if (built.GetStat(Stat.CritChance) != ownerStats.Get(Stat.CritChance))
      {
        builder.AppendText($"Critical Hit Chance: {built.GetStat(Stat.CritChance)}");
      }

      if (built.GetStat(Stat.CritMultiplier) != ownerStats.Get(Stat.CritMultiplier))
      {
        builder.AppendText($"Critical Hit Multiplier: {built.GetStat(Stat.CritMultiplier)}");
      }

      builder.AppendDivider();

      foreach (var affix in data.Affixes.Where(affix => affix.BaseType.Id == data.BaseType.ImplicitAffix?.Id))
      {
        RenderAffix(builder, built, affix);
      }

      foreach (var skill in data.Skills.Where(skill => skill.BaseType.Id != 1))
      {
        builder.AppendText($"Skill: {skill.BaseType.Name}");
      }

      builder.AppendDivider();

      foreach (var affix in data.Affixes.Where(affix => affix.BaseType.Id != data.BaseType.ImplicitAffix?.Id))
      {
        RenderAffix(builder, built, affix);
      }

      return builder;
    }

    static void RenderAffix(TooltipBuilder builder, StatEntity built, AffixData affix)
    {
      builder.StartGroup();

      foreach (var modifier in affix.Modifiers)
      {
        if (modifier.DelegateId.HasValue)
        {
          builder.AppendNullable(DelegateMap.Get(modifier.DelegateId.Value).Describe(built));
        }

        if (modifier.StatModifier != null)
        {
          builder.AppendNullable(modifier.StatModifier.Describe());
        }
      }
    }
  }
}
