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
using TMPro;
using UnityEngine;

#nullable enable

namespace Nighthollow.Components
{
  public sealed class DamageText : MonoBehaviour
  {
    public enum HitSize
    {
      Small,
      Medium,
      Big
    }

    [SerializeField] TextMeshProUGUI _text = null!;
    [SerializeField] float _duration;
    [SerializeField] int _offset;
    [SerializeField] RectTransform _rectTransform = null!;

    void OnValidate()
    {
      _text = GetComponent<TextMeshProUGUI>();
      _rectTransform = GetComponent<RectTransform>();
    }

    public void Initialize(string text, Vector3 position)
    {
      _rectTransform.anchoredPosition = position;
      _text.text = text;
      _text.alpha = 1f;

      var offset = new Vector3(Random.Range(-_offset, _offset), Random.Range(-_offset, _offset));
      DOTween
        .Sequence()
        .Insert(atPosition: 0f, transform.DOMove(transform.position + offset, _duration))
        .Insert(atPosition: 0f, _text.DOFade(endValue: 0f, _duration).SetEase(Ease.InQuad))
        .AppendCallback(() => gameObject.SetActive(value: false));
    }
  }
}
