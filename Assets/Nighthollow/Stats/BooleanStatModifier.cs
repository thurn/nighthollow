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

using System;
using MessagePack;
using Nighthollow.Data;

#nullable enable

namespace Nighthollow.Stats
{
  [MessagePackObject]
  public sealed class BooleanStatModifier : IStatModifier
  {
#pragma warning disable 618 // Using Obsolete
    public static BooleanStatModifier Set(AbstractStat<BooleanStatModifier, bool> stat, bool value) =>
      new BooleanStatModifier(stat.StatId, value);
#pragma warning restore 618

    [SerializationConstructor]
    [Obsolete("This constructor is visible only for use by the serialization system.")]
    public BooleanStatModifier(StatId statId, bool setValue)
    {
      StatId = statId;
      SetValue = setValue;
      Type = ModifierType.Set;
    }

    [Key(0)] public StatId StatId { get; }
    [Key(1)] public bool SetValue { get; }
    [IgnoreMember] public ModifierType Type { get; }

    public string Describe(string template, IValueData? highValue)
    {
      var split = template.Split('/');
      return SetValue ? split[0].Trim() : split[1].Trim();
    }
  }
}
