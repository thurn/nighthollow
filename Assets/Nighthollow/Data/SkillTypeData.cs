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
  public sealed class SkillTypeData
  {
    public int Id { get; }
    public string Name { get; }
    public string? Address { get; }
    public SkillAnimationType SkillAnimationType { get; }
    public bool IsMelee { get; }
    public bool IsProjectile { get; }

    public SkillTypeData(GameDataService service, IReadOnlyDictionary<string, string> row)
    {
      Id = Parse.IntRequired(row, "Skill ID");
      Name = Parse.StringRequired(row, "Name");
      Address = Parse.String(row, "Address");
      SkillAnimationType = (SkillAnimationType) Parse.IntRequired(row, "Animation");
      IsMelee = Parse.Boolean(row, "Is Melee?");
      IsProjectile = Parse.Boolean(row, "Is Projectile?");
    }
  }
}