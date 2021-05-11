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
using Nighthollow.Data;

#nullable enable

namespace Nighthollow.Triggers.Conditions
{
  [MessagePackObject]
  public abstract class IntegerCondition<TEvent> : ICondition<TEvent> where TEvent : TriggerEvent
  {
    protected IntegerCondition(int target, IntegerOperator op = IntegerOperator.EqualTo)
    {
      Operator = op;
      Target = target;
    }

    public enum IntegerOperator
    {
      Unknown = 0,
      EqualTo = 1,
      NotEqualTo = 2,
      LessThan = 3,
      LessThanOrEqualTo = 4,
      GreaterThan = 5,
      GreaterThanOrEqualTo = 6
    }

    [Key(0)] public int Target { get; }
    [Key(1)] public IntegerOperator Operator { get; }

    public bool Satisfied(TEvent triggerEvent, GameData data)
    {
      var source = GetSource(triggerEvent, data);
      return Operator switch
      {
        IntegerOperator.EqualTo => source == Target,
        IntegerOperator.NotEqualTo => source != Target,
        IntegerOperator.LessThan => source < Target,
        IntegerOperator.LessThanOrEqualTo => source <= Target,
        IntegerOperator.GreaterThan => source > Target,
        IntegerOperator.GreaterThanOrEqualTo => source >= Target,
        _ => throw new ArgumentOutOfRangeException()
      };
    }

    public abstract int GetSource(TEvent triggerEvent, GameData data);
  }
}