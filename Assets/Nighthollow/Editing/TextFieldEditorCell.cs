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
  public sealed class TextFieldEditorCell : EditorCell, IEditor
  {
    readonly TextField? _field;
    readonly IEditor _parent;
    readonly TextFieldEditorCellDelegate _cellDelegate;
    bool _active;

    public TextFieldEditorCell(string initialContent, IEditor parent, TextFieldEditorCellDelegate cellDelegate)
    {
      _parent = parent;
      _cellDelegate = cellDelegate;

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

      _field.value = initialContent;

      // TODO: This causes crashes when we change subtypes -- is it needed?
      // reflectivePath.OnEntityUpdated(() => { _field.value = reflectivePath.RenderPreview(); });
    }

    public VisualElement VisualElement => this;

    public override void Activate()
    {
      RemoveFromClassList("selected");
      AddToClassList("active");

      _cellDelegate.OnActivate(_field!, worldBound);
      _active = true;
    }

    public override void Deactivate()
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

    public void OnDataChanged()
    {
      _parent.OnDataChanged();
    }

    // Unity just sort of randomly gives KeyDown events to whoever it feels like, so we need this method to detect
    // the "Enter" key and the below method to detect other key presses :(
    void OnKeyDownInternal(KeyDownEvent evt)
    {
      switch (evt.keyCode)
      {
        case KeyCode.Tab:
        case KeyCode.Escape:
        case KeyCode.KeypadEnter:
        case KeyCode.Return:
          Deactivate();
          break;
      }
    }

    public override void OnParentKeyDown(KeyDownEvent evt)
    {
      _cellDelegate.OnParentKeyDown(evt);
    }

    public override string? Preview() => _field?.value;

    public void FocusRoot()
    {
      _parent.FocusRoot();
    }
  }
}
