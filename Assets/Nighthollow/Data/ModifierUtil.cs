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
using Nighthollow.Generated;
using Nighthollow.Stats;

#nullable enable

namespace Nighthollow.Data
{
  public static class ModifierUtil
  {
    public static IStatValue? ParseValue(ModifierTypeData modifierData, string? value) =>
      modifierData.StatId.HasValue && value != null ? Stat.GetStat(modifierData.StatId.Value).ParseValue(value) : null;

    public static void Validate(ModifierTypeData modifierData, IStatValue? value)
    {
      if (value != null)
      {
        return;
      }

      if (modifierData.Operator == Operator.Add || modifierData.Operator == Operator.Increase)
      {
        throw new ArgumentException($"Expected a stat value for modifier {modifierData}");
      }
    }
  }
}