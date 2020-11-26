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

using Nighthollow.Utils;
using UnityEngine;
using UnityEngine.UIElements;

namespace Nighthollow.Interface
{
  public abstract class HideableElement : VisualElement
  {
    public ScreenController Controller { get; set; } = null!;
    public bool Visible { get; protected set; }

    public new sealed class UxmlTraits : VisualElement.UxmlTraits
    {
    }

    protected HideableElement()
    {
      if (Application.isPlaying)
      {
        RegisterCallback<GeometryChangedEvent>(OnGeometryChange);
      }
    }

    public void Show(bool animate = false)
    {
      Visible = true;
      style.display = new StyleEnum<DisplayStyle>(DisplayStyle.Flex);

      if (animate)
      {
        style.opacity = new StyleFloat(0f);
        InterfaceUtils.FadeIn(this, 0.3f);
      }
    }

    public void Hide(bool animate = false)
    {
      Visible = false;
      style.display = new StyleEnum<DisplayStyle>(DisplayStyle.None);

      if (animate)
      {
        style.opacity = new StyleFloat(1f);
        InterfaceUtils.FadeOut(this, 0.3f);
      }
    }

    protected abstract void OnRegister();

    protected VisualElement FindElement(string elementName) => Find<VisualElement>(elementName);

    protected T Find<T>(string elementName) where T : class => Errors.CheckNotNull(this.Q(elementName) as T);

    void OnGeometryChange(GeometryChangedEvent evt)
    {
      OnRegister();
      UnregisterCallback<GeometryChangedEvent>(OnGeometryChange);
    }
  }
}
