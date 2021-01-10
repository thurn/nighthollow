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
using Nighthollow.Data;
using Nighthollow.Services;
using Nighthollow.Stats;
using Nighthollow.Utils;
using SimpleJSON;

#nullable enable

namespace Nighthollow.Model
{
  public sealed class CreatureItemData : IItemData
  {
    public CreatureItemData(
      string name,
      CreatureTypeData baseType,
      School school,
      StatModifierTable stats,
      IReadOnlyList<SkillItemData> skills,
      IReadOnlyList<AffixData> affixes)
    {
      Name = name;
      BaseType = baseType;
      School = school;
      Stats = stats;
      Skills = skills;
      Affixes = affixes;
    }

    public string Name { get; }
    public CreatureTypeData BaseType { get; }
    public School School { get; }
    public StatModifierTable Stats { get; }
    public IReadOnlyList<SkillItemData> Skills { get; }
    public IReadOnlyList<AffixData> Affixes { get; }

    public string ImageAddress => Errors.CheckNotNull(BaseType.ImageAddress);

    public T Switch<T>(Func<CreatureItemData, T> onCreature, Func<ResourceItemData, T> onResource) => onCreature(this);

    public static CreatureItemData Deserialize(GameDataService gameData, JSONNode node)
    {
      return new CreatureItemData(
        node["name"].Value,
        gameData.GetCreatureType(node["baseType"].AsInt),
        (School) node["school"].AsInt,
        StatModifierTable.Deserialize(node["stats"]),
        node["skills"].FromJsonArray().Select(c => SkillItemData.Deserialize(gameData, c)).ToList(),
        node["affixes"].FromJsonArray().Select(c => AffixData.Deserialize(gameData, c)).ToList());
    }

    public JSONNode Serialize()
    {
      return new JSONObject
      {
        ["name"] = Name,
        ["baseType"] = BaseType.Id,
        ["school"] = (int) School,
        ["stats"] = Stats.Serialize(),
        ["skills"] = Skills.Select(s => s.Serialize()).AsJsonArray(),
        ["affixes"] = Affixes.Select(s => s.Serialize()).AsJsonArray()
      };
    }
  }
}
