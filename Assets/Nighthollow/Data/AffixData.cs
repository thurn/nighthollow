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
using Nighthollow.Stats;
using Nighthollow.Utils;

#nullable enable

namespace Nighthollow.Data
{
  public sealed class ModifierData
  {
    public ModifierTypeData Data { get; }
    public IStatValue? Value { get; }

    public ModifierData(ModifierTypeData modifierData, IStatValue? value)
    {
      ModifierUtil.Validate(modifierData, value);
      Data = modifierData;
      Value = value;
    }
  }

  public sealed class AffixData
  {
    public int AffixTypeId { get; }
    public IReadOnlyList<ModifierData> Modifiers { get; }

    public AffixData(int affixTypeId, IReadOnlyList<ModifierData> modifiers)
    {
      AffixTypeId = affixTypeId;
      Modifiers = modifiers;
    }
  }
}