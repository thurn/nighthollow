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
using System.Text;
using Nighthollow.Stats;
using UnityEngine;
using UnityEngine.UIElements;

#nullable enable

namespace Nighthollow.Interface
{
  public sealed class EditorCell : VisualElement
  {
    readonly ScreenController _controller;
    readonly ObjectEditor _editor;
    readonly Vector2Int _position;
    readonly TextField _field;
    readonly List<string>? _suggestions;
    readonly ObjectEditor.Config? _inlineEditorConfig;
    bool _confirmed;
    VisualElement? _inlineEditor;

    static readonly HashSet<Type> KnownPrimitives = new HashSet<Type>
    {
      typeof(int),
      typeof(string),
      typeof(bool),
      typeof(IntRangeValue),
      typeof(DurationValue),
      typeof(PercentageValue)
    };

    public EditorCell(
      ScreenController controller,
      ObjectEditor editor,
      Vector2Int position,
      Type type,
      int width,
      object? value = null,
      string? name = null)
    {
      _controller = controller;
      _editor = editor;
      _position = position;
      string? rendered;
      bool canModify;
      AddToClassList("editor-cell");
      style.width = width;

      if (name != null && name.Equals("Id"))
      {
        rendered = value?.ToString();
        canModify = false;
      }
      else if (KnownPrimitives.Contains(type))
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
        _inlineEditorConfig = ObjectEditor.ForPrimitiveDictionary(type, value as IDictionary);
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
        focusable = false
      };

      if (!canModify)
      {
        _field.AddToClassList("read-only");
      }

      if (rendered != null)
      {
        _field.value = rendered;
      }

      _field.AddToClassList("editor-text-field");
      Add(_field);

      _field.RegisterCallback<ClickEvent>(OnClick);
    }

    void OnClick(ClickEvent evt)
    {
      _editor.OnClick(_position);
    }

    public void Select()
    {
      _field.focusable = true;
      _field.Focus();
      AddToClassList("selected");

      if (_suggestions != null)
      {
        _editor.ShowTypeahead(_field, _suggestions);
      }

      if (_inlineEditorConfig != null)
      {
        _editor.ShowInlineEditor(this, _inlineEditorConfig);
      }

      _field.SelectAll();

      // Hack: Unity's SelectAll() doesn't always work for some reason, this seems to help
      InterfaceUtils.After(0.01f, () =>
      {
        _field.SelectRange(0, 1);
        _field.SelectAll();
      });
    }

    public void Unselect()
    {
      _field.focusable = false;
      RemoveFromClassList("selected");
    }

    void OnKeyDown(KeyDownEvent evt)
    {
      if (_suggestions != null)
      {
        switch (evt.keyCode)
        {
          case KeyCode.DownArrow:
            // _controller.Get(ScreenController.Typeahead).Next();
            break;
          case KeyCode.UpArrow:
            // _controller.Get(ScreenController.Typeahead).Previous();
            break;
          case KeyCode.Return:
            _confirmed = true;
            // _controller.Get(ScreenController.Typeahead).Confirm();
            break;
        }
      }
    }

    void OnFocusIn(FocusInEvent evt)
    {
      if (_suggestions != null && !_confirmed)
      {
        // _controller.Get(ScreenController.Typeahead).Show(new Typeahead.Args(_field, _suggestions));
      }

      // Focus is received after picking an option from a typeahead with the enter key, but we don't want to
      // immediately show the typeahead *again*, so we ignore one focus event after confirmation.
      _confirmed = false;

      AddToClassList("selected");

      if (_inlineEditorConfig != null)
      {
        _inlineEditor = new VisualElement();
        _inlineEditor.AddToClassList("inline-editor");
        _inlineEditor.Add(new ObjectEditor(_controller, _inlineEditorConfig));
        _inlineEditor.style.top = worldBound.y + worldBound.height;
        _inlineEditor.style.left = worldBound.x - _inlineEditorConfig.Width + worldBound.width;
        _controller.Screen.Add(_inlineEditor);
      }

      // Hack: Unity's text input sometimes doesn't select the text properly on focus, this fixes it for some reason
      InterfaceUtils.After(0.01f, () =>
      {
        _field.SelectRange(0, 1);
        _field.SelectAll();
      });
    }

    void OnFocusOut(FocusOutEvent evt)
    {
      if (evt.relatedTarget != null)
      {
        // Hack: OnFocusOut fired when a typeahead option is clicked as well as on focus change. We look at
        // whether there is a related target to decide whether to hide the typeahead or let it process the event.
        // _controller.Get(ScreenController.Typeahead).Hide();
        RemoveFromClassList("selected");
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
