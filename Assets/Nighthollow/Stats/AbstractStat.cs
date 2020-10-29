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
using Nighthollow.Generated;

namespace Nighthollow.Stats
{
  public abstract class AbstractStat<TOperation, TValue> : IStat
    where TOperation : IOperation where TValue : IStatValue
  {
    protected AbstractStat(int id)
    {
      Id = id;
    }

    public int Id { get; }

    public abstract TValue ComputeValue(IReadOnlyList<TOperation> operations);

    protected abstract TValue ParseStatValue(string value);

    public IStatModifier Modifier(TOperation operation, ILifetime lifetime) =>
      new StatModifier<TOperation, TValue>(this, operation, lifetime);

    protected IStatModifier StaticModifier(TOperation operation) =>
      Modifier(operation, StaticLifetime.Instance);

    public abstract IStatModifier ParseModifier(string value, Operator op);

    public abstract IStatModifier? StaticModifierForOperator(Operator op);

    public IStatValue Lookup(StatTable table) =>
      table.Get(this);
  }

  public abstract class NumericStat<TValue> : AbstractStat<NumericOperation<TValue>, TValue>
    where TValue : struct, IStatValue
  {
    protected NumericStat(int id) : base(id)
    {
    }

    public override IStatModifier ParseModifier(string value, Operator op) =>
      op switch
      {
        Operator.Add => StaticModifier(NumericOperation.Add(ParseStatValue(value))),
        Operator.Increase => StaticModifier(NumericOperation.Increase<TValue>(PercentageStat.ParsePercentage(value))),
        _ => throw new ArgumentException($"Unsupported operator type: {op}")
      };

    public override IStatModifier? StaticModifierForOperator(Operator op) => null;

  }
}