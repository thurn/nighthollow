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
  public sealed partial class AffixTypeData
  {
    [SerializationConstructor]
    public AffixTypeData(
      int minLevel,
      int weight,
      IntRangeValue manaCost,
      ImmutableList<ModifierTypeData>? modifiers = null,
      School? influenceType = null)
    {
      MinLevel = minLevel;
      Weight = weight;
      ManaCost = manaCost;
      Modifiers = modifiers ?? ImmutableList<ModifierTypeData>.Empty;
      InfluenceType = influenceType;
    }

    [Key("minLevel")] public int MinLevel { get; }
    [Key("weight")] public int Weight { get; }
    [Key("manaCost")] public IntRangeValue ManaCost { get; }
    [Key("modifiers")] public ImmutableList<ModifierTypeData> Modifiers { get; }
    [Key("influenceType")] public School? InfluenceType { get; }
  }
}
