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
using Nighthollow.Model;
using Nighthollow.Services;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

namespace Nighthollow.Components
{
  public sealed class Player : MonoBehaviour
  {
    [Header("Config")] [SerializeField] Hand _hand;
    [SerializeField] Image _lifeBar;
    [SerializeField] Text _lifeText;

    [FormerlySerializedAs("_manaBar")] [SerializeField]
    Image _powerBar;

    [FormerlySerializedAs("_manaText")] [SerializeField]
    Text _powerText;

    [SerializeField] RectTransform _influenceRow;
    [SerializeField] Collider2D _projectileCollider;

    public Hand Hand => _hand;

    public Collider2D Collider => _projectileCollider;

    public void UpdatePlayerData(PlayerData newPlayerData)
    {
      if (newPlayerData.MaximumLife == 0)
      {
        _lifeBar.DOFillAmount(0, 0.3f);
      }
      else
      {
        _lifeBar.DOFillAmount(newPlayerData.CurrentLife / (float) newPlayerData.MaximumLife, 0.3f);
      }

      if (newPlayerData.MaximumMana == 0)
      {
        _powerBar.DOFillAmount(0, 0.3f);
      }
      else
      {
        _powerBar.DOFillAmount(newPlayerData.CurrentMana / (float) newPlayerData.MaximumMana, 0.3f);
      }

      _lifeText.text = $"{newPlayerData.CurrentLife}/{newPlayerData.MaximumLife}";
      _powerText.text = $"{newPlayerData.CurrentMana}/{newPlayerData.MaximumMana}";

      foreach (Transform child in _influenceRow)
      {
        Destroy(child.gameObject);
      }

      foreach (var influence in newPlayerData.MaximumInfluence)
      {
        var currentInfluence = InfluenceCount(newPlayerData, influence.School);
        var direction = newPlayerData.PlayerName == PlayerName.Enemy ? -1 : 1;
        for (var i = 0; i < influence.Value; ++i)
        {
          var image = Injector.Instance.Prefabs.CreateInfluence();
          image.sprite = Injector.Instance.Prefabs.SpriteForInfluenceType(influence.School);
          image.transform.SetParent(_influenceRow);
          image.transform.localPosition = new Vector3(i * 60 * direction, 0, 0);
          if (i >= currentInfluence)
          {
            image.color = Color.gray;
          }
        }
      }
    }

    int InfluenceCount(PlayerData playerData, School influenceType) =>
      playerData.CurrentInfluence.ToList().Find(i => i.School == influenceType)?.Value ?? 0;
  }
}