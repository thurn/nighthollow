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
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Nighthollow.Modifiers
{
  [Serializable]
  public sealed class ModifierList
  {
    [SerializeField] List<CreatureModifier> _modifiers;

    public void Activate(Creature self)
    {
      foreach (var m in _modifiers)
      {
        m.Activate(self);
      }
    }

    public void OnDeath(Creature self)
    {
      foreach (var m in _modifiers)
      {
        m.OnDeath(self);
      }
    }

    public ModifierList Clone()
    {
      return new ModifierList
      {
        _modifiers = _modifiers?.Select(m => m.Clone()).ToList()
      };
    }
  }
}
