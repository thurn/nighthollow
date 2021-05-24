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
using MessagePack;

#nullable enable

namespace Nighthollow.Triggers.Conditions
{
  public enum IntegerOperator
  {
    Unknown = 0,
    Is = 1,
    IsNot = 2,
    IsLessThan = 3,
    IsLessThanOrEqualTo = 4,
    IsGreaterThan = 5,
    IsGreaterThanOrEqualTo = 6
  }

  public abstract class IntegerCondition<TEvent> : TriggerCondition
  {
    protected IntegerCondition(int target, IntegerOperator op)
    {
      Operator = op;
      Target = target;
    }

    [Key(0)] public int Target { get; }
    [Key(1)] public IntegerOperator Operator { get; }

    public override bool Satisfied(IScope scope)
    {
      var source = GetSource(scope);
      return Operator switch
      {
        IntegerOperator.Is => source == Target,
        IntegerOperator.IsNot => source != Target,
        IntegerOperator.IsLessThan => source < Target,
        IntegerOperator.IsLessThanOrEqualTo => source <= Target,
        IntegerOperator.IsGreaterThan => source > Target,
        IntegerOperator.IsGreaterThanOrEqualTo => source >= Target,
        _ => throw new ArgumentOutOfRangeException()
      };
    }

    public abstract int GetSource(IScope scope);

    protected abstract IntegerCondition<TEvent> Clone(int target, IntegerOperator op);

    public IntegerCondition<TEvent> WithTarget(int target) => Equals(target, Target)
      ? this
      : Clone(target, Operator);

    public IntegerCondition<TEvent> WithOperator(IntegerOperator op) => Equals(op, Operator)
      ? this
      : Clone(Target, op);
  }
}