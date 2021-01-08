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

using System.Collections.Immutable;
using System.Linq;
using MessagePack;
using Nighthollow.Delegates;
using Nighthollow.Delegates.Core;
using Nighthollow.Generated;
using Nighthollow.Stats2;

#nullable enable

namespace Nighthollow.Data
{
  [MessagePackObject]
  public sealed partial class CreatureItemData
  {
    public CreatureItemData(
      int creatureTypeId,
      string name,
      School school,
      ImmutableList<SkillItemData>? skills = null,
      ImmutableList<AffixData>? affixes = null)
    {
      CreatureTypeId = creatureTypeId;
      Name = name;
      School = school;
      Skills = skills ?? ImmutableList<SkillItemData>.Empty;
      Affixes = affixes ?? ImmutableList<AffixData>.Empty;
    }

    [Key(0)] public int CreatureTypeId { get; }
    [Key(1)] public string Name { get; }
    [Key(2)] public School School { get; }
    [Key(3)] public ImmutableList<SkillItemData> Skills { get; }
    [Key(4)] public ImmutableList<AffixData> Affixes { get; }

    public CreatureData BuildCreature(GameData gameData, UserData userData)
    {
      var baseType = gameData.CreatureTypes[CreatureTypeId];
      var statTable = new StatTable(userData.Stats)
        .InsertModifier(Stat.CreatureSpeed.Set(baseType.Speed))
        .InsertNullableModifier(Stat.IsManaCreature.SetIfTrue(baseType.IsManaCreature));
      var (stats, delegates) = AffixData.ProcessAffixes(statTable, Affixes);

      return new CreatureData(
        new CreatureDelegateList(delegates.Select(DelegateMap.Get).ToImmutableList()),
        stats,
        Skills.Select(s => s.BuildSkill(gameData, stats)).ToImmutableList(),
        baseType,
        this
      );
    }
  }
}
