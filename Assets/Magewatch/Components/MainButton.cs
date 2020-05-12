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

using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

namespace Magewatch.Components
{
  public sealed class MainButton : MonoBehaviour
  {
    [Header("Config")] [SerializeField] Text _text;
    [Header("Config")] [SerializeField] bool _enabled;

    public void SetText(string text)
    {
      _text.text = text;
    }

    public void SetEnabled(bool isEnabled)
    {
      var images = GetComponentsInChildren<Image>();

      if (isEnabled && !_enabled)
      {
        gameObject.SetActive(true);
        foreach (var image in images)
        {
          var imageColor = image.color;
          imageColor.a = 0;
          image.color = imageColor;
          image.DOFade(1.0f, 0.3f);
        }
      }
      else if (!isEnabled && _enabled)
      {
        var sequence = DOTween.Sequence();
        foreach (var image in images)
        {
          sequence.Insert(0, image.DOFade(0.0f, 0.3f));
        }

        sequence.AppendCallback(() => { gameObject.SetActive(false); });
      }

      _enabled = isEnabled;
    }
  }
}