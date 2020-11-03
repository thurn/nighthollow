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
  public sealed class OperationWithLifetime
  {
    public IOperation Operation { get; }
    public ILifetime Lifetime { get; }

    public OperationWithLifetime(IOperation operation, ILifetime lifetime)
    {
      Operation = operation;
      Lifetime = lifetime;
    }
  }

  public class StatModifierTable
  {
    protected readonly Dictionary<int, List<OperationWithLifetime>> Modifiers;

    public StatModifierTable()
    {
      Modifiers = new Dictionary<int, List<OperationWithLifetime>>();
    }

    protected StatModifierTable(Dictionary<int, List<OperationWithLifetime>> modifiers)
    {
      Modifiers = modifiers.ToDictionary(k => k.Key, v => v.Value.Select(o => o).ToList());
    }

    public void InsertModifier<TOperation, TValue>(
      AbstractStat<TOperation, TValue> stat, TOperation operation, ILifetime lifetime) where TOperation : IOperation
    {
      Modifiers.GetOrCreateDefault(stat.Id, new List<OperationWithLifetime>())
        .Add(new OperationWithLifetime(operation, lifetime));
    }

    public void Clear()
    {
      Modifiers.Clear();
    }

    public StatTable Clone(StatTable parent) => new StatTable(parent, Modifiers);
  }

  public sealed class StatTable : StatModifierTable
  {
    public static readonly StatTable Defaults = new StatTable(null, new Dictionary<int, List<OperationWithLifetime>>());
    readonly StatTable? _parent;

    public StatTable(StatTable parent)
    {
      _parent = parent;
    }

    public StatTable(StatTable? parent, Dictionary<int, List<OperationWithLifetime>> modifiers) : base(modifiers)
    {
      _parent = parent;
    }

    public TValue Get<TOperation, TValue>(AbstractStat<TOperation, TValue> stat) where TOperation : IOperation =>
      stat.ComputeValue(OperationsForStatId(stat.Id).Select(op => (TOperation) op.Operation).ToList());

    IEnumerable<OperationWithLifetime> OperationsForStatId(int statId)
    {
      if (Modifiers.ContainsKey(statId))
      {
        var list = Modifiers[statId];
        list.RemoveAll(m => !m.Lifetime.IsValid());
        return _parent == null ? list : _parent.OperationsForStatId(statId).Concat(list);
      }
      else
      {
        return _parent == null ? new List<OperationWithLifetime>() : _parent.OperationsForStatId(statId);
      }
    }
  }
}