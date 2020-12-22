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

using UnityEngine.UIElements;

#nullable enable

namespace Nighthollow.Interface
{
  public sealed class Dialog : VisualElement
  {
    Button _closeButton = null!;
    VisualElement _portrait = null!;
    Label _text = null!;

    public new sealed class UxmlFactory : UxmlFactory<Dialog, UxmlTraits>
    {
    }

    public new sealed class UxmlTraits : VisualElement.UxmlTraits
    {
    }

    public bool Visible { get; private set; }

    public void Show(string portraitName, string text, bool hideCloseButton = false)
    {
      Visible = true;
      style.display = new StyleEnum<DisplayStyle>(DisplayStyle.Flex);
      _portrait.ClearClassList();
      _portrait.AddToClassList("portrait");
      _portrait.AddToClassList(portraitName);

      _text.text = text;
      InterfaceUtils.FadeIn(this, duration: 0.3f);
      _closeButton.style.display = new StyleEnum<DisplayStyle>(hideCloseButton ? DisplayStyle.None : DisplayStyle.Flex);
    }

    public void Hide()
    {
      InterfaceUtils.FadeOut(this, duration: 0.3f, () =>
      {
        style.display = new StyleEnum<DisplayStyle>(DisplayStyle.None);
        Visible = false;
      });
    }

    public void Initialize()
    {
      _portrait = InterfaceUtils.FindByName<VisualElement>(this, "Portrait");
      _text = InterfaceUtils.FindByName<Label>(this, "Text");
      _closeButton = InterfaceUtils.FindByName<Button>(this, "CloseButton");
      _closeButton.RegisterCallback<ClickEvent>(e => { Hide(); });
    }
  }
}
