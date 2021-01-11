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
  [MessagePackObject]
  public sealed partial class CurrentEnemyState
  {
    public CurrentEnemyState(ImmutableList<CreatureItemData>? enemies = null)
    {
      Enemies = enemies ?? ImmutableList<CreatureItemData>.Empty;
    }

    [Field] public ImmutableList<CreatureItemData> Enemies { get; }

    public EnemyData BuildEnemyData(GameData gameData) =>
      new EnemyData(
        Enemies.IsEmpty
          ? gameData.ItemLists.Values.First(list => list.Name == StaticItemListName.TutorialEnemies).Creatures
          : Enemies,
        gameData.UserModifiers.Values.Aggregate(
          new StatTable(StatData.BuildDefaultStatTable(gameData)),
          (current, modifier) => current.InsertNullableModifier(modifier.BuildStatModifier())));
  }

  public sealed partial class EnemyData : StatEntity
  {
    public EnemyData(ImmutableList<CreatureItemData> enemies, StatTable stats)
    {
      Enemies = enemies;
      Stats = stats;
    }

    [Field] public ImmutableList<CreatureItemData> Enemies { get; }
    [Field] public override StatTable Stats { get; }
  }
}
