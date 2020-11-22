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

using System;
using Nighthollow.Generated;

namespace Nighthollow.Stats
{
  public static class NumericOperation
  {
    public static NumericOperation<TValue> Add<TValue>(TValue value) where TValue : struct =>
      new NumericOperation<TValue>(value, null, null);

    public static NumericOperation<TValue> Increase<TValue>(PercentageValue value) where TValue : struct =>
      new NumericOperation<TValue>(null, value, null);

    public static NumericOperation<TValue> Overwrite<TValue>(TValue value) where TValue : struct =>
      new NumericOperation<TValue>(null, null, value);
  }

  public sealed class NumericOperation<TValue> : IOperation where TValue : struct
  {
    public TValue? AddTo { get; }
    public PercentageValue? IncreaseBy { get; }
    public TValue? Overwrite { get; }

    public NumericOperation(TValue? addTo, PercentageValue? increaseBy, TValue? overwrite)
    {
      AddTo = addTo;
      IncreaseBy = increaseBy;
      Overwrite = overwrite;
    }

    public SerializedOperation Serialize()
    {
      if (AddTo != null)
      {
        return new SerializedOperation(AddTo.ToString(), Operator.Add);
      }
      else if (IncreaseBy != null)
      {
        return new SerializedOperation(IncreaseBy.ToString(), Operator.Increase);
      }
      else if (Overwrite != null)
      {
        return new SerializedOperation(Overwrite.ToString(), Operator.Overwrite);
      }
      else
      {
        throw new InvalidOperationException("Invalid NumericOperation");
      }
    }

    public override string ToString() => $"{Serialize().Operator} {Serialize().Value}";
  }
}
