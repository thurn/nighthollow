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
using System.Linq;
using Nighthollow.Delegates.Effects;
using Nighthollow.Delegates.Handlers;
using Nighthollow.Services;
using Nighthollow.Stats;

#nullable enable

namespace Nighthollow.Delegates.Implementations
{
  public sealed class ApplyStatusEffectsOnHitDelegate : AbstractDelegate, IOnApplySkillToTarget
  {
    public override string Describe(IStatDescriptionProvider provider) => "Curses Enemies on Hit With:";

    public IEnumerable<Effect> OnApplySkillToTarget(
      GameContext c, int delegateIndex, IOnApplySkillToTarget.Data d) =>
      d.Skill.ItemData.StatusEffects
        .Select(statusEffect => new ApplyStatusEffectEffect(d.Target.Creature, statusEffect));
  }
}