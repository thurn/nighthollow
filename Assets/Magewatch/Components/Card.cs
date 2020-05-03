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

using System.Collections.Generic;
using DG.Tweening;
using Magewatch.Data;
using Magewatch.Services;
using Magewatch.Utils;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Magewatch.Components
{
  public sealed class Card : MonoBehaviour, IDragHandler, IBeginDragHandler, IEndDragHandler
  {
    [SerializeField] bool _debugMode;
    [SerializeField] float _debugCardScale = 0.65f;

    [Header("Config")] [SerializeField] RectTransform _cardBack;
    [SerializeField] RectTransform _cardFront;
    [SerializeField] Image _cardImage;
    [SerializeField] TextMeshProUGUI _name;
    [SerializeField] TextMeshProUGUI _cost;
    [SerializeField] TextMeshProUGUI _text;
    [SerializeField] List<Image> _influence;
    [SerializeField] Image _outline;

    [Header("Internal")] [SerializeField] CardData _cardData;
    [SerializeField] Hand _hand;
    [SerializeField] bool _previewMode;
    [SerializeField] bool _initialized;
    [SerializeField] bool _isFaceUp;
    [SerializeField] bool _isDragging;
    [SerializeField] bool _isOverBoard;
    [SerializeField] int _initialDragSiblingIndex;
    [SerializeField] Vector3 _initialDragPosition;
    [SerializeField] Quaternion _initialDragRotation;

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
        _isFaceUp = _cardData.Owner == PlayerName.User;
        transform.localScale = Vector2.one * _debugCardScale;
      }

      Errors.CheckNotNull(_cardBack);
      Errors.CheckNotNull(_cardFront);
      Errors.CheckNotNull(_cardImage);
    }

    public bool PreviewMode
    {
      get => _previewMode;
      set => _previewMode = value;
    }

    void OnPlayed()
    {
      _hand.RemoveFromHand(this);
      // DestroyCardImage(_cardData);
      Destroy(gameObject);
    }

    void ReturnToHand()
    {
      _isDragging = false;
      transform.SetSiblingIndex(_initialDragSiblingIndex);
      _hand.ReturnToHand(this);
    }

    bool CanBePlayed()
    {
      return _cardData.CanBePlayed;
    }

    void UpdateCardData(CardData newCardData)
    {
      Errors.CheckNotNull(newCardData);

      if (_cardData?.Owner != newCardData.Owner)
      {
        _hand = Root.Instance.GetPlayer(newCardData.Owner).Hand;
      }

      // RenderCardImage(_cardData, _cardImage);

      if (!_isFaceUp && newCardData.IsRevealed)
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
      else if (_isFaceUp && !newCardData.IsRevealed)
      {
        _isFaceUp = false;

        DOTween.Sequence()
          .Insert(atPosition: 0, _cardBack.transform.DOLocalRotate(new Vector3(x: 0, y: 90, z: 0), duration: 0.2f))
          .InsertCallback(atPosition: 0.2f, () =>
          {
            _cardBack.gameObject.SetActive(value: true);
            _cardBack.transform.localRotation = Quaternion.Euler(x: 0, y: 90, z: 0);
            _cardFront.gameObject.SetActive(value: false);
          })
          .Insert(atPosition: 0.2f, _cardFront.transform.DOLocalRotate(Vector3.zero, duration: 0.3f));
      }

      if (_cardData?.Name != newCardData.Name)
      {
        name = newCardData.Name;
        _name.text = newCardData.Name;
      }

      if (_cardData?.Text != newCardData.Text)
      {
        _text.text = newCardData.Text;
      }

      if (_cardData?.ManaCost != newCardData.ManaCost)
      {
        _cost.text = newCardData.ManaCost.ToString();
      }

      if (_cardData?.Influence != newCardData.Influence)
      {
        var addIndex = 0;
        AddInfluence(newCardData.Influence.Light, ref addIndex);
        AddInfluence(newCardData.Influence.Sky, ref addIndex);
        AddInfluence(newCardData.Influence.Flame, ref addIndex);
        AddInfluence(newCardData.Influence.Ice, ref addIndex);
        AddInfluence(newCardData.Influence.Earth, ref addIndex);
        AddInfluence(newCardData.Influence.Shadow, ref addIndex);
        while (addIndex < _influence.Count)
        {
          _influence[addIndex++].enabled = false;
        }
      }

      if (_cardData?.CanBePlayed != newCardData.CanBePlayed)
      {
        _outline.enabled = newCardData.CanBePlayed;
        if (newCardData.CanBePlayed)
        {
          var color = _outline.color;
          color.a = 0;
          _outline.color = color;
          _outline.DOFade(1.0f, 0.3f);
        }
      }

      _cardData = newCardData;
    }

    void AddInfluence(int value, ref int addIndex)
    {
      for (var i = 0; i < value; ++i)
      {
        _influence[addIndex].enabled = true;
        // _influence[addIndex].sprite = Injector.Instance.Prefabs.SpriteForInfluenceType(key);
        addIndex++;
      }
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
      if (_cardData.Owner == PlayerName.User && !_previewMode)
      {
        _isDragging = true;
        _initialDragPosition = transform.position;
        _initialDragRotation = transform.rotation;
        _initialDragSiblingIndex = transform.GetSiblingIndex();
        transform.SetAsLastSibling();
      }
    }

    public void OnDrag(PointerEventData eventData)
    {
      if (_isDragging)
      {
        if (CanBePlayed() &&
            !_isOverBoard &&
            Physics.Raycast(
              Root.Instance.MainCamera.ScreenPointToRay(Input.mousePosition),
              out var hit,
              maxDistance: 100) && hit.collider.CompareTag(Tags.Board))
        {
          _isOverBoard = true;
        }
        else
        {
          var distanceDragged = transform.position.y - _initialDragPosition.y;
          transform.position = Input.mousePosition;

          var t = Mathf.Clamp(distanceDragged / 200, min: 0f, max: 1f);
          var scale = Mathf.Lerp(a: 0.75f, b: 0.85f, t);
          transform.localScale = new Vector3(scale, scale, z: 1);

          var rotation = Quaternion.Slerp(_initialDragRotation, Quaternion.identity, t);
          transform.rotation = rotation;
          _isOverBoard = false;
        }
      }
    }

    public void OnEndDrag(PointerEventData eventData)
    {
      if (_isDragging)
      {
        if (_isOverBoard)
        {
          OnPlayed();
        }
        else
        {
          ReturnToHand();
        }
      }
    }
  }
}