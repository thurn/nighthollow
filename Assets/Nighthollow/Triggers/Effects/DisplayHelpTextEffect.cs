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

using MessagePack;
using Nighthollow.Interface;
using Nighthollow.Services;
using Nighthollow.Utils;
using UnityEngine;
using UnityEngine.UIElements;

#nullable enable

namespace Nighthollow.Triggers.Effects
{
  /// <summary>
  /// Displays a tooltip with gameplay tutorial information
  /// </summary>
  [MessagePackObject]
  public sealed class DisplayHelpTextEffect : IEffect<TriggerEvent>
  {
    public enum Direction
    {
      Top,
      Right,
      Bottom,
      Left
    }

    [MessagePackObject]
    public readonly struct ArrowPosition
    {
      public ArrowPosition(int x, int y)
      {
        X = x;
        Y = y;
      }

      [Key(0)] public int X { get; }
      [Key(1)] public int Y { get; }
    }

    public DisplayHelpTextEffect(ArrowPosition position, Direction arrowDirection, string text)
    {
      Position = position;
      ArrowDirection = arrowDirection;
      Text = text;
    }

    [Key(0)] public ArrowPosition Position { get; }
    [Key(1)] public Direction ArrowDirection { get; }
    [Key(2)] public string Text { get; }

    public void Execute(TriggerEvent triggerEvent, ServiceRegistry registry)
    {
      Debug.Log($"DisplayHelpTextEffect::Execute");
      var element = new VisualElement();
      element.AddToClassList("helper-text");
      element.AddToClassList(ArrowDirection switch
      {
        Direction.Top => "top-arrow",
        Direction.Right => "right-arrow",
        Direction.Bottom => "bottom-arrow",
        Direction.Left => "left-arrow",
        _ => throw Errors.UnknownEnumValue(ArrowDirection)
      });
      element.style.left = new StyleLength(Position.X);
      element.style.top = new StyleLength(Position.Y);
      element.style.opacity = new StyleFloat(v: 0f);

      var arrow = new VisualElement();
      arrow.AddToClassList("arrow");
      element.Add(arrow);

      var label = new Label {text = Text};
      element.Add(label);
      InterfaceUtils.FadeIn(element, duration: 0.3f);

      InterfaceUtils
        .FindByName<VisualElement>(registry.ScreenController.Screen, "HelperTextContainer")
        .Add(element);
    }
  }
}