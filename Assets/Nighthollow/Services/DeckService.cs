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

using System;
using System.Collections.Immutable;
using System.Linq;
using DataStructures.RandomSelector;
using JetBrains.Annotations;
using Nighthollow.Data;
using Nighthollow.Stats;
using Nighthollow.Utils;

#nullable enable

namespace Nighthollow.Services
{
  public sealed class DeckService
  {
    readonly bool _orderedDraws;
    readonly int _lastDraw;
    readonly ImmutableList<CreatureData> _cards;
    readonly ImmutableList<int> _weights;

    public DeckService(ImmutableList<CreatureData> cards, bool orderedDraws) :
      this(orderedDraws, 0, cards, StartingWeights(cards))
    {
    }

    DeckService(bool orderedDraws, int lastDraw, ImmutableList<CreatureData> cards, ImmutableList<int> cardWeights)
    {
      Errors.CheckState(cards.Count > 0, "No cards in deck");
      _orderedDraws = orderedDraws;
      _lastDraw = lastDraw;
      _cards = cards;
      _weights = cardWeights;
    }

    [MustUseReturnValue] public DeckService DrawCard(out CreatureData card)
    {
      if (_orderedDraws)
      {
        card = _cards[_lastDraw % _cards.Count];
        return new DeckService(_orderedDraws, _lastDraw + 1, _cards, _weights);
      }

      var selector = new DynamicRandomSelector<int>(seed: -1, _cards.Count);
      for (var i = 0; i < _cards.Count; ++i)
      {
        selector.Add(i, _weights[i]);
      }

      selector.Build();

      var index = selector.SelectRandomItem();
      card = _cards[index];
      return DecrementWeight(index);
    }

    DeckService DecrementWeight(int index) => new DeckService(
      _orderedDraws,
      _lastDraw,
      _cards,
      _weights.SetItem(index,
        _weights[index] > 1000 ? _weights[index] - 1000 : _weights[index] / 2));

    static ImmutableList<int> StartingWeights(ImmutableList<CreatureData> cards)
    {
      var manaCreatureCount = cards.Count(c => c.GetBool(Stat.IsManaCreature));
      var manaCreatureWeight = 4000 * ((2.0 * cards.Count - manaCreatureCount) / 3.0);
      return cards
        .Select(card => card.GetBool(Stat.IsManaCreature) ? (int) Math.Round(manaCreatureWeight) : 4000)
        .ToImmutableList();
    }
  }
}