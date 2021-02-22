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

using System.Collections.Generic;
using Nighthollow.Services;

#nullable enable

namespace Nighthollow.Delegates.Effects
{
  public sealed class EventEffect<THandler> : Effect where THandler : IHandler
  {
    public EventEffect(EventData<THandler> eventData)
    {
      EventData = eventData;
    }

    public EventData<THandler> EventData { get; }

    public override void Execute(GameServiceRegistry registry)
    {
    }

    public override IEnumerable<IEventData> Events()
    {
      yield return EventData;
    }
  }
}
