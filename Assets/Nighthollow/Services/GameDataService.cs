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
using Nighthollow.Generated;
using Nighthollow.Stats;
using Nighthollow.Utils;
using SimpleJSON;
using UnityEngine;

#nullable enable

namespace Nighthollow.Services
{
  public sealed class GameDataService : MonoBehaviour
  {
    readonly Dictionary<StatScope, StatTable> _statDefaults = new Dictionary<StatScope, StatTable>();
    readonly Dictionary<int, ModifierTypeData> _modifiers = new Dictionary<int, ModifierTypeData>();
    readonly Dictionary<int, AffixTypeData> _affixes = new Dictionary<int, AffixTypeData>();
    readonly Dictionary<int, SkillTypeData> _skills = new Dictionary<int, SkillTypeData>();
    readonly Dictionary<int, CreatureTypeData> _creatures = new Dictionary<int, CreatureTypeData>();
    readonly Dictionary<ModifierPath, IStatValue> _modifierValues = new Dictionary<ModifierPath, IStatValue>();

    readonly Dictionary<StaticCardList, List<CreatureItemData>> _staticCardLists =
      new Dictionary<StaticCardList, List<CreatureItemData>>();

    public StatTable GetDefaultStats(StatScope scope) =>
      _statDefaults.ContainsKey(scope) ? _statDefaults[scope].Clone() : new StatTable();

    public IEnumerable<CreatureTypeData> AllCreatureTypes => _creatures.Values;

    public IEnumerable<SkillTypeData> AllSkillTypes => _skills.Values;

    public void FetchData(Action onComplete)
    {
      StartCoroutine(FetchDataAsync(onComplete));
    }

    public ModifierTypeData GetModifier(int modifierId) => Lookup(_modifiers, modifierId);

    public AffixTypeData GetAffixType(int affixId) => Lookup(_affixes, affixId);

    public SkillTypeData GetSkillType(int skillId) => Lookup(_skills, skillId);

    public CreatureTypeData GetCreatureType(int creatureId) => Lookup(_creatures, creatureId);

    public IReadOnlyList<CreatureItemData> GetStaticCardList(StaticCardList listName)
    {
      Errors.CheckState(_staticCardLists.ContainsKey(listName), $"List {listName} not found");
      return _staticCardLists[listName];
    }

    static T Lookup<T>(IReadOnlyDictionary<int, T> dictionary, int id) where T : class =>
      dictionary.ContainsKey(id) ? dictionary[id] : throw new ArgumentException($"ID not found: {id}");

    IEnumerator<YieldInstruction> FetchDataAsync(Action onComplete)
    {
      Debug.Log("Fetching Data...");
      var request = SpreadsheetHelper.SpreadsheetRequest(new List<string>
        {"Creatures", "Skills", "Stats", "Modifiers", "Affixes", "CardLists", "CardValues"}
      );
      yield return request.SendWebRequest();
      var node = JSON.Parse(request.downloadHandler.text);
      var parsed = SpreadsheetHelper.ParseResponse(node);
      Debug.Log("Got Response");

      _modifiers.Clear();
      _affixes.Clear();
      _skills.Clear();
      _creatures.Clear();
      _staticCardLists.Clear();

      foreach (var stat in parsed["Stats"])
      {
        if (!stat.ContainsKey("Default Value"))
        {
          continue;
        }

        var statId = Parse.IntRequired(stat, "Stat ID");
        var scope = (StatScope) Parse.IntRequired(stat, "Default Scope");
        if (!_statDefaults.ContainsKey(scope))
        {
          _statDefaults[scope] = new StatTable();
        }

        StatUtil.ParseStat((StatType) Parse.IntRequired(stat, "Type"), stat["Default Value"])
          .AddTo(_statDefaults[scope].UnsafeGet(Stat.GetStat(statId)));
      }

      foreach (var modifier in parsed["Modifiers"].Select(row => new ModifierTypeData(row)))
      {
        _modifiers[modifier.Id] = modifier;
      }

      foreach (var affix in parsed["Affixes"].Select(row => new AffixTypeData(this, row)))
      {
        _affixes[affix.Id] = affix;
      }

      foreach (var skill in parsed["Skills"].Select(row => new SkillTypeData(this, row)))
      {
        _skills[skill.Id] = skill;
      }

      foreach (var row in parsed["Creatures"])
      {
        if (row.ContainsKey("Prefab Address"))
        {
          var creature = new CreatureTypeData(this, row);
          _creatures[creature.Id] = creature;
        }
      }

      foreach (var row in parsed["CardValues"])
      {
        ParseModifierValue(row);
      }

      foreach (var row in parsed["CardLists"])
      {
        ParseStaticCard(row);
      }


      Root.Instance.AssetService.FetchAssets(this, onComplete);
    }

    void ParseStaticCard(IReadOnlyDictionary<string, string> row)
    {
      var cardId = Parse.IntRequired(row, "Card ID");
      var list = (StaticCardList) Parse.IntRequired(row, "Card List");
      if (!_staticCardLists.ContainsKey(list))
      {
        _staticCardLists[list] = new List<CreatureItemData>();
      }

      var school = (School) Parse.IntRequired(row, "School");

      var creatureType = GetCreatureType(Parse.IntRequired(row, "Base Creature"));

      var stats = Root.Instance.GameDataService.GetDefaultStats(StatScope.Creatures);
      stats.Get(Stat.Health).Add(Parse.IntRequired(row, "Health"));
      stats.Get(Stat.ManaCost).Add(Parse.Int(row, "Mana Cost") ?? 0);

      var costString = Parse.String(row, "Influence Cost");
      if (costString != null)
      {
        stats.Get(Stat.InfluenceCost)
          .AddValue<IntValue>(StatUtil.ParseStat(StatType.SchoolInts, costString));
      }

      var baseDamageString = Parse.String(row, "Base Damage");
      if (baseDamageString != null)
      {
        stats.Get(Stat.BaseDamage).AddValue<IntRangeValue>(
          (TaggedStatListValue<DamageType, IntRangeValue, IntRangeStat>?) StatUtil.ParseStat(
            StatType.DamageTypeIntRanges, baseDamageString));
      }

      var cardName = Parse.String(row, "Card Name");
      var result = new CreatureItemData(
        cardName ?? creatureType.Name,
        creatureType,
        school,
        stats,
        BuildSkills(creatureType, cardId),
        BuildAffixes(creatureType, cardId));

      _staticCardLists[list].Add(result);
    }

    List<SkillItemData> BuildSkills(CreatureTypeData creatureType, int cardId)
    {
      var skills = new List<SkillItemData>();
      var affixes = new List<AffixData>();
      var skill = creatureType.ImplicitSkill;
      if (skill != null)
      {
        var modifierList = new List<Modifier>();

        if (skill.ImplicitAffix != null)
        {
          foreach (var range in skill.ImplicitAffix.ModifierRanges)
          {
            var path = new ModifierPath(cardId, skill.ImplicitAffix.Id, range.ModifierData.Id, skill.Id);
            modifierList.Add(new Modifier(
              range.ModifierData,
              _modifierValues.ContainsKey(path) ? _modifierValues[path] : null));
          }

          affixes.Add(new AffixData(skill.ImplicitAffix.Id, modifierList));
        }

        skills.Add(new SkillItemData(
          skill,
          Root.Instance.GameDataService.GetDefaultStats(StatScope.Skills),
          affixes));
      }

      return skills;
    }

    List<AffixData> BuildAffixes(CreatureTypeData creatureType, int cardId)
    {
      var affixes = new List<AffixData>();
      if (creatureType.ImplicitAffix != null)
      {
        var modifierList = new List<Modifier>();

        foreach (var range in creatureType.ImplicitAffix.ModifierRanges)
        {
          var path = new ModifierPath(cardId, creatureType.ImplicitAffix.Id, range.ModifierData.Id);
          modifierList.Add(new Modifier(
            range.ModifierData,
            _modifierValues.ContainsKey(path) ? _modifierValues[path] : null));
        }

        affixes.Add(new AffixData(creatureType.ImplicitAffix.Id, modifierList));
      }

      return affixes;
    }

    void ParseModifierValue(IReadOnlyDictionary<string, string> row)
    {
      var modifierId = Parse.IntRequired(row, "Modifier");
      var modifierPath = new ModifierPath(
        Parse.IntRequired(row, "Card ID"),
        Parse.IntRequired(row, "Affix ID"),
        modifierId,
        Parse.Int(row, "Skill"));
      _modifierValues[modifierPath] = Errors.CheckNotNull(
        ModifierUtil.ParseArgument(GetModifier(modifierId),
          Parse.StringRequired(row, "Value")));
    }
  }
}