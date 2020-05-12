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

using System.Linq;
using DG.Tweening;
using Magewatch.API;
using Magewatch.Services;
using UnityEngine;
using UnityEngine.UI;

namespace Magewatch.Components
{
  public sealed class Player : MonoBehaviour
  {
    [Header("Config")] [SerializeField] Hand _hand;
    [SerializeField] Image _lifeBar;
    [SerializeField] Text _lifeText;
    [SerializeField] Image _manaBar;
    [SerializeField] Text _manaText;
    [SerializeField] RectTransform _influenceRow;
    [SerializeField] Collider2D _projectileCollider;
    [Header("State")] [SerializeField] PlayerData _playerData;

    public Hand Hand => _hand;

    public Collider2D Collider => _projectileCollider;

    public void UpdatePlayerData(PlayerData playerData, IOnComplete onComplete)
    {
      var firstUpdate = _playerData == null;
      if (firstUpdate || _playerData.CurrentLife != playerData.CurrentLife)
      {
        if (playerData.MaximumLife == 0)
        {
          _manaBar.DOFillAmount(0, 0.3f);
        }
        else
        {
          _lifeBar.DOFillAmount(playerData.CurrentLife / (float) playerData.MaximumLife, 0.3f)
            .OnComplete(onComplete.OnComplete);
        }
      }

      if (firstUpdate || _playerData.CurrentMana != playerData.CurrentMana)
      {
        if (playerData.MaximumMana == 0)
        {
          _manaBar.DOFillAmount(1, 0.3f);
        }
        else
        {
          _manaBar.DOFillAmount(playerData.CurrentMana / (float) playerData.MaximumMana, 0.3f);
        }
      }

      _lifeText.text = $"{playerData.CurrentLife}/{playerData.MaximumLife}";
      _manaText.text = $"{playerData.CurrentMana}/{playerData.MaximumMana}";

      foreach (Transform child in _influenceRow)
      {
        Destroy(child.gameObject);
      }

      foreach (var influence in playerData.MaximumInfluence)
      {
        var currentInfluence = InfluenceCount(playerData, influence.InfluenceType);
        for (var i = 0; i < influence.Value; ++i)
        {
          var image = Root.Instance.Prefabs.CreateInfluence();
          image.sprite = Root.Instance.Prefabs.SpriteForInfluenceType(influence.InfluenceType);
          image.transform.SetParent(_influenceRow);
          if (i >= currentInfluence)
          {
            image.color = Color.gray;
          }
        }
      }
    }

    int InfluenceCount(PlayerData playerData, InfluenceType influenceType) =>
      playerData.CurrentInfluence.ToList().Find(i => i.InfluenceType == influenceType)?.Value ?? 0;
  }
}