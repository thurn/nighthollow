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

#nullable enable

using System;
using DG.Tweening;
using UnityEngine.UIElements;

namespace Nighthollow.Interface
{
  public static class InterfaceUtils
  {
    public static void FadeIn(VisualElement ve, float duration = 2f, Action? onComplete = null)
    {
      var sequence = DOTween.Sequence()
        .Append(DOTween.To(() => ve.style.opacity.value, x => ve.style.opacity = x, 1f, duration));
      if (onComplete != null)
      {
        sequence.AppendCallback(() => onComplete());
      }
    }

    public static void FadeOut(VisualElement ve, float duration = 2f, Action? onComplete = null)
    {
      var sequence = DOTween.Sequence()
        .Append(DOTween.To(() => ve.style.opacity.value, x => ve.style.opacity = x, 0f, duration));
      if (onComplete != null)
      {
        sequence.AppendCallback(() => onComplete());
      }
    }
  }
}