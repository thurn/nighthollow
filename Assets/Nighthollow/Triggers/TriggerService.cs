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
      if (_registry.Globals.GetBool(GlobalId.ShouldAutoenableTriggers))
      {
        foreach (var pair in _registry.Database.Snapshot().Triggers.Where(pair => pair.Value.Disabled))
        {
          _registry.Database.Update(TableId.Triggers, pair.Key, t => t.WithDisabled(false));
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
      
      var gameData = _registry.Database.Snapshot();
      foreach (var pair in gameData.Triggers)
      {
        if (!pair.Value.Disabled && pair.Value is TriggerData<TEvent> trigger)
        {
          var fired = trigger.Invoke(triggerEvent, _registry);
          if (fired && !trigger.Looping)
          {
            _registry.Database.Update(TableId.Triggers, pair.Key, t => t.WithDisabled(true));
          }
        }
      }
    }
  }
}