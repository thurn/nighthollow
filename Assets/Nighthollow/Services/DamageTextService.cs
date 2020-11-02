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

using Nighthollow.Components;
using Nighthollow.Delegates.Core;
using Nighthollow.Delegates.Effects;
using Nighthollow.Generated;
using Nighthollow.Utils;
using UnityEngine;

namespace Nighthollow.Services
{
  public sealed class DamageTextService : MonoBehaviour
  {
    [Header("Config")]
    [SerializeField] int _alpha = 0;
    [SerializeField] int _mediumHitThreshold = 0;
    [SerializeField] int _bigHitThreshold = 0;

    [Header("State")]
    [SerializeField] int _count = 0;
    [SerializeField] float _averageDamage = 0;

    public void ShowDamageText(Creature target, int amount)
    {
      if (target.Owner != PlayerName.Enemy)
      {
        return;
      }

      // Exponential moving average
      var alpha = Constants.MultiplierBasisPoints(_alpha);
      _averageDamage = _count == 0 ? amount : alpha * amount + (1 - alpha) * _averageDamage;
      _count++;
      var point = ScreenUtils.WorldToCanvasAnchorPosition(SkillEventEffect.RandomEffectPoint(target));

      DamageText result;
      if (_count < 4 || amount < _averageDamage * Constants.MultiplierBasisPoints(_mediumHitThreshold))
      {
        result = Root.Instance.Prefabs.CreateHitSmall();
      }
      else if (amount < _averageDamage * Constants.MultiplierBasisPoints(_bigHitThreshold))
      {
        result = Root.Instance.Prefabs.CreateHitMedium();
      }
      else
      {
        result = Root.Instance.Prefabs.CreateHitBig();
      }

      result.Initialize(amount.ToString(), point);
    }
  }
}