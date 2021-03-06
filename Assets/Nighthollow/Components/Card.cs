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

using System.Collections.Generic;
using System.Collections.Immutable;
using DG.Tweening;
using Nighthollow.Data;
using Nighthollow.Services;
using Nighthollow.Utils;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

#nullable enable

namespace Nighthollow.Components
{
  public interface ICardCallbacks
  {
    void OnCardOverBoard(int cardId, Card card);

    void OnReturnToHand(int cardId);

    void OnCardPlayed(int cardId);
  }

  public sealed class Card : MonoBehaviour,
    IDragHandler, IBeginDragHandler, IEndDragHandler, IPointerEnterHandler, IPointerExitHandler
  {
    [Header("Config")]
    [SerializeField] RectTransform _cardBack = null!;

    [SerializeField] RectTransform _cardFront = null!;
    [SerializeField] Image _cardImage = null!;
    [SerializeField] TextMeshProUGUI _cost = null!;
    [SerializeField] List<Image> _influence = null!;
    [SerializeField] Image _outline = null!;
    [SerializeField] bool _canPlay;
    [SerializeField] bool _previewMode;
    [SerializeField] bool _isDragging;
    [SerializeField] bool _overBoard;
    [SerializeField] int _initialDragSiblingIndex;
    [SerializeField] Vector3 _initialDragPosition;
    [SerializeField] Quaternion _initialDragRotation;

    Camera _mainCamera = null!;
    int _cardId;
    ICardCallbacks _callbacks = null!;

    void Start()
    {
      Errors.CheckNotNull(_cardBack);
      Errors.CheckNotNull(_cardFront);
      Errors.CheckNotNull(_cardImage);
    }

    public void Initialize(
      Camera mainCamera,
      AssetService assetService,
      ICardCallbacks callbacks,
      int cardId,
      CreatureData data)
    {
      _mainCamera = mainCamera;
      _callbacks = callbacks;
      _cardId = cardId;

      gameObject.name = $"{data.BaseType.Name} Card";
      _cardFront.gameObject.SetActive(value: false);
      _cardBack.gameObject.SetActive(value: true);
      _cardImage.sprite = assetService.GetImage(Errors.CheckNotNull(data.BaseType.ImageAddress));

      DOTween.Sequence()
        .Insert(atPosition: 0, _cardBack.transform.DOLocalRotate(new Vector3(x: 0, y: 90, z: 0), duration: 0.2f))
        .InsertCallback(atPosition: 0.2f, () =>
        {
          _cardFront.gameObject.SetActive(value: true);
          _cardFront.transform.localRotation = Quaternion.Euler(x: 0, y: 90, z: 0);
          _cardBack.gameObject.SetActive(value: false);
        })
        .Insert(atPosition: 0.2f, _cardFront.transform.DOLocalRotate(Vector3.zero, duration: 0.3f));
    }

    public bool PreviewMode
    {
      set => _previewMode = value;
    }

    public void OnUpdate(
      Prefabs prefabs,
      int manaCost,
      ImmutableDictionary<School, int> influenceCost,
      bool canPlay)
    {
      _canPlay = canPlay;
      _outline.enabled = _canPlay;
      _cost.text = manaCost.ToString();

      var addIndex = 0;
      foreach (var pair in influenceCost)
      {
        AddInfluence(prefabs, pair.Key, pair.Value, ref addIndex);
      }

      while (addIndex < _influence.Count)
      {
        _influence[addIndex++].enabled = false;
      }
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
      if (!_previewMode)
      {
        _isDragging = true;
        _initialDragPosition = _mainCamera.ScreenToWorldPoint(Input.mousePosition);
        _initialDragRotation = transform.rotation;
        _initialDragSiblingIndex = transform.GetSiblingIndex();
        transform.SetAsLastSibling();
      }
    }

    public void OnDrag(PointerEventData pointEventData)
    {
      if (_isDragging)
      {
        var mousePosition = _mainCamera.ScreenToWorldPoint(Input.mousePosition);
        transform.position = Input.mousePosition;

        if (_canPlay && mousePosition.x < Constants.IndicatorRightX &&
            mousePosition.y > Constants.IndicatorBottomY)
        {
          if (!_overBoard)
          {
            gameObject.SetActive(value: false);
            // _registry.CreatureController.CreateUserCreature(_data, this);
            _callbacks.OnCardOverBoard(_cardId, this);
            _overBoard = true;
          }
        }
        else
        {
          var distanceDragged = Vector2.Distance(mousePosition, _initialDragPosition);

          var t = Mathf.Clamp01(distanceDragged / 5);
          var scale = Mathf.Lerp(Hand.FinalCardScale, b: 1.2f, t);
          transform.localScale = scale * Vector3.one;

          var rotation = Quaternion.Slerp(_initialDragRotation, Quaternion.identity, t);
          transform.rotation = rotation;

          _overBoard = false;
        }
      }
    }

    public void OnEndDrag(PointerEventData eventData)
    {
      if (_isDragging)
      {
        _isDragging = false;
        transform.SetSiblingIndex(_initialDragSiblingIndex);
        _callbacks.OnReturnToHand(_cardId);
      }
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
      if (_previewMode)
      {
        // var tooltip = TooltipUtil.CreateCreatureTooltip(Database.Instance.UserData.Stats, _data.Item);
        // tooltip.XOffset = 64;
        // Root.Instance.ScreenController.ShowTooltip(
        //   tooltip,
        //   InterfaceUtils.ScreenPointToInterfacePoint(transform.position));
      }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
      if (_previewMode)
      {
        // Root.Instance.ScreenController.HideTooltip();
      }
    }

    public void OnPlayed()
    {
      _callbacks.OnCardPlayed(_cardId);
    }

    void AddInfluence(Prefabs prefabs, School school, int influence, ref int addIndex)
    {
      for (var i = 0; i < influence; ++i)
      {
        _influence[addIndex].enabled = true;
        _influence[addIndex].sprite = prefabs.SpriteForInfluenceType(school);
        addIndex++;
      }
    }
  }
}