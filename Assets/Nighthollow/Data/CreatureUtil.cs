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

namespace Nighthollow.Data
{
  public static class CreatureUtil
  {
    public static CreatureData Build(CreatureItemData item)
    {
      var stats = item.Stats;
      var delegates = new List<CreatureDelegateId>();

      Stat.Speed.Add(item.BaseType.Speed).InsertInto(stats);

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

      var skills = item.Skills.Select(BuildSkill).ToList();
      if (item.BaseType.SkillAnimations.Any(animation => animation.Type == SkillAnimationType.MeleeSkill))
      {
        skills.Add(DefaultMeleeAttack());
      }

      return new CreatureData(
        item.Name, item.BaseType, item.School, skills, stats, delegates);
    }

    static SkillData BuildSkill(SkillItemData item)
    {
      var stats = item.Stats;
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

    public static SkillData DefaultMeleeAttack()
    {
      var stats = Root.Instance.GameDataService.GetDefaultStats(StatScope.Skills);
      Stat.UsesAccuracy.SetTrue().InsertInto(stats);
      Stat.CanCrit.SetTrue().InsertInto(stats);
      Stat.CanStun.SetTrue().InsertInto(stats);

      return new SkillData(
        Root.Instance.GameDataService.GetSkillType(1),
        stats,
        new List<SkillDelegateId>());
    }
  }
}