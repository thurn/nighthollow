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
using UnityEngine.UIElements;

#nullable enable

namespace Nighthollow.Interface.Components
{
  public abstract record Flexbox : ContainerComponent<VisualElement>
  {
    public override string Type => "Flexbox";

    public Align AlignItems { get; init; }
    public Justify JustifyContent { get; init; }
    public string? BackgroundImage { get; init; }
    public int BackgroundSliceLeftRight { get; init; }
    public int BackgroundSliceTopBottom { get; init; }
    public Color? BackgroundColor { get; init; }
    public int? Height { get; init; }
    public int? Width { get; init; }
    public int PaddingLeft { get; init; }
    public int PaddingTop { get; init; }
    public int PaddingBottom { get; init; }
    public int PaddingRight { get; init; }
    public int FlexGrow { get; init; }
    public int FlexShrink { get; init; } = 1;
    public Color? BackgroundImageTintColor { get; init; }

    public EventCallback<MouseOverEvent>? OnMouseOver { get; init; }
    public EventCallback<MouseOutEvent>? OnMouseOut { get; init; }
    public EventCallback<MouseDownEvent>? OnMouseDown { get; init; }
    public EventCallback<MouseUpEvent>? OnMouseUp { get; init; }

    protected abstract FlexDirection GetFlexDirection();

    protected override VisualElement OnCreateMountContent() => new VisualElement();

    protected override void OnMount(VisualElement container)
    {
      container.style.flexDirection = GetFlexDirection();
      container.style.alignItems = AlignItems;
      container.style.justifyContent = JustifyContent;
      var sprite = UseSprite(BackgroundImage);

      if (sprite is not null)
      {
        container.style.backgroundImage = new StyleBackground(sprite);
      }

      if (BackgroundColor is { } b)
      {
        container.style.backgroundColor = b;
      }

      if (Height is { } h)
      {
        container.style.height = h;
      }

      if (Width is { } w)
      {
        container.style.width = w;
      }

      container.style.unitySliceLeft = BackgroundSliceLeftRight;
      container.style.unitySliceRight = BackgroundSliceLeftRight;
      container.style.unitySliceTop = BackgroundSliceTopBottom;
      container.style.unitySliceBottom = BackgroundSliceTopBottom;
      container.style.paddingLeft = PaddingLeft;
      container.style.paddingTop = PaddingTop;
      container.style.paddingBottom = PaddingBottom;
      container.style.paddingRight = PaddingRight;
      container.style.flexGrow = FlexGrow;
      container.style.flexShrink = FlexShrink;
      container.style.unityBackgroundImageTintColor = BackgroundImageTintColor ?? Color.white;

      if (OnMouseOver is not null)
      {
        container.RegisterCallback(OnMouseOver);
      }

      if (OnMouseOut is not null)
      {
        container.RegisterCallback(OnMouseOut);
      }

      if (OnMouseDown is not null)
      {
        container.RegisterCallback(OnMouseDown);
      }

      if (OnMouseUp is not null)
      {
        container.RegisterCallback(OnMouseUp);
      }
    }
  }

  public sealed record Row : Flexbox
  {
    protected override FlexDirection GetFlexDirection() => FlexDirection.Row;
  }

  public sealed record Column : Flexbox
  {
    protected override FlexDirection GetFlexDirection() => FlexDirection.Column;
  }
}