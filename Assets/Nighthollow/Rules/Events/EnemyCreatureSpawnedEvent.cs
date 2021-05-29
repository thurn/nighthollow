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
  public sealed class EnemyCreatureSpawnedEvent : IEvent
  {
    public static readonly Spec Specification = new Spec();

    public sealed class Spec : EventSpec
    {
      public override EventName Game => EventName.EnemyCreatureSpawned;

      public override Description Describe() => new Description("an enemy creature is spawned");

      public override ImmutableHashSet<IKey> Bindings() => ImmutableHashSet.Create<IKey>(Key.Creature);
    }

    public EnemyCreatureSpawnedEvent(CreatureId creatureId)
    {
      Self = creatureId;
    }

    public EventSpec GetSpec() => Specification;

    public CreatureId Self { get; }

    public Scope AddBindings(Scope.Builder builder) => builder.AddValueBinding(Key.Creature, Self).Build();
  }
}
