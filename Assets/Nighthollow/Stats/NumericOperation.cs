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

namespace Nighthollow.Stats
{
  public static class NumericOperation
  {
    public static NumericOperation<TValue> Add<TValue>(TValue value) where TValue : struct, IStatValue =>
      new NumericOperation<TValue>(value, null);

    public static NumericOperation<TValue> Increase<TValue>(PercentageValue value) where TValue : struct, IStatValue =>
      new NumericOperation<TValue>(null, value);
  }

  public class NumericOperation<TValue> : IOperation where TValue : struct, IStatValue
  {
    public TValue? AddTo { get; }
    public PercentageValue? IncreaseBy { get; }

    public NumericOperation(TValue? addTo, PercentageValue? increaseBy)
    {
      AddTo = addTo;
      IncreaseBy = increaseBy;
    }
  }
}