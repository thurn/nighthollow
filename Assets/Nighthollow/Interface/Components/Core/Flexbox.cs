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
using System.Collections.Immutable;
using UnityEngine;
using UnityEngine.UIElements;

#nullable enable

namespace Nighthollow.Interface.Components.Core
{
  public abstract record Flexbox<TElement> : ContainerComponent<TElement> where TElement : VisualElement
  {
    public override string Type => "Flexbox";

    public Align AlignItems { get; init; }
    public Justify JustifyContent { get; init; }
    public string? BackgroundImage { get; init; }
    public int BackgroundSliceLeftRight { get; init; }
    public int BackgroundSliceTopBottom { get; init; }
    public Color? BackgroundColor { get; init; }
    public Color? BackgroundImageTintColor { get; init; }
    public Length? Height { get; init; }
    public Length? Width { get; init; }
    public int PaddingLeft { get; init; }
    public int PaddingTop { get; init; }
    public int PaddingBottom { get; init; }
    public int PaddingRight { get; init; }
    public int FlexGrow { get; init; }
    public int FlexShrink { get; init; } = 1;
    public Length? FlexBasis { get; init; }
    public Wrap FlexWrap { get; init; }
    public Translate? Translate { get; init; }
    public ScaleMode BackgroundScaleMode { get; init; }
    public int BorderBottomLeftRadius { get; init; }
    public int BorderBottomRightRadius { get; init; }
    public int BorderTopLeftRadius { get; init; }
    public int BorderTopRightRadius { get; init; }

    public int PaddingLeftRight
    {
      init
      {
        PaddingLeft = value;
        PaddingRight = value;
      }
    }

    public int PaddingTopBottom
    {
      init
      {
        PaddingTop = value;
        PaddingBottom = value;
      }
    }

    public int PaddingAll
    {
      init
      {
        PaddingLeftRight = value;
        PaddingTopBottom = value;
      }
    }

    public int BackgroundSliceAll
    {
      init
      {
        BackgroundSliceLeftRight = value;
        BackgroundSliceTopBottom = value;
      }
    }

    public int BorderRadiusAll
    {
      init
      {
        BorderBottomLeftRadius = value;
        BorderBottomRightRadius = value;
        BorderTopLeftRadius = value;
        BorderTopRightRadius = value;
      }
    }

    protected abstract FlexDirection GetFlexDirection();

    protected override void OnMount(TElement container)
    {
      container.style.flexDirection = GetFlexDirection();
      container.style.alignItems = AlignItems;
      container.style.justifyContent = JustifyContent;
      var sprite = UseResource<Sprite>(BackgroundImage);
      container.style.backgroundImage =
        sprite is { } s ? new StyleBackground(s) : new StyleBackground(StyleKeyword.Null);
      container.style.backgroundColor = BackgroundColor ?? new StyleColor(StyleKeyword.Null);
      container.style.unityBackgroundImageTintColor = BackgroundImageTintColor ?? new StyleColor(StyleKeyword.Null);
      container.style.height = Height ?? new StyleLength(StyleKeyword.Null);
      container.style.width = Width ?? new StyleLength(StyleKeyword.Null);
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
      container.style.translate = Translate ?? new StyleTranslate(StyleKeyword.Null);
      container.style.unityBackgroundScaleMode = BackgroundScaleMode;
      container.style.borderBottomLeftRadius = BorderBottomLeftRadius;
      container.style.borderBottomRightRadius = BorderBottomRightRadius;
      container.style.borderTopLeftRadius = BorderTopLeftRadius;
      container.style.borderTopRightRadius = BorderTopRightRadius;
      container.style.flexBasis = FlexBasis ?? new StyleLength(StyleKeyword.Null);
      container.style.flexWrap = FlexWrap;
    }
  }

  public abstract record FlexboxElement : Flexbox<ComponentVisualElement>
  {
    public EventCallback<MouseDownEvent>? OnMouseDown { get; init; }
    public EventCallback<MouseOverEvent>? OnMouseOver { get; init; }
    public EventCallback<MouseOutEvent>? OnMouseOut { get; init; }
    public EventCallback<MouseUpEvent>? OnMouseUp { get; init; }
    public EventCallback<ClickEvent>? OnClick { get; init; }
    public IDraggable? Draggable { get; init; }
    public IDragReceiver? DragReceiver { get; init; }

    protected override void OnMount(ComponentVisualElement element)
    {
      base.OnMount(element);
      element.SetClassNames(ClassNames);
      element.DragReceiver = DragReceiver;

      var makeDraggable = UseDraggable();
      if (Draggable != null)
      {
        element.RegisterExclusiveCallback<MouseDownEvent>(e =>
        {
          makeDraggable.OnMouseDown(element, e, Draggable);
        });
      }
      else
      {
        element.RegisterExclusiveCallback(OnMouseDown);
      }

      element.RegisterExclusiveCallback(OnMouseOver);
      element.RegisterExclusiveCallback(OnMouseOut);
      element.RegisterExclusiveCallback(OnMouseUp);
      element.RegisterExclusiveCallback(OnClick);
    }
  }

  public sealed record Row : FlexboxElement
  {
    protected override ComponentVisualElement OnCreateMountContent() => new();

    protected override FlexDirection GetFlexDirection() => FlexDirection.Row;
  }

  public sealed record Column : FlexboxElement
  {
    protected override ComponentVisualElement OnCreateMountContent() => new();

    protected override FlexDirection GetFlexDirection() => FlexDirection.Column;
  }
}