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
using Nighthollow.Stats;
using UnityEngine;
using UnityEngine.UIElements;

#nullable enable

namespace Nighthollow.Editing
{
  public static class EditorCellFactory
  {
    public static EditorCell Create(ScreenController screenController, ReflectivePath reflectivePath, IEditor parent)
    {
      var type = reflectivePath.GetUnderlyingType();
      type = TypeUtils.UnboxNullable(type);

      EditorCellDelegate cellDelegate;
      if (reflectivePath.IsReadOnly)
      {
        cellDelegate = new ReadOnlyEditorCellDelegate();
      }
      else if (type == typeof(string))
      {
        cellDelegate = new PrimitiveEditorCellDelegate<string>(reflectivePath, Identity);
      }
      else if (type == typeof(int))
      {
        cellDelegate = new PrimitiveEditorCellDelegate<int>(reflectivePath, int.TryParse);
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
      else if (type == typeof(AffixTypeData))
      {
        cellDelegate = new NestedEditorCellDelegate(
          screenController,
          new AffixTypeEditorSheetDelegate(
            reflectivePath,
            reflectivePath.Property(type.GetProperty("Modifiers:")!),
            "Modifiers"));
      }
      else
      {
        throw new InvalidOperationException($"No editor registered for type {type}");
      }

      return new EditorCell(reflectivePath, parent, cellDelegate);
    }

    static bool Identity(string input, out string output)
    {
      output = input;
      return true;
    }
  }

  public sealed class ReadOnlyEditorCellDelegate : EditorCellDelegate
  {
    public override string? RenderPreview(object? value) => value?.ToString();

    public override void OnActivate(TextField field, Rect worldBound)
    {
    }
  }
}