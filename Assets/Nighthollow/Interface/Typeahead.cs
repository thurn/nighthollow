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
using UnityEngine;
using UnityEngine.UIElements;

#nullable enable

namespace Nighthollow.Interface
{
  public sealed class Typeahead : HideableElement<Typeahead.Args>
  {
    public sealed class Args
    {
      public Args(TextField field, List<string> suggestions)
      {
        Field = field;
        Suggestions = suggestions;
      }

      public TextField Field { get; }
      public List<string> Suggestions { get; }
    }

    public new sealed class UxmlFactory : UxmlFactory<Typeahead, UxmlTraits>
    {
    }

    int? _selectedIndex;
    TextField _field = null!;
    List<Label> _suggestions = null!;

    protected override void Initialize()
    {
    }

    protected override void OnShow(Args argument)
    {
      Clear();
      _selectedIndex = null;

      _field = argument.Field;
      style.left = _field.worldBound.x;
      style.top = _field.worldBound.y + _field.worldBound.height;
      style.width = _field.worldBound.width;

      _suggestions = new List<Label>();
      foreach (var suggestion in argument.Suggestions)
      {
        var label = new Label {text = suggestion};
        label.AddToClassList("typeahead-option");
        label.RegisterCallback<ClickEvent>(e =>
        {
          _field.value = suggestion;
          _field.SelectAll();
          Hide();
        });
        _suggestions.Add(label);
        Add(label);
      }
    }

    public void Next()
    {
      Highlight(_selectedIndex.HasValue ? (_selectedIndex.Value + 1) % _suggestions.Count : 0);
    }

    public void Previous()
    {
      Highlight(_selectedIndex.HasValue
        ? Mathf.Abs(_selectedIndex.Value - 1 % _suggestions.Count)
        : _suggestions.Count - 1);
    }

    public void Confirm()
    {
      if (_selectedIndex.HasValue)
      {
        _field.value = _suggestions[_selectedIndex.Value].text;
      }

      Hide();
    }

    void Highlight(int selected)
    {
      _selectedIndex = selected;
      for (var i = 0; i < _suggestions.Count; ++i)
      {
        if (i == selected)
        {
          _suggestions[i].AddToClassList("selected");
        }
        else
        {
          _suggestions[i].RemoveFromClassList("selected");
        }
      }
    }
  }
}
