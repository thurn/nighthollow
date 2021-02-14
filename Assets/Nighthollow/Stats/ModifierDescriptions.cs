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

using Nighthollow.Data;

#nullable enable

namespace Nighthollow.Stats
{
  public static class ModifierDescriptions
  {
    static string AddPlusSign(object value)
    {
      var addSign = value switch
      {
        int i => i > 0,
        IntValueData i => i.Int > 0,
        PercentageValue p => p.BasisPoints > 0,
        DurationValue d => d.TimeMilliseconds > 0,
        IntRangeValue r => r.Low > 0,
        _ => false
      };

      return addSign ? $"+{value}" : value.ToString();
    }

    public static string NumericModifierString(
      object? addTo,
      PercentageValue? increaseBy,
      object? setTo,
      object? highValue)
    {
      var highLeft = highValue == null ? "" : "(";
      if (addTo != null && highValue != null)
      {
        highValue = AddPlusSign(highValue);
      }

      var highRight = highValue == null ? "" : $" to {highValue})";
      if (addTo != null)
      {
        return $"{highLeft}{AddPlusSign(addTo)}{highRight}";
      }
      else if (increaseBy.HasValue)
      {
        return increaseBy.Value.IsNegative()
          ? $"{highLeft}{increaseBy}{highRight} Reduced"
          : $"{highLeft}{increaseBy}{highRight} Increased";
      }

      return $"{highLeft}{setTo}{highRight}";
    }
  }
}