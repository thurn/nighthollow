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
using DG.Tweening;
using Nighthollow.Model;
using Nighthollow.Services;
using Nighthollow.Utils;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Nighthollow.Components
{
  public sealed class Card : MonoBehaviour, IDragHandler, IBeginDragHandler, IEndDragHandler
  {
    [Header("Config")] [SerializeField] bool _debugMode;
    [SerializeField] float _debugCardScale = 0.65f;
    [SerializeField] RectTransform _cardBack;
    [SerializeField] RectTransform _cardFront;
    [SerializeField] Image _cardImage;
    [SerializeField] TextMeshProUGUI _cost;
    [SerializeField] List<Image> _influence;
    [SerializeField] Image _outline;

    [Header("State")] [SerializeField] CardData _cardData;
    [SerializeField] Hand _hand;
    [SerializeField] bool _previewMode;
    [SerializeField] bool _initialized;
    [SerializeField] bool _isFaceUp;
    [SerializeField] bool _isDragging;
    [SerializeField] bool _overBoard;
    [SerializeField] int _initialDragSiblingIndex;
    [SerializeField] Vector3 _initialDragPosition;
    [SerializeField] Quaternion _initialDragRotation;

    public CardId CardId => _cardData.CardId;

    public void Initialize(CardData cardData)
    {
      _cardFront.gameObject.SetActive(false);
      _cardBack.gameObject.SetActive(true);
      UpdateCardData(cardData);
      _initialized = true;
    }

    void Start()
    {
      if (!_initialized && _debugMode)
      {
        Initialize(_cardData);
        _isFaceUp = true;
        transform.localScale = Vector2.one * _debugCardScale;
      }

      Errors.CheckNotNull(_cardBack);
      Errors.CheckNotNull(_cardFront);
      Errors.CheckNotNull(_cardImage);
    }

    public void UpdateCardData(CardData newCardData)
    {
      Errors.CheckNotNull(newCardData);

      _hand = Root.Instance.GetPlayer(PlayerName.User).Hand;

      if (!_isFaceUp)
      {
        _isFaceUp = true;

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

      _cardImage.sprite = Root.Instance.AssetService.Get<Sprite>(newCardData.Image);


      _cost.text = newCardData.StandardCost.ManaCost.ToString();
      var addIndex = 0;
      foreach (var influence in newCardData.StandardCost.InfluenceCost)
      {
        AddInfluence(influence, ref addIndex);
      }

      while (addIndex < _influence.Count)
      {
        _influence[addIndex++].enabled = false;
      }

      if (_cardData?.CanBePlayed != newCardData.CanBePlayed)
      {
        UpdateOutline(newCardData.CanBePlayed);
      }

      _cardData = newCardData;
    }

    public void SetCanPlay(bool canPlay)
    {
      _cardData.CanBePlayed = canPlay;
      UpdateOutline(canPlay);
    }

    public bool PreviewMode
    {
      set => _previewMode = value;
    }

    public void OnPlayed()
    {
      _hand.RemoveFromHand(this);
      Destroy(gameObject);
    }

    void AddInfluence(Influence influence, ref int addIndex)
    {
      for (var i = 0; i < influence.Value; ++i)
      {
        _influence[addIndex].enabled = true;
        _influence[addIndex].sprite = Root.Instance.Prefabs.SpriteForInfluenceType(influence.School);
        addIndex++;
      }
    }

    void UpdateOutline(bool canBePlayed)
    {
      // _outline.enabled = canBePlayed;
      // if (canBePlayed)
      // {
      //   var color = _outline.color;
      //   color.a = 0;
      //   _outline.color = color;
      //   _outline.DOFade(1.0f, 0.1f);
      // }
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
      if (!_previewMode)
      {
        _isDragging = true;
        _initialDragPosition = Root.Instance.MainCamera.ScreenToWorldPoint(Input.mousePosition);
        _initialDragRotation = transform.rotation;
        _initialDragSiblingIndex = transform.GetSiblingIndex();
        transform.SetAsLastSibling();
      }
    }

    public void OnDrag(PointerEventData pointEventData)
    {
      if (_isDragging)
      {
        var mousePosition = Root.Instance.MainCamera.ScreenToWorldPoint(Input.mousePosition);
        transform.position = Input.mousePosition;

        if (_cardData.CanBePlayed && mousePosition.x < Constants.IndicatorRightX &&
            mousePosition.y > Constants.IndicatorBottomY)
        {
          if (!_overBoard)
          {
            gameObject.SetActive(false);
            var creature = CreatureService.Create(_cardData.CreatureData);
            creature.gameObject.AddComponent<CreaturePositionSelector>().Initialize(creature, this);
            _overBoard = true;
          }
        }
        else
        {
          var distanceDragged = Vector2.Distance(mousePosition, _initialDragPosition);

          var t = Mathf.Clamp01(distanceDragged / 5);
          var scale = Mathf.Lerp(_hand.FinalCardScale, 1.2f, t);
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
        var mousePosition = Root.Instance.MainCamera.ScreenToWorldPoint(Input.mousePosition);
        _isDragging = false;
        transform.SetSiblingIndex(_initialDragSiblingIndex);
        _hand.ReturnToHand(this);
      }
    }
  }
}