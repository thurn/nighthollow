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
using Nighthollow.Data;
using Nighthollow.Editing;
using Nighthollow.Stats;
using UnityEngine;
using UnityEngine.UIElements;

#nullable enable

namespace Nighthollow.Interface
{
  public sealed class EditorCell : VisualElement
  {
    readonly ScreenController _controller;
    readonly ReflectivePath? _reflectivePath;
    readonly ObjectEditor _editor;
    readonly Vector2Int _position;
    readonly TextField _field;
    readonly List<string>? _suggestions;
    readonly ObjectEditor.Config? _inlineEditorConfig;

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
      ReflectivePath? reflectivePath,
      ObjectEditor editor,
      Vector2Int position,
      Type type,
      int width,
      object? value = null,
      string? name = null)
    {
      _controller = controller;
      _reflectivePath = reflectivePath;
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
      else if (typeof(IDictionary).IsAssignableFrom(type) || type.Name.Contains("IReadOnlyDictionary"))
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

      if (rendered != null)
      {
        _field.value = rendered;
      }

      _field.AddToClassList("editor-text-field");
      Add(_field);

      _field.RegisterCallback<ClickEvent>(OnClick);
      _field.RegisterCallback<ChangeEvent<string>>(OnValueChanged);
    }

    void OnClick(ClickEvent evt)
    {
      _editor.OnClick(_position);
    }

    public bool Focusable
    {
      set
      {
        _field.focusable = value;
        if (value)
        {
          _field.Focus();
        }
      }
    }

    public void Select()
    {
      AddToClassList("selected");

      if (_suggestions != null)
      {
        _editor.ShowTypeahead(_field, _suggestions);
      }

      if (_inlineEditorConfig != null)
      {
        _editor.ShowInlineEditor(this, _inlineEditorConfig);
      }

      _field.focusable = true;
      _field.Focus();

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

    void OnValueChanged(ChangeEvent<string> evt)
    {
      _reflectivePath?.Write(evt.newValue);
    }
  }
}
