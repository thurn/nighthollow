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
using Nighthollow.Generated;
using Nighthollow.Utils;

#nullable enable

namespace Nighthollow.Data
{
  public sealed class ModifierTypeData
  {
    public ModifierTypeData(IReadOnlyDictionary<string, string> row)
    {
      Id = Parse.IntRequired(row, "Modifier ID");
      Description = Parse.StringRequired(row, "Description");
      StatId = Parse.Int(row, "Stat");
      Operator = (Operator?) Parse.Int(row, "Operator");
      DelegateId = (DelegateId?) Parse.Int(row, "Delegate");
    }

    public int Id { get; }
    public string Description { get; }
    public int? StatId { get; }
    public Operator? Operator { get; }
    public DelegateId? DelegateId { get; }

    public override string ToString() =>
      $"[{nameof(ModifierTypeData)}] {nameof(Id)}: {Id}, {nameof(Description)}: {Description}";
  }
}
