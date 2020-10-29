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
using System.Linq;
using Nighthollow.Utils;

namespace Nighthollow.Stats
{
  public sealed class StatTable
  {
    public sealed class OperationLifetime
    {
      public IOperation Operation { get; }
      public ILifetime Lifetime { get; }

      public OperationLifetime(IOperation operation, ILifetime lifetime)
      {
        Operation = operation;
        Lifetime = lifetime;
      }
    }

    readonly Dictionary<int, List<OperationLifetime>> _modifiers;

    public StatTable()
    {
      _modifiers = new Dictionary<int, List<OperationLifetime>>();
    }

    StatTable(Dictionary<int, List<OperationLifetime>> modifiers)
    {
      _modifiers = modifiers.ToDictionary(e => e.Key, e => e.Value);
    }

    public TValue Get<TOperation, TValue>(AbstractStat<TOperation, TValue> stat)
      where TOperation : IOperation where TValue : struct, IStatValue
    {
      if (_modifiers.ContainsKey(stat.Id))
      {
        var list = _modifiers[stat.Id];
        list.RemoveAll(m => !m.Lifetime.IsValid());
        return stat.ComputeValue(list.Select(op => (TOperation) op.Operation).ToList());
      }

      return stat.DefaultValue();
    }

    public void InsertModifier<TOperation, TValue>(
      AbstractStat<TOperation, TValue> stat, TOperation operation, ILifetime lifetime)
      where TOperation : IOperation where TValue : struct, IStatValue
    {
      _modifiers.GetOrCreateDefault(stat.Id, new List<OperationLifetime>())
        .Add(new OperationLifetime(operation, lifetime));
    }

    public StatTable Clone() => new StatTable(_modifiers);
  }
}