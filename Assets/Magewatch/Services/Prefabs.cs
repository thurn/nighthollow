// Copyright The Magewatch Project

// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at

//    https://www.apache.org/licenses/LICENSE-2.0

// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.

using Magewatch.Components;
using Magewatch.Data;
using Magewatch.Utils;
using UnityEngine;
using UnityEngine.UI;

namespace Magewatch.Services
{
  public sealed class Prefabs : MonoBehaviour
  {
    [SerializeField] HealthBar _healthBarPrefab;
    [SerializeField] SpriteRenderer _cursorPrefab;
    [SerializeField] Image _influencePrefab;
    [SerializeField] Attachment _attachmentPrefab;
    [SerializeField] Sprite _lightSymbol;
    [SerializeField] Sprite _skySymbol;
    [SerializeField] Sprite _flameSymbol;
    [SerializeField] Sprite _iceSymbol;
    [SerializeField] Sprite _earthSymbol;
    [SerializeField] Sprite _shadowSymbol;

    public HealthBar CreateHealthBar() => ComponentUtils.Instantiate(_healthBarPrefab,
      Root.Instance.MainCanvas.transform);

    public SpriteRenderer CreateCursor() => ComponentUtils.Instantiate(_cursorPrefab);

    public Image CreateInfluence() => ComponentUtils.Instantiate(_influencePrefab);

    public Attachment CreateAttachment() => ComponentUtils.Instantiate(_attachmentPrefab);

    public Sprite SpriteForInfluenceType(InfluenceType influenceType)
    {
      switch (influenceType)
      {
        case InfluenceType.Light: return Instantiate(_lightSymbol);
        case InfluenceType.Sky: return Instantiate(_skySymbol);
        case InfluenceType.Flame: return Instantiate(_flameSymbol);
        case InfluenceType.Ice: return Instantiate(_iceSymbol);
        case InfluenceType.Earth: return Instantiate(_earthSymbol);
        case InfluenceType.Shadow: return Instantiate(_shadowSymbol);
        default: throw Errors.UnknownEnumValue(influenceType);
      }
    }
  }
}