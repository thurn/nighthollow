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
using Nighthollow.Services;
using Nighthollow.Stats;
using Nighthollow.Utils;
using SimpleJSON;

#nullable enable

namespace Nighthollow.Model
{
  public sealed class SkillItemData
  {
    public SkillItemData(SkillTypeData baseType, StatModifierTable stats, IReadOnlyList<AffixData> affixes)
    {
      BaseType = baseType;
      Stats = stats;
      Affixes = affixes;
    }

    public SkillTypeData BaseType { get; }
    public StatModifierTable Stats { get; }
    public IReadOnlyList<AffixData> Affixes { get; }

    public static SkillItemData Deserialize(GameDataService gameData, JSONNode node)
    {
      return new SkillItemData(
        gameData.GetSkillType(node["baseType"].AsInt),
        StatModifierTable.Deserialize(node["stats"]),
        node["affixes"].FromJsonArray().Select(c => AffixData.Deserialize(gameData, c)).ToList());
    }

    public JSONNode Serialize()
    {
      return new JSONObject
      {
        ["baseType"] = BaseType.Id,
        ["stats"] = Stats.Serialize(),
        ["affixes"] = Affixes.Select(a => a.Serialize()).AsJsonArray()
      };
    }
  }
}
