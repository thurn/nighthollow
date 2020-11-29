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
using Nighthollow.Generated;

#nullable enable

namespace Nighthollow.Stats
{
  public abstract class AbstractStat<TOperation, TValue> : IStat
    where TOperation : IOperation
  {
    protected AbstractStat(StatId id)
    {
      Id = id;
    }

    public StatId Id { get; }

    public abstract IStatModifier ParseModifier(string value, Operator op);

    public abstract IStatModifier? StaticModifierForOperator(Operator op);

    public override string ToString()
    {
      return Id.ToString();
    }

    public abstract TValue ComputeValue(IReadOnlyList<TOperation> operations);

    protected abstract TValue ParseStatValue(string value);

    public IStatModifier Modifier(TOperation operation, ILifetime lifetime)
    {
      return new StatModifier<TOperation, TValue>(this, operation, lifetime);
    }

    protected IStatModifier StaticModifier(TOperation operation)
    {
      return Modifier(operation, StaticLifetime.Instance);
    }
  }

  public abstract class NumericStat<TValue> : AbstractStat<NumericOperation<TValue>, TValue>
    where TValue : struct
  {
    protected NumericStat(StatId id) : base(id)
    {
    }

    public override IStatModifier ParseModifier(string value, Operator op)
    {
      return op switch
      {
        Operator.Add => StaticModifier(NumericOperation.Add(ParseStatValue(value))),
        Operator.Overwrite => StaticModifier(NumericOperation.Overwrite(ParseStatValue(value))),
        Operator.Increase => StaticModifier(NumericOperation.Increase<TValue>(PercentageStat.ParsePercentage(value))),
        _ => throw new ArgumentException($"Unsupported operator type: {op}")
      };
    }

    public override IStatModifier? StaticModifierForOperator(Operator op)
    {
      return null;
    }
  }
}
