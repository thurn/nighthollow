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

using System.Collections.Generic;
using System.Linq;
using DataStructures.RandomSelector;
using Nighthollow.Data;
using UnityEngine;

namespace Nighthollow.Services
{
  public sealed class RewardService : MonoBehaviour
  {
    [SerializeField] RewardPoolData _data;
    readonly Dictionary<int, float> _cardWeights = new Dictionary<int, float>();
    readonly Dictionary<int, float> _affixWeights = new Dictionary<int, float>();

    void Awake()
    {
      _data = _data.Clone();
    }

    public void DraftRewards()
    {
      var selector = Root.Instance.Prefabs.CreateRewardSelector();
      selector.Initialize(new List<CardItemData>
      {
        RandomCard(),
        RandomCard(),
        RandomCard()
      });
    }

    CardItemData RandomCard()
    {
      var cardSelector = new DynamicRandomSelector<int>();
      for (var i = 0; i < _data.BaseCards.Count; ++i)
      {
        var weight = GetValueOrDefault(_cardWeights, i, 1.0f);
        cardSelector.Add(i, weight * 1000000f);
      }

      cardSelector.Build();

      var resultIndex = cardSelector.SelectRandomItem();
      _cardWeights[resultIndex] = GetValueOrDefault(_cardWeights, resultIndex, 1.0f) * 0.1f;

      var baseCard = _data.BaseCards.ElementAt(resultIndex).Clone();
      var item = ScriptableObject.CreateInstance<CardItemData>();
      item.Initialize(baseCard, new List<AffixData>());
      return item;
    }

    public static TValue GetValueOrDefault<TKey, TValue>(
      IDictionary<TKey, TValue> dictionary,
      TKey key,
      TValue defaultValue)
    {
      return dictionary.TryGetValue(key, out var value) ? value : defaultValue;
    }
  }
}