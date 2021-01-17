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
using Nighthollow.Delegates.Core;
using Nighthollow.Stats;

#nullable enable

namespace Nighthollow.Data
{
  public sealed partial class SkillData : StatEntity
  {
    public SkillData(
      DelegateList @delegate,
      StatTable stats,
      int baseTypeId,
      SkillTypeData baseType,
      SkillItemData itemData)
    {
      Delegate = @delegate;
      Stats = stats;
      BaseTypeId = baseTypeId;
      BaseType = baseType;
      ItemData = itemData;
    }

    [Field] public DelegateList Delegate { get; }
    [Field] public override StatTable Stats { get; }
    [Field] public int BaseTypeId { get; }
    [Field] public SkillTypeData BaseType { get; }
    [Field] public SkillItemData ItemData { get; }
    public bool IsMelee() => BaseType.SkillType == SkillType.Melee;

    public bool IsProjectile() => BaseType.SkillType == SkillType.Projectile;

    public SkillData OnTick() => WithStats(Stats.OnTick());

    public override string ToString() => ItemData.Name;
  }
}
