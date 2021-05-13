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
using Nighthollow.Utils;
using UnityEngine;
using UnityEngine.UI;

#nullable enable

namespace Nighthollow.Services
{
  public sealed class Prefabs : MonoBehaviour
  {
    [SerializeField] Transform _mainCanvasTransform = null!;

    [SerializeField] Card _cardPrefab = null!;
    [SerializeField] StatusBarsHolder _statusBars = null!;
    [SerializeField] SpriteRenderer _cursorPrefab = null!;
    [SerializeField] Image _influencePrefab = null!;
    [SerializeField] Attachment _attachmentPrefab = null!;
    [SerializeField] Sprite _lightSymbol = null!;
    [SerializeField] Sprite _skySymbol = null!;
    [SerializeField] Sprite _flameSymbol = null!;
    [SerializeField] Sprite _iceSymbol = null!;
    [SerializeField] Sprite _earthSymbol = null!;
    [SerializeField] Sprite _nightSymbol = null!;
    [SerializeField] TimedEffect _missEffect = null!;
    [SerializeField] TimedEffect _evadeEffect = null!;
    [SerializeField] TimedEffect _critEffect = null!;
    [SerializeField] TimedEffect _stunEffect = null!;
    [SerializeField] DamageText _hitSmall = null!;
    [SerializeField] DamageText _hitMedium = null!;
    [SerializeField] DamageText _hitBig = null!;
    [SerializeField] Sprite _stunIcon = null!;
    ObjectPoolService _objectPoolService = null!;

    void Awake()
    {
      Errors.CheckNotNull(_mainCanvasTransform);
    }

    public void Initialize(ObjectPoolService objectPoolService)
    {
      _objectPoolService = objectPoolService;
    }

    public Card CreateCard() => ComponentUtils.Instantiate(_cardPrefab, _mainCanvasTransform);

    public StatusBarsHolder CreateStatusBars() =>
      ComponentUtils.Instantiate(_statusBars, _mainCanvasTransform);

    public SpriteRenderer CreateCursor() => ComponentUtils.Instantiate(_cursorPrefab);

    public Image CreateInfluence() => ComponentUtils.Instantiate(_influencePrefab);

    public Attachment CreateAttachment() => ComponentUtils.Instantiate(_attachmentPrefab);

    public GameObject CreateMiss(Vector3 position) =>
      _objectPoolService.Create(_missEffect.gameObject, position);

    public GameObject CreateEvade(Vector3 position) =>
      _objectPoolService.Create(_evadeEffect.gameObject, position);

    public GameObject CreateCrit(Vector3 position) =>
      _objectPoolService.Create(_critEffect.gameObject, position);

    public GameObject CreateStun(Vector3 position) =>
      _objectPoolService.Create(_stunEffect.gameObject, position);

    public DamageText CreateHitSmall() =>
      _objectPoolService.Create(_hitSmall, Constants.OffScreen, _mainCanvasTransform);

    public DamageText CreateHitMedium() =>
      _objectPoolService.Create(_hitMedium, Constants.OffScreen, _mainCanvasTransform);

    public DamageText CreateHitBig() =>
      _objectPoolService.Create(_hitBig, Constants.OffScreen, _mainCanvasTransform);

    public Sprite StunIcon() => Instantiate(_stunIcon);

    public Sprite SpriteForInfluenceType(School influenceType)
    {
      return influenceType switch
      {
        School.Light => Instantiate(_lightSymbol),
        School.Sky => Instantiate(_skySymbol),
        School.Flame => Instantiate(_flameSymbol),
        School.Ice => Instantiate(_iceSymbol),
        School.Earth => Instantiate(_earthSymbol),
        School.Shadow => Instantiate(_nightSymbol),
        _ => throw Errors.UnknownEnumValue(influenceType)
      };
    }
  }
}
