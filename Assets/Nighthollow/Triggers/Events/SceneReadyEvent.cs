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
  /// <summary>
  /// Fired whenever a new scene is ready, i.e. all of its startup data fetching is complete.
  /// </summary>
  public sealed class SceneReadyEvent : TriggerEvent
  {
    public enum Name
    {
      Unknown = 0,
      World = 1,
      Battle = 2
    }

    public SceneReadyEvent(Name sceneName)
    {
      SceneName = sceneName;
    }

    public Name SceneName { get; }

    public static string Description => "Whenever a new scene is ready";
  }
}