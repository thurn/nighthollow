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
using System.Linq;
using Nighthollow.Generated;
using Nighthollow.Stats;

namespace Nighthollow.Data
{
  public sealed class CreatureItemData
  {
    public string Name { get; }
    public CreatureTypeData BaseType { get; }
    public School School { get; }
    public StatTable Stats { get; }
    public IReadOnlyList<SkillItemData> Skills { get; }
    public IReadOnlyList<AffixData> Affixes { get; }

    public CreatureItemData(
      string name,
      CreatureTypeData baseType,
      School school,
      StatTable stats,
      IReadOnlyList<SkillItemData> skills,
      IReadOnlyList<AffixData> affixes)
    {
      Name = name;
      BaseType = baseType;
      School = school;
      Stats = stats;
      Skills = skills;
      Affixes = affixes;
    }
  }
}