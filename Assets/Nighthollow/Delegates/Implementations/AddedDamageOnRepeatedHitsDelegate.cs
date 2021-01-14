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


using System.Collections.Immutable;
using Nighthollow.Components;
using Nighthollow.Data;
using Nighthollow.Delegates.Core;
using Nighthollow.Delegates.Effects;
using Nighthollow.State;
using Nighthollow.Stats;

#nullable enable

namespace Nighthollow.Delegates.Implementations
{
  public sealed class AddedDamageOnRepeatedHitsDelegate : AbstractDelegate
  {
    public override string Describe(IStatDescriptionProvider provider) =>
      $"+{provider.Get(Stat.SameTargetAddedDamage)} Damage for Each Hit on the Same Target";

    public override void OnHitTarget(SkillContext c, Creature target, int damage)
    {
      if (c.Self.Data.KeyValueStore.TryGet(Key.LastCreatureHit, out var lastHit) && lastHit == target)
      {
        c.Results.Add(new MutateStateEffect(c.Self, new IncrementIntegerMutation(Key.SequentialHitCount)));
      }
      else
      {
        c.Results.Add(new MutateStateEffect(c.Self, Key.LastCreatureHit.Set(target)));
        c.Results.Add(new MutateStateEffect(c.Self, Key.SequentialHitCount.Set(1)));
      }
    }

    public override ImmutableDictionary<DamageType, int> TransformDamage(
      SkillContext c, Creature target, ImmutableDictionary<DamageType, int> damage)
    {
      if (c.Self.Data.KeyValueStore.TryGet(Key.LastCreatureHit, out var lastHit) && lastHit == target)
      {
        return DamageUtil.Add(damage, DamageUtil.Multiply(
          c.Self.Data.KeyValueStore.Get(Key.SequentialHitCount),
          DamageUtil.RollForDamage(c.Skill.Stats.Get(Stat.SameTargetAddedDamage))));
      }
      else
      {
        return damage;
      }
    }
  }
}
