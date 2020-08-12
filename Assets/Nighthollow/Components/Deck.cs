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
using Nighthollow.Data;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Nighthollow.Components
{
  public class Deck : MonoBehaviour
  {
    [SerializeField] DeckData _data;
    [SerializeField] bool _debugOrderedDraws;
    [SerializeField] List<CardData> _cards;
    [SerializeField] List<int> _weights;
    int _lastDraw;

    void Awake()
    {
      _data = Instantiate(_data);
      _cards = _data.Cards.ToList();
      _weights = Enumerable.Repeat(4000, _cards.Count).ToList();
    }

    public CardData Draw()
    {
      if (_debugOrderedDraws)
      {
        return _cards[_lastDraw++ % _cards.Count].Clone();
      }

      var selector = new DynamicRandomSelector<int>(-1, _cards.Count);
      for (var i = 0; i < _cards.Count; ++i)
      {
        selector.Add(i, _weights[i]);
      }

      selector.Build();

      var index = selector.SelectRandomItem();
      DecrementWeight(index);
      return _cards[index].Clone();
    }

    void DecrementWeight(int index)
    {
      switch (_weights[index])
      {
        case 4000:
          _weights[index] = 3000;
          break;
        case 3000:
          _weights[index] = 2000;
          break;
        case 2000:
          _weights[index] = 1000;
          break;
        default:
          _weights[index] /= 2;
          break;
      }
    }
  }
}