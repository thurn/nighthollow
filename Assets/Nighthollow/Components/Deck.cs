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
using System.Collections.Generic;
using System.Linq;
using DataStructures.RandomSelector;
using Nighthollow.Data;
using UnityEngine;
using Nighthollow.Stats2;

#nullable enable

namespace Nighthollow.Components
{
  public sealed class Deck : MonoBehaviour
  {
    [SerializeField] List<CreatureData> _cards = null!;
    [SerializeField] List<int> _weights = null!;
    [SerializeField] bool _orderedDraws;
    int _lastDraw;

    public void OnStartGame(IEnumerable<CreatureData> cards, bool orderedDraws)
    {
      _weights.Clear();
      _cards = cards.ToList();
      _orderedDraws = orderedDraws;

      var manaCreatureCount = _cards.Count(c => c.GetBool(Stat.IsManaCreature));
      var manaCreatureWeight = 4000 * ((2.0 * _cards.Count - manaCreatureCount) / 3.0);

      foreach (var card in _cards)
      {
        _weights.Add(card.GetBool(Stat.IsManaCreature) ? (int) Math.Round(manaCreatureWeight) : 4000);
      }
    }

    public CreatureData Draw()
    {
      if (_orderedDraws)
      {
        return _cards[_lastDraw++ % _cards.Count];
      }

      var selector = new DynamicRandomSelector<int>(seed: -1, _cards.Count);
      for (var i = 0; i < _cards.Count; ++i)
      {
        selector.Add(i, _weights[i]);
      }

      selector.Build();

      var index = selector.SelectRandomItem();
      DecrementWeight(index);
      return _cards[index];
    }

    void DecrementWeight(int index)
    {
      _weights[index] = _weights[index] > 1000 ? _weights[index] - 1000 : _weights[index] / 2;
    }
  }
}
