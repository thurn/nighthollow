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

namespace Nighthollow.Data
{
  public static class Creatures
  {
    public static CreatureData Build(CreatureItemData item)
    {
      var stats = new StatTable.Builder();
      var delegates = new List<CreatureDelegateId>();

      stats.AddStat(Stat.Health, new IntStat((int) item.Health));
      stats.AddStat(Stat.ManaCost, new IntStat((int) item.ManaCost));
      stats.AddStat(Stat.InfluenceCost, item.InfluenceCost);
      var statTable = new StatTable(stats);

      foreach (var modifier in item.Affixes.SelectMany(affix => affix.Modifiers))
      {
        if (modifier.Data.DelegateId.HasValue)
        {
          delegates.Add(modifier.Data.DelegateId.Value);
        }

        if (modifier.Data.StatId.HasValue)
        {
          ApplyStatModifier(statTable, modifier.Data.StatId.Value, modifier.Data, modifier.Value);
        }
      }

      return new CreatureData(
        item.Name,
        item.BaseType,
        item.Skills,
        statTable,
        delegates);
    }

    static void ApplyStatModifier(StatTable table, int statId, ModifierData data, IStatValue value)
    {

    }
  }
}