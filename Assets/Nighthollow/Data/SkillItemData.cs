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
using Nighthollow.Stats;

#nullable enable

namespace Nighthollow.Data
{
  [MessagePackObject]
  public sealed partial class SkillItemData
  {
    public SkillItemData(
      int skillTypeId,
      ImmutableList<AffixData>? affixes = null,
      ImmutableList<ModifierData>? implicitModifiers = null,
      string name = "",
      ImmutableList<CreatureItemData>? summons = null,
      ImmutableList<StatusEffectItemData>? statusEffects = null,
      DurationValue? cooldown = null)
    {
      SkillTypeId = skillTypeId;
      Affixes = affixes ?? ImmutableList<AffixData>.Empty;
      ImplicitModifiers = implicitModifiers ?? ImmutableList<ModifierData>.Empty;
      Name = name;
      Summons = summons ?? ImmutableList<CreatureItemData>.Empty;
      StatusEffects = statusEffects ?? ImmutableList<StatusEffectItemData>.Empty;
      Cooldown = cooldown;
    }

    [ForeignKey(typeof(SkillTypeData))]
    [Key(0)] public int SkillTypeId { get; }

    [Key(1)] public ImmutableList<AffixData> Affixes { get; }
    [Key(2)] public ImmutableList<ModifierData> ImplicitModifiers { get; }
    [Key(3)] public string Name { get; }
    [Key(4)] public ImmutableList<CreatureItemData> Summons { get; }
    [Key(5)] public ImmutableList<StatusEffectItemData> StatusEffects { get; }
    [Key(6)] public DurationValue? Cooldown { get; }

    public override string ToString() => Name;

    public SkillData BuildSkill(
      GameData gameData,
      StatTable parentStatTable,
      DelegateList parentDelegateList)
    {
      var baseType = gameData.SkillTypes[SkillTypeId];
      var statTable = new StatTable(parentStatTable)
        .InsertNullableModifier(Stat.ProjectileSpeed.Set(baseType.ProjectileSpeed))
        .InsertNullableModifier(Stat.UsesAccuracy.SetIfTrue(baseType.UsesAccuracy))
        .InsertNullableModifier(Stat.CanCrit.SetIfTrue(baseType.CanCrit))
        .InsertNullableModifier(Stat.CanStun.SetIfTrue(baseType.CanStun))
        .InsertNullableModifier(Stat.Cooldown.Set(Cooldown));

      var (stats, delegates) = AffixData.ProcessAffixes(statTable,
        Affixes.SelectMany(a => a.Modifiers).Concat(ImplicitModifiers));

      return new SkillData(
        new DelegateList(delegates.Append(DelegateId.DefaultSkillDelegate)
          .Select(DelegateMap.Get)
          .ToImmutableList(), parentDelegateList),
        stats,
        SkillTypeId,
        baseType,
        this);
    }

    public static SkillData BasicMeleeAttack(
      GameData gameData, StatTable parentStatTable, DelegateList parentDelegateList) =>
      new SkillItemData(
        SkillTypeData.BasicMeleeAttackId,
        ImmutableList<AffixData>.Empty,
        ImmutableList<ModifierData>.Empty,
        "Basic Melee Attack").BuildSkill(gameData, parentStatTable, parentDelegateList);
  }
}