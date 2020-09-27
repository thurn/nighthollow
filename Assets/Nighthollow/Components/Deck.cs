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

using DataStructures.RandomSelector;
using System.Collections.Generic;
using System.Linq;
using Nighthollow.Data;
using Nighthollow.Generated;
using Nighthollow.Model;
using UnityEngine;

namespace Nighthollow.Components
{
  public sealed class Deck : MonoBehaviour
  {
    [SerializeField] bool _debugOrderedDraws;

    [SerializeField] List<CardItemData> _cards;

    [SerializeField] List<int> _weights;
    int _lastDraw;

    public void OnStartGame(IEnumerable<CardItemData> cards)
    {
      _weights.Clear();
      _cards = cards.ToList();

      foreach (var card in _cards)
      {
        _weights.Add(card.Card.GetBool(Stat.IsManaCreature) ? 24000 : 4000);
      }
    }

    public CardData Draw()
    {
      if (_debugOrderedDraws)
      {
        return CardBuilder.Build(_cards[_lastDraw++ % _cards.Count]);
      }

      var selector = new DynamicRandomSelector<int>(-1, _cards.Count);
      for (var i = 0; i < _cards.Count; ++i)
      {
        selector.Add(i, _weights[i]);
      }

      selector.Build();

      var index = selector.SelectRandomItem();
      DecrementWeight(index);
      return CardBuilder.Build(_cards[index]);
    }

    void DecrementWeight(int index)
    {
      _weights[index] = _weights[index] > 1000 ? _weights[index] - 1000 : _weights[index] / 2;
    }
  }
}