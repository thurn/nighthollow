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
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

namespace Nighthollow.Components
{
  public sealed class User : MonoBehaviour
  {
    [Header("Config")] [SerializeField] Hand _hand;
    [SerializeField] TextMeshProUGUI _lifeText;
    [SerializeField] TextMeshProUGUI _manaText;
    [SerializeField] RectTransform _influenceRow;

    [Header("State")] [SerializeField] UserData _userData;

    public Hand Hand => _hand;

    public void UpdateUserData(UserData newUserData)
    {
      gameObject.SetActive(true);

      _lifeText.text = newUserData.Life.ToString();
      _manaText.text = newUserData.Mana.ToString();

      if (_userData == null || !_userData.Influence.SequenceEqual(newUserData.Influence))
      {
        foreach (Transform child in _influenceRow)
        {
          Destroy(child.gameObject);
        }

        foreach (var influence in newUserData.Influence)
        {
          for (var i = 0; i < influence.Value; ++i)
          {
            var image = Root.Instance.Prefabs.CreateInfluence();
            image.sprite = Root.Instance.Prefabs.SpriteForInfluenceType(influence.School);
            image.transform.SetParent(_influenceRow);
            image.transform.localScale = Vector3.one;
            image.transform.localPosition = new Vector3(i * 100, 0, 0);
          }
        }
      }

      _userData = newUserData;
    }
  }
}