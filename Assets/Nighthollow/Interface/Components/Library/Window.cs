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
using Nighthollow.Interface.Components.Core;
using Nighthollow.Rules;
using UnityEngine;
using UnityEngine.UIElements;

#nullable enable

namespace Nighthollow.Interface.Components.Library
{
  public sealed record Window : LayoutComponent
  {
    public enum WindowSize
    {
      /// <summary>Takes up all available screen space</summary>
      FullScreen,

      /// <summary>Leaves room for the advisor bar at the bottom of the World screen</summary>
      WorldFullScreen,

      /// <summary>Only make the window large enough to fit its content</summary>
      FitContent
    }

    public BaseComponent? Content { get; init; }
    public string Title { get; init; } = "";
    public Portrait.CharacterName? TitlePortrait { get; init; }
    public WindowSize Size { get; init; } = WindowSize.FullScreen;

    protected override BaseComponent OnRender(Scope scope)
    {
      var header = new Row
      {
        FlexPosition = Position.Absolute,
        Left = 0,
        Right = 0,
        Top = 0,
        AlignItems = Align.Center,
        JustifyContent = Justify.Center,
        Translate = new Translate(0, Length.Percent(-55), 0),
        Children = List(
          new Row
          {
            AlignItems = Align.Center,
            JustifyContent = Justify.Center,
            PaddingLeftRight = 128,
            PaddingTopBottom = 16,
            BackgroundImage = "GUI/NameFrame",
            BackgroundSliceLeftRight = 512,
            Children = List(
              TitlePortrait is { } t
                ? new Portrait(t)
                {
                  MarginRight = 16,
                }
                : null,
              new Text(Title)
              {
                Type = TextType.Headline,
                HierarchyLevel = HierarchyLevel.One,
              })
          }
        )
      };

      var positions = PositionsForSize(Size);

      return new Row
      {
        FlexPosition = Position.Absolute,
        Left = 0,
        Right = 0,
        Top = 0,
        Bottom = 0,
        JustifyContent = Justify.Center,
        AlignItems = Align.Center,
        Children = List(
          new Column
          {
            FlexPosition = Position.Absolute,
            Left = positions.Left,
            Top = positions.Top,
            Right = positions.Right,
            Bottom = positions.Bottom,
            Translate = positions.Translate,
            PaddingAll = 64,
            AlignItems = Align.Center,
            JustifyContent = Justify.FlexStart,
            BackgroundColor = ColorPalette.WindowBackground,
            BackgroundImage = "GUI/WindowBackground",
            BackgroundSliceLeftRight = 128,
            BackgroundSliceTopBottom = 128,
            Children = List(header, Content)
          }
        )
      };
    }

    sealed record Positions(Translate? Translate, Length? Left, Length? Top, Length? Right, Length? Bottom);

    static Positions PositionsForSize(WindowSize size) =>
      size switch
      {
        WindowSize.FullScreen => new Positions(null, 128, 64, 128, 24),
        WindowSize.WorldFullScreen => new Positions(null, 64, 64, 64, 220),
        WindowSize.FitContent => new Positions(
          new Translate(Length.Percent(-50), Length.Percent(-50), 0),
          Length.Percent(50),
          Length.Percent(50),
          null,
          null),
        _ => throw new ArgumentOutOfRangeException()
      };
  }
}