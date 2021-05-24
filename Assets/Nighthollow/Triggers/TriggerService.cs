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

#nullable enable

namespace Nighthollow.Triggers
{
  public sealed class TriggerService
  {
    readonly ServiceRegistry _registry;
    bool _initialized;

    public TriggerService(ServiceRegistry registry)
    {
      _registry = registry;
    }

    void Initialize()
    {
      var database = _registry.Database;
      if (_registry.Globals.GetBool(GlobalId.ShouldAutoenableTriggers))
      {
        foreach (var pair in database.Snapshot().Triggers.Where(pair => pair.Value?.Disabled == true))
        {
          database.Update(TableId.Triggers, pair.Key, t => t.WithDisabled(false));
        }
      }

      _initialized = true;
    }

    public void Invoke<TEvent>(TEvent triggerEvent, TriggerOutput? output = null) where TEvent : IEvent
    {
      if (!_initialized)
      {
        Initialize();
      }

      var gameData = _registry.Database.Snapshot();
      var scope = triggerEvent.AddBindings(Scope.CreateBuilder(_registry.Scope));

      foreach (var pair in gameData.Triggers.Where(pair => pair.Value.TriggerEvent == triggerEvent.GetSpec().Trigger))
      {
        InvokeInternal(pair.Key, scope, pair.Value, output);
      }
    }

    public void InvokeTriggerId(int triggerId, Scope scope, TriggerOutput? output = null)
    {
      var rule = _registry.Database.Snapshot().Triggers[triggerId];
      InvokeInternal(triggerId, scope, rule, output, true);
    }

    void InvokeInternal(
      int triggerId,
      Scope scope,
      Rule trigger,
      TriggerOutput? output = null,
      bool bypassEnabledCheck = false)
    {
      if (bypassEnabledCheck || !trigger.Disabled)
      {
        var fired = trigger.Invoke(scope, output);
        if (fired && trigger.OneTime)
        {
          _registry.Database.Update(TableId.Triggers, triggerId, t => t.WithDisabled(true));
        }
      }
    }
  }
}