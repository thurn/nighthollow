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
using Nighthollow.Data;

#nullable enable

namespace Nighthollow.Rules.Effects
{
  [MessagePackObject]
  public sealed partial class PlaceCreaturesFromListEffect : RuleEffect
  {
    public override Description Describe() => new Description("place creatures from the list", nameof(ItemList));

    public PlaceCreaturesFromListEffect(int itemList)
    {
      ItemList = itemList;
    }

    [ForeignKey(typeof(StaticItemListData))]
    [Key(0)] public int ItemList { get; }

    public override ImmutableHashSet<IKey> GetDependencies() => ImmutableHashSet.Create<IKey>(
      Key.GameData,
      Key.User,
      Key.CreatureController
    );

    public override void Execute(IEffectScope scope, RuleOutput? output)
    {
      var gameData = scope.Get(Key.GameData);
      var creatures = gameData.ItemLists[ItemList].Creatures;
      var built = creatures.Select(c => c.BuildCreature(gameData, scope.Get(Key.User).UserState));
      var creatureController = scope.Get(Key.CreatureController);

      using var positions = BoardPositions.AllPositions().GetEnumerator();

      foreach (var creature in built)
      {
        positions.MoveNext();
        var (rankValue, fileValue) = positions.Current;
        var creatureId = creatureController.CreateUserCreature(creature);
        creatureController.AddUserCreatureAtPosition(creatureId, rankValue, fileValue);
      }
    }
  }
}