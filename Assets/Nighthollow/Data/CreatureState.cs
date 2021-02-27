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
using Nighthollow.Delegates;
using Nighthollow.Services;
using Nighthollow.Stats;
using Nighthollow.Utils;
using UnityEngine;

#nullable enable

namespace Nighthollow.Data
{
  public sealed partial class CreatureState : StatEntity, IHasDelegateList
  {
    public CreatureState(
      CreatureId creatureId,
      CreatureData data,
      PlayerName owner,
      RankValue? rankPosition = null,
      FileValue? filePosition = null,
      int damageTaken = 0,
      SkillData? currentSkill = null,
      ImmutableDictionary<int, float>? skillLastUsedTimes = null)
    {
      CreatureId = creatureId;
      Data = data;
      Owner = owner;
      RankPosition = rankPosition;
      FilePosition = filePosition;
      DamageTaken = damageTaken;
      CurrentSkill = currentSkill;
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
    [Field] public ImmutableDictionary<int, float> SkillLastUsedTimes { get; }

    public Vector2 GetProjectileSourcePosition(GameContext c) =>
      c.CreatureService.GetCreature(CreatureId).ProjectileSource.position;

    /// <summary>
    ///   Returns the timestamp at which the provided skill was last used by this creature, or 0 if it has never been used
    /// </summary>
    public float? SkillLastUsedTimeSeconds(int skillId) =>
      SkillLastUsedTimes.ContainsKey(skillId) ? (float?) SkillLastUsedTimes[skillId] : null;

    public bool HasOverlapWithOpponentCreature(GameContext c) =>
      c.CreatureService.GetCreature(CreatureId).Collider.IsTouchingLayers(
        Constants.LayerMaskForCreatures(Owner.GetOpponent()));

    public Collider2D GetCollider(GameContext c) => c.CreatureService.GetCreature(CreatureId).Collider;

    public Vector2 GetColliderCenter(GameContext c) =>
      c.CreatureService.GetCreature(CreatureId).Collider.bounds.center;

    public Vector2 GetCurrentPosition(GameContext c) =>
      c.CreatureService.GetCreature(CreatureId).transform.position;

    public DelegateList DelegateList => CurrentSkill != null ? CurrentSkill.DelegateList : Data.DelegateList;
  }
}