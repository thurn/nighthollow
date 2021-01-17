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
using System.Linq;
using MessagePack;

#nullable enable

namespace Nighthollow.Data
{
  [MessagePackObject]
  public sealed partial class SkillTypeData
  {
    public const int BasicMeleeAttackId = 24;

    public SkillTypeData(
      string name,
      SkillAnimationType skillAnimationType,
      SkillType skillType,
      ImmutableList<ModifierData>? implicitModifiers = null,
      string? address = null,
      int? projectileSpeed = null,
      bool usesAccuracy = false,
      bool canCrit = false,
      bool canStun = false,
      ImmutableList<int>? summonCreatures = null,
      ImmutableList<int>? statusEffectIds = null)
    {
      Name = name;
      SkillAnimationType = skillAnimationType;
      SkillType = skillType;
      ImplicitModifiers = implicitModifiers ?? ImmutableList<ModifierData>.Empty;
      Address = address;
      ProjectileSpeed = projectileSpeed;
      UsesAccuracy = usesAccuracy;
      CanCrit = canCrit;
      CanStun = canStun;
      SummonCreatures = summonCreatures ?? ImmutableList<int>.Empty;
      StatusEffects = statusEffectIds ?? ImmutableList<int>.Empty;
    }

    [Key(0)] public string Name { get; }
    [Key(1)] public SkillAnimationType SkillAnimationType { get; }
    [Key(2)] public SkillType SkillType { get; }
    [Key(3)] public ImmutableList<ModifierData> ImplicitModifiers { get; }
    [Key(4)] public string? Address { get; }
    [Key(5)] public int? ProjectileSpeed { get; }
    [Key(6)] public bool UsesAccuracy { get; }
    [Key(7)] public bool CanCrit { get; }
    [Key(8)] public bool CanStun { get; }
    [Key(9)] public ImmutableList<int> SummonCreatures { get; }

    [ForeignKeyList(typeof(StatusEffectTypeData))]
    [Key(10)] public ImmutableList<int> StatusEffects { get; }

    public override string ToString() => Name;

    public static SkillItemData DefaultItem(int skillTypeId, GameData gameData)
    {
      var value = gameData.SkillTypes[skillTypeId];
      return new SkillItemData(
        skillTypeId,
        ImmutableList<AffixData>.Empty,
        value.ImplicitModifiers.Select(m => m.Value != null ? m : m.WithValue(m.ValueLow)).ToImmutableList(),
        value.Name);
    }
  }
}
