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
using Nighthollow.Data;
using Nighthollow.Interface;
using Nighthollow.Stats2;

#nullable enable

namespace Nighthollow.Editing
{
  public static class EditorCellFactory
  {
    public static EditorCell CreateBlank() => new LabelEditorCell(null);

    public static EditorCell Create(
      ScreenController screenController,
      IEditor parent,
      EditorSheetDelegate.ICellContent cellContent) =>
      cellContent.Switch(
        reflectivePath => CreateTextFieldEditorCell(screenController, parent, reflectivePath),
        CreateLabelEditorCell,
        button => CreateButtonCell(button, parent),
        dropdown => CreateDropdownCell(screenController, dropdown, parent),
        CreateImageCell);

    static EditorCell CreateTextFieldEditorCell(
      ScreenController screenController,
      IEditor parent,
      ReflectivePath reflectivePath)
    {
      var type = reflectivePath.GetUnderlyingType();
      type = TypeUtils.UnboxNullable(type);

      TextFieldEditorCellDelegate cellDelegate;
      if (type == typeof(string))
      {
        cellDelegate = new PrimitiveEditorCellDelegate<string>(reflectivePath, Identity);
      }
      else if (type == typeof(int))
      {
        cellDelegate = new PrimitiveEditorCellDelegate<int>(reflectivePath, int.TryParse);
      }
      else if (type == typeof(long))
      {
        cellDelegate = new PrimitiveEditorCellDelegate<long>(reflectivePath, long.TryParse);
      }
      else if (type == typeof(bool))
      {
        cellDelegate = new PrimitiveEditorCellDelegate<bool>(reflectivePath, bool.TryParse);
      }
      else if (type == typeof(int))
      {
        cellDelegate = new PrimitiveEditorCellDelegate<int>(reflectivePath, int.TryParse);
      }
      else if (type == typeof(IntRangeValue))
      {
        cellDelegate = new PrimitiveEditorCellDelegate<IntRangeValue>(reflectivePath, IntRangeValue.TryParse);
      }
      else if (type == typeof(IValueData))
      {
        cellDelegate = new PrimitiveEditorCellDelegate<IValueData>(reflectivePath, ValueDataUtil.TryParse);
      }
      else if (type.IsSubclassOf(typeof(Enum)))
      {
        cellDelegate = new EditorDropdownCellDelegate(screenController, reflectivePath);
      }
      else if (type.GetInterface("IList") != null)
      {
        cellDelegate = new NestedEditorCellDelegate(
          screenController,
          new NestedListEditorSheetDelegate(reflectivePath));
      }
      else
      {
        throw new InvalidOperationException($"No editor registered for type {type}");
      }

      return new TextFieldEditorCell(reflectivePath, parent, cellDelegate);
    }

    static EditorCell CreateLabelEditorCell(string? text) => new LabelEditorCell(text);

    static EditorCell CreateButtonCell(EditorSheetDelegate.ButtonCell content, IEditor parent) =>
      new ButtonEditorCell(content, parent);

    static EditorCell CreateDropdownCell(
      ScreenController screenController,
      EditorSheetDelegate.DropdownCell content,
      IEditor parent) =>
      new SelectorDropdownEditorCell(screenController, content, parent);

    static EditorCell CreateImageCell(EditorSheetDelegate.ImageCell cell) => new ImageEditorCell(cell.ImagePath);

    static bool Identity(string input, out string output)
    {
      output = input;
      return true;
    }
  }
}
