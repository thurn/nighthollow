// Copyright 2020 © Derek Thurn

// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at

//    https://www.apache.org/licenses/LICENSE-2.0

// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.

using Magewatch.API;
using Magewatch.Services;
using UnityEngine;

namespace Magewatch.Components
{
  public sealed class Attachment : MonoBehaviour
  {
    [Header("Config")] [SerializeField] SpriteRenderer _spriteRenderer;
    [Header("State")] [SerializeField] AttachmentData _attachmentData;

    public void Initialize(AttachmentData attachmentData)
    {
      _attachmentData = attachmentData;
      _spriteRenderer.sprite = Root.Instance.AssetService.Get<Sprite>(attachmentData.Image);
      transform.localScale = 0.5f * Vector2.one;
    }

    public AttachmentData AttachmentData => _attachmentData;

    public void SetColor(Color color)
    {
      _spriteRenderer.color = color;
    }
  }
}