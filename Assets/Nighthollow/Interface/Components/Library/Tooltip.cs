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
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using Nighthollow.Interface.Components.Core;
using Nighthollow.Rules;
using UnityEngine;
using UnityEngine.UIElements;

#nullable enable

namespace Nighthollow.Interface.Components.Library
{
  public sealed record Tooltip : LayoutComponent
  {
    public interface ITooltipContent
    {
      T Switch<T>(
        Func<ImmutableList<string>, T> onTextGroup,
        Func<T> onDivider);
    }

    public sealed record TextGroup : ITooltipContent
    {
      public TextGroup(ImmutableList<string> content)
      {
        Content = content;
      }

      public TextGroup(string content) : this(ImmutableList.Create(content))
      {
      }

      public ImmutableList<string> Content { get; }

      public T Switch<T>(Func<ImmutableList<string>, T> onTextGroup, Func<T> onDivider) => onTextGroup(Content);
    }

    public sealed record Divider : ITooltipContent
    {
      public T Switch<T>(Func<ImmutableList<string>, T> onTextGroup, Func<T> onDivider) => onDivider();
    }

    public string Title { get; init; } = "";

    public ImmutableList<ITooltipContent> Content { get; init; } = ImmutableList<ITooltipContent>.Empty;

    /// <summary>
    /// Screen rectangle which this tooltip points to. We will attempt to place the tooltip close to (but not
    /// overlapping with) this rectangle. Typically derived from the 'worldBound' of a VisualElement.
    /// </summary>
    public Rect Anchor { get; init; }

    /// <summary>
    /// If true, dividers at the end of the content and empty text groups will not be displayed
    /// </summary>
    public bool ShouldTrimContent { get; init; } = true;

    public Color? TitleColor { get; init; }

    protected override BaseComponent OnRender(Scope scope)
    {
      var position = ComputeAnchorPosition(Anchor, 16);

      return new Column
      {
        AlignItems = Align.Center,
        JustifyContent = Justify.FlexStart,
        BackgroundImage = "GUI/BasicBar",
        BackgroundSliceLeftRight = 32,
        BackgroundSliceTopBottom = 32,
        Width = 512,
        FlexPosition = Position.Absolute,
        Left = position.Left,
        Top = position.Top,
        Right = position.Right,
        Bottom = position.Bottom,
        Children = List(
          new Row
          {
            FlexPosition = Position.Absolute,
            Left = 0,
            Right = 0,
            Top = 0,
            AlignItems = Align.Center,
            JustifyContent = Justify.Center,
            Translate = new Translate(0, Length.Percent(-50), 0),
            Children = List(new Row
            {
              AlignItems = Align.Center,
              JustifyContent = Justify.Center,
              PaddingLeftRight = 64,
              PaddingTopBottom = 8,
              BackgroundImage = "GUI/BasicBar",
              BackgroundSliceLeftRight = 32,
              BackgroundSliceTopBottom = 32,
              Children = List(
                new Text(Title)
                {
                  Type = TextType.Headline,
                  HierarchyLevel = HierarchyLevel.Three,
                  TextColor = TitleColor
                })
            })
          },
          new Column
          {
            PaddingTop = 32,
            PaddingBottom = 16,
            PaddingLeftRight = 32,
            AlignItems = Align.Center,
            Children = TrimContent(Content).Select(c => c.Switch(RenderTextGroup, RenderDivider))
          }
        )
      };
    }

    static BaseComponent RenderTextGroup(ImmutableList<string> lines)
    {
      return new Text(string.Join("\n", lines.Where(l => !string.IsNullOrWhiteSpace(l))))
      {
        HierarchyLevel = HierarchyLevel.Three,
        Type = TextType.Body,
        MarginTopBottom = 4,
        TextAlign = TextAnchor.MiddleCenter
      };
    }

    static BaseComponent RenderDivider()
    {
      return new Row
      {
        Width = Length.Percent(50),
        Height = 1,
        BackgroundColor = ColorPalette.PrimaryText,
        MarginTopBottom = 8
      };
    }

    sealed record AnchorPosition(int? Left, int? Top, int? Right, int? Bottom);

    enum AnchorCorner
    {
      TopLeft,
      TopRight,
      BottomLeft,
      BottomRight
    }

    static AnchorPosition ComputeAnchorPosition(Rect anchor, int xOffset)
    {
      // We anchor the tooltip by determining which corner of the target is closest to the center of the screen and
      // then placing the tooltip in that direction.
      var screenCenter = new Vector2(Screen.width / 2f, Screen.height / 2f);
      var distances = new List<(float, AnchorCorner)>
      {
        (Vector2.Distance(new Vector2(anchor.x, anchor.y), screenCenter), AnchorCorner.TopLeft),
        (Vector2.Distance(new Vector2(anchor.xMax, anchor.y), screenCenter), AnchorCorner.TopRight),
        (Vector2.Distance(new Vector2(anchor.x, anchor.yMax), screenCenter), AnchorCorner.BottomLeft),
        (Vector2.Distance(new Vector2(anchor.xMax, anchor.yMax), screenCenter), AnchorCorner.BottomRight)
      };

      var targetCorner = distances.OrderBy(pair => pair.Item1).Min().Item2;
      return targetCorner switch
      {
        AnchorCorner.TopLeft =>
          new AnchorPosition(
            null,
            null,
            Screen.width - Mathf.RoundToInt(anchor.x) + xOffset,
            Screen.height - Mathf.RoundToInt(anchor.yMax)),
        AnchorCorner.TopRight =>
          new AnchorPosition(
            xOffset + Mathf.RoundToInt(anchor.xMax),
            null,
            null,
            Screen.height - Mathf.RoundToInt(anchor.yMax)),
        AnchorCorner.BottomLeft =>
          new AnchorPosition(
            null,
            Mathf.RoundToInt(anchor.y),
            Screen.width - Mathf.RoundToInt(anchor.x) + xOffset,
            null),
        AnchorCorner.BottomRight =>
          new AnchorPosition(xOffset + Mathf.RoundToInt(anchor.xMax), Mathf.RoundToInt(anchor.y), null, null),
        _ => throw new ArgumentOutOfRangeException()
      };
    }

    ImmutableList<ITooltipContent> TrimContent(ImmutableList<ITooltipContent> list)
    {
      if (!ShouldTrimContent)
      {
        return list;
      }

      var result = ImmutableList.CreateBuilder<ITooltipContent>();

      var foundNonDivider = false;
      foreach (var content in list.Reverse())
      {
        var d = foundNonDivider;
        var shouldAdd = content.Switch(textGroup => !textGroup.IsEmpty, () => d);

        if (shouldAdd)
        {
          foundNonDivider = true;
          result.Add(content);
        }
      }

      result.Reverse();
      return result.ToImmutable();
    }
  }
}