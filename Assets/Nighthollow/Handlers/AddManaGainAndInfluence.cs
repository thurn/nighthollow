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
using Nighthollow.Events;
using Nighthollow.Services;
using System;
using UnityEngine;

namespace Nighthollow.Handlers
{
  [CreateAssetMenu(menuName = "Handlers/AddManaGainAndInfluence")]
  public class AddManaGainAndInfluence : CreatureEventHandler
  {
    public override void Execute(Creature creature)
    {
      var user = Root.Instance.User;
      user.AddManaGain(Modifier.WhileAlive(Operator.Add, creature.Data.Parameters.ManaGained.Value, creature));
      foreach (School school in Enum.GetValues(typeof(School)))
      {
        var influence = creature.Data.Parameters.InfluenceAdded.Get(school).Value;
        if (influence > 0)
        {
          user.Influence.Get(school).AddModifier(Modifier.WhileAlive(Operator.Add, influence, creature));
        }
      }
    }
  }
}
