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

using System.Collections.Generic;
using Nighthollow.Components;
using Nighthollow.Generated;
using Nighthollow.Stats;

namespace Nighthollow.Delegates.SkillDelegates
{
  public sealed class SkillDelegateChain
  {
    readonly List<SkillDelegateId> _delegateIds;

    public SkillDelegateChain(List<SkillDelegateId> delegateIds)
    {
      _delegateIds = delegateIds;
      _delegateIds.Add(SkillDelegateId.DefaultSkillDelegate);
    }

    public void OnStart(SkillContext c)
    {
    }

    public void OnUse(SkillContext c)
    {
    }

    public void OnImpact(SkillContext c)
    {
    }

    public void PopulateTargets(SkillContext c, List<Creature> targets)
    {
    }

    public void ApplyToTarget(SkillContext c, Creature target, Results<TargetedSkillEffect> results)
    {
    }

    public bool CheckForHit(SkillContext c, Creature target) => false;

    public bool CheckForCrit(SkillContext c, Creature target) => false;

    public int ComputeDamage(SkillContext c, Creature target, TaggedIntsStat<DamageType> damage) => 0;

    public int ComputeCritDamage(SkillContext c, Creature target, TaggedIntsStat<DamageType> damage) => 0;

    public int ComputeLifeDrain(SkillContext c, Creature creature, int damageAmount) => 0;

    public bool CheckForStun(SkillContext c, Creature target, int damageAmount) => false;
  }
}