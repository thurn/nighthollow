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

using UnityEngine;

namespace Nighthollow.Components
{
  public sealed class Attachment : MonoBehaviour
  {
#pragma warning disable 0649
    [Header("Config")] [SerializeField] SpriteRenderer _spriteRenderer;
#pragma warning restore 0649

    public void Initialize(Sprite sprite)
    {
      _spriteRenderer.sprite = sprite;
      transform.localScale = 0.5f * Vector2.one;
    }

    public void SetColor(Color color)
    {
      _spriteRenderer.color = color;
    }
  }
}