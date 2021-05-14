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

using Nighthollow.Services;

#nullable enable

namespace Nighthollow.Triggers.Events
{
  public sealed class GlobalTriggerInvokedEvent : TriggerEvent
  {
    public static Description Describe => new Description("this global trigger is manually invoked");

    public GlobalTriggerInvokedEvent(ServiceRegistry registry) : base(registry)
    {
    }
  }

  public sealed class WorldTriggerInvokedEvent : WorldEvent
  {
    public static Description Describe => new Description("this world trigger is manually invoked");

    public WorldTriggerInvokedEvent(WorldServiceRegistry registry) : base(registry)
    {
    }
  }

  public sealed class BattleTriggerInvokedEvent : BattleEvent
  {
    public static Description Describe => new Description("this battle trigger is manually invoked");

    public BattleTriggerInvokedEvent(BattleServiceRegistry registry) : base(registry)
    {
    }
  }
}