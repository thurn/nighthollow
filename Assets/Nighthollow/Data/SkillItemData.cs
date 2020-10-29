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
using Nighthollow.Stats;

#nullable enable

namespace Nighthollow.Data
{
  public sealed class SkillItemData
  {
    public SkillTypeData BaseType { get; }
    public StatModifierTable Stats { get; }
    public IReadOnlyList<AffixData> Affixes { get; }

    public SkillItemData(SkillTypeData baseType, StatModifierTable stats, IReadOnlyList<AffixData> affixes)
    {
      BaseType = baseType;
      Stats = stats;
      Affixes = affixes;
    }
  }
}