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
using Nighthollow.Components;
using Nighthollow.Data;
using Nighthollow.Stats;
using Nighthollow.Utils;

#nullable enable

namespace Nighthollow.Services
{
  public sealed class UserService : StatEntity
  {
    readonly User _component;

    public UserService(
      User component,
      GameData gameData) : this(
      component,
      BuildStatTable(gameData),
      ImmutableList<CreatureData>.Empty,
      ImmutableList<int>.Empty,
      ImmutableList<CreatureData>.Empty)
    {
    }

    UserService(
      User component,
      StatTable stats,
      ImmutableList<CreatureData> deck,
      ImmutableList<int> drawWeights,
      ImmutableList<CreatureData> hand)
    {
      _component = component;
      Stats = stats;
      Deck = deck;
      DrawWeights = drawWeights;
      Hand = hand;
    }

    public ImmutableList<CreatureData> Deck { get; }

    public ImmutableList<int> DrawWeights { get; }

    public ImmutableList<CreatureData> Hand { get; }

    public override StatTable Stats { get; }

    public static void DrawOpeningHand(GameServiceRegistry registry)
    {
      var gameData = registry.Database.Snapshot();

      var cards =
        gameData.BattleData.UserDeckOverride.HasValue
          ? gameData.ItemLists[gameData.BattleData.UserDeckOverride.Value].Creatures
          : gameData.Deck.Values.ToImmutableList();
      Errors.CheckState(cards.Count > 0, "No cards in deck");
      registry.MutateUser(self => new UserService(
        self._component,
        self.Stats,
        cards.Select(card => card.BuildCreature(registry)).ToImmutableList(),
        self.DrawWeights,
        self.Hand));

      // _deck.OnStartGame(builtDeck, orderedDraws: gameData.BattleData.UserDeckOverride.HasValue);
      // var openingHand = new List<CreatureData>();
      // for (var i = 0; i < Errors.CheckPositive(Data.GetInt(Stat.StartingHandSize)); ++i)
      // {
      //   openingHand.Add(_deck.Draw());
      // }
      //
      // _hand.OverrideHandPosition(value: true);
      // _hand.DrawCards(openingHand, () => OnDrewHand(registry));
    }

    static StatTable BuildStatTable(GameData gameData) =>
      gameData.UserModifiers.Values.Aggregate(
        new StatTable(StatData.BuildDefaultStatTable(gameData)),
        (current, modifier) => current.InsertNullableModifier(modifier.BuildStatModifier()));
  }
}