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

using System.ComponentModel;
using System.Linq;
using Nighthollow.Data;
using Nighthollow.Services;
using Nighthollow.Triggers.Events;

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

      foreach (var pair in gameData.Triggers)
      {
        if (pair.Value is TriggerData<TEvent> trigger)
        {
          InvokeInternal(pair.Key, scope, trigger, output);
        }
      }
    }

    public void InvokeTriggerId(int triggerId, Scope scope, TriggerOutput? output = null)
    {
      var t = _registry.Database.Snapshot().Triggers[triggerId];
      switch (t)
      {
        case TriggerData<WorldTriggerInvokedEvent> worldTrigger:
          InvokeInternal(triggerId, scope, worldTrigger, output, true);
          break;
        case TriggerData<BattleTriggerInvokedEvent> battleTrigger:
          InvokeInternal(triggerId, scope, battleTrigger, output, true);
          break;
        case TriggerData<GlobalTriggerInvokedEvent> globalTrigger:
          InvokeInternal(triggerId, scope, globalTrigger, output, true);
          break;
        default:
          throw new InvalidEnumArgumentException(
            $"Attempted to manually invoke a trigger {t} which was not a valid TriggerInvokedEvent");
      }
    }

    void InvokeInternal<TEvent>(
      int triggerId,
      Scope scope,
      TriggerData<TEvent> trigger,
      TriggerOutput? output = null,
      bool bypassEnabledCheck = false) where TEvent : IEvent
    {
      if (bypassEnabledCheck || !trigger.Disabled)
      {
        var fired = trigger.Invoke(scope, output);
        if (fired && !trigger.Looping)
        {
          _registry.Database.Update(TableId.Triggers, triggerId, t => t.WithDisabled(true));
        }
      }
    }
  }
}