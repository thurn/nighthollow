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
    readonly Database _database;
    readonly GlobalsService _globals;
    bool _initialized;

    public TriggerService(Database database, GlobalsService globals)
    {
      _database = database;
      _globals = globals;
    }

    void Initialize()
    {
      if (_globals.GetBool(GlobalId.ShouldAutoenableTriggers))
      {
        foreach (var pair in _database.Snapshot().Triggers.Where(pair => pair.Value?.Disabled == true))
        {
          _database.Update(TableId.Triggers, pair.Key, t => t.WithDisabled(false));
        }
      }

      _initialized = true;
    }

    public void Invoke<TEvent>(Scope scope, TriggerOutput? output = null) where TEvent : TriggerEvent
    {
      if (!_initialized)
      {
        Initialize();
      }

      var gameData = _database.Snapshot();
      foreach (var pair in gameData.Triggers)
      {
        if (pair.Value is TriggerData<TEvent> trigger)
        {
          InvokeInternal(pair.Key, trigger, scope, output);
        }
      }
    }

    public void InvokeTriggerId(Scope scope, int triggerId, TriggerOutput? output = null)
    {
      var t = _database.Snapshot().Triggers[triggerId];
      switch (t)
      {
        case TriggerData<WorldTriggerInvokedEvent> worldTrigger:
          InvokeInternal(triggerId, worldTrigger, scope, output, true);
          break;
        case TriggerData<BattleTriggerInvokedEvent> battleTrigger:
          InvokeInternal(triggerId, battleTrigger, scope, output, true);
          break;
        case TriggerData<GlobalTriggerInvokedEvent> globalTrigger:
          InvokeInternal(triggerId, globalTrigger, scope, output, true);
          break;
        default:
          throw new InvalidEnumArgumentException(
            $"Attempted to manually invoke a trigger {t} which was not a valid TriggerInvokedEvent");
      }
    }

    void InvokeInternal<TEvent>(
      int triggerId,
      TriggerData<TEvent> trigger,
      Scope scope,
      TriggerOutput? output = null,
      bool bypassEnabledCheck = false) where TEvent : TriggerEvent
    {
      if (bypassEnabledCheck || !trigger.Disabled)
      {
        var fired = trigger.Invoke(scope, output);
        if (fired && !trigger.Looping)
        {
          _database.Update(TableId.Triggers, triggerId, t => t.WithDisabled(true));
        }
      }
    }
  }
}