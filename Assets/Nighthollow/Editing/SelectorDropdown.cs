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
using Nighthollow.Interface;
using UnityEngine;
using UnityEngine.UIElements;

#nullable enable

namespace Nighthollow.Editing
{
  public sealed class SelectorDropdownEditorCell : EditorCell
  {
    readonly ScreenController _screenController;
    readonly IEditor _parent;
    readonly List<string> _options;
    readonly Action<int> _onSelected;
    SelectorDropdown? _dropdown;
    int? _currentlySelected;

    public SelectorDropdownEditorCell(
      ScreenController screenController,
      EditorSheetDelegate.DropdownCell content,
      IEditor parent)
    {
      _screenController = screenController;
      _options = content.Options;
      _onSelected = content.OnSelected;
      _currentlySelected = content.CurrentlySelected;
      _parent = parent;

      var button = new Button {text = _currentlySelected.HasValue ? _options[_currentlySelected.Value] : "Dropdown:"};
      button.AddToClassList("editor-cell-button");
      Add(button);
    }

    public override void Activate()
    {
      _dropdown = new SelectorDropdown(
        _screenController,
        _parent!,
        worldBound,
        _options,
        _currentlySelected,
        selected =>
        {
          _currentlySelected = selected;
          _onSelected(selected);
        });
    }

    public override void Deactivate()
    {
      _dropdown?.Hide();
    }

    public override void OnParentKeyDown(KeyDownEvent evt)
    {
      _dropdown?.OnParentKeyDown(evt);
    }
  }

  public sealed class SelectorDropdown : VisualElement
  {
    readonly IEditor _parentEditor;
    readonly List<Label> _options;
    readonly Action<int> _onSelected;
    int? _currentlySelected;

    public SelectorDropdown(
      ScreenController controller,
      IEditor parentEditor,
      Rect worldAnchor,
      IReadOnlyList<string> options,
      int? currentlySelected,
      Action<int> onSelected)
    {
      _parentEditor = parentEditor;
      _onSelected = onSelected;

      AddToClassList("dropdown");
      style.left = worldAnchor.x;
      style.top = worldAnchor.y + worldAnchor.height;
      style.position = new StyleEnum<Position>(Position.Absolute);
      controller.Screen.Add(this);

      _options = new List<Label>();
      for (var i = 0; i < options.Count; i++)
      {
        var option = options[i];
        var label = new Label {text = option};
        label.AddToClassList("dropdown-option");
        var index = i;
        label.RegisterCallback<ClickEvent>(e =>
        {
          _parentEditor.FocusRoot();
          _onSelected(index);
          Hide();
        });
        _options.Add(label);
        Add(label);
      }

      if (currentlySelected.HasValue)
      {
        Select(currentlySelected.Value);
      }
    }

    public void OnParentKeyDown(KeyDownEvent evt)
    {
      switch (evt.keyCode)
      {
        case KeyCode.DownArrow:
          Select(_currentlySelected.HasValue ? (_currentlySelected.Value + 1) % _options.Count : 0);
          break;
        case KeyCode.UpArrow:
          Select(_currentlySelected.HasValue
            ? Mathf.Abs(_currentlySelected.Value - 1 % _options.Count)
            : _options.Count - 1);
          break;
        case KeyCode.Backspace:
        case KeyCode.Escape:
          Hide();
          break;
        case KeyCode.KeypadEnter:
        case KeyCode.Return:
          if (_currentlySelected.HasValue)
          {
            _onSelected(_currentlySelected.Value);
          }
          Hide();

          break;
      }
    }

    public void Hide()
    {
      _currentlySelected = null;
      RemoveFromHierarchy();
      _parentEditor.OnChildEditingComplete();
    }

    void Select(int index)
    {
      if (_currentlySelected.HasValue)
      {
        _options[_currentlySelected.Value].RemoveFromClassList("selected");
      }

      _options[index].AddToClassList("selected");
      _currentlySelected = index;
    }
  }
}
