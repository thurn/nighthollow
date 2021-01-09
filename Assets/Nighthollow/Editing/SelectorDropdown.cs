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
using System.Collections.Immutable;
using System.Linq;
using Nighthollow.Interface;
using UnityEngine;
using UnityEngine.UIElements;

#nullable enable

namespace Nighthollow.Editing
{
  public class SelectorDropdownEditorCell : EditorCell
  {
    readonly ScreenController _screenController;
    readonly IEditor _parent;
    readonly IReadOnlyList<string> _options;
    readonly Action<int> _onSelected;
    readonly TextField _field;
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

      _field = new TextField
      {
        multiline = false,
        doubleClickSelectsWord = true,
        tripleClickSelectsLine = true,
        isDelayed = false,
        focusable = false,
        value = _currentlySelected.HasValue ? _options[_currentlySelected.Value] : content.DefaultText
      };

      _field.AddToClassList("editor-text-field");
      Add(_field);

      _field.RegisterCallback<KeyDownEvent>(OnKeyDownInternal);
      _field.RegisterCallback<ChangeEvent<string>>(OnTextFieldChanged);
    }

    void OnTextFieldChanged(ChangeEvent<string> evt)
    {
      _dropdown?.FilterOptions(evt.newValue);
    }

    protected void SetTextFieldValue(string? newValue)
    {
      _field.value = newValue;
    }

    public override void Activate()
    {
      InterfaceUtils.FocusTextField(_field);

      _dropdown = new SelectorDropdown(
        _screenController,
        _parent!,
        worldBound,
        _options,
        _currentlySelected,
        selected =>
        {
          var index = _options.ToList().FindIndex(o => o.Equals(selected));
          _currentlySelected = index;
          _onSelected(index);
        });
    }

    public override void Deactivate()
    {
      _dropdown?.Hide();
      _parent.OnChildEditingComplete();
    }

    void OnKeyDownInternal(KeyDownEvent evt)
    {
      switch (evt.keyCode)
      {
        case KeyCode.Tab:
        case KeyCode.Escape:
        case KeyCode.KeypadEnter:
        case KeyCode.Return:
        case KeyCode.DownArrow:
        case KeyCode.UpArrow:
          _dropdown?.OnParentKeyDown(evt);
          break;
      }
    }

    public override void OnParentKeyDown(KeyDownEvent evt)
    {
      _dropdown?.OnParentKeyDown(evt);
    }
  }

  public sealed class EnumDropdownEditorCell : SelectorDropdownEditorCell
  {
    public EnumDropdownEditorCell(
      ScreenController screenController,
      ReflectivePath reflectivePath,
      Type type,
      IEditor parent) : base(screenController, BuildContent(reflectivePath, type), parent)
    {
      reflectivePath.OnEntityUpdated(_ => { SetTextFieldValue(reflectivePath.Read()?.ToString()); });
    }

    static EditorSheetDelegate.DropdownCell BuildContent(ReflectivePath reflectivePath, Type type)
    {
      var values = Enum.GetValues(type)!;
      var options = new List<string>();
      for (var i = 0; i < values.Length; ++i)
      {
        var value = values.GetValue(i);
        if (value.ToString().Equals("Unknown"))
        {
          continue;
        }

        options.Add(value.ToString());
      }

      var current = reflectivePath.Read()?.ToString();
      var currentlySelected = options.FindIndex(o => o.Equals(current));
      return new EditorSheetDelegate.DropdownCell(options,
        currentlySelected == -1 ? (int?) null : currentlySelected,
        selected =>
        {
          var value = Enum.Parse(type, options[selected]);
          reflectivePath.Write(value);
        });
    }
  }

  public sealed class SelectorDropdown : VisualElement
  {
    readonly IEditor _parentEditor;
    readonly ISet<string> _options;
    readonly List<Label> _optionLabels;
    readonly Action<string> _onSelected;
    readonly VisualElement _content;
    readonly ScrollView _scrollView;
    int? _currentlySelected;

    public SelectorDropdown(
      ScreenController controller,
      IEditor parentEditor,
      Rect worldAnchor,
      IReadOnlyList<string> options,
      int? currentlySelected,
      Action<string> onSelected)
    {
      _parentEditor = parentEditor;
      _onSelected = onSelected;
      _options = options.ToImmutableHashSet();
      _optionLabels = new List<Label>();

      AddToClassList("dropdown");
      style.left = worldAnchor.x;
      style.top = worldAnchor.y + worldAnchor.height;
      style.position = new StyleEnum<Position>(Position.Absolute);
      controller.Screen.Add(this);

      _content = new VisualElement();
      _content.AddToClassList("dropdown-content");
      if (_options.Count >= 5)
      {
        _content.style.height = 300;
      }

      _scrollView = new ScrollView();
      _scrollView.AddToClassList("dropdown-scroll-view");
      _scrollView.Add(_content);

      Add(_scrollView);

      RenderOptions(currentlySelected.HasValue ? options[currentlySelected.Value] : null, "");
    }

    void RenderOptions(string? currentlySelected, string inputFilter)
    {
      _content.Clear();
      _optionLabels.Clear();
      var lookup = _options.ToLookup(o => MatchesFilter(o, inputFilter));

      foreach (var option in lookup[true].Concat(lookup[false]))
      {
        var label = new Label {text = option};
        label.AddToClassList("dropdown-option");
        label.RegisterCallback<ClickEvent>(e =>
        {
          _parentEditor.FocusRoot();
          OnSelected(option);
          Hide();
        });
        _optionLabels.Add(label);
        _content.Add(label);
      }

      if (currentlySelected != null && MatchesFilter(currentlySelected, inputFilter))
      {
        Select(_optionLabels.FindIndex(label => label.text.Equals(currentlySelected)));
      }
      else
      {
        ClearSelection();
      }
    }

    public void OnParentKeyDown(KeyDownEvent evt)
    {
      switch (evt.keyCode)
      {
        case KeyCode.DownArrow:
          Select(_currentlySelected.HasValue ? (_currentlySelected.Value + 1) % _optionLabels.Count : 0);
          break;
        case KeyCode.UpArrow:
          if (_currentlySelected.HasValue)
          {
            Select(_currentlySelected.Value == 0
              ? _optionLabels.Count - 1
              : (_currentlySelected.Value - 1) % _options.Count);
          }
          else
          {
            Select(_optionLabels.Count - 1);
          }

          break;
        case KeyCode.Backspace:
        case KeyCode.Escape:
          Hide();
          break;
        case KeyCode.KeypadEnter:
        case KeyCode.Return:
          var currentlySelected = _currentlySelected;
          Hide();

          if (currentlySelected.HasValue)
          {
            OnSelected(_optionLabels[currentlySelected.Value].text);
          }

          break;
      }
    }

    void OnSelected(string value)
    {
      // Insert a delay before invoking callback, otherwise you get into a weird situation where the key event
      // handler from the parent is still running and sees the event too.
      InterfaceUtils.After(0.01f, () => _onSelected(value));
    }

    public void Hide()
    {
      _currentlySelected = null;
      RemoveFromHierarchy();
      _parentEditor.OnChildEditingComplete();
    }

    void Select(int value)
    {
      if (value == -1)
      {
        return;
      }

      if (_currentlySelected != null)
      {
        _optionLabels[_currentlySelected.Value].RemoveFromClassList("selected");
      }

      _optionLabels[value].AddToClassList("selected");
      _scrollView.ScrollTo(_optionLabels[value]);
      _currentlySelected = value;
    }

    void ClearSelection()
    {
      if (_currentlySelected != null)
      {
        _optionLabels[_currentlySelected.Value].RemoveFromClassList("selected");
      }

      _currentlySelected = null;
    }

    public void FilterOptions(string input)
    {
      RenderOptions(_currentlySelected.HasValue ? _optionLabels[_currentlySelected.Value].text : null, input);
    }

    static bool MatchesFilter(string text, string filter) =>
      text.ToLowerInvariant().Contains(filter.ToLowerInvariant());
  }
}