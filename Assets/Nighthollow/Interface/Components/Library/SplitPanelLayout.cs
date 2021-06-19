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

using System.Security.Principal;
using Nighthollow.Interface.Components.Core;
using Nighthollow.Rules;
using UnityEngine;
using UnityEngine.UIElements;

#nullable enable

namespace Nighthollow.Interface.Components.Library
{
  public sealed record SplitPanelLayout : LayoutComponent
  {
    public sealed record Panel
    {
      public string Title { get; init; } = "";
      public BaseComponent? Content { get; init; }
    }

    public Panel? LeftPanel { get; init; }
    public Panel? RightPanel { get; init; }

    protected override BaseComponent OnRender(Scope scope)
    {
      return new Row
      {
        FlexPosition = Position.Absolute,
        InsetAll = 32,
        AlignItems = Align.Stretch,
        Children = List(
          LeftPanel is null ? null : CreatePanel(LeftPanel),
          RightPanel is null ? null : CreatePanel(RightPanel))
      };
    }

    static Column CreatePanel(Panel panel) =>
      new()
      {
        FlexGrow = 1,
        MarginAll = 24,
        PaddingAll = 16,
        AlignItems = Align.Stretch,
        BackgroundImage = "GUI/BasicBar",
        BackgroundSliceAll = 32,
        FlexBasis = 0,
        Children = List(
          new Row
          {
            JustifyContent = Justify.Center,
            Children = List(
              new Text(panel.Title)
              {
                Type = TextType.Headline,
                HierarchyLevel = HierarchyLevel.Two
              }
            )
          },
          new Row
          {
            FlexGrow = 1,
            AlignItems = Align.FlexStart,
            JustifyContent = Justify.FlexStart,
            Children = List(panel.Content)
          })
      };
  }
}