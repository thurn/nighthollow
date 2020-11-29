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


using Nighthollow.Generated;
using Nighthollow.Utils;
using SimpleJSON;

#nullable enable

namespace Nighthollow.Stats
{
  public interface IStatModifier
  {
    public IStat Stat { get; }

    public IOperation Operation { get; }

    public ILifetime Lifetime { get; }

    IStatModifier WithLifetime(ILifetime lifetime);

    string? Describe();

    JSONNode Serialize();
  }

  public static class StatModifierUtil
  {
    public static IStatModifier Deserialize(JSONNode node)
    {
      Errors.CheckNotNull(node);
      var stat = Stat.GetStat(node["statId"].AsInt);
      return stat.ParseModifier(node["value"].Value, (Operator) node["operator"].AsInt);
    }
  }

  public sealed class StatModifier<TOperation, TValue> : IStatModifier where TOperation : IOperation
  {
    readonly ILifetime _lifetime;
    readonly TOperation _operation;
    readonly AbstractStat<TOperation, TValue> _stat;

    public StatModifier(AbstractStat<TOperation, TValue> stat, TOperation operation, ILifetime lifetime)
    {
      _stat = stat;
      _operation = operation;
      _lifetime = lifetime;
    }

    public IStat Stat => _stat;

    public IOperation Operation => _operation;

    public ILifetime Lifetime => _lifetime;

    public IStatModifier WithLifetime(ILifetime lifetime)
    {
      return new StatModifier<TOperation, TValue>(_stat, _operation, lifetime);
    }

    public string? Describe()
    {
      var statDescription = Generated.Stat.GetDescription(Stat.Id);
      return statDescription == null ? null : _operation.Describe(statDescription);
    }

    public JSONNode Serialize()
    {
      var serialized = _operation.Serialize();
      return new JSONObject
      {
        ["statId"] = (int) _stat.Id,
        ["value"] = serialized.Value,
        ["operator"] = (int) serialized.Operator
      };
    }

    public override string ToString()
    {
      return $"{Stat}: {Operation}";
    }
  }
}
