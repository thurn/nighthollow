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

using System.Collections.Generic;

namespace Nighthollow.Stats
{
  public abstract class AbstractStat<TOperation, TValue> : IStat
    where TOperation : IOperation where TValue : struct, IStatValue
  {
    protected AbstractStat(int id)
    {
      Id = id;
    }

    public int Id { get; }

    public abstract TValue DefaultValue();

    public abstract TValue ComputeValue(IReadOnlyList<TOperation> operations);

    protected abstract TValue ParseStatValue(string value);

    public IModifier Modifier(TOperation operation, ILifetime lifetime) =>
      new Modifier<TOperation, TValue>(this, operation, lifetime);

    public IModifier StaticModifier(TOperation operation) =>
      Modifier(operation, StaticLifetime.Instance);

    public IStatValue ParseValue(string value) =>
      ParseStatValue(value);

    public abstract void InsertDefault(StatTable table, string value);

    public IStatValue Lookup(StatTable table) =>
      table.Get(this);
  }

  public abstract class NumericStat<TValue> : AbstractStat<NumericOperation<TValue>, TValue>
    where TValue : struct, IStatValue
  {
    protected NumericStat(int id) : base(id)
    {
    }

    public override void InsertDefault(StatTable table, string value) =>
      StaticModifier(NumericOperation.Add(ParseStatValue(value))).InsertInto(table);
  }
}