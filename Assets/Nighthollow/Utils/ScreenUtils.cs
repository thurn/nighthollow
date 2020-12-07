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


using Nighthollow.Services;
using UnityEngine;

#nullable enable

namespace Nighthollow.Utils
{
  public static class ScreenUtils
  {
    const int ReferencePixelsPerUnit = 100;

    public static Vector2 WorldToCanvasPosition(Vector2 worldPosition) => ReferencePixelsPerUnit * worldPosition;

    public static Vector2 WorldToCanvasAnchorPosition(Vector2 worldPosition)
    {
      var canvasRect = Root.Instance.MainCanvas;
      var viewportPosition = Root.Instance.MainCamera.WorldToViewportPoint(worldPosition);
      return new Vector2(
        viewportPosition.x * canvasRect.sizeDelta.x - canvasRect.sizeDelta.x * 0.5f,
        viewportPosition.y * canvasRect.sizeDelta.y - canvasRect.sizeDelta.y * 0.5f);
    }
  }
}
