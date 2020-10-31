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

using System.Collections.Generic;
using Nighthollow.Components;
using Nighthollow.Generated;
using Nighthollow.Stats;
using UnityEngine;

namespace Nighthollow.Delegates.Core
{
  public class AbstractSkillDelegate : AbstractCreatureOrSkillDelegate<SkillContext>, ISkillDelegate
  {
    public virtual void OnStart(SkillContext c)
    {
    }

    public virtual void OnUse(SkillContext c)
    {
    }

    public virtual void OnImpact(SkillContext c)
    {
    }

    public virtual void OnApplyToTarget(SkillContext c, Creature target)
    {
    }

    public virtual Collider2D GetCollider(SkillContext c) =>
      c.MarkNotImplemented<Collider2D>();

    public virtual IEnumerable<Creature> PopulateTargets(SkillContext c) =>
      c.MarkNotImplemented<IEnumerable<Creature>>();

    public virtual IEnumerable<Creature> SelectTargets(SkillContext c, IEnumerable<Creature> hits) =>
      c.MarkNotImplemented<IEnumerable<Creature>>();

    public virtual bool RollForHit(SkillContext c, Creature target) =>
      c.MarkNotImplemented<bool>();

    public virtual bool RollForCrit(SkillContext c, Creature target) =>
      c.MarkNotImplemented<bool>();

    public virtual TaggedValues<DamageType, IntValue> RollForBaseDamage(SkillContext c, Creature target) =>
      c.MarkNotImplemented<TaggedValues<DamageType, IntValue>>();

    public virtual TaggedValues<DamageType, IntValue> ApplyDamageReduction(
      SkillContext c, Creature target, TaggedValues<DamageType, IntValue> damage) =>
      c.MarkNotImplemented<TaggedValues<DamageType, IntValue>>();

    public virtual TaggedValues<DamageType, IntValue> ApplyDamageResistance(
      SkillContext c, Creature target, TaggedValues<DamageType, IntValue> damage) =>
      c.MarkNotImplemented<TaggedValues<DamageType, IntValue>>();

    public virtual int ComputeFinalDamage(
      SkillContext c, Creature target, TaggedValues<DamageType, IntValue> damage, bool isCriticalHit) =>
      c.MarkNotImplemented<int>();

    public virtual int ComputeHealthDrain(SkillContext c, Creature creature, int damageAmount) =>
      c.MarkNotImplemented<int>();

    public virtual bool RollForStun(SkillContext c, Creature target, int damageAmount) =>
      c.MarkNotImplemented<bool>();
  }
}