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

using UnityEngine.UIElements;

namespace Nighthollow.Interface
{
  public sealed class AdvisorBar : VisualElement
  {
    public ScreenController Controller { get; set; } = null!;
    public bool Visible { get; private set; }

    public new sealed class UxmlFactory : UxmlFactory<AdvisorBar, UxmlTraits>
    {
    }

    public new sealed class UxmlTraits : VisualElement.UxmlTraits
    {
    }

    public void Show()
    {
      Visible = true;
      style.display = new StyleEnum<DisplayStyle>(DisplayStyle.Flex);
    }

    public void Hide()
    {
      Visible = false;
      style.display = new StyleEnum<DisplayStyle>(DisplayStyle.None);
    }

    public AdvisorBar()
    {
      RegisterCallback<GeometryChangedEvent>(OnGeometryChange);
    }

    void OnGeometryChange(GeometryChangedEvent evt)
    {
      this.Q("CardsButton").RegisterCallback<ClickEvent>(e => Controller.ShowCardsWindow());
      UnregisterCallback<GeometryChangedEvent>(OnGeometryChange);
    }
  }
}
