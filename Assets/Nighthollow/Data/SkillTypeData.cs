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
using Nighthollow.Utils;

#nullable enable

namespace Nighthollow.Data
{
  public sealed class SkillTypeData
  {
    public SkillTypeData(GameDataService service, IReadOnlyDictionary<string, string> row)
    {
      Id = Parse.IntRequired(row, "Skill ID");
      Name = Parse.StringRequired(row, "Name");
      Address = Parse.String(row, "Address");
      SkillAnimationType = (SkillAnimationType) Parse.IntRequired(row, "Animation");
      SkillType = (SkillType) Parse.IntRequired(row, "Skill Type");
      ProjectileSpeed = Parse.Int(row, "Projectile Speed");
      UsesAccuracy = Parse.Boolean(row, "Uses Accuracy?");
      CanCrit = Parse.Boolean(row, "Can Crit?");
      CanStun = Parse.Boolean(row, "Can Stun?");

      var implicitAffixes = new List<AffixTypeData>();
      if (row.ContainsKey("Implicit Affix IDs"))
        implicitAffixes.AddRange(
          row["Implicit Affix IDs"]
            .Split(',')
            .Select(id => service.GetAffixType(int.Parse(id))));

      ImplicitAffixes = implicitAffixes;
    }

    public int Id { get; }
    public string Name { get; }
    public string? Address { get; }
    public SkillAnimationType SkillAnimationType { get; }
    public SkillType SkillType { get; }
    public int? ProjectileSpeed { get; }
    public bool UsesAccuracy { get; }
    public bool CanCrit { get; }
    public bool CanStun { get; }
    public IReadOnlyList<AffixTypeData> ImplicitAffixes { get; }
  }
}
