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

#nullable enable

using System;
using System.Collections.Generic;
using DG.Tweening;
using Nighthollow.Data;
using Nighthollow.Services;
using UnityEngine;

namespace Nighthollow.Components
{
  public sealed class Hand : MonoBehaviour
  {
#pragma warning disable 0649

    [Header("Config")]
    [SerializeField] bool _debugMode;
    [SerializeField] Transform _deckPosition = null!;
    [SerializeField] float _initialCardScale;
    [SerializeField] float _finalCardScale;
    [SerializeField] int _zRotationMultiplier;

    [Header("State")]
    [SerializeField] Transform _controlPoint1 = null!;
    [SerializeField] Transform _controlPoint2 = null!;
    [SerializeField] Transform _controlPoint3 = null!;
    [SerializeField] Transform _controlPoint4 = null!;
    [SerializeField] List<Card> _cards = null!;
    [SerializeField] Hand _handOverridePosition = null!;
    bool _previewMode;

#pragma warning restore 0649

    public float FinalCardScale => _finalCardScale;

    void Start()
    {
      if (_debugMode)
      {
        var children = GetComponentsInChildren<Card>();
        foreach (var child in children)
        {
          _cards.Add(child);
        }

        AnimateCardsToPosition();
      }
    }

    public void DrawCards(IEnumerable<CreatureData> cards, Action? onComplete = null)
    {
      StartCoroutine(DrawsCardAsync(cards, onComplete));
    }

    IEnumerator<YieldInstruction> DrawsCardAsync(IEnumerable<CreatureData> cards, Action? onComplete)
    {
      foreach (var cardData in cards)
      {
        var card = Root.Instance.Prefabs.CreateCard();
        card.Initialize(cardData);
        card.transform.position = _deckPosition.position;
        card.transform.localScale = Vector2.one * _initialCardScale;
        AddToHand(card);
        yield return new WaitForSeconds(0.2f);
      }

      onComplete?.Invoke();
    }

    public void RemoveFromHand(Card card)
    {
      _cards.Remove(card);
      AnimateCardsToPosition();
    }

    public void AddToHand(Card card, bool animate = true)
    {
      card.transform.SetParent(transform);
      _cards.Add(card);
      card.PreviewMode = _previewMode;

      if (animate)
      {
        AnimateCardsToPosition();
      }
    }

    public bool PreviewMode
    {
      set
      {
        _previewMode = value;

        foreach (var card in _cards)
        {
          card.PreviewMode = value;
        }

        AnimateCardsToPosition();
      }
    }

    public void DestroyAllCards()
    {
      foreach (var card in _cards)
      {
        Destroy(card.gameObject);
      }

      _cards.Clear();
    }

    public void AnimateCardsToPosition(Action? onComplete = null)
    {
      var sequence = DOTween.Sequence();
      for (var i = 0; i < _cards.Count; ++i)
      {
        var curvePosition = CalculateCurvePosition(i);
        var t = _cards[i].transform;
        t.SetSiblingIndex(i);
        sequence.Insert(0, t.DOScale(endValue: _finalCardScale, duration: 0.3f));
        sequence.Insert(0,
          t.DOMove(
            _previewMode
              ? _handOverridePosition!.CalculateBezierPosition(curvePosition)
              : CalculateBezierPosition(curvePosition), duration: 0.3f));
        sequence.Insert(0,
          t.DOLocalRotate(new Vector3(0, 0,
            _zRotationMultiplier * CalculateZRotation(curvePosition)), duration: 0.3f));
      }

      sequence.AppendCallback(() => onComplete?.Invoke());
    }

    void Update()
    {
      if (_debugMode)
      {
        AnimateCardsToPosition();
      }
    }

    void OnDrawGizmosSelected()
    {
      for (var t = 0.0f; t <= 1; t += 0.05f)
      {
        var position = CalculateBezierPosition(t);
        Gizmos.DrawSphere(position, radius: 10);
      }

      Gizmos.color = Color.green;
      Gizmos.DrawSphere(_controlPoint1.position, radius: 10);
      Gizmos.DrawSphere(_controlPoint2.position, radius: 10);
      Gizmos.DrawSphere(_controlPoint3.position, radius: 10);
      Gizmos.DrawSphere(_controlPoint4.position, radius: 10);
    }

    float CalculateCurvePosition(int cardIndex)
    {
      if (cardIndex < 0 || cardIndex >= _cards.Count)
      {
        throw new ArgumentException("Index out of bounds");
      }

      switch (_cards.Count)
      {
        case 1:
          return 0.5f;
        case 2:
          return PositionWithinRange(start: 0.4f, end: 0.6f, cardIndex, _cards.Count);
        case 3:
          return PositionWithinRange(start: 0.3f, end: 0.7f, cardIndex, _cards.Count);
        case 4:
          return PositionWithinRange(start: 0.2f, end: 0.8f, cardIndex, _cards.Count);
        case 5:
          return PositionWithinRange(start: 0.15f, end: 0.85f, cardIndex, _cards.Count);
        case 6:
          return PositionWithinRange(start: 0.1f, end: 0.9f, cardIndex, _cards.Count);
        default:
          return PositionWithinRange(start: 0.0f, end: 1.0f, cardIndex, _cards.Count);
      }
    }

    // Given a start,end range on the 0,1 line, returns the position within that range where card 'index' of of
    // 'count' total cards should be positioned
    float PositionWithinRange(float start, float end, int index, int count) =>
      start + (index * ((end - start) / (count - 1.0f)));

    // Card rotation ranges from 5 to -5
    float CalculateZRotation(float t) => (-10.0f * t) + 5.0f;

    Vector3 CalculateBezierPosition(float t)
    {
      return Mathf.Pow(1 - t, p: 3) * _controlPoint1.position +
             3 * Mathf.Pow(1 - t, p: 2) * t * _controlPoint2.position +
             3 * (1 - t) * Mathf.Pow(t, p: 2) * _controlPoint3.position +
             Mathf.Pow(t, p: 3) * _controlPoint4.position;
    }
  }
}
