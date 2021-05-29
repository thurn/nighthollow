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

using System.Collections.Immutable;
using MessagePack;
using Nighthollow.Interface;
using Nighthollow.Utils;
using UnityEngine.UIElements;

#nullable enable

namespace Nighthollow.Rules.Effects
{
  /// <summary>
  /// Displays a tooltip with gameplay tutorial information
  /// </summary>
  [MessagePackObject]
  public sealed partial class DisplayHelpTextEffect : RuleEffect
  {
    public static Description Describe => new Description(
      "display the helper text",
      nameof(Text),
      "at position",
      nameof(XPosition),
      nameof(YPosition),
      "with arrow direction",
      nameof(ArrowDirection));

    public enum Direction
    {
      Top,
      Right,
      Bottom,
      Left
    }

    public DisplayHelpTextEffect(string text, int xPosition, int yPosition, Direction arrowDirection)
    {
      Text = text;
      XPosition = xPosition;
      YPosition = yPosition;
      ArrowDirection = arrowDirection;
    }

    [Key(0)] public string Text { get; }
    [Key(1)] public int XPosition { get; }
    [Key(2)] public int YPosition { get; }
    [Key(3)] public Direction ArrowDirection { get; }

    public override ImmutableHashSet<IKey> GetDependencies() => ImmutableHashSet.Create<IKey>(
      Key.ScreenController
    );

    public override void Execute(IEffectScope scope, RuleOutput? output)
    {
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
      element.style.left = new StyleLength(XPosition);
      element.style.top = new StyleLength(YPosition);
      element.style.opacity = new StyleFloat(v: 0f);

      var arrow = new VisualElement();
      arrow.AddToClassList("arrow");
      element.Add(arrow);

      var label = new Label {text = Text};
      element.Add(label);
      InterfaceUtils.FadeIn(element, duration: 0.3f);

      InterfaceUtils
        .FindByName<VisualElement>(scope.Get(Key.ScreenController).Screen, "HelperTextContainer")
        .Add(element);
    }
  }
}