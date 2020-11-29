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
using Nighthollow.Generated;
using Nighthollow.Utils;
using SimpleJSON;

#nullable enable

namespace Nighthollow.Stats
{
  public class StatModifierTable
  {
    protected readonly Dictionary<StatId, List<IStatModifier>> Modifiers;

    public StatModifierTable()
    {
      Modifiers = new Dictionary<StatId, List<IStatModifier>>();
    }

    protected StatModifierTable(Dictionary<StatId, List<IStatModifier>> modifiers)
    {
      Modifiers = modifiers.ToDictionary(k => k.Key, v => v.Value.Select(o => o).ToList());
    }

    public void InsertModifier(IStatModifier modifier)
    {
      Modifiers.GetOrCreateDefault(modifier.Stat.Id, new List<IStatModifier>()).Add(modifier);
    }

    public void InsertModifier<TOperation, TValue>(
      AbstractStat<TOperation, TValue> stat, TOperation operation, ILifetime lifetime) where TOperation : IOperation
    {
      InsertModifier(new StatModifier<TOperation, TValue>(stat, operation, lifetime));
    }

    public void Clear()
    {
      Modifiers.Clear();
    }

    public StatTable Clone(StatTable parent)
    {
      return new StatTable(parent, Modifiers);
    }

    public static StatModifierTable Deserialize(JSONNode node)
    {
      return new StatModifierTable(DeserializeInternal(node));
    }

    protected static Dictionary<StatId, List<IStatModifier>> DeserializeInternal(JSONNode node)
    {
      var dictionary = new Dictionary<StatId, List<IStatModifier>>();
      foreach (var row in node["modifiers"].AsArray.Children)
      {
        var statId = (StatId) row["statId"].AsInt;
        dictionary.GetOrCreateDefault(statId, new List<IStatModifier>()).Add(StatModifierUtil.Deserialize(row));
      }

      return dictionary;
    }

    public JSONNode Serialize()
    {
      return new JSONObject
      {
        ["modifiers"] = Modifiers.SelectMany(pair => pair.Value).Select(modifier => modifier.Serialize()).AsJsonArray()
      };
    }

    public override string ToString()
    {
      return string.Join(", ", Modifiers.SelectMany(pair => pair.Value).Select(m => m.ToString()));
    }
  }

  public sealed class StatTable : StatModifierTable
  {
    public static readonly StatTable Defaults =
      new StatTable(parent: null, new Dictionary<StatId, List<IStatModifier>>());

    readonly StatTable? _parent;

    public StatTable(StatTable parent)
    {
      _parent = parent;
    }

    public StatTable(StatTable? parent, Dictionary<StatId, List<IStatModifier>> modifiers) : base(modifiers)
    {
      _parent = parent;
    }

    public TValue Get<TOperation, TValue>(AbstractStat<TOperation, TValue> stat) where TOperation : IOperation
    {
      return stat.ComputeValue(OperationsForStatId(stat.Id).Select(op => (TOperation) op.Operation).ToList());
    }

    public static StatTable Deserialize(JSONNode node, StatTable parent)
    {
      return new StatTable(parent, DeserializeInternal(node));
    }

    IEnumerable<IStatModifier> OperationsForStatId(StatId statId)
    {
      if (Modifiers.ContainsKey(statId))
      {
        var list = Modifiers[statId];
        list.RemoveAll(m => !m.Lifetime.IsValid());
        return _parent == null ? list : _parent.OperationsForStatId(statId).Concat(list);
      }
      else
      {
        return _parent == null ? new List<IStatModifier>() : _parent.OperationsForStatId(statId);
      }
    }
  }
}
