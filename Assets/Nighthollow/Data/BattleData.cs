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
using MessagePack;

#nullable enable

namespace Nighthollow.Data
{
  /// <summary>
  /// Configuration and state for a single battle (individual game/match), describing things like the set of
  /// enemies which will be be faced.
  /// </summary>
  [MessagePackObject]
  public sealed partial class BattleData
  {
    [SerializationConstructor]
    public BattleData(
      ImmutableList<CreatureItemData>? enemies = null,
      ImmutableList<ModifierData>? enemyModifiers = null,
      int? rewardChoicesOverride = null,
      int? fixedRewardsOverride = null)
    {
      Enemies = enemies ?? ImmutableList<CreatureItemData>.Empty;
      EnemyModifiers = enemyModifiers ?? ImmutableList<ModifierData>.Empty;
      RewardChoicesOverride = rewardChoicesOverride;
      FixedRewardsOverride = fixedRewardsOverride;
    }

    [Key(0)] public ImmutableList<CreatureItemData> Enemies { get; }

    [Key(1)] public ImmutableList<ModifierData> EnemyModifiers { get; }

    /// <summary>Override list of reward choices for this battle.</summary>
    [ForeignKey(typeof(StaticItemListData))]
    [Key(2)] public int? RewardChoicesOverride { get; }

    /// <summary>Override list of fixed rewards for this battle.</summary>
    [ForeignKey(typeof(StaticItemListData))]
    [Key(3)] public int? FixedRewardsOverride { get; }
  }
}