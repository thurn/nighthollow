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
using System.Linq;
using System.Threading.Tasks;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UIElements;

#nullable enable

namespace Nighthollow.Interface
{
  public static class InterfaceUtils
  {
    public static void FadeIn(VisualElement ve, float duration = 2f, Action? onComplete = null)
    {
      ve.style.opacity = 0f;
      var sequence = DOTween.Sequence().SetUpdate(isIndependentUpdate: true)
        .Append(DOTween
          .To(() => ve.style.opacity.value, x => ve.style.opacity = x, endValue: 1f, duration)
          .SetUpdate(isIndependentUpdate: true));
      if (onComplete != null)
      {
        sequence.AppendCallback(() => onComplete());
      }
    }

    public static void FadeOut(VisualElement ve, float duration = 2f, Action? onComplete = null)
    {
      ve.style.opacity = 1f;
      var sequence = DOTween.Sequence().SetUpdate(true)
        .Append(DOTween
          .To(() => ve.style.opacity.value, x => ve.style.opacity = x, endValue: 0f, duration)
          .SetUpdate(isIndependentUpdate: true));
      if (onComplete != null)
      {
        sequence.AppendCallback(() => onComplete());
      }
    }

    public static T FindByName<T>(VisualElement parent, string name) where T : class
    {
      var element = parent.Q(name);
      if (element == null)
      {
        throw new NullReferenceException($"Expected to find an element named {name}");
      }

      if (!(element is T result))
      {
        throw new ArgumentException($"Expected element {name} to be of type {typeof(T)}");
      }

      return result;
    }

    public static bool ContainsScreenPoint(VisualElement element, Vector2 point) =>
      // This is supposed to be just element.ContainsPoint(element.WorldToLocal(point)), but that doesn't work
      element.ContainsPoint(element.WorldToLocal(ScreenPointToInterfacePoint(point)));

    public static Vector2 ScreenPointToInterfacePoint(Vector2 screenPoint) =>
      // Not sure if there's a less hacky way to convert to GUI coordinates than Screen.height - y...
      new Vector2(screenPoint.x, Screen.height - screenPoint.y);

    public static void After(float seconds, Action action)
    {
      if (Application.isPlaying)
      {
        DOTween.Sequence().SetUpdate(isIndependentUpdate: true).InsertCallback(seconds, () => action()).Play();
      }
      else
      {
        InvokeAfterInEditor(seconds, action);
      }
    }

    static async void InvokeAfterInEditor(float seconds, Action action)
    {
      await Task.Delay((int) (1000f * seconds));
      action();
    }

    public static void FocusTextField(TextField field)
    {
      // TextField needs time after setting focusable and calling Focus() before it works, but there is no obvious
      // event which corresponds to these operations completing.
      field.focusable = true;
      After(0.01f, () =>
      {
        field.Focus();
        After(0.01f, () =>
        {
          // For some reason SelectAll() only works if you SelectRange() first?
          field.SelectRange(0, 1);
          field.SelectAll();
        });
      });
    }

    public static VisualElement FirstLeaf(VisualElement visualElement) =>
      visualElement.childCount == 0 ? visualElement : FirstLeaf(visualElement.Children().First());
  }
}