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
        foreach (var pair in _database.Snapshot().Triggers.Where(pair => pair.Value.Disabled))
        {
          _database.Update(TableId.Triggers, pair.Key, t => t.WithDisabled(false));
        }
      }

      _initialized = true;
    }

    public void Invoke<TEvent>(TEvent triggerEvent) where TEvent : TriggerEvent
    {
      if (!_initialized)
      {
        Initialize();
      }

      var gameData = _database.Snapshot();
      foreach (var pair in gameData.Triggers)
      {
        if (!pair.Value.Disabled && pair.Value is TriggerData<TEvent> trigger)
        {
          var fired = trigger.Invoke(triggerEvent);
          if (fired && !trigger.Looping)
          {
            _database.Update(TableId.Triggers, pair.Key, t => t.WithDisabled(true));
          }
        }
      }
    }

    public void InvokeTriggerId(TriggerInvokedEvent triggerEvent, int triggerId)
    {
      var trigger = _database.Snapshot().Triggers[triggerId] as TriggerData<TriggerInvokedEvent>;
      if (trigger == null)
      {
        throw new InvalidEnumArgumentException(
          $"Attempted to manually invoke a trigger {trigger} which was not of type TriggerInvokedEvent");
      }

      if (!trigger.Disabled)
      {
        var fired = trigger.Invoke(triggerEvent);
        if (fired && !trigger.Looping)
        {
          _database.Update(TableId.Triggers, triggerId, t => t.WithDisabled(true));
        }
      }
    }
  }
}