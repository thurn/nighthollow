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

#nullable enable

namespace Nighthollow.Triggers.Events
{
  public sealed class WorldSceneReadyEvent : IEvent
  {
    public static Description Describe => new Description("the world scene is loaded");

    public EventType Type => EventType.WorldSceneReady;

    public Scope AddBindings(Scope.Builder builder) => builder.Build();
  }

  public sealed class BattleSceneReadyEvent : IEvent
  {
    public static Description Describe => new Description("the battle scene is loaded");

    public EventType Type => EventType.BattleSceneReady;

    public Scope AddBindings(Scope.Builder builder) => builder.Build();
  }

  public sealed class SchoolSelectionSceneReadyEvent : IEvent
  {
    public static Description Describe => new Description("the school selection scene is loaded");

    public EventType Type => EventType.SchoolSelectionSceneReady;

    public Scope AddBindings(Scope.Builder builder) => builder.Build();
  }
}