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
using System.Collections.Immutable;
using MessagePack;
using Nighthollow.Generated;
using Nighthollow.Stats;

#nullable enable

namespace Nighthollow.Data
{
  [MessagePackObject]
  public sealed class AffixTypeData
  {
    public AffixTypeData(
      int minLevel,
      int weight,
      IntRangeValue manaCost,
      IReadOnlyList<ModifierTypeData>? modifiers = null,
      bool isTargeted = false,
      School? influenceType = null)
    {
      MinLevel = minLevel;
      Weight = weight;
      ManaCost = manaCost;
      Modifiers = modifiers?.ToImmutableList() ?? ImmutableList<ModifierTypeData>.Empty;
      IsTargeted = isTargeted;
      InfluenceType = influenceType;
    }

    [Key(0)] public int MinLevel { get; }
    [Key(1)] public int Weight { get; }
    [Key(2)] public IntRangeValue ManaCost { get; }
    [Key(3)] public IReadOnlyList<ModifierTypeData> Modifiers { get; }
    [Key(4)] public bool IsTargeted { get; }
    [Key(5)] public School? InfluenceType { get; }
  }
}
