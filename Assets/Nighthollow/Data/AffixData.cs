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

using System.Collections.Immutable;
using System.Linq;
using MessagePack;
using Nighthollow.Delegates.Core;
using Nighthollow.Stats2;

#nullable enable

namespace Nighthollow.Data
{
  [MessagePackObject]
  public sealed partial class AffixData
  {
    public AffixData(int affixTypeId, ImmutableList<ModifierData> modifiers)
    {
      AffixTypeId = affixTypeId;
      Modifiers = modifiers;
    }

    [Key(0)] public int AffixTypeId { get; }
    [Key(1)] public ImmutableList<ModifierData> Modifiers { get; }

    public static (StatTable, ImmutableList<DelegateId>) ProcessAffixes(
      StatTable stats,
      ImmutableList<AffixData> affixes)
    {
      var delegates = ImmutableList.CreateBuilder<DelegateId>();
      foreach (var modifier in affixes.SelectMany(a => a.Modifiers).Where(modifier => !modifier.Targeted))
      {
        if (modifier.DelegateId.HasValue)
        {
          delegates.Add(modifier.DelegateId.Value);
        }

        stats = stats.InsertNullableModifier(modifier.BuildStatModifier());
      }

      return (stats, delegates.ToImmutable());
    }
  }
}
