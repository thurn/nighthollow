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
using SimpleJSON;
using UnityEngine;

#nullable enable

namespace Nighthollow.Services
{
  public sealed class DataService : MonoBehaviour
  {
    readonly Dictionary<uint, ModifierData> _modifiers = new Dictionary<uint, ModifierData>();
    readonly Dictionary<uint, AffixTypeData> _affixes = new Dictionary<uint, AffixTypeData>();
    readonly Dictionary<uint, SkillTypeData> _skills = new Dictionary<uint, SkillTypeData>();
    readonly Dictionary<uint, CreatureTypeData> _creatures = new Dictionary<uint, CreatureTypeData>();

    public void FetchData(Action onComplete)
    {
      StartCoroutine(FetchDataAsync(onComplete));
    }

    public ModifierData GetModifier(uint modifierId) => Lookup(_modifiers, modifierId);

    public AffixTypeData GetAffixType(uint affixId) => Lookup(_affixes, affixId);

    public SkillTypeData GetSkillType(uint skillId) => Lookup(_skills, skillId);

    public CreatureTypeData GetCreatureType(uint creatureId) => Lookup(_creatures, creatureId);

    static T Lookup<T>(IReadOnlyDictionary<uint, T> dictionary, uint id) where T : class =>
      dictionary.ContainsKey(id) ? dictionary[id] : throw new ArgumentException($"ID not found: {id}");

    IEnumerator<YieldInstruction> FetchDataAsync(Action onComplete)
    {
      Debug.Log("Fetching Data...");
      var request = SpreadsheetHelper.SpreadsheetRequest(new List<string>
        {"Creatures", "Skills", "Modifiers", "Affixes"}
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

      foreach (var creature in parsed["Creatures"].Select(row => new CreatureTypeData(this, row)))
      {
        _creatures[creature.Id] = creature;
      }

      onComplete();
    }
  }
}