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
using Nighthollow.Generated;
using Nighthollow.Services;
using Nighthollow.Stats;
using Nighthollow.Utils;

#nullable enable

namespace Nighthollow.Data
{
  public sealed class ModifierValues
  {
    sealed class AffixModifierValues
    {
      // Affix ID -> Modifier ID -> Modifier Value
      readonly Dictionary<int, Dictionary<int, IStatModifier>> _affixModifiers =
        new Dictionary<int, Dictionary<int, IStatModifier>>();

      public IEnumerable<int> AffixIds => _affixModifiers.Keys;

      public void AddValueForAffix(GameDataService service, int affixId, IReadOnlyDictionary<string, string> row)
      {
        var modifierId = Parse.IntRequired(row, "Modifier");
        var modifier = service.GetModifier(modifierId);
        var value = Parse.String(row, "Value");
        if (value != null)
        {
          _affixModifiers.GetOrCreateDefault(affixId, new Dictionary<int, IStatModifier>())[modifierId] =
            Stat.GetStat(Errors.CheckNotNull(modifier.StatId))
              .ParseModifier(Parse.StringRequired(row, "Value"), Errors.CheckNotNull(modifier.Operator));
        }
      }

      public AffixData BuildAffix(AffixTypeData affix)
      {
        var modifierValues = _affixModifiers.GetValueOrDefault(affix.Id, new Dictionary<int, IStatModifier>());
        var result = new List<ModifierData>();
        foreach (var modifierRange in affix.ModifierRanges)
        {
          var baseType = modifierRange.BaseType;

          IStatModifier? statModifier = null;
          if (modifierValues.ContainsKey(baseType.Id))
          {
            statModifier = modifierValues[baseType.Id];
          }
          else if (baseType.StatId.HasValue && baseType.Operator.HasValue)
          {
            statModifier = Stat.GetStat(baseType.StatId!.Value).StaticModifierForOperator(baseType.Operator!.Value);
          }

          result.Add(new ModifierData(
            baseType.DelegateId,
            statModifier));
        }

        return new AffixData(affix.Id, result);
      }
    }

    readonly AffixModifierValues _creatureModifiers =
      new AffixModifierValues();

    // Skill ID -> Affix Values
    readonly Dictionary<int, AffixModifierValues> _skillModifiers =
      new Dictionary<int, AffixModifierValues>();

    public void AddValue(GameDataService service, IReadOnlyDictionary<string, string> row)
    {
      var skillId = Parse.Int(row, "Skill");
      var affixId = Parse.IntRequired(row, "Affix ID");
      var values = skillId.HasValue
        ? _skillModifiers.GetOrCreateDefault(skillId.Value, new AffixModifierValues())
        : _creatureModifiers;
      values.AddValueForAffix(service, affixId, row);
    }

    public IReadOnlyList<AffixData> BuildAffixes(GameDataService service, CreatureTypeData creatureTypeData)
    {
      var affixes = new List<AffixData>();
      if (creatureTypeData.ImplicitAffix != null)
      {
        affixes.Add(_creatureModifiers.BuildAffix(creatureTypeData.ImplicitAffix));
      }

      affixes.AddRange(_creatureModifiers.AffixIds
        .Where(affixId => affixId != creatureTypeData.ImplicitAffix?.Id)
        .Select(affixId => _creatureModifiers.BuildAffix(service.GetAffixType(affixId))));

      return affixes;
    }

    public IReadOnlyList<SkillItemData> BuildSkills(GameDataService service, CreatureTypeData creatureTypeData)
    {
      var skills = new List<SkillItemData>();

      var implicitSkill = creatureTypeData.ImplicitSkill;
      if (implicitSkill != null)
      {
        skills.Add(BuildSkill(service, implicitSkill));
      }

      skills.AddRange(_skillModifiers.Keys
        .Where(skillId => skillId != implicitSkill?.Id)
        .Select(skillId => BuildSkill(service, service.GetSkillType(skillId))));

      return skills;
    }

    SkillItemData BuildSkill(GameDataService service, SkillTypeData skill)
    {
      var affixes = new List<AffixData>();
      if (skill.ImplicitAffix != null)
      {
        if (_skillModifiers.ContainsKey(skill.Id))
        {
          affixes.Add(_skillModifiers[skill.Id].BuildAffix(skill.ImplicitAffix));
        }
        else
        {
          // Affix may have no required arguments
          var result = new List<ModifierData>();
          foreach (var modifier in skill.ImplicitAffix.ModifierRanges)
          {
            var baseType = modifier.BaseType;
            IStatModifier? statModifier = null;
            if (baseType.StatId.HasValue)
            {
              statModifier = Stat.GetStat(Errors.CheckNotNull(baseType.StatId))
                .StaticModifierForOperator(Errors.CheckNotNull(baseType.Operator));
            }

            result.Add(new ModifierData(modifier.BaseType.DelegateId, statModifier));
          }

          affixes.Add(new AffixData(skill.ImplicitAffix.Id, result));
        }
      }

      if (_skillModifiers.ContainsKey(skill.Id))
      {
        affixes.AddRange(
          _skillModifiers[skill.Id].AffixIds
            .Where(affixId => affixId != skill.ImplicitAffix?.Id)
            .Select(affixId => _skillModifiers[skill.Id].BuildAffix(service.GetAffixType(affixId))));
      }

      return new SkillItemData(skill, new StatModifierTable(), affixes);
    }
  }
}