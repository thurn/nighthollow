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

using MessagePack;
using Nighthollow.Generated;

#nullable enable

namespace Nighthollow.Data
{
  [MessagePackObject]
  public sealed partial class SkillTypeData
  {
    public SkillTypeData(
      string name,
      SkillAnimationType skillAnimationType,
      SkillType skillType,
      AffixTypeData? implicitAffix = null,
      string? address = null,
      int? projectileSpeed = null,
      bool usesAccuracy = false,
      bool canCrit = false,
      bool canStun = false)
    {
      Name = name;
      SkillAnimationType = skillAnimationType;
      SkillType = skillType;
      ImplicitAffix = implicitAffix;
      Address = address;
      ProjectileSpeed = projectileSpeed;
      UsesAccuracy = usesAccuracy;
      CanCrit = canCrit;
      CanStun = canStun;
    }

    [Key("name")] public string Name { get; }
    [Key("skillAnimationType")] public SkillAnimationType SkillAnimationType { get; }
    [Key("skillType")] public SkillType SkillType { get; }
    [Key("implicitAffix")] public AffixTypeData? ImplicitAffix { get; }
    [Key("address")] public string? Address { get; }
    [Key("projectileSpeed")] public int? ProjectileSpeed { get; }
    [Key("usesAccuracy")] public bool UsesAccuracy { get; }
    [Key("canCrit")] public bool CanCrit { get; }
    [Key("canStun")] public bool CanStun { get; }
  }
}
