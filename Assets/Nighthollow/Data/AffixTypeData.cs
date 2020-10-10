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
    public ModifierData Modifier { get; }
    public IStatValue? Low { get; }
    public IStatValue? High { get; }

    public ModifierRange(DataService service, IReadOnlyDictionary<string, string> row, int number)
    {
      Modifier = service.GetModifier(Parse.IntRequired(row, $"Modifier {number}"));

      if (row.ContainsKey($"Low {number}") && row.ContainsKey($"High {number}"))
      {
        Low = Modifiers.ParseArgument(Modifier, Parse.String(row, $"Low {number}"));
        High = Modifiers.ParseArgument(Modifier, Parse.String(row, $"High {number}"));
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
    public IReadOnlyList<ModifierRange> Modifiers { get; }

    public AffixTypeData(DataService service, IReadOnlyDictionary<string, string> row)
    {
      Id = Parse.IntRequired(row, "Affix ID");
      MinLevel = Parse.IntRequired(row, "Min Level");
      Weight = Parse.IntRequired(row, "Weight");
      ManaCostLow = Parse.IntRequired(row, "Mana Cost Low");
      ManaCostHigh = Parse.IntRequired(row, "Mana Cost High");
      InfluenceType = (School?) Parse.Int(row, "Influence Type");
      AffixPool = (AffixPool) Parse.IntRequired(row, "Affix Pool");
      var modifiers = new List<ModifierRange>();

      if (row.ContainsKey("Modifier 1"))
      {
        modifiers.Add(new ModifierRange(service, row, 1));
      }

      if (row.ContainsKey("Modifier 2"))
      {
        modifiers.Add(new ModifierRange(service, row, 2));
      }

      if (row.ContainsKey("Modifier 3"))
      {
        modifiers.Add(new ModifierRange(service, row, 3));
      }

      Modifiers = modifiers;
    }
  }
}