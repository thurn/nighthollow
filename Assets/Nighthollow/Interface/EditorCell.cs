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
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEditorInternal;
using UnityEngine;
using UnityEngine.UIElements;

#nullable enable

namespace Nighthollow.Interface
{
  public sealed class EditorCell : VisualElement
  {
    readonly ScreenController _controller;
    readonly TextField _field;
    readonly List<string>? _suggestions = null;
    bool _confirmed;

    public EditorCell(ScreenController controller, string name, Type type, object? value)
    {
      _controller = controller;
      string? rendered;
      bool canModify;
      AddToClassList("editor-cell");

      if (name.Equals("Id"))
      {
        rendered = value?.ToString();
        canModify = false;
      }
      else if (typeof(string).IsAssignableFrom(type))
      {
        rendered = value?.ToString();
        canModify = true;
      }
      else if (typeof(int).IsAssignableFrom(type))
      {
        rendered = value?.ToString();
        canModify = true;
      }
      else if (type.IsSubclassOf(typeof(Enum)))
      {
        rendered = value?.ToString();
        canModify = true;

        _suggestions = new List<string>();
        foreach (var suggestion in Enum.GetValues(type))
        {
          if (!suggestion.ToString().Equals("Unknown"))
          {
            _suggestions.Add(suggestion.ToString());
          }
        }
      }
      else if (typeof(IDictionary).IsAssignableFrom(type))
      {
        rendered = DictionaryPreview(value as IDictionary);
        canModify = false;
      }
      else
      {
        rendered = value?.ToString();
        canModify = false;
      }

      _field = new TextField
      {
        multiline = false,
        doubleClickSelectsWord = true,
        tripleClickSelectsLine = true,
        isDelayed = true,
        isReadOnly = !canModify,
      };

      if (rendered != null)
      {
        _field.value = rendered;
      }

      _field.AddToClassList("editor-text-field");
      Add(_field);

      _field.RegisterCallback<FocusInEvent>(OnFocusIn);
      _field.RegisterCallback<FocusOutEvent>(OnFocusOut);
      _field.RegisterCallback<KeyDownEvent>(OnKeyDown);
    }

    void OnKeyDown(KeyDownEvent evt)
    {
      if (_suggestions != null)
      {
        switch (evt.keyCode)
        {
          case KeyCode.DownArrow:
            _controller.Get(ScreenController.Typeahead).Next();
            break;
          case KeyCode.UpArrow:
            _controller.Get(ScreenController.Typeahead).Previous();
            break;
          case KeyCode.Return:
            _confirmed = true;
            _controller.Get(ScreenController.Typeahead).Confirm();
            break;
        }
      }
    }

    void OnFocusIn(FocusInEvent evt)
    {
      if (_suggestions != null && !_confirmed)
      {
        _controller.Get(ScreenController.Typeahead).Show(new Typeahead.Args(_field, _suggestions));
      }

      // Focus is received after picking an option from a typeahead with the enter key, but we don't want to
      // immediately show the typeahead *again*, so we ignore one focus event after confirmation.
      _confirmed = false;
    }

    void OnFocusOut(FocusOutEvent evt)
    {
      if (evt.relatedTarget != null)
      {
        // Hack: OnFocusOut fired when a typeahead option is selected as well as on focus change. We look at
        // whether there is a related target to decide whether to hide the typeahead or let it process the event.
        _controller.Get(ScreenController.Typeahead).Hide();
      }
    }

    static string DictionaryPreview(IDictionary? dictionary)
    {
      var preview = new StringBuilder();
      if (dictionary != null)
      {
        foreach (var key in dictionary.Keys)
        {
          preview.Append($"{key} => {dictionary[key]}\n");
        }
      }

      return preview.ToString();
    }
  }
}
