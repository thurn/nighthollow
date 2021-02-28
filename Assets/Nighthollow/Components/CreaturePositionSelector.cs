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
    [SerializeField] RankValue _rank;
    [SerializeField] FileValue _file;
    [SerializeField] GameObject _cursor = null!;
    [SerializeField] List<SpriteRenderer> _spriteRenderers = null!;

    GameServiceRegistry _registry = null!;
    CreatureId _creatureId;

    public void Initialize(GameServiceRegistry registry, CreatureId creatureId, Card? card = null)
    {
      _registry = registry;
      _card = card;
      _creatureId = creatureId;
      registry.CreatureService.SetAnimationPaused(_creatureId, true);
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
      if (!_card || mousePosition.y >= Constants.IndicatorBottomY &&
        mousePosition.x <= Constants.IndicatorRightX)
      {
        var (rank, file) = GetClosestAvailablePosition(_registry.CreatureService, mousePosition);

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
              _registry.CreatureService.SetAnimationPaused(_creatureId, false);
              _registry.CreatureService.AddUserCreatureAtPosition(_creatureId, rank, file);
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

    void SwitchToCard()
    {
      _card!.gameObject.SetActive(value: true);
      _card!.transform.position = Input.mousePosition;

      Destroy(_cursor);
      _registry.CreatureService.DespawnCreature(_creatureId);
    }

    /// <summary>Gets the position closest file to 'filePosition' which is not full.</summary>
    public static (RankValue, FileValue) GetClosestAvailablePosition(ICreatureService service, Vector2 position)
    {
      RankValue? closestRank = null;
      FileValue? closestFile = null;
      var closestDistance = float.MaxValue;

      foreach (var rank in BoardPositions.AllRanks)
      foreach (var file in BoardPositions.AllFiles)
      {
        if (rank == RankValue.Unknown ||
            file == FileValue.Unknown ||
            service.PlacedCreatures.ContainsKey((rank, file)))
        {
          continue;
        }

        var distance = Vector2.Distance(position,
          new Vector2(rank.ToXPosition(), file.ToYPosition()));
        if (distance < closestDistance)
        {
          closestDistance = distance;
          closestRank = rank;
          closestFile = file;
        }
      }

      if (closestRank == null || closestFile == null)
      {
        throw new InvalidOperationException("Board is full!");
      }

      return (closestRank.Value, closestFile.Value);
    }
  }
}
