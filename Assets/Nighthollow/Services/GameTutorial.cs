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

using Nighthollow.Interface;
using Nighthollow.Utils;
using UnityEngine;
using UnityEngine.UIElements;

#nullable enable

namespace Nighthollow.Services
{
  public static class GameTutorial
  {
    public enum ArrowDirection
    {
      Top,
      Right,
      Bottom,
      Left
    }

    public static void OnDrewOpeningHand()
    {
      if (Database.Instance.UserData.TutorialState == UserDataService.Tutorial.GameOne)
      {
        ShowHelperText(
          new Vector2(250, 300),
          ArrowDirection.Bottom,
          "This is your opening hand of creature cards to play.");

        ShowHelperText(
          new Vector2(1400, 400),
          ArrowDirection.Left,
          "To play a card, you must pay its Mana cost and have the required amount of Influence.");
      }
    }

    static void ShowHelperText(Vector2 interfacePosition, ArrowDirection arrowDirection, string text)
    {
      var element = new VisualElement();
      element.AddToClassList("helper-text");
      element.AddToClassList(arrowDirection switch
      {
        ArrowDirection.Top => "top-arrow",
        ArrowDirection.Right => "right-arrow",
        ArrowDirection.Bottom => "bottom-arrow",
        ArrowDirection.Left => "left-arrow",
        _ => throw Errors.UnknownEnumValue(arrowDirection)
      });
      element.style.left = new StyleLength(interfacePosition.x);
      element.style.top = new StyleLength(interfacePosition.y);
      element.style.opacity = new StyleFloat(0f);

      var arrow = new VisualElement();
      arrow.AddToClassList("arrow");
      element.Add(arrow);

      var label = new Label {text = text};
      element.Add(label);
      InterfaceUtils.FadeIn(element, 0.3f);

      Root.Instance.ScreenController.Screen.Add(element);
    }
  }
}
