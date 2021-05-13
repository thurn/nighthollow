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
using System.Linq;
using System.Reflection;
using Nighthollow.Data;
using Nighthollow.Utils;

#nullable enable

namespace Nighthollow.Editing
{
  public sealed class TableEditorSheetDelegate : EditorSheetDelegate
  {
    const int AddButtonKey = 1;

    readonly ReflectivePath _reflectivePath;
    readonly Type _underlyingType;
    readonly DropdownCellContent _tableSelector;

    public TableEditorSheetDelegate(ReflectivePath path, DropdownCellContent tableSelector)
    {
      _reflectivePath = path;
      _underlyingType = _reflectivePath.GetUnderlyingType();
      _tableSelector = tableSelector;
    }

    public override string SheetName() => TypeUtils.NameWithSpaces(_reflectivePath.GetUnderlyingType().Name);

    public override void Initialize(Action onModified)
    {
      GetType().GetMethod(nameof(InitializeInternal), BindingFlags.Instance | BindingFlags.NonPublic)!
        .MakeGenericMethod(_reflectivePath.GetUnderlyingType()).Invoke(this, new object[] {onModified});
    }

    void InitializeInternal<T>(Action onModified) where T : class
    {
      // We can eventually consider more fine-grained update logic to improve performance, but you can have problems
      // with stored ReflectivePath instances becoming invalid (e.g. when a trigger changes to a different subtype the
      // PropertyInfo references it stores become invalid).
      _reflectivePath.Database.OnTableUpdated((TableId<T>) _reflectivePath.TableId, onModified);
    }

    public override TableContent GetCells()
    {
      var properties = _underlyingType.GetProperties();
      var imageProperty = properties.FirstOrDefault(p => p.Name.Contains("ImageAddress"));
      var staticHeadings = new List<ICellContent> {new LabelCellContent("x")};
      if (imageProperty != null)
      {
        staticHeadings.Add(new LabelCellContent("Image"));
      }

      var result = new List<List<ICellContent>>
      {
        new List<ICellContent> {_tableSelector},
        staticHeadings
          .Concat(properties.Select(p => new LabelCellContent(TypeUtils.NameWithSpaces(p.Name))))
          .ToList()
      };

      foreach (int entityId in _reflectivePath.GetTable().Keys)
      {
        var staticColumns = new List<ICellContent>
        {
          new ButtonCellContent("x", () => DatabaseDelete(entityId)),
        };
        if (imageProperty != null)
        {
          staticColumns.Add(new ImageCellContent(_reflectivePath.EntityId(entityId).Property(imageProperty)));
        }

        result.Add(staticColumns.Concat(properties
            .Select(property => new ReflectivePathCellContent(_reflectivePath.EntityId(entityId).Property(property))))
          .ToList());
      }

      var addRow = EditorControllerRegistry.GetAddButtonRow(_reflectivePath);
      if (addRow != null)
      {
        result.Add(addRow);
      }
      else
      {
        result.Add(CollectionUtils
          .Single(new ButtonCellContent(
            $"Add {TypeUtils.NameWithSpaces(_underlyingType.Name).Replace("Data", "")}",
            () => { DatabaseInsert(TypeUtils.InstantiateWithDefaults(_underlyingType)); },
            (AddButtonKey, 0)))
          .ToList<ICellContent>());
      }

      var columnWidths = new List<int> {50};
      if (imageProperty != null)
      {
        columnWidths.Add(ImageEditorCell.ImageSize);
      }

      columnWidths.AddRange(
        _underlyingType
          .GetProperties()
          .Select(property => EditorControllerRegistry.GetColumnWidth(_underlyingType, property)));

      return new TableContent(result, columnWidths);
    }

    void DatabaseDelete(int entityId)
    {
      typeof(Database).GetMethod(nameof(Database.Delete))!
        .MakeGenericMethod(_underlyingType)
        .Invoke(_reflectivePath.Database, new object[] {_reflectivePath.TableId, entityId});
    }

    void DatabaseInsert(object value)
    {
      typeof(Database).GetMethod(nameof(Database.Insert))!
        .MakeGenericMethod(_reflectivePath.GetUnderlyingType())
        .Invoke(_reflectivePath.Database, new[] {_reflectivePath.TableId, value});
    }
  }
}