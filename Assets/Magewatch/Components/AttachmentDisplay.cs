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

using DG.Tweening;
using UnityEngine;

namespace Magewatch.Components
{
  public sealed class AttachmentDisplay : MonoBehaviour
  {
    [Header("Config")] [SerializeField] int _orderInLayer;

    public void AddAttachment(GameObject attachment)
    {
      attachment.transform.SetParent(transform, worldPositionStays: true);
      foreach (var child in attachment.GetComponentsInChildren<SpriteRenderer>())
      {
        child.sortingOrder = _orderInLayer;
      }

      attachment.transform.DOMove(transform.position, 0.2f);
      attachment.transform.localScale = new Vector3(0.1f, 0.1f);
      attachment.transform.DOScale(new Vector3(0.25f, 0.25f), 0.2f);
    }
  }
}