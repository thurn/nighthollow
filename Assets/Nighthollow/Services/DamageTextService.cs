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
using Nighthollow.Data;
using Nighthollow.Delegates.Effects;
using Nighthollow.Utils;
using UnityEngine;

#nullable enable

namespace Nighthollow.Services
{
  public sealed class DamageTextService : MonoBehaviour
  {
    [SerializeField] int _alpha;
    [SerializeField] int _mediumHitThreshold;
    [SerializeField] int _bigHitThreshold;
    [SerializeField] int _count;
    [SerializeField] float _averageDamage;

    public void ShowDamageText(GameServiceRegistry registry, CreatureId target, int amount)
    {
      var state = registry.Creatures[target];
      if (state.Owner != PlayerName.Enemy)
      {
        return;
      }

      // Exponential moving average
      var alpha = Constants.MultiplierBasisPoints(_alpha);
      _averageDamage = _count == 0 ? amount : alpha * amount + (1 - alpha) * _averageDamage;
      _count++;
      var point = ScreenUtils.WorldToCanvasAnchorPosition(
        registry.MainCamera,
        registry.MainCanvas,
        SkillEventEffect.RandomEffectPoint(registry.Creatures.GetCollider(target)));

      DamageText result;
      if (_count < 4 || amount < _averageDamage * Constants.MultiplierBasisPoints(_mediumHitThreshold))
      {
        result = registry.Prefabs.CreateHitSmall();
      }
      else if (amount < _averageDamage * Constants.MultiplierBasisPoints(_bigHitThreshold))
      {
        result = registry.Prefabs.CreateHitMedium();
      }
      else
      {
        result = registry.Prefabs.CreateHitBig();
      }

      result.Initialize(amount.ToString(), point);
    }
  }
}