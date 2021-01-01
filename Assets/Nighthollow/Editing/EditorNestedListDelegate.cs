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

using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Nighthollow.Interface;
using UnityEngine;
using UnityEngine.UIElements;

#nullable enable

namespace Nighthollow.Editing
{
  public sealed class EditorNestedListSheetDelegate : EditorSheetDelegate
  {
    public EditorNestedListSheetDelegate(ReflectivePath path)
    {
      var listType = path.GetUnderlyingType();
      var listContentType = listType.GetGenericArguments()[0];
      var properties = listContentType.GetProperties();
      Headings = properties.Select(TableEditorSheetDelegate.PropertyName).ToList();
      Cells = new List<List<ReflectivePath>>();
      if (path.Read() is IList value)
      {
        for (var index = 0; index < value.Count; ++index)
        {
          Cells.Add(properties.Select(property => path.ListIndex(listContentType, index).Property(property)).ToList());
        }
      }
    }

    public override List<string> Headings { get; }

    public override List<List<ReflectivePath>> Cells { get; }
  }

  public sealed class EditorNestedListCellDelegate : EditorCellDelegate
  {
    readonly ScreenController _screenController;
    readonly ReflectivePath _reflectivePath;
    readonly EditorSheetDelegate _sheetDelegate;

    IEditor _parent = null!;
    EditorSheet? _sheet;
    VisualElement? _editorContainer;

    public EditorNestedListCellDelegate(ScreenController screenController, ReflectivePath reflectivePath)
    {
      _screenController = screenController;
      _reflectivePath = reflectivePath;
      _sheetDelegate = new EditorNestedListSheetDelegate(reflectivePath);
    }

    public override void Initialize(TextField field, IEditor parent)
    {
      _parent = parent;
    }

    public override string? RenderPreview(object? value) =>
      value is IList list ?
        string.Join(",\n", list.Cast<object>().Take(3).Select(o => o.ToString())) :
        null;

    public override void OnActivate(TextField field, Rect worldBound)
    {
      _sheet = new EditorSheet(_screenController, _sheetDelegate, _parent);
      _editorContainer = new VisualElement();
      _editorContainer.AddToClassList("inline-editor");
      _editorContainer.Add(_sheet);
      _editorContainer.style.top = worldBound.y + worldBound.height;
      _editorContainer.style.left = worldBound.x - _sheet.Width + worldBound.width;
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
