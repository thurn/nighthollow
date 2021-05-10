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
using System.Linq;
using MessagePack;
using Nighthollow.Stats;

#nullable enable

namespace Nighthollow.Data
{
  /// <summary>
  /// Configuration and state for a single battle (individual game/match), describing things such as the set of
  /// enemies which will be be faced. This data can represent different things depending on the overall game state:
  ///
  /// - Immediately before a battle begins, it provides the configuration for the upcoming conflict
  /// - During a battle, it represents the current state of the ongoing battle
  /// - Otherwise, it contains a snapshot of the final configuration & state for the most-recently-completed battle
  /// </summary>
  [MessagePackObject]
  public sealed partial class BattleData
  {
    [SerializationConstructor]
    public BattleData(
      ImmutableList<CreatureItemData>? enemies = null,
      ImmutableList<ModifierData>? enemyModifiers = null,
      int? userDeckOverride = null,
      int? enemyListOverride = null)
    {
      Enemies = enemies ?? ImmutableList<CreatureItemData>.Empty;
      EnemyModifiers = enemyModifiers ?? ImmutableList<ModifierData>.Empty;
      UserDeckOverride = userDeckOverride;
      EnemyListOverride = enemyListOverride;
    }

    [Key(0)] public ImmutableList<CreatureItemData> Enemies { get; }

    [Key(1)] public ImmutableList<ModifierData> EnemyModifiers { get; }

    [ForeignKey(typeof(StaticItemListData))]
    [Key(2)] public int? UserDeckOverride { get; }

    [ForeignKey(typeof(StaticItemListData))]
    [Key(3)] public int? EnemyListOverride { get; }
  }
}