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

using System;
using System.Collections.Generic;
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
    [SerializeField] RectTransform _cardBack;
    [SerializeField] RectTransform _cardFront;
    [SerializeField] Image _cardImage;
    [SerializeField] TextMeshProUGUI _name;
    [SerializeField] TextMeshProUGUI _cost;
    [SerializeField] TextMeshProUGUI _text;
    [SerializeField] List<Image> _influence;
    [SerializeField] Image _outline;
    [SerializeField] CardData _cardData;
    [SerializeField] Hand _hand;

    bool _initialized;
    bool _isFaceUp;
    bool _isRevealed;
    bool _canBePlayed;
    bool _isDragging;
    bool _isOverBoard;
    int _initialDragSiblingIndex;
    Vector3 _initialDragPosition;
    Quaternion _initialDragRotation;
    string _currentName;
    string _currentText;

    public void Initialize(CardData card)
    {
      name = card.Name;
      _cardFront.gameObject.SetActive(false);
      _cardBack.gameObject.SetActive(true);
      _outline.enabled = false;
      _hand = Root.Instance.GetPlayer(card.Owner).Hand;
      // Get(Keys.CardImage).RenderCardImage(_cardData, _cardImage);
      _initialized = true;
    }

    public void OnPlayed()
    {
      _hand.RemoveFromHand(this);
      // Get(Keys.CardImage).DestroyCardImage(_cardData);
      Destroy(gameObject);
    }

    public void ReturnToHand()
    {
      _isDragging = false;
      transform.SetSiblingIndex(_initialDragSiblingIndex);
      _hand.ReturnToHand(this);
    }

    public bool CanBePlayed()
    {
      return _cardData.CanBePlayed;
    }

    void Start()
    {
      if (!_initialized)
      {
        if (_debugMode)
        {
          Initialize(_cardData);
          _isFaceUp = _cardData.Owner == PlayerName.User;
          transform.localScale = Vector2.one * 0.65f;
        }
        else
        {
          throw Errors.MustInitialize(name);
        }
      }

      Errors.CheckNotNull(_cardData);
      Errors.CheckNotNull(_hand);
      Errors.CheckNotNull(_cardBack);
      Errors.CheckNotNull(_cardFront);
      Errors.CheckNotNull(_cardImage);
    }

    void Update()
    {
      if (!_isFaceUp && _isRevealed)
      {
        _isFaceUp = true;
        var cardName = _cardData.Name;
        if (_currentName != cardName)
        {
          _name.text = cardName;
          _currentName = cardName;
        }

        var cardText = _cardData.Text;
        if (_currentText != cardText)
        {
          _text.text = cardText;
          _currentText = cardText;
        }

        // DOTween.Sequence()
        //   .Insert(atPosition: 0, _cardBack.transform.DOLocalRotate(new Vector3(x: 0, y: 90, z: 0), duration: 0.2f))
        //   .InsertCallback(atPosition: 0.2f, () =>
        //   {
        //     _cardFront.gameObject.SetActive(value: true);
        //     _cardFront.transform.localRotation = Quaternion.Euler(x: 0, y: 90, z: 0);
        //     _cardBack.gameObject.SetActive(value: false);
        //   })
        //   .Insert(atPosition: 0.2f, _cardFront.transform.DOLocalRotate(Vector3.zero, duration: 0.3f));
      }
      else if (_isFaceUp && !_isRevealed)
      {
        throw new InvalidOperationException("Cannot hide revealed card");
      }

      _cost.text = _cardData.ManaCost.ToString();

      var addIndex = 0;
      AddInfluence(_cardData.Influence.Light, ref addIndex);
      AddInfluence(_cardData.Influence.Sky, ref addIndex);
      AddInfluence(_cardData.Influence.Flame, ref addIndex);
      AddInfluence(_cardData.Influence.Ice, ref addIndex);
      AddInfluence(_cardData.Influence.Earth, ref addIndex);
      AddInfluence(_cardData.Influence.Shadow, ref addIndex);
      while (addIndex < _influence.Count)
      {
        _influence[addIndex++].enabled = false;
      }

      if (_cardFront.gameObject.activeSelf)
      {
        var canBePlayed = CanBePlayed();

        if (_canBePlayed != canBePlayed)
        {
          _outline.enabled = canBePlayed;
          if (canBePlayed)
          {
            var color = _outline.color;
            color.a = 0;
            _outline.color = color;
            // _outline.DOFade(1.0f, 0.3f);
          }

          _canBePlayed = canBePlayed;
        }
      }
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
      if (_cardData.Owner == PlayerName.User)
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
              maxDistance: 100) /*&& hit.collider.CompareTag(Tags.Board)*/)
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