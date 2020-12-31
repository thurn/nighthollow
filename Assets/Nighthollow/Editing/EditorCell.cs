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

using UnityEngine;
using UnityEngine.UIElements;

#nullable enable

namespace Nighthollow.Editing
{
  public abstract class EditorCellDelegate
  {
    public abstract string? Initialize(TextField field, ReflectivePath reflectivePath);

    public abstract void OnActivate(TextField field);

    public abstract void OnKeyDown(KeyDownEvent evt, IEditor parent);
  }

  public sealed class EditorCell : VisualElement
  {
    readonly ReflectivePath _reflectivePath;
    readonly TextField _field;
    readonly IEditor _parent;
    readonly EditorCellDelegate _cellDelegate;

    public EditorCell(ReflectivePath reflectivePath, IEditor parent, EditorCellDelegate cellDelegate)
    {
      _reflectivePath = reflectivePath;
      _parent = parent;
      _cellDelegate = cellDelegate;
      AddToClassList("editor-cell");

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

      _field.RegisterCallback<KeyDownEvent>(OnKeyDown);
      _field.value = _cellDelegate.Initialize(_field, _reflectivePath);
    }

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
      if (_reflectivePath.IsReadOnly)
      {
        _parent.OnChildEditingComplete();
        return;
      }

      RemoveFromClassList("selected");
      AddToClassList("active");

      _cellDelegate.OnActivate(_field);
    }

    public void Deactivate()
    {
      _field.focusable = false;
      RemoveFromClassList("active");
      AddToClassList("selected");
      _parent.OnChildEditingComplete();
    }

    void OnKeyDown(KeyDownEvent evt)
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
  }
}
