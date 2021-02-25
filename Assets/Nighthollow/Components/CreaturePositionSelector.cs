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
using Nighthollow.Services;
using Nighthollow.Utils;
using UnityEngine;

#nullable enable

namespace Nighthollow.Components
{
  public sealed class CreaturePositionSelector : MonoBehaviour
  {
    [Header("State")] [SerializeField] Card? _card;
    [SerializeField] Creature2 _creature = null!;
    [SerializeField] RankValue _rank;
    [SerializeField] FileValue _file;
    [SerializeField] CreatureService _creatureService = null!;
    [SerializeField] GameObject _cursor = null!;
    [SerializeField] List<SpriteRenderer> _spriteRenderers = null!;

    void Update()
    {
      var mousePosition = Root.Instance.MainCamera.ScreenToWorldPoint(Input.mousePosition);
      if (!_card || mousePosition.y >= Constants.IndicatorBottomY &&
        mousePosition.x <= Constants.IndicatorRightX)
      {
        var (rank, file) = _creatureService.GetClosestAvailablePosition(mousePosition);

        if (Input.GetMouseButtonUp(button: 0))
        {
          if (_card)
          {
            _card!.OnPlayed();
          }

          Destroy(_cursor);
          Destroy(this);

          foreach (var spriteRenderer in _spriteRenderers)
          {
            spriteRenderer.color = Color.white;
          }

          DOTween.Sequence()
            .Append(transform.DOMove(new Vector3(rank.ToXPosition(), file.ToYPosition(), z: 0), duration: 0.3f))
            .AppendCallback(() =>
            {
              _creature.AnimationPaused = false;
              _creatureService.AddUserCreatureAtPosition(_creature, rank, file);
            });
        }
        else
        {
          if (rank != _rank || file != _file)
          {
            var position = new Vector3(rank.ToXPosition(), file.ToYPosition(), z: 0);
            _cursor.transform.position = position + new Vector3(x: 0, y: 1, z: 0);
            _rank = rank;
            _file = file;
          }

          transform.position = Vector2.one * mousePosition;
        }
      }
      else if (_card)
      {
        SwitchToCard();
      }
    }

    public void Initialize(Creature2 creature, Card? card = null)
    {
      _card = card;
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

    void SwitchToCard()
    {
      _card!.gameObject.SetActive(value: true);
      _card!.transform.position = Input.mousePosition;

      Destroy(_cursor);
      _creature.DestroyCreature();
    }
  }
}
