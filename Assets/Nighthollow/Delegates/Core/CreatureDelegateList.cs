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
using Nighthollow.Delegates.Creatures;
using Nighthollow.Delegates.Effects;

namespace Nighthollow.Delegates.Core
{
  public sealed class CreatureDelegateList : AbstractDelegateList<CreatureContext, ICreatureDelegate>, ICreatureDelegate
  {
    public CreatureDelegateList(IEnumerable<ICreatureDelegate> delegates) :
      base(delegates.Append(new DefaultCreatureDelegate()).ToList())
    {
    }

    public void OnActivate(CreatureContext context)
    {
      ExecuteEvent(context, (d, c) => d.OnActivate(c));
      ExecuteForCurrentSkill(context, (d, c) => d.OnActivate(c));
    }

    public void OnDeath(CreatureContext context)
    {
      ExecuteEvent(context, (d, c) => d.OnDeath(c));
      ExecuteForCurrentSkill(context, (d, c) => d.OnDeath(c));
    }

    public void OnKilledEnemy(CreatureContext context, Creature enemy, int damageAmount)
    {
      ExecuteEvent(context, (d, c) => d.OnKilledEnemy(c, enemy, damageAmount));
      ExecuteForCurrentSkill(context, (d, c) => d.OnKilledEnemy(c, enemy, damageAmount));
    }

    public void OnFiredProjectile(CreatureContext context, FireProjectileEffect effect)
    {
      ExecuteEvent(context, (d, c) => d.OnFiredProjectile(c, effect));
      ExecuteForCurrentSkill(context, (d, c) => d.OnFiredProjectile(c, effect));
    }

    public void OnHitTarget(CreatureContext context, SkillData skill, Creature target)
    {
      ExecuteEvent(context, (d, c) => d.OnHitTarget(c, skill, target));
      ExecuteForCurrentSkill(context, (d, c) => d.OnHitTarget(c, skill, target));
    }

    public bool ProjectileCouldHit(CreatureContext context) =>
      AnyReturnedTrue(context, (d, c) => d.ProjectileCouldHit(c));

    public bool MeleeCouldHit(CreatureContext context) =>
      GetFirstImplemented(context, (d, c) => d.MeleeCouldHit(c));

    public SkillData? SelectSkill(CreatureContext context) =>
      GetFirstImplemented(context, (d, c) => d.SelectSkill(c));

    void ExecuteForCurrentSkill(CreatureContext c, Action<ISkillDelegate, SkillContext> action)
    {
      var skill = c.Self.CurrentSkill;
      if (skill != null)
      {
        action(skill.Delegate, new SkillContext(c.Self, skill));
      }
    }
  }
}