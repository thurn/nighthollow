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

using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using Nighthollow.Interface.Components.Core;
using Nighthollow.Interface.Components.Library;
using Nighthollow.Rules;
using Nighthollow.Utils;
using Nighthollow.World.Data;
using UnityEngine;
using UnityEngine.UIElements;

#nullable enable

namespace Nighthollow.Interface.Components.Windows
{
  public sealed record FocusTree : LayoutComponent
  {
    const float NodeSize = 100;
    const float TierSpacing = 250;
    const int HeightSpacing = 175;

    public FocusTree(FocusTreeData tree)
    {
      Tree = tree;
    }

    public FocusTreeData Tree { get; }

    protected override BaseComponent OnRender(Scope scope)
    {
      var tiers = Tree.Nodes
        .Select((node, index) => (node, index))
        .GroupBy(n => n.Item1.Tier)
        .ToImmutableDictionary(p => p.Key, v => v.ToImmutableList());
      var height = tiers.Values.Max(t => t.Count) * HeightSpacing;

      var positionList = tiers.SelectMany(group => PositionNodesInTier(height, group.Value)).ToImmutableList();

      var minX = positionList.Min(pair => pair.Item1.xMin);
      var maxX = positionList.Max(pair => pair.Item1.xMax);
      var minY = positionList.Min(pair => pair.Item1.yMin);
      var maxY = positionList.Max(pair => pair.Item1.yMax);

      var positions = positionList.ToDictionary(pair => pair.Item2, pair => pair.Item1);
      var children = ImmutableList.CreateBuilder<BaseComponent>();

      for (var i = 0; i < Tree.Nodes.Count; ++i)
      {
        var node = Tree.Nodes[i];
        foreach (var connection in node.ConnectionIndices)
        {
          children.Add(LineBetween(positions[i], positions[connection]));
        }

        foreach (var (next, nextIndex) in tiers.GetOrReturnDefault(
          node.Tier + 1,
          ImmutableList<(FocusNodeData node, int index)>.Empty))
        {
          if (next.Line == node.Line)
          {
            children.Add(LineBetween(positions[i], positions[nextIndex]));
          }
        }
      }

      children.AddRange(Tree.Nodes.Select((node, index) => FocusNode(positions[index], node)));

      return new Row
      {
        AlignItems = Align.Center,
        JustifyContent = Justify.Center,
        FlexGrow = 1,
        Children = List(
          new Row
          {
            Width = maxX + minX,
            Height = maxY + minY,
            Children = children.ToImmutable()
          }
        )
      };
    }

    IEnumerable<(Rect, int)> PositionNodesInTier(int height, ImmutableList<(FocusNodeData, int)> nodes)
    {
      var yStepSize = height / ((float) (nodes.Count + 1));
      return nodes.Select((node, index) =>
      (
        new Rect(
          x: 50 + TierSpacing * node.Item1.Tier,
          y: ((index + 1) * yStepSize) - (NodeSize / 2),
          width: NodeSize,
          height: NodeSize), node.Item2));
    }

    static BaseComponent FocusNode(Rect rect, FocusNodeData data) =>
      new Row
      {
        Width = rect.width,
        Height = rect.height,
        FlexPosition = Position.Absolute,
        Left = rect.x,
        Top = rect.y,
        BackgroundImage = data.ImageAddress,
        BackgroundScaleMode = ScaleMode.ScaleToFit
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