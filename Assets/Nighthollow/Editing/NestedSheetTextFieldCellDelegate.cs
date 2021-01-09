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
using Nighthollow.Interface;
using UnityEngine;
using UnityEngine.UIElements;

#nullable enable

namespace Nighthollow.Editing
{
  public sealed class NestedSheetTextFieldCellDelegate : TextFieldEditorCellDelegate
  {
    readonly ScreenController _screenController;
    readonly EditorSheetDelegate _sheetDelegate;

    IEditor _parent = null!;
    EditorSheet? _sheet;
    VisualElement? _editorContainer;

    public NestedSheetTextFieldCellDelegate(ScreenController screenController, EditorSheetDelegate sheetDelegate)
    {
      _screenController = screenController;
      _sheetDelegate = sheetDelegate;
    }

    public override void Initialize(TextField field, IEditor parent)
    {
      _parent = parent;
    }

    public override void OnActivate(TextField field, Rect worldBound)
    {
      _sheet = new EditorSheet(_screenController, _sheetDelegate, _parent);
      _editorContainer = new VisualElement();
      _editorContainer.AddToClassList("inline-editor");
      _editorContainer.Add(_sheet);
      _editorContainer.style.top = Math.Max(16, worldBound.y + worldBound.height);
      _editorContainer.style.left = Math.Max(16, worldBound.x - _sheet.Width + worldBound.width);
      _screenController.Screen.Add(_editorContainer);
    }

    public override void OnParentKeyDown(KeyDownEvent evt)
    {
      _sheet!.OnKeyDown(evt);
    }

    public override void OnDeactivate(TextField field)
    {
      _sheet!.DeactivateAllCells();
      _editorContainer!.RemoveFromHierarchy();
      _sheet = null;
      _editorContainer = null;
    }
  }
}
