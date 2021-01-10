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
using Nighthollow.Delegates.Core;
using Nighthollow.Stats2;

#nullable enable

namespace Nighthollow.Data
{
  [MessagePackObject]
  public sealed partial class SkillItemData
  {
    public SkillItemData(
      int skillTypeId,
      ImmutableList<AffixData>? affixes = null,
      ImmutableList<ModifierData>? implicitModifiers = null)
    {
      SkillTypeId = skillTypeId;
      Affixes = affixes ?? ImmutableList<AffixData>.Empty;
      ImplicitModifiers = implicitModifiers ?? ImmutableList<ModifierData>.Empty;
    }

    // TODO: Handle skills which do not have a type ID because they're embedded within a creature type
    [ForeignKey(typeof(SkillTypeData))]
    [Key(0)] public int SkillTypeId { get; }

    [Key(1)] public ImmutableList<AffixData> Affixes { get; }
    [Key(2)] public ImmutableList<ModifierData> ImplicitModifiers { get; }

    public SkillData BuildSkill(GameData gameData, StatTable parentStatTable)
    {
      var baseType = gameData.SkillTypes[SkillTypeId];
      var statTable = new StatTable(parentStatTable)
        .InsertNullableModifier(Stat.ProjectileSpeed.Set(baseType.ProjectileSpeed))
        .InsertNullableModifier(Stat.UsesAccuracy.SetIfTrue(baseType.UsesAccuracy))
        .InsertNullableModifier(Stat.CanCrit.SetIfTrue(baseType.CanCrit))
        .InsertNullableModifier(Stat.CanStun.SetIfTrue(baseType.CanStun));

      var (stats, delegates) = AffixData.ProcessAffixes(statTable, Affixes);

      return new SkillData(
        new SkillDelegateList(delegates.Select(DelegateMap.Get).ToImmutableList()),
        stats,
        baseType,
        this);
    }
  }
}
