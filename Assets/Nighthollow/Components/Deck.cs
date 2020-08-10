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
using UnityEngine;

namespace Nighthollow.Components
{
  public class Deck : MonoBehaviour
  {
    [SerializeField] DeckData _data;
    [SerializeField] bool _debugOrderedDraws;
    int _lastDraw;

    void Awake()
    {
      _data = Instantiate(_data);
    }

    public CardData Draw()
    {
      if (_debugOrderedDraws)
      {
        return Instantiate(_data.Cards[_lastDraw++ % _data.Cards.Count].Card);
      }
      else
      {
        var selector = new DynamicRandomSelector<int>(-1, _data.Cards.Count);
        for (var i = 0; i < _data.Cards.Count; ++i)
        {
          selector.Add(i, _data.Cards[i].Weight);
        }
        selector.Build();

        var cardWithWeight = _data.Cards[selector.SelectRandomItem()];
        DecrementWeight(cardWithWeight);

        return Instantiate(cardWithWeight.Card);
      }
    }

    void DecrementWeight(CardWithWeight card)
    {
      switch (card.Weight)
      {
        case 4000:
          card.Weight = 3000;
          break;
        case 3000:
          card.Weight = 2000;
          break;
        case 2000:
          card.Weight = 1000;
          break;
        default:
          card.Weight /= 2;
          break;
      }
    }
  }
}