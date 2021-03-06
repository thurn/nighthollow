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


using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using DG.Tweening;
using Nighthollow.Services;
using UnityEngine;

#nullable enable

namespace Nighthollow.Components
{
  public sealed class Hand : MonoBehaviour
  {
    public const float FinalCardScale = 0.6f;

    [SerializeField] Transform _deckPosition = null!;
    [SerializeField] float _initialCardScale;
    [SerializeField] int _zRotationMultiplier;
    [SerializeField] Transform _controlPoint1 = null!;
    [SerializeField] Transform _controlPoint2 = null!;
    [SerializeField] Transform _controlPoint3 = null!;
    [SerializeField] Transform _controlPoint4 = null!;
    [SerializeField] Hand _handOverridePosition = null!;

    public ImmutableList<(int, Card)> Cards { get; private set; } = ImmutableList<(int, Card)>.Empty;

    bool _previewMode;
    bool _shouldOverrideHandPosition;

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

    /// <summary>
    /// Synchronizes the displayed hand with the logical 'cardId -> data' model provided, drawing cards if they are
    /// missing from the displayed content and destroying them if they are missing from the input.
    /// </summary>
    public void SynchronizeHand(BattleServiceRegistry registry, Action? onComplete = null)
    {
      StartCoroutine(SynchronizeHandAsync(registry, onComplete));
    }

    IEnumerator<YieldInstruction> SynchronizeHandAsync(BattleServiceRegistry registry, Action? onComplete = null)
    {
      var hand = registry.UserService.Hand;
      var newIds = hand.Keys.ToImmutableHashSet();
      var cards = ImmutableList.CreateBuilder<(int, Card)>();
      foreach (var (cardId, card) in Cards)
      {
        newIds = newIds.Remove(cardId);
        if (hand.ContainsKey(cardId))
        {
          // Card is still present
          cards.Add((cardId, card));
        }
        else
        {
          // Card is no longer in hand
          Destroy(card);
        }
      }

      Cards = cards.ToImmutable();

      foreach (var cardId in newIds)
      {
        // Card should be drawn
        var card = registry.Prefabs.CreateCard();
        card.Initialize(registry.MainCamera, registry.AssetService, registry.UserController, cardId, hand[cardId]);
        card.transform.position = _deckPosition.position;
        card.transform.localScale = Vector2.one * _initialCardScale;
        card.transform.SetParent(transform);
        Cards = Cards.Add((cardId, card));
        card.PreviewMode = _previewMode;
        AnimateCardsToPosition();
        yield return new WaitForSeconds(seconds: 0.2f);
      }

      AnimateCardsToPosition(onComplete);
    }

    public void OverrideHandPosition(bool value, Action? onComplete = null)
    {
      _shouldOverrideHandPosition = value;
      AnimateCardsToPosition(onComplete);
    }

    public void SetCardsToPreviewMode(bool value)
    {
      _previewMode = value;

      foreach (var (_, card) in Cards)
      {
        card.PreviewMode = value;
      }
    }

    void AnimateCardsToPosition(Action? onComplete = null)
    {
      var sequence = DOTween.Sequence();
      for (var i = 0; i < Cards.Count; ++i)
      {
        var card = Cards[i].Item2;
        var curvePosition = CalculateCurvePosition(i);
        var t = card.transform;
        t.SetSiblingIndex(i);
        sequence.Insert(atPosition: 0, t.DOScale(FinalCardScale, duration: 0.3f));
        sequence.Insert(atPosition: 0,
          t.DOMove(
            _shouldOverrideHandPosition
              ? _handOverridePosition!.CalculateBezierPosition(curvePosition)
              : CalculateBezierPosition(curvePosition), duration: 0.3f));
        sequence.Insert(atPosition: 0,
          t.DOLocalRotate(new Vector3(x: 0, y: 0,
            _zRotationMultiplier * CalculateZRotation(curvePosition)), duration: 0.3f));
      }

      sequence.AppendCallback(() => onComplete?.Invoke());
    }

    float CalculateCurvePosition(int cardIndex)
    {
      if (cardIndex < 0 || cardIndex >= Cards.Count)
      {
        throw new ArgumentException("Index out of bounds");
      }

      switch (Cards.Count)
      {
        case 1:
          return 0.5f;
        case 2:
          return PositionWithinRange(start: 0.4f, end: 0.6f, cardIndex, Cards.Count);
        case 3:
          return PositionWithinRange(start: 0.3f, end: 0.7f, cardIndex, Cards.Count);
        case 4:
          return PositionWithinRange(start: 0.2f, end: 0.8f, cardIndex, Cards.Count);
        case 5:
          return PositionWithinRange(start: 0.15f, end: 0.85f, cardIndex, Cards.Count);
        case 6:
          return PositionWithinRange(start: 0.1f, end: 0.9f, cardIndex, Cards.Count);
        default:
          return PositionWithinRange(start: 0.0f, end: 1.0f, cardIndex, Cards.Count);
      }
    }

    // Given a start,end range on the 0,1 line, returns the position within that range where card 'index' of of
    // 'count' total cards should be positioned
    float PositionWithinRange(float start, float end, int index, int count) =>
      start + index * ((end - start) / (count - 1.0f));

    // Card rotation ranges from 5 to -5
    float CalculateZRotation(float t) => -10.0f * t + 5.0f;

    Vector3 CalculateBezierPosition(float t) =>
      Mathf.Pow(1 - t, p: 3) * _controlPoint1.position +
      3 * Mathf.Pow(1 - t, p: 2) * t * _controlPoint2.position +
      3 * (1 - t) * Mathf.Pow(t, p: 2) * _controlPoint3.position +
      Mathf.Pow(t, p: 3) * _controlPoint4.position;
  }
}