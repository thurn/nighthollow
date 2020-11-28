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

using System.Collections.Generic;
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
    HashSet<int> _shown = new HashSet<int>();
    bool _active;

    public enum ArrowDirection
    {
      Top,
      Right,
      Bottom,
      Left
    }

    readonly struct HelperText
    {
      public int Id { get; }
      public Vector2 Position { get; }
      public ArrowDirection ArrowDirection { get; }
      public string Text { get; }

      public HelperText(int id, Vector2 position, ArrowDirection arrowDirection, string text)
      {
        Id = id;
        Position = position;
        ArrowDirection = arrowDirection;
        Text = text;
      }
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
        _shown.Remove(0);
        InterfaceUtils.FindByName<VisualElement>(Root.Instance.ScreenController.Screen, "HelperTextContainer").Clear();
        ShowHelperText(new HelperText(0, _debugPosition, _debugArrowDirection, _debugText));
        _toggleDebugMode = false;
      }
    }

    public void OnDrewOpeningHand()
    {
      if (Database.Instance.UserData.TutorialState == UserDataService.Tutorial.GameOne)
      {
        ShowHelperText(
          new HelperText(1,
            new Vector2(250, 300),
            ArrowDirection.Bottom,
            "This is your opening hand of creature cards to play"));

        ShowHelperText(
          new HelperText(2,
            new Vector2(1400, 400),
            ArrowDirection.Left,
            "To play a card, you must pay its Mana cost and have the required amount of Influence"));
      }
    }

    public void OnGameStarted()
    {
      if (Database.Instance.UserData.TutorialState == UserDataService.Tutorial.GameOne)
      {
        ShowHelperText(
          new HelperText(3,
            new Vector2(160, 50),
            ArrowDirection.Left,
            "Your current Mana and Influence are shown here"));

        ShowHelperText(
          new HelperText(4,
            new Vector2(940, 710),
            ArrowDirection.Bottom,
            "You can play an Adept to add Influence and increase your Mana generation"));
      }
    }

    public void OnCreaturePlayed()
    {
      ShowHelperText(
        new HelperText(5,
          new Vector2(100, 450),
          ArrowDirection.Left,
          "To win the game, make sure no enemies get past your defenses to here"));
    }

    void ShowHelperText(HelperText helperText)
    {
      if (_shown.Contains(helperText.Id))
      {
        return;
      }

      _shown.Add(helperText.Id);
      var element = new VisualElement();
      element.AddToClassList("helper-text");
      element.AddToClassList(helperText.ArrowDirection switch
      {
        ArrowDirection.Top => "top-arrow",
        ArrowDirection.Right => "right-arrow",
        ArrowDirection.Bottom => "bottom-arrow",
        ArrowDirection.Left => "left-arrow",
        _ => throw Errors.UnknownEnumValue(helperText.ArrowDirection)
      });
      element.style.left = new StyleLength(helperText.Position.x);
      element.style.top = new StyleLength(helperText.Position.y);
      element.style.opacity = new StyleFloat(0f);

      var arrow = new VisualElement();
      arrow.AddToClassList("arrow");
      element.Add(arrow);

      var label = new Label {text = helperText.Text};
      element.Add(label);
      InterfaceUtils.FadeIn(element, 0.3f);

      _active = true;
      InterfaceUtils
        .FindByName<VisualElement>(Root.Instance.ScreenController.Screen, "HelperTextContainer")
        .Add(element);
    }
  }
}
