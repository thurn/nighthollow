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
using System.Security.Permissions;
using UnityEngine;
using UnityEngine.UIElements;

#nullable enable

namespace Nighthollow.Editing
{
  public abstract class EditorCellDelegate
  {
    public virtual void Initialize(TextField field, IEditor parent)
    {
    }

    public abstract string? RenderPreview(object? value);

    public abstract void OnActivate(TextField field, Rect worldBound);

    public virtual void OnParentKeyDown(KeyDownEvent evt) { }

    public virtual void OnDeactivate(TextField field) { }
  }

  public sealed class EditorCell : VisualElement, IEditor
  {
    readonly TextField? _field;
    readonly IEditor _parent;
    readonly EditorCellDelegate _cellDelegate;
    bool _active;

    public EditorCell(ReflectivePath reflectivePath, IEditor parent, EditorCellDelegate cellDelegate)
    {
      _parent = parent;
      _cellDelegate = cellDelegate;
      AddToClassList("editor-cell");

      if (reflectivePath.IsReadOnly)
      {
        AddToClassList("editor-emphasized");
        var label = new Label {text = _cellDelegate.RenderPreview(reflectivePath.Read())};
        Add(label);
        CanActivate = false;
      }
      else
      {
        _field = new TextField
        {
          multiline = false,
          doubleClickSelectsWord = true,
          tripleClickSelectsLine = true,
          isDelayed = true,
          focusable = false,
        };
        _field.AddToClassList("editor-text-field");
        Add(_field);
        _field.RegisterCallback<KeyDownEvent>(OnKeyDownInternal);
        _cellDelegate.Initialize(_field, this);

        reflectivePath.Subscribe(v =>
        {
          _field.value = _cellDelegate.RenderPreview(v);
        });
        CanActivate = true;
      }
    }

    public bool CanActivate { get; }

    public void Select()
    {
      AddToClassList("selected");
    }

    public void Unselect()
    {
      RemoveFromClassList("selected");
    }

    public void Activate()
    {
      if (!CanActivate)
      {
        throw new InvalidOperationException($"Cannot activate {this}");
      }

      RemoveFromClassList("selected");
      AddToClassList("active");

      _cellDelegate.OnActivate(_field!, worldBound);
      _active = true;
    }

    public void Deactivate()
    {
      if (_active)
      {
        RemoveFromClassList("active");
        AddToClassList("selected");
        _cellDelegate.OnDeactivate(_field!);
        _active = false;
        _parent.OnChildEditingComplete();
      }
    }

    public void OnChildEditingComplete()
    {
      Deactivate();
    }

    // Unity just sort of randomly gives KeyDown events to whoever it feels like, so we need this method to detect
    // the "Enter" key and the below method to detect other key presses :(
    void OnKeyDownInternal(KeyDownEvent evt)
    {
      switch (evt.keyCode)
      {
        case KeyCode.Escape:
        case KeyCode.KeypadEnter:
        case KeyCode.Return:
          Deactivate();
          break;
      }
    }

    public void OnParentKeyDown(KeyDownEvent evt)
    {
      _cellDelegate.OnParentKeyDown(evt);
    }

    public void FocusRoot()
    {
      _parent.FocusRoot();
    }
  }
}
