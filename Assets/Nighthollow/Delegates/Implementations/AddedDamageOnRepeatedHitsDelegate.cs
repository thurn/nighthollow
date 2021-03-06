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
using System.Collections.Immutable;
using Nighthollow.Data;
using Nighthollow.Delegates.Effects;
using Nighthollow.Delegates.Handlers;
using Nighthollow.Services;
using Nighthollow.State;
using Nighthollow.Stats;

#nullable enable

namespace Nighthollow.Delegates.Implementations
{
  public sealed class AddedDamageOnRepeatedHitsDelegate : IDelegate, IOnHitTarget, ITransformDamage
  {
    public string Describe(IStatDescriptionProvider provider) =>
      $"+{provider.Get(Stat.SameTargetAddedDamage)} Damage for Each Hit on the Same Target";

    public IEnumerable<Effect> OnHitTarget(IGameContext c, IOnHitTarget.Data d)
    {
      if (c[d.Self].Data.KeyValueStore.TryGet(Key.LastCreatureHit, out var lastHit) && lastHit == d.Target)
      {
        yield return new MutateCreatureStateEffect(d.Self,
          new IncrementIntegerMutation(Key.SequentialHitCount));
      }
      else
      {
        yield return new MutateCreatureStateEffect(d.Self, Key.LastCreatureHit.Set(d.Target));
        yield return new MutateCreatureStateEffect(d.Self, Key.SequentialHitCount.Set(1));
      }
    }

    public ImmutableDictionary<DamageType, int> TransformDamage(
      IGameContext c, ITransformDamage.Data d, ImmutableDictionary<DamageType, int> current)
    {
      if (c[d.Self].Data.KeyValueStore.TryGet(Key.LastCreatureHit, out var lastHit) && lastHit == d.Target)
      {
        return DamageUtil.Add(current, DamageUtil.Multiply(
          c[d.Self].Data.KeyValueStore.Get(Key.SequentialHitCount),
          DamageUtil.RollForDamage(d.Skill.Stats.Get(Stat.SameTargetAddedDamage))));
      }
      else
      {
        return current;
      }
    }
  }
}