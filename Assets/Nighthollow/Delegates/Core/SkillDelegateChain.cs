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
    sealed class DelegateWithContext
    {
      public SkillDelegate Delegate { get; }
      public SkillContext Context { get; }

      public DelegateWithContext(SkillDelegate skillDelegate, SkillContext context, int delegateIndex)
      {
        Delegate = skillDelegate;
        Context = context.WithIndex(delegateIndex);
      }
    }

    readonly List<SkillDelegateId> _delegateIds;

    IEnumerable<DelegateWithContext> Delegates(SkillContext c) =>
      _delegateIds.Select(SkillDelegateMap.Get).Select((d, index) => new DelegateWithContext(d, c, index));

    public SkillDelegateChain(List<SkillDelegateId> delegateIds)
    {
      _delegateIds = delegateIds;
      _delegateIds.Add(SkillDelegateId.DefaultSkillDelegate);
    }

    public void OnStart(SkillContext c) => Execute(c, (d, r) => d.Delegate.OnStart(d.Context, r));

    public void OnUse(SkillContext c) => Execute(c, (d, r) => d.Delegate.OnUse(d.Context, r));

    public void OnImpact(SkillContext c) => Execute(c, (d, r) => d.Delegate.OnImpact(d.Context, r));

    public Collider2D? GetCollider(SkillContext c)
      => Delegates(c)
        .Select(d => d.Delegate.GetCollider(d.Context))
        .FirstOrDefault(collider => (bool) collider);

    public IReadOnlyList<Creature> PopulateTargets(SkillContext c)
    {
      var result = new List<Creature>();
      foreach (var d in Delegates(c))
      {
        d.Delegate.PopulateTargets(d.Context, result);
      }

      return result;
    }

    public IEnumerable<Creature> SelectTargets(SkillContext c, IEnumerable<Creature> hits) =>
      FirstNotNull(c, d => d.Delegate.SelectTargets(d.Context, hits));

    public void ApplyToTarget(SkillContext c, Creature target, Results results)
    {
      foreach (var d in Delegates(c))
      {
        d.Delegate.ApplyToTarget(d.Context, target, results);
      }
    }

    public bool RollForHit(SkillContext c, Creature target)
      => Delegates(c).Any(d => d.Delegate.RollForHit(d.Context, target));

    public bool RollForCrit(SkillContext c, Creature target)
      => Delegates(c).Any(d => d.Delegate.RollForCrit(d.Context, target));

    public TaggedValues<DamageType, IntValue> RollForBaseDamage(SkillContext c, Creature target) =>
      FirstNotNull(c, d => d.Delegate.RollForBaseDamage(d.Context, target));

    public TaggedValues<DamageType, IntValue> ApplyDamageReduction(
      SkillContext c,
      Creature target,
      TaggedValues<DamageType, IntValue> damage) =>
      FirstNotNull(c, d => d.Delegate.ApplyDamageReduction(d.Context, target, damage));

    public TaggedValues<DamageType, IntValue> ApplyDamageResistance(
      SkillContext c,
      Creature target,
      TaggedValues<DamageType, IntValue> damage) =>
      FirstNotNull(c, d => d.Delegate.ApplyDamageResistance(d.Context, target, damage));

    public int ComputeFinalDamage(SkillContext c,
      Creature target,
      TaggedValues<DamageType, IntValue> damage,
      bool isCriticalHit) =>
      FirstNotNull(c, d => d.Delegate.ComputeFinalDamage(d.Context, target, damage, isCriticalHit));

    public int ComputeHealthDrain(SkillContext c, Creature target, int damageAmount)
      => Delegates(c).Sum(d => d.Delegate.ComputeHealthDrain(d.Context, target, damageAmount));

    public bool CheckForStun(SkillContext c, Creature target, int damageAmount)
      => Delegates(c).Any(d => d.Delegate.RollForStun(d.Context, target, damageAmount));

    void Execute(SkillContext c, Action<DelegateWithContext, Results> action)
    {
      var results = new Results();

      foreach (var d in Delegates(c))
      {
        action(d, results);
      }

      foreach (var effect in results.Values)
      {
        effect.Execute();
      }

      foreach (var effect in results.Values)
      {
        effect.RaiseEvents();
      }
    }

    T FirstNotNull<T>(SkillContext c, Func<DelegateWithContext, T?> function) where T : class
      => Errors.CheckNotNull(Delegates(c).Select(function).SkipWhile(a => a == null).First());

    T FirstNotNull<T>(SkillContext c, Func<DelegateWithContext, T?> function) where T : struct
      => Errors.CheckNotNull(Delegates(c).Select(function).SkipWhile(a => a == null).First());
  }
}