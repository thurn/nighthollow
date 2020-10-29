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
using System.Linq;

#nullable enable

namespace Nighthollow.Stats
{
  public readonly struct BoolValue : IStatValue
  {
    public bool Bool { get; }

    public BoolValue(bool b)
    {
      Bool = b;
    }
  }

  public sealed class BoolStat : AbstractStat<BooleanOperation, BoolValue>
  {
    public BoolStat(int id) : base(id)
    {
    }

    public override BoolValue DefaultValue() => new BoolValue(false);

    public override BoolValue ComputeValue(IReadOnlyList<BooleanOperation> operations)
    {
      return new BoolValue(
        operations.Count(op => op.SetBoolean) > 0 && operations.Count(op => op.SetBoolean == false) == 0);
    }

    protected override BoolValue ParseStatValue(string value) => new BoolValue(bool.Parse(value));

    public IModifier SetTrue() => StaticModifier(new BooleanOperation(true));

    public IModifier SetFalse() => StaticModifier(new BooleanOperation(true));

    public override void InsertDefault(StatTable table, string value) =>
      StaticModifier(new BooleanOperation(ParseStatValue(value).Bool)).InsertInto(table);
  }
}