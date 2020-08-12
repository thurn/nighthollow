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
    [SerializeField] BoxCollider2D _collider;

    public BoxCollider2D Collider => _collider;

    public static CustomTriggerCollider Add(Creature creature, Vector2 offset, Vector2 size)
    {
      var trigger = new GameObject(creature.name + "Trigger")
      {
        layer = Constants.LayerForCreatures(creature.Owner)
      };

      trigger.transform.SetParent(creature.transform, worldPositionStays: false);
      var result = trigger.AddComponent<CustomTriggerCollider>();
      result._creature = creature;

      var body = trigger.AddComponent<Rigidbody2D>();
      body.mass = 1f;
      body.gravityScale = 0f;

      var collider = trigger.AddComponent<BoxCollider2D>();
      collider.isTrigger = true;
      collider.offset = creature.transform.InverseTransformVector(offset) +
        ((Vector3)creature.GetComponent<BoxCollider2D>().offset);
      collider.size = creature.transform.InverseTransformVector(size);
      result._collider = collider;
      return result;
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