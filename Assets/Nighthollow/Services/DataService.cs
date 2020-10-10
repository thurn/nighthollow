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
  public sealed class DataService : MonoBehaviour
  {
    readonly Dictionary<int, ModifierData> _modifiers = new Dictionary<int, ModifierData>();
    readonly Dictionary<int, AffixTypeData> _affixes = new Dictionary<int, AffixTypeData>();
    readonly Dictionary<int, SkillTypeData> _skills = new Dictionary<int, SkillTypeData>();
    readonly Dictionary<int, CreatureTypeData> _creatures = new Dictionary<int, CreatureTypeData>();

    readonly Dictionary<StaticCardList, List<CreatureItemData>> _staticCardLists =
      new Dictionary<StaticCardList, List<CreatureItemData>>();

    public void FetchData(Action onComplete)
    {
      StartCoroutine(FetchDataAsync(onComplete));
    }

    public ModifierData GetModifier(int modifierId) => Lookup(_modifiers, modifierId);

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
        {"Creatures", "Skills", "Modifiers", "Affixes", "CardLists"}
      );
      yield return request.SendWebRequest();
      var node = JSON.Parse(request.downloadHandler.text);
      var parsed = SpreadsheetHelper.ParseResponse(node);
      Debug.Log("Got Response");

      foreach (var modifier in parsed["Modifiers"].Select(row => new ModifierData(row)))
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

      foreach (var row in parsed["CardLists"])
      {
        var list = (StaticCardList) Parse.IntRequired(row, "Card List");
        if (!_staticCardLists.ContainsKey(list))
        {
          _staticCardLists[list] = new List<CreatureItemData>();
        }

        var influenceCost = new TaggedStats<School, IntStat>();
        influenceCost.Add((School) Parse.IntRequired(row, "School"),
          new IntStat((int) Parse.IntRequired(row, "Influence Cost")));

        _staticCardLists[list].Add(new CreatureItemData(
          Parse.StringRequired(row, "Card Name"),
          GetCreatureType(Parse.IntRequired(row, "Base Creature")),
          Parse.IntRequired(row, "Health"),
          Parse.IntRequired(row, "Mana Cost"),
          influenceCost,
          new List<SkillData>(),
          new List<AffixData>()));
      }

      onComplete();
    }
  }
}