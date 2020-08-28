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
using Nighthollow.Data;
using Nighthollow.Services;
using Nighthollow.Utils;
using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Nighthollow.Components
{
  public sealed class Card : MonoBehaviour, IDragHandler, IBeginDragHandler, IEndDragHandler
  {
    [Header("Config")]
    [SerializeField] bool _debugMode;
    [SerializeField] float _debugCardScale = 0.65f;
    [SerializeField] RectTransform _cardBack;
    [SerializeField] RectTransform _cardFront;
    [SerializeField] Image _cardImage;
    [SerializeField] TextMeshProUGUI _cost;
    [SerializeField] List<Image> _influence;
    [SerializeField] Image _outline;

    [Header("State")]
    [SerializeField] CardData _data;
    [SerializeField] User _user;
    [SerializeField] bool _canPlay;
    [SerializeField] bool _disableDragging;
    [SerializeField] bool _initialized;
    [SerializeField] bool _isDragging;
    [SerializeField] bool _overBoard;
    [SerializeField] int _initialDragSiblingIndex;
    [SerializeField] Vector3 _initialDragPosition;
    [SerializeField] Quaternion _initialDragRotation;

    public void Initialize(CardData cardData)
    {
      _cardFront.gameObject.SetActive(false);
      _cardBack.gameObject.SetActive(true);
      _user = Root.Instance.User;
      _data = cardData;

      DOTween.Sequence()
        .Insert(0, _cardBack.transform.DOLocalRotate(new Vector3(0, 90, 0), 0.2f))
        .InsertCallback(0.2f, () =>
        {
          _cardFront.gameObject.SetActive(true);
          _cardFront.transform.localRotation = Quaternion.Euler(0, 90, 0);
          _cardBack.gameObject.SetActive(false);
        })
        .Insert(0.2f, _cardFront.transform.DOLocalRotate(Vector3.zero, 0.3f));

      _initialized = true;
    }

    void Start()
    {
      if (!_initialized && _debugMode)
      {
        _cardBack.gameObject.SetActive(true);
        _cardFront.gameObject.SetActive(false);
        Initialize(Instantiate(_data));
        transform.localScale = Vector2.one * _debugCardScale;
      }

      Errors.CheckNotNull(_cardBack);
      Errors.CheckNotNull(_cardFront);
      Errors.CheckNotNull(_cardImage);
    }

    void Update()
    {
      Errors.CheckNotNull(_data);
      _cardImage.sprite = _data.Image;

      _canPlay = _data.Cost.ManaCost <= _user.Mana &&
        _data.Cost.InfluenceCost.LessThanOrEqual(_user.Influence);

      _outline.enabled = _canPlay;
      _cost.text = _data.Cost.ManaCost.ToString();

      var addIndex = 0;
      foreach (School school in Influence.AllSchools)
      {
        AddInfluence(school, _data.Cost.InfluenceCost.Get(school).Value, ref addIndex);
      }

      while (addIndex < _influence.Count)
      {
        _influence[addIndex++].enabled = false;
      }
    }

    public bool DisableDragging
    {
      set => _disableDragging = value;
    }

    public void OnPlayed()
    {
      _user.Hand.RemoveFromHand(this);
      _user.SpendMana(_data.Cost.ManaCost);
      Destroy(gameObject);
    }

    void AddInfluence(School school, int influence, ref int addIndex)
    {
      for (var i = 0; i < influence; ++i)
      {
        _influence[addIndex].enabled = true;
        _influence[addIndex].sprite = Root.Instance.Prefabs.SpriteForInfluenceType(school);
        addIndex++;
      }
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
      if (!_disableDragging)
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

        if (_canPlay && mousePosition.x < Constants.IndicatorRightX &&
            mousePosition.y > Constants.IndicatorBottomY)
        {
          if (!_overBoard)
          {
            gameObject.SetActive(false);
            var creature = Root.Instance.CreatureService.CreateUserCreature(_data.Creature);
            creature.gameObject.AddComponent<CreaturePositionSelector>().Initialize(creature, this);
            _overBoard = true;
          }
        }
        else
        {
          var distanceDragged = Vector2.Distance(mousePosition, _initialDragPosition);

          var t = Mathf.Clamp01(distanceDragged / 5);
          var scale = Mathf.Lerp(_user.Hand.FinalCardScale, 1.2f, t);
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
        _user.Hand.AnimateCardsToPosition();
      }
    }
  }
}