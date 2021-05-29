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

using System.Linq;
using Nighthollow.Data;
using Nighthollow.Services;
using UnityEngine;

#nullable enable

namespace Nighthollow.Rules
{
  public sealed class RulesEngine
  {
    readonly ServiceRegistry _registry;

    public RulesEngine(ServiceRegistry registry)
    {
      _registry = registry;

      foreach (var rule in _registry.Database.Snapshot().Rules.Values)
      {
        var missingBindings = rule.MissingBindings();
        if (!missingBindings.IsEmpty)
        {
          var missing = string.Join(", ", missingBindings.Select(k => k.Name));
          Debug.LogError($"Rule {rule.Name} is invalid, missing bindings for [{missing}]");
        }
      }
    }

    public void Invoke<TEvent>(TEvent ruleEvent, RuleOutput? output = null) where TEvent : IEvent
    {
      var gameData = _registry.Database.Snapshot();
      var scope = ruleEvent.AddBindings(Scope.CreateBuilder(ruleEvent.GetSpec().Bindings(), _registry.Scope));

      foreach (var pair in gameData.Rules.Where(pair => pair.Value.EventName == ruleEvent.GetSpec().Name))
      {
        InvokeInternal(pair.Key, scope, pair.Value, output);
      }
    }

    public void InvokeRuleId(int ruleId, Scope scope, RuleOutput? output = null)
    {
      var rule = _registry.Database.Snapshot().Rules[ruleId];
      InvokeInternal(ruleId, scope, rule, output, true);
    }

    void InvokeInternal(
      int ruleId,
      Scope scope,
      Rule rule,
      RuleOutput? output = null,
      bool bypassEnabledCheck = false)
    {
      if (bypassEnabledCheck || !rule.Disabled)
      {
        var fired = rule.Invoke(scope, output);
        if (fired && rule.OneTime)
        {
          _registry.Database.Update(TableId.Rules, ruleId, t => t.WithDisabled(true));
        }
      }
    }
  }
}