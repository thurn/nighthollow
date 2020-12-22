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
using Nighthollow.Data;
using Nighthollow.Generated;
using Nighthollow.Interface;
using Nighthollow.Items;
using Nighthollow.Services;
using Nighthollow.Utils;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

#nullable enable

namespace Nighthollow.Components
{
  public sealed class Card : MonoBehaviour,
    IDragHandler, IBeginDragHandler, IEndDragHandler, IPointerEnterHandler, IPointerExitHandler
  {
    [Header("Config")] [SerializeField] bool _debugMode;
    [SerializeField] float _debugCardScale = 0.65f;
    [SerializeField] RectTransform _cardBack = null!;
    [SerializeField] RectTransform _cardFront = null!;
    [SerializeField] Image _cardImage = null!;
    [SerializeField] TextMeshProUGUI _cost = null!;
    [SerializeField] List<Image> _influence = null!;
    [SerializeField] Image _outline = null!;
    [SerializeField] User _user = null!;
    [SerializeField] bool _canPlay;
    [SerializeField] bool _previewMode;
    [SerializeField] bool _initialized;
    [SerializeField] bool _isDragging;
    [SerializeField] bool _overBoard;
    [SerializeField] int _initialDragSiblingIndex;
    [SerializeField] Vector3 _initialDragPosition;
    [SerializeField] Quaternion _initialDragRotation;

    [Header("State")] CreatureData _data = null!;

    public bool PreviewMode
    {
      set => _previewMode = value;
    }

    void Start()
    {
      if (!_initialized && _debugMode)
      {
        _cardBack.gameObject.SetActive(value: true);
        _cardFront.gameObject.SetActive(value: false);
        Initialize(_data.Clone(_user.Data.Stats));
        transform.localScale = Vector2.one * _debugCardScale;
      }

      Errors.CheckNotNull(_cardBack);
      Errors.CheckNotNull(_cardFront);
      Errors.CheckNotNull(_cardImage);
    }

    void Update()
    {
      Errors.CheckNotNull(_data);
      _cardImage.sprite = Database.Instance.Assets.GetImage(Errors.CheckNotNull(_data.BaseType.ImageAddress));

      var manaCost = _data.GetInt(Stat.ManaCost);
      var influenceCost = _data.Stats.Get(Stat.InfluenceCost);
      _canPlay = manaCost <= _user.Mana &&
                 Influence.LessThanOrEqualTo(influenceCost, _user.Data.Stats.Get(Stat.Influence));

      _outline.enabled = _canPlay;
      _cost.text = manaCost.ToString();

      var addIndex = 0;
      foreach (var pair in influenceCost.Values)
      {
        AddInfluence(pair.Key, pair.Value, ref addIndex);
      }

      while (addIndex < _influence.Count)
      {
        _influence[addIndex++].enabled = false;
      }
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
      if (!_previewMode && !Root.Instance.User.GameOver)
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
            gameObject.SetActive(value: false);
            var creature = Root.Instance.CreatureService.CreateUserCreature(_data);
            creature.gameObject.AddComponent<CreaturePositionSelector>().Initialize(creature, this);
            _overBoard = true;
          }
        }
        else
        {
          var distanceDragged = Vector2.Distance(mousePosition, _initialDragPosition);

          var t = Mathf.Clamp01(distanceDragged / 5);
          var scale = Mathf.Lerp(_user.Hand.FinalCardScale, b: 1.2f, t);
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

    public void OnPointerEnter(PointerEventData eventData)
    {
      if (_previewMode)
      {
        var tooltip = CreatureItemTooltip.Create(Database.Instance.UserData.Stats, _data.Item);
        tooltip.XOffset = 64;
        Root.Instance.ScreenController.ShowTooltip(
          tooltip,
          InterfaceUtils.ScreenPointToInterfacePoint(transform.position));
      }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
      if (_previewMode)
      {
        Root.Instance.ScreenController.HideTooltip();
      }
    }

    public void Initialize(CreatureData cardData)
    {
      _cardFront.gameObject.SetActive(value: false);
      _cardBack.gameObject.SetActive(value: true);
      _user = Root.Instance.User;
      _data = cardData;

      DOTween.Sequence()
        .Insert(atPosition: 0, _cardBack.transform.DOLocalRotate(new Vector3(x: 0, y: 90, z: 0), duration: 0.2f))
        .InsertCallback(atPosition: 0.2f, () =>
        {
          _cardFront.gameObject.SetActive(value: true);
          _cardFront.transform.localRotation = Quaternion.Euler(x: 0, y: 90, z: 0);
          _cardBack.gameObject.SetActive(value: false);
        })
        .Insert(atPosition: 0.2f, _cardFront.transform.DOLocalRotate(Vector3.zero, duration: 0.3f));

      _initialized = true;
    }

    public void OnPlayed()
    {
      _user.Hand.RemoveFromHand(this);
      _user.SpendMana(_data.GetInt(Stat.ManaCost));
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
  }
}
