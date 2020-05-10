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
using DG.Tweening;
using Magewatch.Data;
using Magewatch.Services;
using UnityEngine;

namespace Magewatch.Components
{
  public sealed class AttachmentDisplay : MonoBehaviour
  {
    [Header("State")] [SerializeField] List<Transform> _attachments;

    public void AddAttachment(Attachment attachment, Action onComplete = null, bool animate = true)
    {
      attachment.transform.SetParent(transform, worldPositionStays: true);
      foreach (var child in attachment.GetComponentsInChildren<SpriteRenderer>())
      {
        child.sortingOrder = 0;
      }

      _attachments.Add(attachment.transform);
      UpdatePositions(onComplete, animate);
    }

    public void SetAttachments(IEnumerable<AttachmentData> attachments)
    {
      foreach (var attachment in _attachments)
      {
        Destroy(attachment.gameObject);
      }

      _attachments.Clear();

      foreach (var attachment in attachments)
      {
        var newInstance = Root.Instance.Prefabs.CreateAttachment();
        newInstance.Initialize(attachment);
        AddAttachment(newInstance, null, false);
      }
    }

    void UpdatePositions(Action onComplete = null, bool animate = true)
    {
      var sequence = DOTween.Sequence();
      for (var i = 0; i < _attachments.Count; ++i)
      {
        if (animate)
        {
          sequence.Insert(0, _attachments[i].transform.DOLocalMove(PositionForAttachment(i % 4), 0.2f));
          sequence.Insert(0,
            _attachments[i].DOScale(new Vector2(ScaleForAttachmentCount(), ScaleForAttachmentCount()), 0.2f));
        }
        else
        {
          _attachments[i].transform.localPosition = PositionForAttachment(i % 4);
          _attachments[i].localScale = new Vector2(ScaleForAttachmentCount(), ScaleForAttachmentCount());
        }
      }

      sequence.AppendCallback(() => onComplete?.Invoke());
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
              return new Vector2(-0.25f, 0f);
            default:
              return new Vector2(0.25f, 0f);
          }
        case 3:
          switch (index)
          {
            case 1:
              return new Vector2(-0.2f, -0.2f);
            case 2:
              return new Vector2(0.2f, -0.2f);
            default:
              return new Vector2(-0.2f, 0.2f);
          }
        default:
          switch (index)
          {
            case 1:
              return new Vector2(-0.2f, -0.2f);
            case 2:
              return new Vector2(0.2f, -0.2f);
            case 3:
              return new Vector2(-0.2f, 0.2f);
            default:
              return new Vector2(0.2f, 0.2f);
          }
      }
    }
  }
}