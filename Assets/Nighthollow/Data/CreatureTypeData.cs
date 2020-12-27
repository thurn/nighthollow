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
using Nighthollow.Stats;

#nullable enable

namespace Nighthollow.Data
{
  [MessagePackObject]
  public sealed class CreatureTypeData
  {
    public CreatureTypeData(
      int id,
      string name,
      string prefabAddress,
      PlayerName owner,
      IntRangeValue health,
      ImmutableDictionary<SkillAnimationNumber, SkillAnimationType> skillAnimations,
      string? imageAddress = null,
      int baseManaCost = 0,
      int speed = 0,
      AffixTypeData? implicitAffix = null,
      SkillTypeData? implicitSkill = null,
      bool isManaCreature = false)
    {
      Id = id;
      Name = name;
      PrefabAddress = prefabAddress;
      Owner = owner;
      Health = health;
      SkillAnimations = skillAnimations;
      ImageAddress = imageAddress;
      BaseManaCost = baseManaCost;
      Speed = speed;
      ImplicitAffix = implicitAffix;
      ImplicitSkill = implicitSkill;
      IsManaCreature = isManaCreature;
    }

    [Key(0)] public int Id { get; }
    [Key(1)] public string Name { get; }
    [Key(2)] public string PrefabAddress { get; }
    [Key(3)] public PlayerName Owner { get; }
    [Key(4)] public IntRangeValue Health { get; }
    [Key(5)] public ImmutableDictionary<SkillAnimationNumber, SkillAnimationType> SkillAnimations { get; }
    [Key(6)] public string? ImageAddress { get; }
    [Key(7)] public int BaseManaCost { get; }
    [Key(8)] public int Speed { get; }
    [Key(9)] public AffixTypeData? ImplicitAffix { get; }
    [Key(10)] public SkillTypeData? ImplicitSkill { get; }
    [Key(11)] public bool IsManaCreature { get; }
  }
}