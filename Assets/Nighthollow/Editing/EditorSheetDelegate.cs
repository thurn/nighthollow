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
    public abstract void Initialize(Action onModified);

    public abstract TableContent GetCells();

    public virtual string? RenderPreview(object? value) => null;

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
        Func<ButtonCell, T> onButton,
        Func<DropdownCell, T> onDropdown,
        Func<ImageCell, T> onImage);
    }

    protected sealed class ReflectivePathCell : ICellContent
    {
      public ReflectivePathCell(ReflectivePath reflectivePath)
      {
        ReflectivePath = reflectivePath;
      }

      public ReflectivePath ReflectivePath { get; }

      public T Switch<T>(
        Func<ReflectivePath, T> onReflectivePath,
        Func<string?, T> onLabel,
        Func<ButtonCell, T> onButton,
        Func<DropdownCell, T> onDropdown,
        Func<ImageCell, T> onImage) =>
        onReflectivePath(ReflectivePath);
    }

    protected sealed class LabelCell : ICellContent
    {
      public LabelCell(string? text)
      {
        Text = text;
      }

      public string? Text { get; }

      public T Switch<T>(
        Func<ReflectivePath, T> onReflectivePath,
        Func<string?, T> onLabel,
        Func<ButtonCell, T> onButton,
        Func<DropdownCell, T> onDropdown,
        Func<ImageCell, T> onImage) =>
        onLabel(Text);
    }

    public sealed class ButtonCell : ICellContent
    {
      public ButtonCell(string text, Action onClick, (int, int)? key = null)
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
        Func<ButtonCell, T> onButton,
        Func<DropdownCell, T> onDropdown,
        Func<ImageCell, T> onImage) => onButton(this);
    }

    public sealed class DropdownCell : ICellContent
    {
      public DropdownCell(
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
        Func<ButtonCell, T> onButton,
        Func<DropdownCell, T> onDropdown,
        Func<ImageCell, T> onImage) => onDropdown(this);
    }

    public sealed class ImageCell : ICellContent
    {
      public ImageCell(ReflectivePath imagePath)
      {
        ImagePath = imagePath;
      }

      public ReflectivePath ImagePath { get; }

      public T Switch<T>(
        Func<ReflectivePath, T> onReflectivePath,
        Func<string?, T> onLabel,
        Func<ButtonCell, T> onButton,
        Func<DropdownCell, T> onDropdown,
        Func<ImageCell, T> onImage) => onImage(this);
    }
  }
}
