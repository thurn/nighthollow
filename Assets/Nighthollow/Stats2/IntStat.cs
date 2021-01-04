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
using Nighthollow.Generated;
using Nighthollow.Utils;

#nullable enable

namespace Nighthollow.Stats2
{
  public sealed class IntStat : AbstractStat<NumericOperation<int>, int>
  {
    public IntStat(StatId statId) : base(statId)
    {
    }

    public static int Compute<TValue>(
      IReadOnlyDictionary<OperationType, IEnumerable<NumericOperation<TValue>>> groups,
      Func<TValue, int> toInt)
      where TValue : struct
    {
      var result = 0;
      var overwrites = groups[OperationType.Overwrite].ToList();
      if (overwrites.Any())
      {
        result = toInt(overwrites.Last().OvewriteWith!.Value);
      }

      result += groups[OperationType.Add].Select(op => toInt(op.AddTo!.Value)).Sum();

      var increaseBy =
        10000 + groups[OperationType.Increase]
          .Select(op => op.IncreaseBy!.Value)
          .Sum(increase => increase.AsBasisPoints());
      return Constants.FractionBasisPoints(result, increaseBy);
    }

    public override int ComputeValue(
      IReadOnlyDictionary<OperationType, IEnumerable<NumericOperation<int>>> groups) =>
      Compute(groups, i => i);
  }
}