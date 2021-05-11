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
using System.Reflection;
using Nighthollow.Data;
using Nighthollow.Interface;
using Nighthollow.Stats;

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
        text => new LabelEditorCell(text),
        button => new ButtonEditorCell(button, parent),
        dropdown => new SelectorDropdownEditorCell(screenController, dropdown, parent),
        cell => new ImageEditorCell(cell.ImagePath),
        foreignKey => new ForeignKeyDropdownEditorCell(
          screenController, foreignKey.ReflectivePath, foreignKey.ForeignType, parent));

    static EditorCell CreateTextFieldEditorCell(
      ScreenController screenController,
      IEditor parent,
      ReflectivePath reflectivePath)
    {
      var type = TypeUtils.UnboxNullable(reflectivePath.GetUnderlyingType());

      var foreignKey = reflectivePath.AsPropertyInfo()?.GetCustomAttribute<ForeignKey>();
      if (foreignKey != null)
      {
        return new ForeignKeyDropdownEditorCell(
          screenController, reflectivePath, foreignKey.TableType, parent);
      }

      var nestedSheet = reflectivePath.AsPropertyInfo()?.GetCustomAttribute<NestedSheet>();
      if (nestedSheet != null)
      {
        return new TextFieldEditorCell(reflectivePath, parent,
          new NestedSheetTextFieldCellDelegate(screenController, reflectivePath));
      }

      if (type.IsSubclassOf(typeof(Enum)))
      {
        return new EnumDropdownEditorCell(screenController, reflectivePath, type, parent);
      }

      TextFieldEditorCellDelegate cellDelegate;
      if (type == typeof(string))
      {
        cellDelegate = new PrimitiveTextFieldCellDelegate<string>(reflectivePath, Identity);
      }
      else if (type == typeof(int))
      {
        cellDelegate = new PrimitiveTextFieldCellDelegate<int>(reflectivePath, int.TryParse);
      }
      else if (type == typeof(long))
      {
        cellDelegate = new PrimitiveTextFieldCellDelegate<long>(reflectivePath, long.TryParse);
      }
      else if (type == typeof(bool))
      {
        cellDelegate = new PrimitiveTextFieldCellDelegate<bool>(reflectivePath, bool.TryParse);
      }
      else if (type == typeof(int))
      {
        cellDelegate = new PrimitiveTextFieldCellDelegate<int>(reflectivePath, int.TryParse);
      }
      else if (type == typeof(DurationValue))
      {
        cellDelegate = new PrimitiveTextFieldCellDelegate<DurationValue>(reflectivePath, DurationValue.TryParse);
      }
      else if (type == typeof(PercentageValue))
      {
        cellDelegate = new PrimitiveTextFieldCellDelegate<PercentageValue>(reflectivePath, PercentageValue.TryParse);
      }
      else if (type == typeof(IntRangeValue))
      {
        cellDelegate = new PrimitiveTextFieldCellDelegate<IntRangeValue>(reflectivePath, IntRangeValue.TryParse);
      }
      else if (type.IsGenericType && type.GetGenericTypeDefinition() == typeof(ImmutableDictionaryValue<,>))
      {
        cellDelegate = new PrimitiveTextFieldCellDelegate<IValueData>(reflectivePath,
          (string s, out IValueData d) => AnyStatCanParse(type, s, out d));
      }
      else if (type == typeof(IValueData))
      {
        cellDelegate = new PrimitiveTextFieldCellDelegate<IValueData>(reflectivePath,
          (string s, out IValueData d) => TryParseValueData(reflectivePath, s, out d));
      }
      else if (type.GetInterface("IList") != null)
      {
        cellDelegate = new NestedSheetTextFieldCellDelegate(screenController, reflectivePath);
      }
      else
      {
        cellDelegate = new NestedSheetTextFieldCellDelegate(screenController, reflectivePath);
      }

      return new TextFieldEditorCell(reflectivePath, parent, cellDelegate);
    }

    static bool Identity(string input, out string output)
    {
      output = input;
      return true;
    }

    static bool TryParseValueData(ReflectivePath path, string input, out IValueData result)
    {
      var parent = path.Parent()?.Read();
      if (parent == null)
      {
        result = null!;
        return false;
      }

      result = (parent.GetType().GetMethod("Parse")!.Invoke(parent, new object[] {input}) as IValueData)!;
      // ReSharper disable once ConditionIsAlwaysTrueOrFalse
      return result != null;
    }

    static bool AnyStatCanParse(Type targetType, string input, out IValueData result)
    {
      foreach (var field in typeof(Stat).GetFields(BindingFlags.Public | BindingFlags.Static))
      {
        if (field.GetValue(typeof(Stat)) is IStat stat)
        {
          if (stat.TryParse(input, ModifierType.Set, out result) && result.GetType() == targetType)
          {
            return true;
          }
        }
      }

      result = null!;
      return false;
    }
  }
}