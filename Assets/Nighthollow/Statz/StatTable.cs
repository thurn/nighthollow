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

using System.Collections.Generic;
using System.Linq;
using Nighthollow.Utils;

#nullable enable

namespace Nighthollow.Statz
{
  public sealed class StatTable
  {
    readonly Dictionary<int, List<IModifier>> _modifiers = new Dictionary<int, List<IModifier>>();

    public TValue Get<TOperation, TValue>(AbstractStat<TOperation, TValue> stat)
      where TOperation : IOperation where TValue : struct, IStatValue
    {
      if (_modifiers.ContainsKey(stat.Id))
      {
        var list = _modifiers[stat.Id];
        list.RemoveAll(m => !m.IsValid());
        return stat.ComputeValue(list.Cast<IModifier<TOperation>>().Select(m => m.Operation).ToList());
      }

      return stat.DefaultValue();
    }

    public void InsertModifier<TOperation, TValue>(
      AbstractStat<TOperation, TValue> stat, IModifier<TOperation> modifier)
      where TOperation : IOperation where TValue : struct, IStatValue
    {
      _modifiers.GetValueOrDefault(stat.Id, new List<IModifier>()).Add(modifier);
    }
  }
}