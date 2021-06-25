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
using System.Collections;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Reflection;
using Nighthollow.Services;
using Nighthollow.Utils;

#nullable enable

namespace Nighthollow.Editing
{
  public sealed class NestedListEditorSheetDelegate : EditorSheetDelegate
  {
    const int AddButtonKey = 2;
    readonly ServiceRegistry _registry;
    readonly ReflectivePath _reflectivePath;
    readonly Type _contentType;

    public NestedListEditorSheetDelegate(ServiceRegistry registry, ReflectivePath path)
    {
      _registry = registry;
      _reflectivePath = path;
      _contentType = _reflectivePath.GetUnderlyingType().GetGenericArguments()[0];
    }

    public override string SheetName() => TypeUtils.NameWithSpaces(_reflectivePath.GetUnderlyingType().Name);

    public override void Initialize(Action onModified)
    {
      _reflectivePath.OnEntityUpdated(onModified);
    }

    public override TableContent GetCells()
    {
      var isPrimitive = _contentType.IsPrimitive || _contentType == typeof(string);
      var customColumn = EditorControllerRegistry.GetCustomColumn(_contentType);
      var imageProperty = _contentType.GetProperties().FirstOrDefault(p => p.Name.Contains("ImageAddress"));

      var headings = new List<ICellContent>
      {
        new LabelCellContent("x")
      };

      if (customColumn != null)
      {
        headings.Add(new LabelCellContent(customColumn.Heading));
      }

      if (imageProperty != null)
      {
        headings.Add(new LabelCellContent("Image"));
      }

      if (isPrimitive)
      {
        headings.Add(new LabelCellContent("Value"));
      }
      else
      {
        headings.AddRange(_contentType.GetProperties()
          .Select(p => new LabelCellContent(TypeUtils.NameWithSpaces(p.Name))));
      }

      var result = new List<List<ICellContent>> {headings};

      if (_reflectivePath.Read() is IList value)
      {
        for (var index = 0; index < value.Count; ++index)
        {
          var childIndexPath = _reflectivePath.ListIndex(_contentType, index);
          var entityIndex = index;

          var cells = new List<ICellContent>
          {
            new ButtonCellContent("x", () => Delete(entityIndex))
          };

          if (customColumn != null)
          {
            cells.Add(customColumn.GetContent(_registry, index, childIndexPath));
          }

          if (imageProperty != null)
          {
            cells.Add(new ImageCellContent(childIndexPath.Property(imageProperty)));
          }

          if (isPrimitive)
          {
            cells.Add(new ReflectivePathCellContent(childIndexPath));
          }
          else
          {
            cells.AddRange(_contentType.GetProperties()
              .Select(property =>
                new ReflectivePathCellContent(childIndexPath.Property(property))));
          }

          result.Add(cells);
        }
      }

      result.Add(CollectionUtils
        .Single(new ButtonCellContent(
          $"Add {TypeUtils.NameWithSpaces(_contentType.Name)}",
          () => { Insert(TypeUtils.InstantiateWithDefaults(_contentType)); },
          (AddButtonKey, 0)))
        .ToList<ICellContent>());

      var columnWidths = new List<int> {50};

      if (customColumn != null)
      {
        columnWidths.Add(customColumn.Width);
      }

      if (imageProperty != null)
      {
        columnWidths.Add(ImageEditorCell.ImageSize);
      }

      if (isPrimitive)
      {
        columnWidths.Add(500);
      }
      else
      {
        columnWidths.AddRange(Enumerable.Repeat(
          EditorSheet.DefaultCellWidth,
          _contentType.GetProperties().Length));
      }

      return new TableContent(result, columnWidths);
    }

    void Insert(object value)
    {
      GetType()
        .GetMethod(nameof(InsertInternal), BindingFlags.Instance | BindingFlags.NonPublic)!
        .MakeGenericMethod(_contentType)
        .Invoke(this, new[] {value});
    }

    void InsertInternal<T>(object value)
    {
      var list = (ImmutableList<T>?) _reflectivePath.Read();
      _reflectivePath.Write(list == null ? ImmutableList.Create<T>((T) value) : list.Add((T) value));
    }

    void Delete(int index)
    {
      GetType()
        .GetMethod(nameof(DeleteInternal), BindingFlags.Instance | BindingFlags.NonPublic)!
        .MakeGenericMethod(_contentType)
        .Invoke(this, new object[] {index});
    }

    void DeleteInternal<T>(int index)
    {
      var list = (ImmutableList<T>) _reflectivePath.Read()!;
      _reflectivePath.Write(list.RemoveAt(index));
    }
  }
}