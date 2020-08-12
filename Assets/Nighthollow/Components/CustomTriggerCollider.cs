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

using Nighthollow.Utils;
using UnityEngine;

namespace Nighthollow.Components
{
  public sealed class CustomTriggerCollider : MonoBehaviour
  {
    [SerializeField] Creature _creature;

    public static void Add(Creature creature, Vector2 offset, Vector2 size)
    {
      var trigger = new GameObject(creature.name + "Trigger")
      {
        layer = Constants.LayerForCreatures(creature.Owner)
      };

      trigger.transform.SetParent(creature.transform, worldPositionStays: false);
      trigger.AddComponent<CustomTriggerCollider>()._creature = creature;

      var body = trigger.AddComponent<Rigidbody2D>();
      body.mass = 1f;
      body.gravityScale = 0f;

      var collide = trigger.AddComponent<BoxCollider2D>();
      collide.isTrigger = true;
      collide.offset = creature.transform.InverseTransformVector(offset) +
        ((Vector3)creature.GetComponent<BoxCollider2D>().offset);
      collide.size = creature.transform.InverseTransformVector(size);
    }

    void OnTriggerEnter2D(Collider2D other)
    {
      var enemy = other.GetComponent<Creature>();
      if (enemy)
      {
        _creature.OnCustomTriggerCollision(enemy);
      }
    }
  }
}