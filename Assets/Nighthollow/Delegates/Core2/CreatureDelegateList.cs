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
using Nighthollow.Components;
using Nighthollow.Data;

namespace Nighthollow.Delegates.Core2
{
  public sealed class CreatureDelegateList : AbstractDelegateList<CreatureContext, ICreatureDelegate>, ICreatureDelegate
  {
    public CreatureDelegateList(IReadOnlyList<ICreatureDelegate> delegates) : base(delegates)
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

    public bool CanUseMeleeSkill(CreatureContext context) =>
      GetFirstImplemented(context, (d, c) => d.CanUseMeleeSkill(c));

    public bool CanUseProjectileSkill(CreatureContext context) =>
      GetFirstImplemented(context, (d, c) => d.CanUseProjectileSkill(c));

    public SkillData? SelectSkill(CreatureContext context) =>
      GetFirstImplemented(context, (d, c) => d.SelectSkill(c));

    void ExecuteForCurrentSkill(CreatureContext c, Action<ISkillDelegate, SkillContext> action)
    {

    }
  }
}