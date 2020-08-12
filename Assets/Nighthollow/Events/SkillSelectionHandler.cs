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
using UnityEngine;

namespace Nighthollow.Events
{
  public abstract class SkillSelectionHandler : ScriptableObject
  {
    /// <summary>Invoked when this creature is first activated
    public virtual void OnActivate(Creature creature) { }

    /// <summary>
    /// Invoked when this creature's box collider hits another creature while it is idle.
    /// </summary>
    public abstract void OnCollision(Creature source, Creature other);

    /// <summary>
    /// Invoked when this creature's custom trigger collider hits another creature
    /// </summary>
    public abstract void OnCustomTriggerCollision(Creature source, Creature other);

    /// <summary>Invoked when this creature is first created or finishes a skill.</summary>
    public abstract void OnSkillCompleted(Creature creature);
  }
}