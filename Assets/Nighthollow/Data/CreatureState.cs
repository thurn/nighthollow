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
using Nighthollow.Stats;

#nullable enable

namespace Nighthollow.Data
{
  public sealed partial class CreatureState : StatEntity
  {
    public CreatureState(
      Creature2 creature,
      CreatureData data,
      CreatureAnimation animation,
      PlayerName owner,
      RankValue? rankPosition = null,
      FileValue? filePosition = null,
      SkillData? currentSkill = null,
      int damageTaken = 0,
      ImmutableDictionary<int, float>? skillLastUsedTimes = null,
      float? startingX = null)
    {
      Creature = creature;
      Data = data;
      Animation = animation;
      RankPosition = rankPosition;
      FilePosition = filePosition;
      CurrentSkill = currentSkill;
      Creature = creature;
      Owner = owner;
      DamageTaken = damageTaken;
      SkillLastUsedTimes = skillLastUsedTimes ?? ImmutableDictionary<int, float>.Empty;
      StartingX = startingX;
    }

    [Field] public Creature2 Creature { get; }
    [Field] public CreatureData Data { get; }
    [Field] public override StatTable Stats => Data.Stats;
    [Field] public CreatureAnimation Animation { get; }
    [Field] public PlayerName Owner { get; }
    [Field] public RankValue? RankPosition { get; }
    [Field] public FileValue? FilePosition { get; }
    [Field] public SkillData? CurrentSkill { get; }
    [Field] public int DamageTaken { get; }
    [Field] public ImmutableDictionary<int, float> SkillLastUsedTimes { get; }
    [Field] public float? StartingX { get; }
  }
}