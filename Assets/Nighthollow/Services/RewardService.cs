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

#nullable enable

using System.Collections.Generic;
using System.Linq;
using DataStructures.RandomSelector;
using Nighthollow.Data;
using Nighthollow.Model;
using UnityEngine;

namespace Nighthollow.Services
{
  public sealed class RewardService : MonoBehaviour
  {
    [SerializeField] TemplatePoolData _data;
    readonly Dictionary<int, float> _cardWeights = new Dictionary<int, float>();
    readonly Dictionary<int, float> _affixWeights = new Dictionary<int, float>();

    void Awake()
    {
//      _data = _data.Clone();
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
      for (var i = 0; i < _data.Cards.Count; ++i)
      {
        var weight = GetValueOrDefault(_cardWeights, i, 1.0f);
        cardSelector.Add(i, weight * 1000000f);
      }

      cardSelector.Build();

      var resultIndex = cardSelector.SelectRandomItem();
      _cardWeights[resultIndex] = GetValueOrDefault(_cardWeights, resultIndex, 1.0f) * 0.1f;

      var baseCard = _data.Cards.ElementAt(resultIndex).Clone();

      var affixes = new List<AffixTemplateData>();
      for (var i = 0; i < Random.Range(3, 7); ++i)
      {
        affixes.Add(RandomAffix());
      }

      return CardBuilder.RollStats(baseCard, affixes);
    }

    AffixTemplateData RandomAffix()
    {
      var affixSelector = new DynamicRandomSelector<int>();
      for (var i = 0; i < _data.Affixes.Count; ++i)
      {
        var weight = GetValueOrDefault(_affixWeights, i, 1.0f);
        affixSelector.Add(i, weight * 1000000f);
      }

      affixSelector.Build();

      var resultIndex = affixSelector.SelectRandomItem();
      _affixWeights[resultIndex] = GetValueOrDefault(_affixWeights, resultIndex, 1.0f) * 0.1f;

      var affix = _data.Affixes.ElementAt(resultIndex).Clone();
      return affix;
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