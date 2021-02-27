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
    [Header("Config")] [SerializeField] int _alpha;

    [SerializeField] int _mediumHitThreshold;
    [SerializeField] int _bigHitThreshold;

    [Header("State")] [SerializeField] int _count;

    [SerializeField] float _averageDamage;

    public void ShowDamageText(Creature target, int amount)
    {
      if (target.AsCreatureState().Owner != PlayerName.Enemy)
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
