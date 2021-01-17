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

#nullable enable

namespace Nighthollow.Editing
{
  public abstract class EditorSheetDelegate
  {
    public abstract string SheetName();

    public abstract void Initialize(Action onModified);

    public abstract TableContent GetCells();

    public sealed class TableContent
    {
      public TableContent(List<List<ICellContent>> cells, List<int> columnWidths)
      {
        Cells = cells;
        ColumnWidths = columnWidths;
      }

      public List<List<ICellContent>> Cells { get; }
      public List<int> ColumnWidths { get; }
    }

    public interface ICellContent
    {
      T Switch<T>(
        Func<ReflectivePath, T> onReflectivePath,
        Func<string?, T> onLabel,
        Func<ButtonCellContent, T> onButton,
        Func<DropdownCellContent, T> onDropdown,
        Func<ImageCellContent, T> onImage,
        Func<ForeignKeyDropdownCellContent, T> onForeignKeyDropdown);
    }

    protected sealed class ReflectivePathCellContent : ICellContent
    {
      public ReflectivePathCellContent(ReflectivePath reflectivePath)
      {
        ReflectivePath = reflectivePath;
      }

      public ReflectivePath ReflectivePath { get; }

      public T Switch<T>(
        Func<ReflectivePath, T> onReflectivePath,
        Func<string?, T> onLabel,
        Func<ButtonCellContent, T> onButton,
        Func<DropdownCellContent, T> onDropdown,
        Func<ImageCellContent, T> onImage,
        Func<ForeignKeyDropdownCellContent, T> onForeignKeyDropdown) =>
        onReflectivePath(ReflectivePath);
    }

    protected sealed class LabelCellContent : ICellContent
    {
      public LabelCellContent(string? text)
      {
        Text = text;
      }

      public string? Text { get; }

      public T Switch<T>(
        Func<ReflectivePath, T> onReflectivePath,
        Func<string?, T> onLabel,
        Func<ButtonCellContent, T> onButton,
        Func<DropdownCellContent, T> onDropdown,
        Func<ImageCellContent, T> onImage,
        Func<ForeignKeyDropdownCellContent, T> onForeignKeyDropdown) =>
        onLabel(Text);
    }

    public sealed class ButtonCellContent : ICellContent
    {
      public ButtonCellContent(string text, Action onClick, (int, int)? key = null)
      {
        Text = text;
        Key = key;
        OnClick = onClick;
      }

      public string Text { get; }
      public (int, int)? Key { get; }
      public Action OnClick { get; }

      public T Switch<T>(
        Func<ReflectivePath, T> onReflectivePath,
        Func<string?, T> onLabel,
        Func<ButtonCellContent, T> onButton,
        Func<DropdownCellContent, T> onDropdown,
        Func<ImageCellContent, T> onImage,
        Func<ForeignKeyDropdownCellContent, T> onForeignKeyDropdown) => onButton(this);
    }

    public sealed class DropdownCellContent : ICellContent
    {
      public DropdownCellContent(
        List<string> options,
        int? currentlySelected,
        Action<int> onSelected,
        string? defaultText = null)
      {
        Options = options;
        CurrentlySelected = currentlySelected;
        OnSelected = onSelected;
        DefaultText = defaultText ?? "None";
      }

      public List<string> Options { get; }
      public int? CurrentlySelected { get; }
      public Action<int> OnSelected { get; }
      public string DefaultText { get; }

      public T Switch<T>(
        Func<ReflectivePath, T> onReflectivePath,
        Func<string?, T> onLabel,
        Func<ButtonCellContent, T> onButton,
        Func<DropdownCellContent, T> onDropdown,
        Func<ImageCellContent, T> onImage,
        Func<ForeignKeyDropdownCellContent, T> onForeignKeyDropdown) => onDropdown(this);
    }

    public sealed class ImageCellContent : ICellContent
    {
      public ImageCellContent(ReflectivePath imagePath)
      {
        ImagePath = imagePath;
      }

      public ReflectivePath ImagePath { get; }

      public T Switch<T>(
        Func<ReflectivePath, T> onReflectivePath,
        Func<string?, T> onLabel,
        Func<ButtonCellContent, T> onButton,
        Func<DropdownCellContent, T> onDropdown,
        Func<ImageCellContent, T> onImage,
        Func<ForeignKeyDropdownCellContent, T> onForeignKeyDropdown) => onImage(this);
    }

    public sealed class ForeignKeyDropdownCellContent : ICellContent
    {
      public ForeignKeyDropdownCellContent(ReflectivePath reflectivePath, Type foreignType)
      {
        ReflectivePath = reflectivePath;
        ForeignType = foreignType;
      }

      public ReflectivePath ReflectivePath { get; }
      public Type ForeignType { get; }

      public T Switch<T>(
        Func<ReflectivePath, T> onReflectivePath,
        Func<string?, T> onLabel,
        Func<ButtonCellContent, T> onButton,
        Func<DropdownCellContent, T> onDropdown,
        Func<ImageCellContent, T> onImage,
        Func<ForeignKeyDropdownCellContent, T> onForeignKeyDropdown) => onForeignKeyDropdown(this);
    }
  }
}
