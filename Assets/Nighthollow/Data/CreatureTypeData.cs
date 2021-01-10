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
using Nighthollow.Stats2;

#nullable enable

namespace Nighthollow.Data
{
  [MessagePackObject]
  public sealed partial class CreatureSkillAnimation
  {
    [SerializationConstructor]
    public CreatureSkillAnimation(
      SkillAnimationNumber skillAnimationNumber,
      SkillAnimationType skillAnimationType,
      DurationValue? duration)
    {
      SkillAnimationNumber = skillAnimationNumber;
      SkillAnimationType = skillAnimationType;
      Duration = duration;
    }

    [Key(0)] public SkillAnimationNumber SkillAnimationNumber { get; }
    [Key(1)] public SkillAnimationType SkillAnimationType { get; }
    [Key(2)] public DurationValue? Duration { get; }

    public override string ToString() => $"{SkillAnimationNumber}: {SkillAnimationType}";
  }

  [MessagePackObject]
  public sealed partial class CreatureTypeData
  {
    public CreatureTypeData(
      string name,
      string prefabAddress,
      PlayerName owner,
      IntRangeValue health,
      string? imageAddress = null,
      int baseManaCost = 0,
      int speed = 0,
      ImmutableList<ModifierData>? implicitModifiers = null,
      ImmutableList<SkillTypeData>? implicitSkills = null,
      ImmutableList<CreatureSkillAnimation>? skillAnimations = null,
      bool isManaCreature = false)
    {
      Name = name;
      PrefabAddress = prefabAddress;
      Owner = owner;
      Health = health;
      ImageAddress = imageAddress;
      BaseManaCost = baseManaCost;
      Speed = speed;
      ImplicitModifiers = implicitModifiers ?? ImmutableList<ModifierData>.Empty;
      ImplicitSkills = implicitSkills ?? ImmutableList<SkillTypeData>.Empty;
      SkillAnimations = skillAnimations ?? ImmutableList<CreatureSkillAnimation>.Empty;
      IsManaCreature = isManaCreature;
    }

    [Key(0)] public string Name { get; }
    [Key(1)] public string PrefabAddress { get; }
    [Key(2)] public PlayerName Owner { get; }
    [Key(3)] public IntRangeValue Health { get; }
    [Key(4)] public string? ImageAddress { get; }
    [Key(5)] public int BaseManaCost { get; }
    [Key(6)] public int Speed { get; }
    [Key(7)] public ImmutableList<ModifierData> ImplicitModifiers { get; }
    [Key(8)] public ImmutableList<SkillTypeData> ImplicitSkills { get; }
    [Key(9)] public ImmutableList<CreatureSkillAnimation> SkillAnimations { get; }
    [Key(10)] public bool IsManaCreature { get; }

    public override string ToString() => Name;
  }
}
