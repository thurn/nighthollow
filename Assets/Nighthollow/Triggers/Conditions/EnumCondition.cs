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
using System.Collections.Immutable;
using MessagePack;
using Nighthollow.Data;
using Nighthollow.Triggers.Events;

#nullable enable

namespace Nighthollow.Triggers.Conditions
{
  public enum EnumOperator
  {
    Unknown = 0,
    Is = 1,
    IsNot = 2
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

    public abstract ImmutableHashSet<IKey> Dependencies { get; }

    public bool Satisfied(IScope scope)
    {
      var source = GetSource(scope);
      return Operator switch
      {
        EnumOperator.Is => Equals(source, Target),
        EnumOperator.IsNot => !Equals(source, Target),
        _ => throw new ArgumentOutOfRangeException()
      };
    }

    protected abstract TEnum GetSource(IScope scope);

    protected abstract EnumCondition<TEvent, TEnum> Clone(TEnum target, EnumOperator op);

    public EnumCondition<TEvent, TEnum> WithTarget(TEnum target) => Equals(target, Target)
      ? this
      : Clone(target, Operator);

    public EnumCondition<TEvent, TEnum> WithOperator(EnumOperator op) => Equals(op, Operator)
      ? this
      : Clone(Target, op);
  }
}