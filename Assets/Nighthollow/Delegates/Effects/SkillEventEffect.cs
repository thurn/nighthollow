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
using Nighthollow.Utils;
using UnityEngine;

#nullable enable

namespace Nighthollow.Delegates.Effects
{
  public sealed class SkillEventEffect : Effect
  {
    public enum Event
    {
      Missed,
      Evade,
      Crit,
      Stun
    }

    public SkillEventEffect(Event eventName, CreatureId creature)
    {
      EventName = eventName;
      CreatureId = creature;
    }

    public Event EventName { get; }
    public CreatureId CreatureId { get; }

    public override void Execute(BattleServiceRegistry registry)
    {
      var collider = registry.Creatures.GetCollider(CreatureId);
      switch (EventName)
      {
        case Event.Missed:
          registry.Prefabs.CreateMiss(RandomEffectPoint(collider));
          break;
        case Event.Evade:
          registry.Prefabs.CreateEvade(RandomEffectPoint(collider));
          break;
        case Event.Crit:
          registry.Prefabs.CreateCrit(RandomEffectPoint(collider));
          break;
        case Event.Stun:
          registry.Prefabs.CreateStun(RandomEffectPoint(collider));
          break;
        default:
          throw Errors.UnknownEnumValue(EventName);
      }
    }

    public static Vector3 RandomEffectPoint(Collider2D collider)
    {
      var bounds = collider.bounds;
      return bounds.center + new Vector3(
        (Random.value - 0.5f) * bounds.size.x,
        (Random.value - 0.5f) * bounds.size.y,
        z: 0
      );
    }
  }
}
