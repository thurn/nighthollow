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

namespace Nighthollow.Rules.Events
{
  public sealed class DrewOpeningHandEvent : IEvent
  {
    public static readonly Spec Specification = new Spec();

    public sealed class Spec : EventSpec
    {
      public override EventName Name => EventName.DrewOpeningHand;

      public override ServiceRegistryName? ParentRegistry => ServiceRegistryName.Battle;

      public override Description Describe() => new Description("an opening hand is drawn");

      public override ImmutableHashSet<IKey> Bindings() => ImmutableHashSet<IKey>.Empty;
    }

    public EventSpec GetSpec() => Specification;

    public Scope AddBindings(Scope.Builder builder) => builder.Build();
  }
}