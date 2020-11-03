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

#nullable enable

namespace Nighthollow.Stats
{
  public sealed class BoolStat : AbstractStat<BooleanOperation, bool>
  {
    public BoolStat(int id) : base(id)
    {
    }

    public override bool ComputeValue(IReadOnlyList<BooleanOperation> operations) =>
      operations.Count(op => op.SetBoolean) > 0 && operations.Count(op => op.SetBoolean == false) == 0;

    protected override bool ParseStatValue(string value) => bool.Parse(value);

    public IStatModifier SetTrue() => StaticModifier(new BooleanOperation(true));

    public IStatModifier SetFalse() => StaticModifier(new BooleanOperation(true));

    public override IStatModifier ParseModifier(string value, Operator op) =>
      op switch
      {
        Operator.Add => StaticModifier(new BooleanOperation(ParseStatValue(value))),
        _ => throw new ArgumentException($"Unsupported operator type: {op}")
      };

    public override IStatModifier? StaticModifierForOperator(Operator op) =>
      op switch
      {
        Operator.SetFalse => StaticModifier(new BooleanOperation(false)),
        Operator.SetTrue => StaticModifier(new BooleanOperation(true)),
        _ => null
      };
  }
}