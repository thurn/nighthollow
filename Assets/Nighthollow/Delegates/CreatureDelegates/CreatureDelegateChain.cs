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

using System;
using System.Collections.Generic;
using Nighthollow.Components;
using Nighthollow.Data;
using Nighthollow.Generated;
using SkillData = Nighthollow.Model.SkillData;

namespace Nighthollow.Delegates.CreatureDelegates
{
  public sealed class CreatureDelegateChain
  {
    readonly List<CreatureDelegateId> _delegateIds;

    public CreatureDelegateChain(List<CreatureDelegateId> delegateIds)
    {
      _delegateIds = delegateIds;
      _delegateIds.Add(CreatureDelegateId.DefaultCreatureDelegate);
    }

    public void OnActivate(Creature self) =>
      Execute(self, (d, c) => d.OnActivate(c));

    public void OnDeath(Creature self) =>
      Execute(self, (d, c) => d.OnDeath(c));

    public Optional<SkillData> SelectSkill(Creature self)
    {
      foreach (var id in _delegateIds)
      {
        var skill = CreatureDelegateMap.Get(id).SelectSkill(self);
        if (skill.HasValue)
        {
          return skill;
        }
      }

      return Optional<SkillData>.None();
    }

    public void OnKilledEnemy(Creature self, Creature enemy, int damageAmount) =>
      Execute(self, (d, c) => d.OnKilledEnemy(c, enemy, damageAmount));

    void Execute(Creature self, Action<CreatureDelegate, CreatureContext> action)
    {
      var context = new CreatureContext(self);

      foreach (var id in _delegateIds)
      {
        action(CreatureDelegateMap.Get(id), context);
      }

      foreach (var effect in context.Results.Values)
      {
        effect.Execute(self);
      }

      foreach (var effect in context.Results.Values)
      {
        effect.InvokeEvent(this, self);
      }
    }
  }
}