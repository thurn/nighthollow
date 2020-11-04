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
using System.Linq;
using Nighthollow.Delegates.Core;
using Nighthollow.Delegates.Effects;
using Nighthollow.Generated;
using Nighthollow.Services;
using Nighthollow.Stats;
using Nighthollow.Utils;

namespace Nighthollow.Delegates.Implementations
{
  public sealed class ApplyToAdjacentAlliesOnUseDelegate : AbstractDelegate
  {
    public override void OnUse(SkillContext c)
    {
      foreach (var creature in
        Root.Instance.CreatureService.GetAdjacentUserCreatures(
          Errors.CheckNotNull(c.Self.RankPosition), c.Self.FilePosition))
      {
        foreach (var modifier in c.Skill.TargetedAffixes.SelectMany(affix => affix.Modifiers))
        {
          if (modifier.DelegateId != null)
          {
            throw new NotSupportedException("Not yet implemented");
          }

          if (modifier.StatModifier != null)
          {
            c.Results.Add(new ApplyModifierEffect(
              creature.Data,
              modifier.StatModifier
                .WithLifetime(new TimedLifetime(c.GetDurationMilliseconds(Stat.BuffDuration)))));
          }
        }
      }
    }
  }
}