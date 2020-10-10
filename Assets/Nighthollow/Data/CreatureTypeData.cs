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

using System.Collections.Generic;
using Nighthollow.Generated;
using Nighthollow.Services;
using Nighthollow.Utils;

namespace Nighthollow.Data
{
  public sealed class CreatureTypeData
  {
    public int Id { get; }
    public string Name { get; }
    public string PrefabAddress { get; }
    public int HealthLow { get; }
    public int HealthHigh { get; }
    public PlayerName Owner { get; }
    public string? ImageAddress { get; }
    public int BaseManaCost { get; }
    public AffixTypeData? ImplicitAffix { get; }
    public SkillTypeData? ImplicitSkill { get; }
    public SkillAnimationType? Skill1Type { get; }
    public SkillAnimationType? Skill2Type { get; }
    public SkillAnimationType? Skill3Type { get; }
    public SkillAnimationType? Skill4Type { get; }
    public SkillAnimationType? Skill5Type { get; }
    public bool IsManaCreature { get; }

    public CreatureTypeData(DataService service, IReadOnlyDictionary<string, string> row)
    {
      Id = Parse.IntRequired(row, "Creature ID");
      Name = Parse.StringRequired(row, "Name");
      PrefabAddress = Parse.StringRequired(row, "Prefab Address");
      HealthLow = Parse.IntRequired(row, "Health Low");
      HealthHigh = Parse.IntRequired(row, "Health High");
      Owner = (PlayerName) Parse.IntRequired(row, "Owner ID");
      ImageAddress = Parse.String(row, "Image Address");
      BaseManaCost = Parse.IntRequired(row, "Base Mana Cost");

      var affixId = Parse.Int(row, "Implicit Affix ID");
      if (affixId.HasValue)
      {
        ImplicitAffix = service.GetAffixType(affixId.Value);
      }

      var skillId = Parse.Int(row, "Implicit Skill");
      if (skillId.HasValue)
      {
        ImplicitSkill = service.GetSkillType(skillId.Value);
      }

      Skill1Type = (SkillAnimationType?) Parse.Int(row, "Skill 1");
      Skill2Type = (SkillAnimationType?) Parse.Int(row, "Skill 2");
      Skill3Type = (SkillAnimationType?) Parse.Int(row, "Skill 3");
      Skill4Type = (SkillAnimationType?) Parse.Int(row, "Skill 4");
      Skill5Type = (SkillAnimationType?) Parse.Int(row, "Skill 5");
      IsManaCreature = Parse.Boolean(row, "Mana Creature?");
    }
  }
}