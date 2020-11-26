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

#nullable enable

using Nighthollow.Interface;
using Nighthollow.Utils;
using UnityEngine;
using UnityEngine.UIElements;

namespace Nighthollow.Services
{
  public sealed class HelperTextService : MonoBehaviour
  {
    [SerializeField] bool _toggleDebugMode;
    [SerializeField] string _debugText = null!;
    [SerializeField] Vector2 _debugPosition;
    [SerializeField] ArrowDirection _debugArrowDirection;
    bool _active;

    public enum ArrowDirection
    {
      Top,
      Right,
      Bottom,
      Left
    }

    void Update()
    {
      if (Input.GetMouseButtonDown(0) && _active)
      {
        InterfaceUtils.FindByName<VisualElement>(Root.Instance.ScreenController.Screen, "HelperTextContainer").Clear();
        _active = false;
      }

      if (_toggleDebugMode)
      {
        InterfaceUtils.FindByName<VisualElement>(Root.Instance.ScreenController.Screen, "HelperTextContainer").Clear();
        ShowHelperText(_debugPosition, _debugArrowDirection, _debugText);
        _toggleDebugMode = false;
      }
    }

    public void OnDrewOpeningHand()
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

    public void OnGameStarted()
    {
      if (Database.Instance.UserData.TutorialState == UserDataService.Tutorial.GameOne)
      {
        ShowHelperText(
          new Vector2(5, 175),
          ArrowDirection.Top,
          "Your current Mana, Life, and Influence are shown here.");

        ShowHelperText(
          new Vector2(940, 710),
          ArrowDirection.Bottom,
          "You can play an Adept to add Influence and increase your Mana generation");
      }
    }

    void ShowHelperText(Vector2 interfacePosition, ArrowDirection arrowDirection, string text)
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

      _active = true;
      InterfaceUtils
        .FindByName<VisualElement>(Root.Instance.ScreenController.Screen, "HelperTextContainer")
        .Add(element);
    }
  }
}
