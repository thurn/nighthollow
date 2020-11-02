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
using Nighthollow.Services;
using Nighthollow.Stats;
using Nighthollow.Utils;

namespace Nighthollow.Data
{
  public sealed class ModifierRange
  {
    public ModifierTypeData BaseType { get; }
    public IStatModifier? Low { get; }
    public IStatModifier? High { get; }

    public ModifierRange(GameDataService service, IReadOnlyDictionary<string, string> row)
    {
      BaseType = service.GetModifier(Parse.IntRequired(row, $"Modifier"));

      if (row.ContainsKey($"Low") && row.ContainsKey($"High"))
      {
        Low = ModifierUtil.ParseModifier(BaseType, Parse.String(row, $"Low"));
        High = ModifierUtil.ParseModifier(BaseType, Parse.String(row, $"High"));
      }
    }
  }

  public sealed class AffixTypeData
  {
    public int Id { get; }
    public int MinLevel { get; }
    public int Weight { get; }
    public int ManaCostLow { get; }
    public int ManaCostHigh { get; }
    public School? InfluenceType { get; }
    public AffixPool AffixPool { get; }
    public IReadOnlyList<ModifierRange> ModifierRanges { get; }

    public sealed class Builder
    {
      public int Id { get; set; }
      public int MinLevel { get; set; }
      public int Weight { get; set; }
      public int ManaCostLow { get; set; }
      public int ManaCostHigh { get; set; }
      public School? InfluenceType { get; set; }
      public AffixPool AffixPool { get; set; }
      public List<ModifierRange> ModifierRanges { get; } = new List<ModifierRange>();
    }

    public AffixTypeData(Builder builder)
    {
      Id = builder.Id;
      MinLevel = builder.MinLevel;
      Weight = builder.Weight;
      ManaCostLow = builder.ManaCostLow;
      ManaCostHigh = builder.ManaCostHigh;
      InfluenceType = builder.InfluenceType;
      AffixPool = builder.AffixPool;
      ModifierRanges = builder.ModifierRanges;
    }
  }
}