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

#nullable enable

using System;
using System.Collections.Generic;
using System.Linq;
using Nighthollow.Components;
using Nighthollow.Data;
using Nighthollow.Generated;

namespace Nighthollow.Delegates.Core
{
  public sealed class CreatureDelegateChain
  {
    readonly List<CreatureDelegateId> _delegateIds;

    IEnumerable<CreatureDelegate> Delegates() => _delegateIds.Select(CreatureDelegateMap.Get);

    public CreatureDelegateChain(List<CreatureDelegateId> delegateIds)
    {
      _delegateIds = delegateIds;
      _delegateIds.Add(CreatureDelegateId.DefaultCreatureDelegate);
    }

    public void OnActivate(Creature self) =>
      Execute(self, (d, c, r) => d.OnActivate(c, r));

    public void OnDeath(Creature self) =>
      Execute(self, (d, c, r) => d.OnDeath(c, r));

    public bool CanUseMeleeSkill(CreatureContext c) => Delegates().Any(d => d.CanUseMeleeSkill(c));

    public bool CanUseProjectileSkill(CreatureContext c) => Delegates().Any(d => d.CanUseProjectileSkill(c));

    public SkillData? SelectSkill(CreatureContext c)
      => _delegateIds
        .Select(id => CreatureDelegateMap.Get(id).SelectSkill(c))
        .FirstOrDefault(skill => skill != null);

    public void OnKilledEnemy(Creature self, Creature enemy, int damageAmount) =>
      Execute(self, (d, c, r) => d.OnKilledEnemy(c, enemy, damageAmount, r));

    void Execute(Creature self, Action<CreatureDelegate, CreatureContext, Results> action)
    {
      var context = new CreatureContext(self);
      var results = new Results();

      foreach (var id in _delegateIds)
      {
        action(CreatureDelegateMap.Get(id), context, results);
      }

      foreach (var effect in results.Values)
      {
        effect.Execute();
      }
    }
  }
}