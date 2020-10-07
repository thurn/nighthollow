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
using Nighthollow.Utils;

namespace Nighthollow.Data
{
  public sealed class ModifierRange
  {
    public ModifierData Modifier { get; }
    public string? Low { get; }
    public string? High { get; }

    public ModifierRange(DataService service, IReadOnlyDictionary<string, string> row, int number)
    {
      Modifier = service.GetModifier(Parse.UIntRequired(row, $"Modifier {number}"));
      Low = Parse.String(row, $"Low {number}");
      High = Parse.String(row, $"High {number}");
    }
  }

  public sealed class AffixTypeData
  {
    public uint Id { get; }
    public uint MinLevel { get; }
    public uint Weight { get; }
    public AffixPool AffixPool { get; }
    public IReadOnlyList<ModifierRange> Modifiers { get; }

    public AffixTypeData(DataService service, IReadOnlyDictionary<string, string> row)
    {
      Id = Parse.UIntRequired(row, "Affix ID");
      MinLevel = Parse.UIntRequired(row, "Min Level");
      Weight = Parse.UIntRequired(row, "Weight");
      AffixPool = (AffixPool) Parse.UIntRequired(row, "Affix Pool");
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