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
using Nighthollow.Services;
using Nighthollow.Stats;

#nullable enable

namespace Nighthollow.Data
{
  public sealed partial class CreatureState : StatEntity
  {
    public CreatureState(
      CreatureId creatureId,
      CreatureData data,
      PlayerName owner,
      RankValue? rankPosition = null,
      FileValue? filePosition = null,
      int damageTaken = 0,
      SkillData? currentSkill = null,
      bool isAlive = true,
      bool isStunned = false,
      ImmutableDictionary<int, float>? skillLastUsedTimes = null)
    {
      CreatureId = creatureId;
      Data = data;
      Owner = owner;
      RankPosition = rankPosition;
      FilePosition = filePosition;
      DamageTaken = damageTaken;
      CurrentSkill = currentSkill;
      IsAlive = isAlive;
      IsStunned = isStunned;
      SkillLastUsedTimes = skillLastUsedTimes ?? ImmutableDictionary<int, float>.Empty;
    }

    [Field] public CreatureId CreatureId { get; }
    [Field] public CreatureData Data { get; }
    [Field] public override StatTable Stats => Data.Stats;
    [Field] public PlayerName Owner { get; }
    [Field] public RankValue? RankPosition { get; }
    [Field] public FileValue? FilePosition { get; }
    [Field] public int DamageTaken { get; }
    [Field] public SkillData? CurrentSkill { get; }
    [Field] public bool IsAlive { get; }
    [Field] public bool IsStunned { get; }
    [Field] public ImmutableDictionary<int, float> SkillLastUsedTimes { get; }

    /// <summary>
    ///   Returns the timestamp at which the provided skill was last used by this creature, or 0 if it has never been used
    /// </summary>
    public float? SkillLastUsedTimeSeconds(int skillId) =>
      SkillLastUsedTimes.ContainsKey(skillId) ? (float?) SkillLastUsedTimes[skillId] : null;
  }
}
