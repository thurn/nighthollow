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
using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using Nighthollow.Utils;

namespace Nighthollow.Stats
{
  public readonly struct IntValue : IStatValue
  {
    public static readonly IntValue Zero = new IntValue(0);
    public int Int { get; }

    public IntValue(int i)
    {
      Int = i;
    }
  }

  public sealed class IntStat : NumericStat<IntValue>
  {
    public IntStat(int id) : base(id)
    {
    }

    public override IntValue ComputeValue(IReadOnlyList<NumericOperation<IntValue>> operations) =>
      Compute(operations, op => op);

    public static IntValue Compute<TValue>(
      IReadOnlyList<NumericOperation<TValue>> operations, Func<TValue, IntValue> toInt)
      where TValue : struct, IStatValue
    {
      var overwrite = operations.Select(op => op.Overwrite).WhereNotNull().ToList();
      var result = 0;
      if (overwrite.Count > 0)
      {
        result = toInt(overwrite.Last()).Int;
      }

      result += operations.Select(op => op.AddTo).WhereNotNull().Sum(addTo => toInt(addTo).Int);

      var increaseBy =
        10000 + operations.Select(op => op.IncreaseBy).WhereNotNull().Sum(increase => increase.AsBasisPoints());
      return new IntValue(Constants.FractionBasisPoints(result, increaseBy));
    }

    protected override IntValue ParseStatValue(string value) => ParseInt(value);

    public static IntValue ParseInt(string value) => new IntValue(int.Parse(value.Replace(",", "")));

    [MustUseReturnValue("Return value should be used")]
    public IStatModifier Add(int value) => StaticModifier(NumericOperation.Add(new IntValue(value)));
  }
}