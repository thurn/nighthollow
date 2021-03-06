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

using Nighthollow.Services;
using UnityEngine;

#nullable enable

namespace Nighthollow.Delegates.Effects
{
  public sealed class MarkSkillUsedEffect : Effect
  {
    public MarkSkillUsedEffect(CreatureId creatureId, int skillId)
    {
      CreatureId = creatureId;
      SkillId = skillId;
    }

    public CreatureId CreatureId { get; }
    public int SkillId { get; }

    public override void Execute(BattleServiceRegistry registry)
    {
      registry.CreatureController.Mutate(
        CreatureId,
        s => s.WithSkillLastUsedTimes(s.SkillLastUsedTimes.SetItem(SkillId, Time.time)));
    }
  }
}
