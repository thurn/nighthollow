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
using Nighthollow.Stats;

#nullable enable

namespace Nighthollow.Editing
{
  public static class EditorCellFactory
  {
    public static EditorCell Create(ScreenController screenController, ReflectivePath reflectivePath, IEditor parent)
    {
      var type = reflectivePath.GetUnderlyingType();

      EditorCellDelegate cellDelegate;
      if (type == typeof(string))
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
      else if (type.IsSubclassOf(typeof(Enum)))
      {
        cellDelegate = new EditorDropdownCellDelegate(screenController, reflectivePath);
      }
      else if (type.GetInterface("IList") != null)
      {
        cellDelegate = new EditorNestedListCellDelegate(screenController, reflectivePath);
      }
      else
      {
        cellDelegate = new PrimitiveEditorCellDelegate<string>(reflectivePath, Identity);
      }

      return new EditorCell(reflectivePath, parent, cellDelegate);
    }

    static bool Identity(string input, out string output)
    {
      output = input;
      return true;
    }
  }
}
