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
using Nighthollow.Data;
using Nighthollow.Delegates.Core;
using Nighthollow.Generated;
using Nighthollow.Stats;

#nullable enable

namespace Nighthollow.Model
{
  public sealed class CreatureData : StatEntity
  {
    public CreatureData(
      string name,
      CreatureTypeData baseType,
      School school,
      IReadOnlyList<SkillData> skills,
      StatTable stats,
      DelegateList creatureDelegate,
      IReadOnlyList<AffixData> targetedAffixes,
      CreatureItemData item)
    {
      Name = name;
      BaseType = baseType;
      School = school;
      Skills = skills;
      Stats = stats;
      Delegate = creatureDelegate;
      TargetedAffixes = targetedAffixes;
      Item = item;
    }

    public string Name { get; }
    public CreatureTypeData BaseType { get; }
    public School School { get; }
    public IReadOnlyList<SkillData> Skills { get; }
    public override StatTable Stats { get; }
    public DelegateList Delegate { get; }
    public IReadOnlyList<AffixData> TargetedAffixes { get; }
    public CreatureItemData Item { get; }

    public CreatureData Clone(StatTable parentStats)
    {
      var statTable = Stats.Clone(parentStats);
      return new CreatureData(
        Name,
        BaseType,
        School,
        Skills.Select(s => s.Clone(statTable)).ToList(),
        statTable,
        Delegate,
        TargetedAffixes,
        Item);
    }
  }
}
