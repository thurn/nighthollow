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

using Nighthollow.Components;
using Nighthollow.Delegates.Core;
using Nighthollow.Delegates.Effects;

#nullable enable

namespace Nighthollow.Delegates.Implementations
{
  public sealed class ApplyStatusEffectsOnHitDelegate : AbstractDelegate
  {
    public override string Describe(IStatDescriptionProvider provider) => "Curses Enemies on Hit With:";

    public override void OnApplyToTarget(SkillContext c, Creature target)
    {
      foreach (var statusEffect in c.Skill.ItemData.StatusEffects)
      {
        c.Results.Add(new ApplyStatusEffectEffect(target, statusEffect));
      }
    }
  }
}