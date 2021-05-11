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
  public enum EnumOperator
  {
    Unknown = 0,
    EqualTo = 1,
    NotEqualTo = 2
  }

  /// <summary>
  /// Parent class for standard operations on enum values
  /// </summary>
  public abstract class EnumCondition<TEvent, TEnum> : ICondition<TEvent>
    where TEvent : TriggerEvent where TEnum : struct, Enum
  {
    protected EnumCondition(TEnum target, EnumOperator op)
    {
      Target = target;
      Operator = op;
    }

    [Key(0)] public TEnum Target { get; }
    [Key(1)] public EnumOperator Operator { get; }

    public bool Satisfied(TEvent triggerEvent, GameData data)
    {
      var source = GetSource(triggerEvent, data);
      return Operator switch
      {
        EnumOperator.EqualTo => Equals(source, Target),
        EnumOperator.NotEqualTo => !Equals(source, Target),
        _ => throw new ArgumentOutOfRangeException()
      };
    }

    public abstract TEnum GetSource(TEvent triggerEvent, GameData data);

    public abstract string SourceDescription { get; }

    public string Description => $"{SourceDescription} {OperatorDescription} {Target}";

    string OperatorDescription => Operator switch
    {
      EnumOperator.EqualTo => "is",
      EnumOperator.NotEqualTo => "is not",
      _ => "<Operator?>"
    };
  }
}