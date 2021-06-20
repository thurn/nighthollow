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

namespace Nighthollow.Interface.Components.Core
{
  public sealed record ScrollElement
  {
    public static readonly ScrollElement Default = new();

    public Color? BackgroundColor { get; init; }

    public void ApplyTo(VisualElement element)
    {
      element.style.backgroundColor = BackgroundColor ?? new StyleColor(StyleKeyword.Null);
    }
  }

  public sealed record ScrollBar
  {
    public ScrollElement Dragger { get; init; } = ScrollElement.Default;
    public ScrollElement DragContainer { get; init; } = ScrollElement.Default;
    public ScrollElement Tracker { get; init; } = ScrollElement.Default;
    public ScrollElement HighButton { get; init; } = ScrollElement.Default;
    public ScrollElement LowButton { get; init; } = ScrollElement.Default;
  }

  public sealed record ScrollViewComponent : Flexbox<ScrollView>
  {
    protected override ScrollView OnCreateMountContent() => new();

    protected override FlexDirection GetFlexDirection() => FlexDirection.Column;

    public ScrollBar VerticalScrollBar { get; init; } = new ScrollBar();
    public ScrollBar HorizontalScrollBar { get; init; } = new ScrollBar();

    public override string Type => "ScrollView";

    protected override void OnMount(ScrollView scrollView)
    {
      base.OnMount(scrollView);
      UpdateScroller(VerticalScrollBar, scrollView.verticalScroller);
      UpdateScroller(HorizontalScrollBar, scrollView.horizontalScroller);
    }

    static void UpdateScroller(ScrollBar scrollBar, Scroller scroller)
    {
      scrollBar.Dragger.ApplyTo(scroller.Q(className: Slider.draggerUssClassName));
      scrollBar.DragContainer.ApplyTo(scroller.Q(className: Slider.dragContainerUssClassName));
      scrollBar.Tracker.ApplyTo(scroller.Q(className: Slider.trackerUssClassName));
      scrollBar.HighButton.ApplyTo(scroller.highButton);
      scrollBar.LowButton.ApplyTo(scroller.lowButton);
    }
  }
}