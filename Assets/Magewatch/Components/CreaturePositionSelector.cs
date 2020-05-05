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
using UnityEngine;

namespace Magewatch.Components
{
  public sealed class CreaturePositionSelector : MonoBehaviour
  {
    [SerializeField] Card _card;
    [SerializeField] Creature _creature;
    [SerializeField] RankValue _rank;
    [SerializeField] FileValue _file;
    [SerializeField] CreatureService _creatureService;
    [SerializeField] GameObject _cursor;
    [SerializeField] List<SpriteRenderer> _spriteRenderers;

    public void Initialize(Card card, Creature creature)
    {
      _card = Errors.CheckNotNull(card);
      _card.gameObject.SetActive(false);
      _creature = creature;
      _creature.AnimationPaused = true;
      _creatureService = Root.Instance.CreatureService;
      _cursor = Root.Instance.Prefabs.CreateCursor().gameObject;

      _spriteRenderers = new List<SpriteRenderer>();
      foreach (var spriteRenderer in GetComponentsInChildren<SpriteRenderer>())
      {
        spriteRenderer.color = Color.gray;
        _spriteRenderers.Add(spriteRenderer);
      }
    }

    void Update()
    {
      var mousePosition = Root.Instance.MainCamera.ScreenToWorldPoint(Input.mousePosition);
      if (mousePosition.y >= Constants.IndicatorBottomY &&
          mousePosition.x <= Constants.IndicatorRightX)
      {
        var rank = BoardPositions.ClosestRankForXPosition(mousePosition.x);
        var file = _creatureService.GetClosestAvailableFile(BoardPositions.ClosestFileForYPosition(mousePosition.y));

        if (Input.GetMouseButtonUp(0))
        {
          _card.OnPlayed();
          Destroy(_cursor);
          Destroy(this);

          foreach (var spriteRenderer in _spriteRenderers)
          {
            spriteRenderer.color = Color.white;
          }

          transform.DOMove(new Vector3(rank.ToXPosition(), file.ToYPosition(), 0), 0.3f);
        }
        else
        {
          if (rank != _rank || file != _file)
          {
            _creatureService.ShiftPositions(rank, file);
            var position = new Vector3(rank.ToXPosition(), file.ToYPosition(), 0);
            _cursor.transform.position = position;
            _rank = rank;
            _file = file;
          }

          transform.position = Vector2.one * mousePosition;
        }
      }
      else
      {
        SwitchToCard();
      }
    }

    void SwitchToCard()
    {
      _card.gameObject.SetActive(true);
      _card.transform.position = Input.mousePosition;

      Destroy(_cursor);
      _creature.Destroy();
    }
  }
}