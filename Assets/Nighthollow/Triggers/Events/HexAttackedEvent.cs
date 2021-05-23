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
  /// Fired when the user initiates an attack on a hex
  /// </summary>
  public sealed class HexAttackedEvent : IEvent
  {
    public static Description Describe => new Description("the user attacks a hex");

    public HexAttackedEvent(int hexId)
    {
      HexId = hexId;
    }

    public int HexId { get; }

    public EventType Type => EventType.HexAttacked;

    public Scope AddBindings(Scope.Builder builder) => builder.AddValueBinding(Key.Hex, HexId).Build();
  }
}