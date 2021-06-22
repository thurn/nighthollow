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
using Nighthollow.Interface.Components.Library;
using Nighthollow.Rules;
using Nighthollow.World.Data;
using UnityEngine;
using UnityEngine.UIElements;

#nullable enable

namespace Nighthollow.Interface.Components.Windows
{
  public sealed record FocusTree : LayoutComponent
  {
    public FocusTree(HexData hexData)
    {
      HexData = hexData;
    }

    public HexData HexData { get; }

    protected override BaseComponent OnRender(Scope scope)
    {
      return new Row
      {
        FlexGrow = 1,
        AlignItems = Align.Stretch,
        Children = List(
          LineBetween(new Rect(100, 250, 100, 100), new Rect(300, 50, 100, 100)),
          FocusNode(100, 250),
          FocusNode(300, 50),
          FocusNode(300, 450)
        )
      };
    }

    static BaseComponent FocusNode(int left, int top) =>
      new Row
      {
        Width = 100,
        Height = 100,
        FlexPosition = Position.Absolute,
        Left = left,
        Top = top,
        BackgroundColor = Color.gray
      };

    static BaseComponent LineBetween(Rect start, Rect end)
    {
      var startPoint = new Vector2(start.xMax, start.center.y) - new Vector2(5, 0);
      var endPoint = new Vector2(end.xMin, end.center.y) + new Vector2(5, 0);
      return new Column
      {
        FlexPosition = Position.Absolute,
        Width = Vector2.Distance(startPoint, endPoint),
        Height = 2,
        BackgroundColor = ColorPalette.PrimaryText,
        Left = startPoint.x,
        Top = startPoint.y,
        Rotate = new Rotate(new Angle(
          Mathf.Atan((endPoint.y - startPoint.y) / (endPoint.x - startPoint.x)),
          AngleUnit.Radian)),
        TransformOrigin = new StyleTransformOrigin(new TransformOrigin(0, 0, 0))
      };
    }
  }
}