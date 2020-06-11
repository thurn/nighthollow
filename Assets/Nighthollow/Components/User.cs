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
  public sealed class User : MonoBehaviour
  {
    [Header("Config")] [SerializeField] Hand _hand;
    [SerializeField] Image _lifeBar;
    [SerializeField] Text _lifeText;

    [FormerlySerializedAs("_manaBar")] [SerializeField]
    Image _powerBar;

    [FormerlySerializedAs("_manaText")] [SerializeField]
    Text _powerText;

    [SerializeField] RectTransform _influenceRow;

    public Hand Hand => _hand;

    public void UpdateUserData(UserData newUserData)
    {
      _lifeBar.DOFillAmount(
        newUserData.MaximumLife == 0 ? 0 : newUserData.CurrentLife / (float) newUserData.MaximumLife, 0.3f);
      _powerBar.DOFillAmount(
        newUserData.MaximumMana == 0 ? 0 : newUserData.CurrentMana / (float) newUserData.MaximumMana, 0.3f);

      _lifeText.text = $"{newUserData.CurrentLife}/{newUserData.MaximumLife}";
      _powerText.text = $"{newUserData.CurrentMana}/{newUserData.MaximumMana}";

      foreach (Transform child in _influenceRow)
      {
        Destroy(child.gameObject);
      }

      foreach (var influence in newUserData.MaximumInfluence)
      {
        var currentInfluence = InfluenceCount(newUserData, influence.School);
        for (var i = 0; i < influence.Value; ++i)
        {
          var image = Root.Instance.Prefabs.CreateInfluence();
          image.sprite = Root.Instance.Prefabs.SpriteForInfluenceType(influence.School);
          image.transform.SetParent(_influenceRow);
          image.transform.localPosition = new Vector3(i * 60, 0, 0);
          if (i >= currentInfluence)
          {
            image.color = Color.gray;
          }
        }
      }
    }

    int InfluenceCount(UserData userData, School influenceType) =>
      userData.CurrentInfluence.ToList().Find(i => i.School == influenceType)?.Value ?? 0;
  }
}