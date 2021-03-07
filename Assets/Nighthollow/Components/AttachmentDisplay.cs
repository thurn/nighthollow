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
using System.Collections.Immutable;
using System.Linq;
using Nighthollow.Data;
using Nighthollow.Services;
using UnityEngine;

#nullable enable

namespace Nighthollow.Components
{
  public sealed class AttachmentDisplay : MonoBehaviour
  {
    readonly List<Transform> _attachments = new List<Transform>();
    ImmutableList<(StatusEffectTypeData, int)>? _currentStatusEffects;
    AssetService _assetService = null!;

    public void Initialize(AssetService assetService)
    {
      _assetService = assetService;
    }

    public void SetStatusEffects(
      ImmutableList<(StatusEffectTypeData, int)> statusEffects)
    {
      if (_currentStatusEffects == null || !statusEffects.SequenceEqual(_currentStatusEffects))
      {
        ClearAttachments();
        foreach (var effect in statusEffects)
        {
          if (effect.Item1.ImageAddress is { } address)
          {
            var newInstance = Root.Instance.Prefabs.CreateAttachment();
            newInstance.Initialize(_assetService.GetImage(address));
            AddAttachment(newInstance.transform);
          }
        }

        _currentStatusEffects = statusEffects;
      }
    }

    void AddAttachment(Transform attachment)
    {
      attachment.SetParent(transform);
      foreach (var child in attachment.GetComponentsInChildren<SpriteRenderer>())
      {
        child.sortingOrder = 0;
      }

      _attachments.Add(attachment);
      UpdatePositions();
    }

    public void ClearAttachments()
    {
      foreach (var attachment in _attachments)
      {
        Destroy(attachment.gameObject);
      }

      _attachments.Clear();
    }

    void UpdatePositions()
    {
      for (var i = 0; i < _attachments.Count; ++i)
      {
        _attachments[i].transform.localPosition = PositionForAttachment(i % 4);
        _attachments[i].localScale = new Vector2(ScaleForAttachmentCount(), ScaleForAttachmentCount());
      }
    }

    float ScaleForAttachmentCount()
    {
      switch (_attachments.Count)
      {
        case 1:
          return 0.45f;
        case 2:
          return 0.35f;
        default:
          return 0.25f;
      }
    }

    Vector2 PositionForAttachment(int index)
    {
      switch (_attachments.Count)
      {
        case 1:
          return Vector2.zero;
        case 2:
          switch (index)
          {
            case 1:
              return new Vector2(x: -0.25f, y: 0f);
            default:
              return new Vector2(x: 0.25f, y: 0f);
          }

        case 3:
          switch (index)
          {
            case 1:
              return new Vector2(x: -0.2f, y: -0.2f);
            case 2:
              return new Vector2(x: 0.2f, y: -0.2f);
            default:
              return new Vector2(x: -0.2f, y: 0.2f);
          }

        default:
          switch (index)
          {
            case 1:
              return new Vector2(x: -0.2f, y: -0.2f);
            case 2:
              return new Vector2(x: 0.2f, y: -0.2f);
            case 3:
              return new Vector2(x: -0.2f, y: 0.2f);
            default:
              return new Vector2(x: 0.2f, y: 0.2f);
          }
      }
    }
  }
}