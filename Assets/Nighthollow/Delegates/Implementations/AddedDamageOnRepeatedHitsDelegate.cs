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

using Nighthollow.Components;
using Nighthollow.Data;
using Nighthollow.Delegates.Core;
using Nighthollow.Delegates.Effects;
using Nighthollow.Generated;
using Nighthollow.State;
using Nighthollow.Stats;

namespace Nighthollow.Delegates.Implementations
{
  public sealed class AddedDamageOnRepeatedHitsDelegate : AbstractDelegate
  {
    public override void OnHitTarget(SkillContext c, Creature target, int damage)
    {
      var lastHit = c.Skill.Values.Get(Key.LastCreatureHit);
      if (lastHit && lastHit == target)
      {
        c.Results.Add(new MutateStateEffect(c.Skill, new IncrementIntegerMutation(Key.TimesHit)));
      }
      else
      {
        c.Results.Add(new MutateStateEffect(c.Skill, SetValueMutation.New(Key.LastCreatureHit, target)));
        c.Results.Add(new MutateStateEffect(c.Skill, SetValueMutation.New(Key.TimesHit, 1)));
      }
    }

    public override TaggedValues<DamageType, IntValue> TransformDamage(
      SkillContext c, Creature target, TaggedValues<DamageType, IntValue> damage)
    {
      var lastHit = c.Skill.Values.Get(Key.LastCreatureHit);
      if (lastHit && lastHit == target)
      {
        return DamageUtil.Add(damage, DamageUtil.Multiply(
          c.Skill.Values.Get(Key.TimesHit),
          DamageUtil.RollForDamage(c.Skill.Stats.Get(Stat.SameTargetAddedDamage))));
      }
      else
      {
        return damage;
      }
    }
  }
}