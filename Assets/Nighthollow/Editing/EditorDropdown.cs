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
using System.Linq;
using Nighthollow.Interface;
using UnityEngine;
using UnityEngine.UIElements;

#nullable enable

namespace Nighthollow.Editing
{
  public sealed class EditorDropdownCellDelegate : EditorCellDelegate
  {
    readonly ReflectivePath _reflectivePath;
    readonly List<string> _options;
    IEditor _parent = null!;
    EditorDropdown _dropdown;

    public EditorDropdownCellDelegate(ScreenController screenController, ReflectivePath reflectivePath)
    {
      _reflectivePath = reflectivePath;
      _options = Enum.GetValues(_reflectivePath.GetUnderlyingType())
        .Cast<object>()
        .Select(v => v.ToString())
        .Where(v => !v.Equals("Unknown"))
        .ToList();
      _dropdown = screenController.Get(ScreenController.EditorDropdown);
    }

    public override void Initialize(TextField field, IEditor parent)
    {
      _parent = parent;
    }

    public override string? RenderPreview(object? value) => value?.ToString();

    public override void OnActivate(TextField field, Rect worldBound)
    {
      _dropdown.Show(new EditorDropdown.Args(_options, worldBound, _reflectivePath, _parent));
    }

    public override void OnParentKeyDown(KeyDownEvent evt)
    {
      _dropdown.OnParentKeyDown(evt);
    }

    public override void OnDeactivate(TextField field)
    {
      _dropdown.OnDeactivate();
    }
  }

  public sealed class EditorDropdown : HideableElement<EditorDropdown.Args>
  {
    public readonly struct Args
    {
      public Args(List<string> options, Rect anchor, ReflectivePath reflectivePath, IEditor parentEditor)
      {
        Options = options;
        Anchor = anchor;
        ReflectivePath = reflectivePath;
        ParentEditor = parentEditor;
      }

      public List<string> Options { get; }
      public Rect Anchor { get; }
      public ReflectivePath ReflectivePath { get; }
      public IEditor ParentEditor { get; }
    }

    int? _selectedIndex;
    List<Label> _options = null!;
    IEditor _parentEditor = null!;
    ReflectivePath _reflectivePath = null!;

    protected override void Initialize()
    {
      AddToClassList("dropdown");
    }

    protected override void OnShow(Args argument)
    {
      Clear();
      BringToFront();

      _parentEditor = argument.ParentEditor;
      _reflectivePath = argument.ReflectivePath;
      _selectedIndex = null;

      style.left = argument.Anchor.x;
      style.top = argument.Anchor.y + argument.Anchor.height;
      style.width = argument.Anchor.width;

      _options = new List<Label>();
      foreach (var option in argument.Options)
      {
        var label = new Label {text = option};
        label.AddToClassList("dropdown-option");
        label.RegisterCallback<ClickEvent>(e =>
        {
          _parentEditor.OnChildClickEvent(e);
          WriteSelection(option);
        });
        _options.Add(label);
        Add(label);
      }

      var current = _reflectivePath.Read();
      var match = _options.FindIndex(option => option.text.Equals(current?.ToString()));
      if (match != -1)
      {
        Select(match);
      }
    }

    public void OnParentKeyDown(KeyDownEvent evt)
    {
      switch (evt.keyCode)
      {
        case KeyCode.DownArrow:
          Select(_selectedIndex.HasValue ? (_selectedIndex.Value + 1) % _options.Count : 0);
          break;
        case KeyCode.UpArrow:
          Select(_selectedIndex.HasValue
            ? Mathf.Abs(_selectedIndex.Value - 1 % _options.Count)
            : _options.Count - 1);
          break;
        case KeyCode.Backspace:
        case KeyCode.Escape:
          WriteSelection(null);
          break;
        case KeyCode.KeypadEnter:
        case KeyCode.Return:
          WriteSelection(_selectedIndex.HasValue ? _options[_selectedIndex.Value].text : null);
          break;
      }
    }

    void Select(int index)
    {
      if (_selectedIndex.HasValue)
      {
        _options[_selectedIndex.Value].RemoveFromClassList("selected");
      }

      _options[index].AddToClassList("selected");
      _selectedIndex = index;
    }

    void WriteSelection(string? value)
    {
      if (value != null)
      {
        _reflectivePath.Write(Enum.Parse(_reflectivePath.GetUnderlyingType(), value));
      }

      Hide();
      _parentEditor.OnChildEditingComplete();
    }

    public void OnDeactivate()
    {
      _selectedIndex = null;
      Hide();
    }
  }
}
