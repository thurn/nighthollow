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
using Nighthollow.Utils;

namespace Nighthollow.Data
{
  public sealed class ModifierTypeData
  {
    public int Id { get; }
    public string Description { get; }
    public int? StatId { get; }
    public Operator? Operator { get; }
    public CreatureDelegateId? DelegateId { get; }
    public SkillDelegateId? SkillDelegateId { get; }
    public DamageType? DamageType { get; }
    public School? School { get; }

    public ModifierTypeData(IReadOnlyDictionary<string, string> row)
    {
      Id = Parse.IntRequired(row, "Modifier ID");
      Description = Parse.StringRequired(row, "Description");
      StatId = Parse.Int(row, "Stat");
      Operator = (Operator?) Parse.Int(row, "Operator");
      DelegateId = (CreatureDelegateId?) Parse.Int(row, "Delegate");
      SkillDelegateId = (SkillDelegateId?) Parse.Int(row, "Skill Delegate");
      DamageType = (DamageType?) Parse.Int(row, "Damage Type");
      School = (School?) Parse.Int(row, "School");
    }

    public ModifierTypeData(
      int id,
      string description,
      int? statId,
      Operator? @operator,
      CreatureDelegateId? delegateId,
      DamageType? damageType,
      School? school)
    {
      Id = id;
      Description = description;
      StatId = statId;
      Operator = @operator;
      DelegateId = delegateId;
      DamageType = damageType;
      School = school;
    }

    public override string ToString()
    {
      return $"[{nameof(ModifierTypeData)}] {nameof(Id)}: {Id}, {nameof(Description)}: {Description}";
    }
  }
}