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

using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.UIElements;

#nullable enable

namespace Nighthollow.Interface.Components.Core
{
  public abstract record Flexbox : ContainerComponent<VisualElement>
  {
    static readonly Callbacks CallbackTracker = new();

    public override string Type => "Flexbox";

    public Align AlignItems { get; init; }
    public Justify JustifyContent { get; init; }
    public string? BackgroundImage { get; init; }
    public int BackgroundSliceLeftRight { get; init; }
    public int BackgroundSliceTopBottom { get; init; }
    public Color? BackgroundColor { get; init; }
    public Color? BackgroundImageTintColor { get; init; }
    public int? Height { get; init; }
    public int? Width { get; init; }
    public Position Position { get; init; }
    public int? Left { get; init; }
    public int? Top { get; init; }
    public int? Right { get; init; }
    public int? Bottom { get; init; }
    public int MarginLeft { get; init; }
    public int MarginTop { get; init; }
    public int MarginRight { get; init; }
    public int MarginBottom { get; init; }
    public int PaddingLeft { get; init; }
    public int PaddingTop { get; init; }
    public int PaddingBottom { get; init; }
    public int PaddingRight { get; init; }

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

    public int FlexGrow { get; init; }
    public int FlexShrink { get; init; } = 1;

    public EventCallback<MouseOverEvent>? OnMouseOver { get; init; }
    public EventCallback<MouseOutEvent>? OnMouseOut { get; init; }
    public EventCallback<MouseDownEvent>? OnMouseDown { get; init; }
    public EventCallback<MouseUpEvent>? OnMouseUp { get; init; }
    public EventCallback<ClickEvent>? OnClick { get; init; }

    protected abstract FlexDirection GetFlexDirection();

    protected override VisualElement OnCreateMountContent() => new VisualElement();

    protected override void OnMount(VisualElement container)
    {
      container.style.flexDirection = GetFlexDirection();
      container.style.alignItems = AlignItems;
      container.style.justifyContent = JustifyContent;
      var sprite = UseResource<Sprite>(BackgroundImage);
      container.style.backgroundImage =
        sprite is { } s ? new StyleBackground(s) : new StyleBackground(StyleKeyword.None);
      container.style.backgroundColor = BackgroundColor is { } c ? c : Color.clear;
      container.style.unityBackgroundImageTintColor = BackgroundImageTintColor ?? Color.white;
      container.style.height = Height is { } h ? h : new StyleLength(StyleKeyword.Null);
      container.style.width = Width is { } w ? w : new StyleLength(StyleKeyword.Null);
      container.style.position = Position;
      container.style.left = Left is { } l ? l : new StyleLength(StyleKeyword.Null);
      container.style.top = Top is { } t ? t : new StyleLength(StyleKeyword.Null);
      container.style.right = Right is { } r ? r : new StyleLength(StyleKeyword.Null);
      container.style.bottom = Bottom is { } b ? b : new StyleLength(StyleKeyword.Null);
      container.style.unitySliceLeft = BackgroundSliceLeftRight;
      container.style.unitySliceRight = BackgroundSliceLeftRight;
      container.style.unitySliceTop = BackgroundSliceTopBottom;
      container.style.unitySliceBottom = BackgroundSliceTopBottom;
      container.style.marginLeft = MarginLeft;
      container.style.marginTop = MarginTop;
      container.style.marginRight = MarginRight;
      container.style.marginBottom = MarginBottom;
      container.style.paddingLeft = PaddingLeft;
      container.style.paddingTop = PaddingTop;
      container.style.paddingBottom = PaddingBottom;
      container.style.paddingRight = PaddingRight;
      container.style.flexGrow = FlexGrow;
      container.style.flexShrink = FlexShrink;

      CallbackTracker.MouseOver.SetCallback(container, OnMouseOver);
      CallbackTracker.MouseOut.SetCallback(container, OnMouseOut);
      CallbackTracker.MouseDown.SetCallback(container, OnMouseDown);
      CallbackTracker.MouseUp.SetCallback(container, OnMouseUp);
      CallbackTracker.Click.SetCallback(container, OnClick);
    }

    sealed class CallbackManager<T> where T : EventBase<T>, new()
    {
      readonly ConditionalWeakTable<VisualElement, EventCallback<T>> _callbacks = new();

      public void SetCallback(VisualElement element, EventCallback<T>? eventCallback)
      {
        if (_callbacks.TryGetValue(element, out var previous))
        {
          element.UnregisterCallback(previous);
          _callbacks.Remove(element);
        }

        if (eventCallback != null)
        {
          _callbacks.Add(element, eventCallback);
          element.RegisterCallback(eventCallback);
        }
      }
    }

    sealed class Callbacks
    {
      public CallbackManager<MouseOverEvent> MouseOver { get; } = new();
      public CallbackManager<MouseOutEvent> MouseOut { get; } = new();
      public CallbackManager<MouseDownEvent> MouseDown { get; } = new();
      public CallbackManager<MouseUpEvent> MouseUp { get; } = new();
      public CallbackManager<ClickEvent> Click { get; } = new();
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