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

using MessagePack;
using Nighthollow.Generated;

#nullable enable

namespace Nighthollow.Data
{
  [MessagePackObject]
  public sealed class ModifierTypeData
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
      Operator = statOperator;
      DelegateId = delegateId;
      ValueLow = valueLow;
      ValueHigh = valueHigh;
      Targeted = targeted;
    }

    [Key(0)] public StatId? StatId { get; }
    [Key(1)] public Operator? Operator { get; }
    [Key(2)] public DelegateId? DelegateId { get; }
    [Key(3)] public IValueData? ValueLow { get; }
    [Key(4)] public IValueData? ValueHigh { get; }

    /// <summary>
    /// If false, modifier is applied to owner automatically. If true, it requires manual application to a target.
    /// </summary>
    [Key(5)]
    public bool Targeted { get; }
  }
}
