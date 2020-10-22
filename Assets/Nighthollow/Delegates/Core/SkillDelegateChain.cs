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
using Nighthollow.Generated;
using Nighthollow.Stats;
using Nighthollow.Utils;
using UnityEngine;

namespace Nighthollow.Delegates.Core
{
  public sealed class SkillDelegateChain
  {
    readonly List<SkillDelegateId> _delegateIds;

    IEnumerable<SkillDelegate> Delegates() => _delegateIds.Select(SkillDelegateMap.Get);

    public SkillDelegateChain(List<SkillDelegateId> delegateIds)
    {
      _delegateIds = delegateIds;
      _delegateIds.Add(SkillDelegateId.DefaultSkillDelegate);
    }

    public void OnStart(SkillContext c) => Execute(c, (d, r) => d.OnStart(c, r));

    public void OnUse(SkillContext c) => Execute(c, (d, r) => d.OnUse(c, r));

    public void OnImpact(SkillContext c) => Execute(c, (d, r) => d.OnImpact(c, r));

    public Collider2D? GetCollider(SkillContext c)
      => Delegates()
        .Select(d => d.GetCollider(c))
        .FirstOrDefault(collider => (bool) collider);

    public IReadOnlyList<Creature> PopulateTargets(SkillContext c)
    {
      var result = new List<Creature>();
      foreach (var d in Delegates())
      {
        d.PopulateTargets(c, result);
      }

      return result;
    }

    public void ApplyToTarget(SkillContext c, Creature target, Results results)
    {
      foreach (var d in Delegates())
      {
        d.ApplyToTarget(c, target, results);
      }
    }

    public bool RollForHit(SkillContext c, Creature target)
      => Delegates().Any(d => d.RollForHit(c, target));

    public bool RollForCrit(SkillContext c, Creature target)
      => Delegates().Any(d => d.RollForCrit(c, target));

    public TaggedStatListValue<DamageType, IntValue, IntStat> RollForBaseDamage(SkillContext c, Creature target) =>
      Errors.CheckNotNull(Delegates().Select(d => d.RollForBaseDamage(c, target)).First());

    public TaggedStatListValue<DamageType, IntValue, IntStat> ApplyDamageReduction(
      SkillContext c,
      Creature target,
      TaggedStatListValue<DamageType, IntValue, IntStat> damage) =>
      Errors.CheckNotNull(Delegates().Select(d => d.ApplyDamageReduction(c, target, damage)).First());

    public TaggedStatListValue<DamageType, IntValue, IntStat> ApplyDamageResistance(
      SkillContext c,
      Creature target,
      TaggedStatListValue<DamageType, IntValue, IntStat> damage) =>
      Errors.CheckNotNull(Delegates().Select(d => d.ApplyDamageResistance(c, target, damage)).First());

    public int ComputeFinalDamage(SkillContext c,
      Creature target,
      TaggedStatListValue<DamageType, IntValue, IntStat> damage,
      bool isCriticalHit) =>
      Errors.CheckNotNull(Delegates().Select(d => d.ComputeFinalDamage(c, target, damage, isCriticalHit)).First());

    public int ComputeHealthDrain(SkillContext c, Creature target, int damageAmount)
      => Delegates().Sum(d => d.ComputeHealthDrain(c, target, damageAmount));

    public bool CheckForStun(SkillContext c, Creature target, int damageAmount)
      => Delegates().Any(d => d.CheckForStun(c, target, damageAmount));

    void Execute(SkillContext c, Action<SkillDelegate, Results> action)
    {
      var results = new Results();

      foreach (var id in _delegateIds)
      {
        action(SkillDelegateMap.Get(id), results);
      }

      foreach (var effect in results.Values)
      {
        effect.Execute();
      }
    }
  }
}