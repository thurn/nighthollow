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
using System.Collections.Immutable;
using System.Linq;
using MessagePack;
using Nighthollow.Delegates.Core;
using Nighthollow.Stats2;

#nullable enable

namespace Nighthollow.Data
{
  [MessagePackObject]
  public sealed partial class CreatureItemData : IItemData
  {
    public CreatureItemData(
      int creatureTypeId,
      string name,
      School school,
      ImmutableList<SkillItemData>? skills = null,
      ImmutableList<AffixData>? affixes = null,
      ImmutableList<ModifierData>? implicitModifiers = null,
      int health = 0,
      int manaCost = 0,
      ImmutableDictionaryValue<School, int>? influenceCost = null,
      ImmutableDictionaryValue<DamageType, IntRangeValue>? baseDamage = null)
    {
      CreatureTypeId = creatureTypeId;
      Name = name;
      School = school;
      Skills = skills ?? ImmutableList<SkillItemData>.Empty;
      Affixes = affixes ?? ImmutableList<AffixData>.Empty;
      ImplicitModifiers = implicitModifiers ?? ImmutableList<ModifierData>.Empty;
      Health = health;
      ManaCost = manaCost;
      InfluenceCost = influenceCost ?? ImmutableDictionaryValue<School, int>.Empty;
      BaseDamage = baseDamage ?? ImmutableDictionaryValue<DamageType, IntRangeValue>.Empty;
    }

    [ForeignKey(typeof(CreatureTypeData))]
    [Key(0)] public int CreatureTypeId { get; }

    [Key(1)] public string Name { get; }
    [Key(2)] public School School { get; }
    [Key(3)] public ImmutableList<SkillItemData> Skills { get; }
    [Key(4)] public ImmutableList<AffixData> Affixes { get; }
    [Key(5)] public ImmutableList<ModifierData> ImplicitModifiers { get; }
    [Key(6)] public int Health { get; }
    [Key(7)] public int ManaCost { get; }
    [Key(8)] public ImmutableDictionaryValue<School, int> InfluenceCost { get; }
    [Key(9)] public ImmutableDictionaryValue<DamageType, IntRangeValue> BaseDamage { get; }

    public override string ToString() => Name;

    public T Switch<T>(
      Func<CreatureItemData, T> onCreature,
      Func<ResourceItemData, T> onResource) => onCreature(this);

    public CreatureData BuildCreature(GameData gameData, StatTable parentStats)
    {
      var baseType = gameData.CreatureTypes[CreatureTypeId];
      var statTable = new StatTable(parentStats)
        .InsertModifier(Stat.Health.Set(Health))
        .InsertModifier(Stat.CreatureSpeed.Set(baseType.Speed))
        .InsertNullableModifier(Stat.IsManaCreature.SetIfTrue(baseType.IsManaCreature))
        .InsertModifier(Stat.ManaCost.Set(ManaCost))
        .InsertModifier(Stat.InfluenceCost.Set(InfluenceCost.Dictionary))
        .InsertModifier(Stat.BaseDamage.Set(BaseDamage.Dictionary));
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
