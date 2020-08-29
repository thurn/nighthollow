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

using Nighthollow.Components;
using System;
using UnityEngine;

namespace Nighthollow.Data
{
  [Serializable]
  public sealed class Cost
  {
    [SerializeField] int _manaCost;
    public int ManaCost => _manaCost;

    [SerializeField] Influence _influenceCost;
    public Influence InfluenceCost => _influenceCost;
  }

  [CreateAssetMenu(menuName = "Data/Card")]
  public sealed class CardData : ScriptableObject
  {
    [SerializeField] Card _cardPrefab;
    public Card CardPrefab => _cardPrefab;

    [SerializeField] Cost _cost;
    public Cost Cost => _cost;

    [SerializeField] Sprite _image;
    public Sprite Image => _image;

    [SerializeField] CreatureData _creature;
    public CreatureData Creature => _creature;

    public CardData Clone()
    {
      var result = Instantiate(this);

      if (_creature)
      {
        result._creature = _creature.Clone();
      }

      return result;
    }
  }
}