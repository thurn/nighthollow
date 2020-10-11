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
using Nighthollow.Stats;
using Nighthollow.Utils;

namespace Nighthollow.Data
{
  public static class Creatures
  {
    public static CreatureData Build(CreatureItemData item)
    {
      var stats = new StatTable();
      var delegates = new List<CreatureDelegateId>();

      stats.Get(Stat.Health).Add(item.Health);
      stats.Get(Stat.ManaCost).Add(item.ManaCost);
      if (item.BaseType.IsManaCreature)
      {
        stats.Get(Stat.IsManaCreature).AddSetTrueModifier(new StaticModifier<NoValue>());
      }

      foreach (var cost in item.InfluenceCost)
      {
        stats.Get(Stat.InfluenceCost).AddAddedModifier(new StaticModifier<TaggedStatValue<School, IntValue>>(cost));
      }

      foreach (var modifier in item.Affixes.SelectMany(affix => affix.Modifiers))
      {
        if (modifier.Data.DelegateId.HasValue)
        {
          delegates.Add(modifier.Data.DelegateId.Value);
        }

        if (modifier.Data.StatId.HasValue)
        {
          ApplyStatModifier(
            stats,
            modifier.Data.StatId.Value,
            modifier.Data,
            modifier.Value);
        }
      }

      return new CreatureData(item.Name, item.BaseType, item.Skills, stats, delegates);
    }

    static void ApplyStatModifier(StatTable table, int statId, ModifierTypeData data, IStatValue? value)
    {
      Modifiers.ApplyModifierUnchecked(
        Errors.CheckNotNull(data.Operator),
        table.UnsafeGet(Stat.GetStat(statId)),
        value?.AsStaticModifier() ?? new StaticModifier<NoValue>());
    }
  }
}