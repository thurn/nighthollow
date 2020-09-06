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
using Random = UnityEngine.Random;

namespace Nighthollow.Components
{
  public sealed class DamageText : MonoBehaviour
  {
    [SerializeField] TextMeshProUGUI _text;
    [SerializeField] float _duration;
    [SerializeField] int _offset;
    [SerializeField] RectTransform _rectTransform;

    public enum HitSize
    {
      Small,
      Medium,
      Big
    }

    public void Initialize(string text, Vector3 position)
    {
      _rectTransform.anchoredPosition = position;
      _text.text = text;
      _text.alpha = 1f;

      var offset = new Vector3(Random.Range(-_offset, _offset), Random.Range(-_offset, _offset));
      DOTween
        .Sequence()
        .Insert(0f, transform.DOMove(transform.position + offset, _duration))
        .Insert(0f, _text.DOFade(0f, _duration).SetEase(Ease.InQuad))
        .AppendCallback(() => gameObject.SetActive(false));
    }

    void OnValidate()
    {
      _text = GetComponent<TextMeshProUGUI>();
      _rectTransform = GetComponent<RectTransform>();
    }
  }
}