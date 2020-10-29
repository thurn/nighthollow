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
    readonly Dictionary<int, List<(IOperation, ILifetime)>> _modifiers;

    public StatTable()
    {
      _modifiers = new Dictionary<int, List<(IOperation, ILifetime)>>();
    }

    StatTable(Dictionary<int, List<(IOperation, ILifetime)>> modifiers)
    {
      _modifiers = modifiers.ToDictionary(e => e.Key, e => e.Value);
    }

    public TValue Get<TOperation, TValue>(AbstractStat<TOperation, TValue> stat)
      where TOperation : IOperation where TValue : struct, IStatValue
    {
      if (_modifiers.ContainsKey(stat.Id))
      {
        var list = _modifiers[stat.Id];
        list.RemoveAll(m => !m.Item2.IsValid());
        return stat.ComputeValue(list.Cast<(TOperation, ILifetime)>().Select(m => m.Item1).ToList());
      }

      return stat.DefaultValue();
    }

    public void InsertModifier<TOperation, TValue>(
      AbstractStat<TOperation, TValue> stat, TOperation operation, ILifetime lifetime)
      where TOperation : IOperation where TValue : struct, IStatValue
    {
      _modifiers.GetValueOrDefault(stat.Id, new List<(IOperation, ILifetime)>()).Add((operation, lifetime));
    }

    public StatTable Clone() => new StatTable(_modifiers);
  }
}