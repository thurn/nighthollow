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

using MessagePack;
using Nighthollow.Services;
using Nighthollow.Triggers.Effects;

#nullable enable

namespace Nighthollow.Triggers
{
  [Union(0, typeof(DisplayHelpTextEffect))]
  public interface IEffect
  {
    string Description { get; }
  }

  public interface IEffect<in TEvent> : IEffect where TEvent : TriggerEvent
  {
    void Execute(TEvent triggerEvent, ServiceRegistry registry);
  }
}