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


using Nighthollow.Components;
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

    public SkillEventEffect(Event eventName, Creature creature)
    {
      EventName = eventName;
      Creature = creature;
    }

    public Event EventName { get; }
    public Creature Creature { get; }

    public override void Execute()
    {
      switch (EventName)
      {
        case Event.Missed:
          Root.Instance.Prefabs.CreateMiss(RandomEffectPoint(Creature));
          break;
        case Event.Evade:
          Root.Instance.Prefabs.CreateEvade(RandomEffectPoint(Creature));
          break;
        case Event.Crit:
          Root.Instance.Prefabs.CreateCrit(RandomEffectPoint(Creature));
          break;
        case Event.Stun:
          Root.Instance.Prefabs.CreateStun(RandomEffectPoint(Creature));
          break;
        default:
          throw Errors.UnknownEnumValue(EventName);
      }
    }

    public static Vector3 RandomEffectPoint(Creature creature)
    {
      var bounds = creature.Collider.bounds;
      return bounds.center + new Vector3(
        (Random.value - 0.5f) * bounds.size.x,
        (Random.value - 0.5f) * bounds.size.y,
        z: 0
      );
    }
  }
}
