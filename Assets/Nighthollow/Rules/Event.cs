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

using System.Collections.Immutable;
using Nighthollow.Services;

#nullable enable

namespace Nighthollow.Rules
{
  public abstract class EventSpec
  {
    public abstract EventName Name { get; }

    /// <summary>
    /// Defines the scope for this event. If null, no scope-checking will be performed for rules using this event.
    /// </summary>
    public abstract ServiceRegistryName? ParentRegistry { get; }

    public abstract Description Describe();
    public abstract ImmutableHashSet<IKey> Bindings();

    public string Snippet() => $"When {Describe().First()}";
  }

  public interface IEvent
  {
    EventSpec GetSpec();

    Scope AddBindings(Scope.Builder builder);
  }
}