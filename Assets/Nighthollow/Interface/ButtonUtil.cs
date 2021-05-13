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
using UnityEngine.UIElements;

#nullable enable

namespace Nighthollow.Interface
{
  public static class ButtonUtil
  {
    public readonly struct Button
    {
      public readonly string Text;
      public readonly Action OnClick;
      public readonly bool Large;

      public Button(string text, Action onClick, bool large = false)
      {
        Text = text;
        OnClick = onClick;
        Large = large;
      }

      public Button WithAction(Action action) => new Button(Text, action, Large);
    }

    public static VisualElement Create(Button button)
    {
      var result = new VisualElement();
      result.AddToClassList("button");
      if (button.Large)
      {
        result.AddToClassList("large");
      }

      result.RegisterCallback<ClickEvent>(e => button.OnClick());

      // Hack: The Unity :active selector is not working. This is not a great solution either, because we only
      // get MouseUpEvent if you release the mouse *over* the element... check if this is fixed in a future version
      // or report a bug.
      result.RegisterCallback<MouseDownEvent>(e => { result.AddToClassList("button-down"); });
      result.RegisterCallback<MouseUpEvent>(e => { result.RemoveFromClassList("button-down"); });

      var label = new Label {text = button.Text};
      result.Add(label);
      return result;
    }

    public static void DisplayMainButtons(ScreenController controller, IEnumerable<Button> buttons)
    {
      var parent = controller.Screen.Q("ChoiceButtons");
      parent.style.display = new StyleEnum<DisplayStyle>(DisplayStyle.Flex);
      parent.Clear();

      foreach (var button in buttons)
      {
        parent.Add(Create(button.WithAction(() =>
        {
          button.OnClick();
          parent.style.display = new StyleEnum<DisplayStyle>(DisplayStyle.None);
        })));
      }
    }
  }
}