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

using System.Collections.Immutable;
using MessagePack;
using Nighthollow.Generated;
using Nighthollow.Stats;

#nullable enable

namespace Nighthollow.Data
{
  [MessagePackObject]
  public sealed partial class CreatureSkillAnimation
  {
    [SerializationConstructor]
    public CreatureSkillAnimation(
      SkillAnimationNumber skillAnimationNumber,
      SkillAnimationType skillAnimationType)
    {
      SkillAnimationNumber = skillAnimationNumber;
      SkillAnimationType = skillAnimationType;
    }

    [Key("skillAnimationNumber")] public SkillAnimationNumber SkillAnimationNumber { get; }
    [Key("skillAnimationType")] public SkillAnimationType SkillAnimationType { get; }

    public override string ToString() => $"{SkillAnimationNumber} => {SkillAnimationType}";
  }

  [MessagePackObject]
  public sealed partial class CreatureTypeData
  {
    public CreatureTypeData(
      string name,
      string prefabAddress,
      PlayerName owner,
      IntRangeValue health,
      ImmutableList<CreatureSkillAnimation>? skillAnimations = null,
      string? imageAddress = null,
      int baseManaCost = 0,
      int speed = 0,
      AffixTypeData? implicitAffix = null,
      ImmutableList<SkillTypeData>? implicitSkills = null,
      bool isManaCreature = false)
    {
      Name = name;
      PrefabAddress = prefabAddress;
      Owner = owner;
      Health = health;
      SkillAnimations = skillAnimations ?? ImmutableList<CreatureSkillAnimation>.Empty;
      ImageAddress = imageAddress;
      BaseManaCost = baseManaCost;
      Speed = speed;
      ImplicitAffix = implicitAffix;
      ImplicitSkills = implicitSkills ?? ImmutableList<SkillTypeData>.Empty;
      IsManaCreature = isManaCreature;
    }

    [Key("name")] public string Name { get; }
    [Key("prefabAddress")] public string PrefabAddress { get; }
    [Key("owner")] public PlayerName Owner { get; }
    [Key("health")] public IntRangeValue Health { get; }
    [Key("skillAnimations")] public ImmutableList<CreatureSkillAnimation> SkillAnimations { get; }
    [Key("imageAddress")] public string? ImageAddress { get; }
    [Key("baseManaCost")] public int BaseManaCost { get; }
    [Key("speed")] public int Speed { get; }
    [Key("implicitAffix")] public AffixTypeData? ImplicitAffix { get; }
    [Key("implicitSkills")] public ImmutableList<SkillTypeData> ImplicitSkills { get; }
    [Key("isManaCreature")] public bool IsManaCreature { get; }
  }
}
