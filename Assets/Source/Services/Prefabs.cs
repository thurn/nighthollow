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
using Nighthollow.Model;
using Nighthollow.Utils;
using UnityEngine;
using UnityEngine.UI;

namespace Nighthollow.Services
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
      Injector.Instance.MainCanvas);

    public SpriteRenderer CreateCursor() => ComponentUtils.Instantiate(_cursorPrefab);

    public Image CreateInfluence() => ComponentUtils.Instantiate(_influencePrefab);

    public Attachment CreateAttachment() => ComponentUtils.Instantiate(_attachmentPrefab);

    public Sprite SpriteForInfluenceType(School influenceType)
    {
      switch (influenceType)
      {
        case School.Light: return Instantiate(_lightSymbol);
        case School.Sky: return Instantiate(_skySymbol);
        case School.Flame: return Instantiate(_flameSymbol);
        case School.Ice: return Instantiate(_iceSymbol);
        case School.Earth: return Instantiate(_earthSymbol);
        case School.Shadow: return Instantiate(_shadowSymbol);
        default: throw Errors.UnknownEnumValue(influenceType);
      }
    }
  }
}