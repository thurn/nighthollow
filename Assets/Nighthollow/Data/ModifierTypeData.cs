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
using System.Text;
using MessagePack;
using Nighthollow.Generated;

#nullable enable

namespace Nighthollow.Data
{
  [MessagePackObject]
  public sealed partial class ModifierTypeData
  {
    public ModifierTypeData(
      StatId? statId = null,
      Operator? statOperator = null,
      DelegateId? delegateId = null,
      IValueData? valueLow = null,
      IValueData? valueHigh = null,
      bool targeted = false)
    {
      StatId = statId;
      StatOperator = statOperator;
      DelegateId = delegateId;
      ValueLow = valueLow;
      ValueHigh = valueHigh;
      Targeted = targeted;
    }

    [Key("statId")] public StatId? StatId { get; }
    [Key("statOperator")] public Operator? StatOperator { get; }
    [Key("delegateId")] public DelegateId? DelegateId { get; }
    [Key("valueLow")] public IValueData? ValueLow { get; }
    [Key("valueHigh")] public IValueData? ValueHigh { get; }
    [Key("targeted")] public bool Targeted { get; } // If false, modifier automatically applies to owner

    public override string ToString()
    {
      var result = new List<string>();

      if (StatOperator.HasValue)
      {
        result.Add(StatOperator.ToString());
      }

      if (StatId.HasValue)
      {
        result.Add(StatId.ToString());
      }

      if (DelegateId.HasValue)
      {
        result.Add(DelegateId.ToString());
      }

      return string.Join(" ", result);
    }
  }
}