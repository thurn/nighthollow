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

using System.Collections.Generic;
using System.Linq;
using Nighthollow.Generated;
using Nighthollow.Services;
using Nighthollow.Stats;

namespace Nighthollow.Data
{
  public static class CreatureUtil
  {
    public static CreatureData Build(StatTable parentStats, CreatureItemData item)
    {
      var stats = item.Stats.Clone(parentStats);
      var delegates = new List<CreatureDelegateId>();

      Stat.CreatureSpeed.Add(item.BaseType.Speed).InsertInto(stats);

      if (item.BaseType.IsManaCreature)
      {
        Stat.IsManaCreature.SetTrue().InsertInto(stats);
      }

      foreach (var modifier in item.Affixes.SelectMany(affix => affix.Modifiers))
      {
        if (modifier.CreatureDelegateId.HasValue)
        {
          delegates.Add(modifier.CreatureDelegateId.Value);
        }

        modifier.StatModifier?.InsertInto(stats);
      }

      var skills = item.Skills.Select(s => BuildSkill(stats, s)).ToList();

      return new CreatureData(
        item.Name, item.BaseType, item.School, skills, stats, delegates);
    }

    static SkillData BuildSkill(StatTable parentStats, SkillItemData item)
    {
      var stats = item.Stats.Clone(parentStats);
      if (item.BaseType.ProjectileSpeed.HasValue)
      {
        Stat.ProjectileSpeed.Add(item.BaseType.ProjectileSpeed.Value).InsertInto(stats);
      }

      if (item.BaseType.UsesAccuracy)
      {
        Stat.UsesAccuracy.SetTrue().InsertInto(stats);
      }

      if (item.BaseType.CanCrit)
      {
        Stat.CanCrit.SetTrue().InsertInto(stats);
      }

      if (item.BaseType.CanStun)
      {
        Stat.CanStun.SetTrue().InsertInto(stats);
      }

      var delegates = new List<SkillDelegateId>();

      foreach (var modifier in item.Affixes.SelectMany(affix => affix.Modifiers))
      {
        if (modifier.SkillDelegateId.HasValue)
        {
          delegates.Add(modifier.SkillDelegateId.Value);
        }

        modifier.StatModifier?.InsertInto(stats);
      }

      return new SkillData(item.BaseType, stats, delegates);
    }

    public static SkillItemData DefaultMeleeAttack()
    {
      return new SkillItemData(
        Root.Instance.GameDataService.GetSkillType(1),
        new StatModifierTable(),
        new List<AffixData>());
    }
  }
}