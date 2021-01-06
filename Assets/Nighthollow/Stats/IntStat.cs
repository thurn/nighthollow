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
using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using Nighthollow.Generated;
using Nighthollow.Utils;

#nullable enable

namespace Nighthollow.Stats
{
  public sealed class IntStat : NumericStat<int>
  {
    public IntStat(OldStatId id) : base(id)
    {
    }

    public override int ComputeValue(IReadOnlyList<NumericOperation<int>> operations)
    {
      return Compute(operations, op => op);
    }

    public static int Compute<TValue>(
      IReadOnlyList<NumericOperation<TValue>> operations, Func<TValue, int> toInt)
      where TValue : struct
    {
      var overwrite = operations.Select(op => op.Overwrite).WhereNotNull().ToList();
      var result = 0;
      if (overwrite.Count > 0)
      {
        result = toInt(overwrite.Last());
      }

      result += operations.Select(op => op.AddTo).WhereNotNull().Sum(toInt);

      var increaseBy =
        10000 + operations.Select(op => op.IncreaseBy).WhereNotNull().Sum(increase => increase.AsBasisPoints());
      return Constants.FractionBasisPoints(result, increaseBy);
    }

    protected override int ParseStatValue(string value) => ParseInt(value);

    public static int ParseInt(string value) => int.Parse(value.Replace(",", ""));

    [MustUseReturnValue("Return value should be used")]
    public IStatModifier Add(int value) => StaticModifier(NumericOperation.Add(value));
  }
}
