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
using Nighthollow.Generated;
using Nighthollow.Services;
using Nighthollow.Utils;

#nullable enable

namespace Nighthollow.Data
{
  public readonly struct CreatureSkillAnimation
  {
    public readonly SkillAnimationNumber Number;
    public readonly SkillAnimationType Type;

    public CreatureSkillAnimation(SkillAnimationNumber number, SkillAnimationType type)
    {
      Number = number;
      Type = type;
    }
  }

  public sealed class CreatureTypeData
  {
    public CreatureTypeData(GameDataService service, IReadOnlyDictionary<string, string> row)
    {
      Id = Parse.IntRequired(row, "Creature ID");
      Name = Parse.StringRequired(row, "Name");
      PrefabAddress = Parse.StringRequired(row, "Prefab Address");
      HealthLow = Parse.IntRequired(row, "Health Low");
      HealthHigh = Parse.IntRequired(row, "Health High");
      Owner = (PlayerName) Parse.IntRequired(row, "Owner ID");
      ImageAddress = Parse.String(row, "Image Address");
      BaseManaCost = Parse.Int(row, "Base Mana Cost") ?? 0;
      Speed = Parse.Int(row, "Speed") ?? 0;

      var affixId = Parse.Int(row, "Implicit Affix ID");
      if (affixId.HasValue) ImplicitAffix = service.GetAffixType(affixId.Value);

      var skillId = Parse.Int(row, "Implicit Skill");
      if (skillId.HasValue) ImplicitSkill = service.GetSkillType(skillId.Value);

      var animations = new List<CreatureSkillAnimation>();
      AddAnimation(row, "Skill 1", SkillAnimationNumber.Skill1, animations);
      AddAnimation(row, "Skill 2", SkillAnimationNumber.Skill2, animations);
      AddAnimation(row, "Skill 3", SkillAnimationNumber.Skill3, animations);
      AddAnimation(row, "Skill 4", SkillAnimationNumber.Skill4, animations);
      AddAnimation(row, "Skill 5", SkillAnimationNumber.Skill5, animations);
      SkillAnimations = animations;

      IsManaCreature = Parse.Boolean(row, "Mana Creature?");
    }

    public int Id { get; }
    public string Name { get; }
    public string PrefabAddress { get; }
    public int HealthLow { get; }
    public int HealthHigh { get; }
    public PlayerName Owner { get; }
    public string? ImageAddress { get; }
    public int BaseManaCost { get; }
    public int Speed { get; }
    public AffixTypeData? ImplicitAffix { get; }
    public SkillTypeData? ImplicitSkill { get; }
    public bool IsManaCreature { get; }
    public List<CreatureSkillAnimation> SkillAnimations { get; }

    void AddAnimation(
      IReadOnlyDictionary<string, string> row,
      string label,
      SkillAnimationNumber number,
      ICollection<CreatureSkillAnimation> list)
    {
      var type = (SkillAnimationType?) Parse.Int(row, label);
      if (type.HasValue) list.Add(new CreatureSkillAnimation(number, type.Value));
    }
  }
}
