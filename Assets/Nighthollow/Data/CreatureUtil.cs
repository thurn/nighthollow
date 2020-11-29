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


using System.Collections.Generic;
using System.Linq;
using Nighthollow.Delegates.Core;
using Nighthollow.Generated;
using Nighthollow.Stats;

#nullable enable

namespace Nighthollow.Data
{
  public static class CreatureUtil
  {
    public static CreatureData Build(StatTable parentStats, CreatureItemData item)
    {
      var stats = item.Stats.Clone(parentStats);

      stats.InsertModifier(Stat.CreatureSpeed.Add(item.BaseType.Speed));

      if (item.BaseType.IsManaCreature) stats.InsertModifier(Stat.IsManaCreature.SetTrue());

      var (delegates, targetedAffixes) = ProcessAffixes(item.Affixes, stats);

      var skills = item.Skills.Select(s => BuildSkill(stats, s)).ToList();

      return new CreatureData(
        item.Name,
        item.BaseType,
        item.School,
        skills,
        stats,
        new CreatureDelegateList(delegates.Select(DelegateMap.Get).ToList()),
        targetedAffixes.ToList(),
        item);
    }

    static SkillData BuildSkill(StatTable parentStats, SkillItemData item)
    {
      var stats = item.Stats.Clone(parentStats);
      if (item.BaseType.ProjectileSpeed.HasValue)
        stats.InsertModifier(Stat.ProjectileSpeed.Add(item.BaseType.ProjectileSpeed.Value));

      if (item.BaseType.UsesAccuracy) stats.InsertModifier(Stat.UsesAccuracy.SetTrue());

      if (item.BaseType.CanCrit) stats.InsertModifier(Stat.CanCrit.SetTrue());

      if (item.BaseType.CanStun) stats.InsertModifier(Stat.CanStun.SetTrue());

      var (delegates, targetedAffixes) = ProcessAffixes(item.Affixes, stats);

      return new SkillData(
        item.BaseType, stats,
        new SkillDelegateList(delegates.Select(DelegateMap.Get).ToList()),
        targetedAffixes.ToList());
    }

    static (IEnumerable<DelegateId>, IEnumerable<AffixData>)
      ProcessAffixes(IReadOnlyList<AffixData> affixes, StatModifierTable stats)
    {
      var delegates = new List<DelegateId>();
      foreach (var modifier in affixes.Where(affix => !affix.BaseType.IsTargeted).SelectMany(affix => affix.Modifiers))
      {
        if (modifier.DelegateId.HasValue) delegates.Add(modifier.DelegateId.Value);

        if (modifier.StatModifier != null) stats.InsertModifier(modifier.StatModifier);
      }

      return (delegates, affixes.Where(affix => affix.BaseType.IsTargeted));
    }
  }
}
