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

using System;
using System.Collections.Generic;
using System.Linq;
using Nighthollow.Data;
using Nighthollow.Generated;
using Nighthollow.Stats;
using Nighthollow.Utils;
using SimpleJSON;
using UnityEngine;

namespace Nighthollow.Services
{
  public sealed class GameDataService : MonoBehaviour
  {
    readonly Dictionary<int, ModifierTypeData> _modifiers = new Dictionary<int, ModifierTypeData>();
    readonly Dictionary<int, AffixTypeData> _affixes = new Dictionary<int, AffixTypeData>();
    readonly Dictionary<int, SkillTypeData> _skills = new Dictionary<int, SkillTypeData>();
    readonly Dictionary<int, CreatureTypeData> _creatures = new Dictionary<int, CreatureTypeData>();
    readonly Dictionary<int, ModifierValues> _modifierValues = new Dictionary<int, ModifierValues>();

    readonly Dictionary<StaticCardList, List<CreatureItemData>> _staticCardLists =
      new Dictionary<StaticCardList, List<CreatureItemData>>();

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

      StatTable.Defaults.Clear();
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
        Stat.GetStat(statId).ParseModifier(stat["Default Value"], Operator.Overwrite).InsertInto(StatTable.Defaults);
      }

      foreach (var modifier in parsed["Modifiers"].Select(row => new ModifierTypeData(row)))
      {
        _modifiers[modifier.Id] = modifier;
      }

      ParseAffixes(parsed);

      foreach (var skill in parsed["Skills"].Select(row => new SkillTypeData(this, row)))
      {
        _skills[skill.Id] = skill;
      }

      foreach (var creature in parsed["Creatures"]
        .Where(row => row.ContainsKey("Prefab Address"))
        .Select(row => new CreatureTypeData(this, row)))
      {
        _creatures[creature.Id] = creature;
      }

      foreach (var row in parsed["CardValues"])
      {
        _modifierValues
          .GetOrCreateDefault(Parse.IntRequired(row, "Card ID"), new ModifierValues())
          .AddValue(this, row);
      }

      foreach (var row in parsed["CardLists"])
      {
        ParseStaticCard(row);
      }


      Root.Instance.AssetService.FetchAssets(this, onComplete);
    }

    void ParseAffixes(IReadOnlyDictionary<string, List<Dictionary<string, string>>> parsed)
    {
      AffixTypeData.Builder? currentlyBuilding = null;
      foreach (var row in parsed["Affixes"])
      {
        var affixId = Parse.Int(row, "Affix ID");
        if (affixId != null)
        {
          if (currentlyBuilding != null)
          {
            _affixes[currentlyBuilding.Id] = new AffixTypeData(currentlyBuilding);
          }

          currentlyBuilding = new AffixTypeData.Builder
          {
            Id = affixId.Value,
            MinLevel = Parse.Int(row, "Min Level").GetValueOrDefault(),
            Weight = Parse.Int(row, "Weight").GetValueOrDefault(),
            ManaCostLow = Parse.Int(row, "Mana Cost Low").GetValueOrDefault(),
            ManaCostHigh = Parse.Int(row, "Mana Cost High").GetValueOrDefault(),
            InfluenceType = (School?) Parse.Int(row, "Influence Type"),
            AffixPool = (AffixPool) Parse.IntRequired(row, "Affix Pool")
          };

          currentlyBuilding.ModifierRanges.Add(new ModifierRange(this, row));
        }
        else
        {
          currentlyBuilding?.ModifierRanges.Add(new ModifierRange(this, row));
        }
      }

      if (currentlyBuilding != null)
      {
        _affixes[currentlyBuilding.Id] = new AffixTypeData(currentlyBuilding);
      }
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

      var stats = new StatModifierTable();
      Stat.Health.Add(Parse.IntRequired(row, "Health")).InsertInto(stats);
      Stat.ManaCost.Add(Parse.Int(row, "Mana Cost") ?? 0).InsertInto(stats);

      var costString = Parse.String(row, "Influence Cost");
      if (costString != null)
      {
        Stat.InfluenceCost.ParseModifier(costString, Operator.Add).InsertInto(stats);
      }

      var baseDamageString = Parse.String(row, "Base Damage");
      if (baseDamageString != null)
      {
        Stat.BaseDamage.ParseModifier(baseDamageString, Operator.Overwrite).InsertInto(stats);
      }

      var cardName = Parse.String(row, "Card Name");
      var result = new CreatureItemData(
        cardName ?? creatureType.Name,
        creatureType,
        school,
        stats,
        _modifierValues.GetOrCreateDefault(cardId, new ModifierValues()).BuildSkills(this, creatureType),
        _modifierValues.GetOrCreateDefault(cardId, new ModifierValues()).BuildAffixes(this, creatureType));

      _staticCardLists[list].Add(result);
    }
  }
}