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
using Nighthollow.Data;
using Nighthollow.Services;
using UnityEngine;

namespace Nighthollow.Delegate
{
  [CreateAssetMenu(menuName = "Delegate/ManaGeneratorDelegate")]
  public sealed class ManaGeneratorDelegate : AbstractCreatureDelegate
  {
    [SerializeField] int _manaGain;
    [SerializeField] Influence _influence;

    public override void OnActivate(Creature self)
    {
      Parent.OnActivate(self);

      var user = Root.Instance.User;
      user.Data.ManaGain.AddModifier(Modifier.WhileAlive(Operator.Add, _manaGain, self));

      foreach (var school in Influence.AllSchools)
      {
        var influence = _influence.Get(school).Value;
        if (influence > 0)
        {
          user.Influence.Get(school)
            .AddModifier(Modifier.WhileAlive(Operator.Add, influence, self));
        }
      }
    }
  }
}