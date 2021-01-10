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

using System.Linq;
using Nighthollow.Data;
using Nighthollow.Delegates.Core;
using Nighthollow.Generated;
using Nighthollow.Model;
using Nighthollow.Stats;
using Nighthollow.Utils;
using AffixData = Nighthollow.Model.AffixData;
using CreatureItemData = Nighthollow.Model.CreatureItemData;

#nullable enable

namespace Nighthollow.Items
{
  public static class TooltipUtil
  {
    public static TooltipBuilder CreateTooltip(StatTable ownerStats, IItemData item) =>
      item.Switch(
        creature => CreateCreatureTooltip(ownerStats, creature),
        CreateResourceTooltip);

    public static TooltipBuilder CreateResourceTooltip(ResourceItemData data)
    {
      var builder = new TooltipBuilder(data.Name);
      builder.AppendText(data.Description);
      return builder;
    }

    public static TooltipBuilder CreateCreatureTooltip(StatTable ownerStats, CreatureItemData data)
    {
      var builder = new TooltipBuilder(data.Name);
      var built = CreatureUtil.Build(ownerStats, data);
      builder.AppendText($"Health: {built.GetInt(OldStat.Health)}");

      var baseDamage = built.Stats.Get(OldStat.BaseDamage);
      foreach (var damageType in CollectionUtils.AllNonDefaultEnumValues<DamageType>(typeof(DamageType)))
      {
        var range = baseDamage.Get(damageType, IntRangeValue.Zero);
        if (range != IntRangeValue.Zero)
        {
          builder.AppendText($"Base Attack: {range} {damageType} Damage");
        }
      }

      if (built.GetInt(OldStat.Accuracy) != ownerStats.Get(OldStat.Accuracy))
      {
        builder.AppendText($"Accuracy: {built.GetInt(OldStat.Accuracy)}");
      }

      if (built.GetInt(OldStat.Evasion) != ownerStats.Get(OldStat.Evasion))
      {
        builder.AppendText($"Evasion: {built.GetInt(OldStat.Evasion)}");
      }

      if (built.GetStat(OldStat.CritChance) != ownerStats.Get(OldStat.CritChance))
      {
        builder.AppendText($"Critical Hit Chance: {built.GetStat(OldStat.CritChance)}");
      }

      if (built.GetStat(OldStat.CritMultiplier) != ownerStats.Get(OldStat.CritMultiplier))
      {
        builder.AppendText($"Critical Hit Multiplier: {built.GetStat(OldStat.CritMultiplier)}");
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
          builder.AppendNullable(DelegateMap.Get(modifier.DelegateId.Value).DescribeOld(built));
        }

        if (modifier.StatModifier != null)
        {
          builder.AppendNullable(modifier.StatModifier.Describe());
        }
      }
    }
  }
}
