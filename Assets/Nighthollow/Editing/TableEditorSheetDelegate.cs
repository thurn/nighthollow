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
using Nighthollow.Services;
using Nighthollow.Utils;

#nullable enable

namespace Nighthollow.Editing
{
  public sealed class TableEditorSheetDelegate : EditorSheetDelegate
  {
    const int AddButtonKey = 1;

    readonly ServiceRegistry _registry;
    readonly ReflectivePath _reflectivePath;
    readonly Type _underlyingType;
    readonly DropdownCellContent _tableSelector;
    readonly string _tableName;

    public TableEditorSheetDelegate(ServiceRegistry registry, ReflectivePath path, DropdownCellContent tableSelector,
      string tableName)
    {
      _registry = registry;
      _reflectivePath = path;
      _underlyingType = _reflectivePath.GetUnderlyingType();
      _tableSelector = tableSelector;
      _tableName = tableName;
    }

    public override string SheetName() => TypeUtils.NameWithSpaces(_reflectivePath.GetUnderlyingType().Name);

    public override void Initialize(Action onModified)
    {
      GetType().GetMethod(nameof(InitializeInternal),
          BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic)!
        .MakeGenericMethod(_reflectivePath.GetUnderlyingType()).Invoke(this, new object[] {onModified});
    }

    void InitializeInternal<T>(Action onModified) where T : class
    {
      // We can eventually consider more fine-grained update logic to improve performance, but you can have problems
      // with stored ReflectivePath instances becoming invalid (e.g. when a rule changes to a different subtype the
      // PropertyInfo references it stores become invalid).
      _reflectivePath.Database.OnTableUpdated((TableId<T>) _reflectivePath.TableId, onModified);
    }

    public override TableContent GetCells() => RenderTable(_reflectivePath, _tableSelector);

    TableContent RenderTable(ReflectivePath reflectivePath, DropdownCellContent tableSelector)
    {
      var properties = _underlyingType.GetProperties();
      var imageProperty = properties.FirstOrDefault(p => p.Name.Contains("ImageAddress"));
      var showEditButton = EditorControllerRegistry.ShowEditButton(reflectivePath.GetUnderlyingType());
      var customColumn = EditorControllerRegistry.GetCustomColumn(reflectivePath.GetUnderlyingType());

      var staticHeadings = new List<ICellContent>();

      if (showEditButton)
      {
        staticHeadings.Add(new LabelCellContent("Edit"));
      }

      if (customColumn != null)
      {
        staticHeadings.Add(new LabelCellContent(customColumn.Heading));
      }

      staticHeadings.Add(new LabelCellContent("x"));

      var filters = new List<ICellContent>();
      if (showEditButton)
      {
        filters.Add(new LabelCellContent("-"));
      }

      if (customColumn != null)
      {
        filters.Add(new LabelCellContent("-"));
      }

      filters.Add(new LabelCellContent("-"));
      if (imageProperty != null)
      {
        staticHeadings.Add(new LabelCellContent("Image"));
        filters.Add(new LabelCellContent("-"));
      }

      var result = new List<List<ICellContent>>
      {
        new() {tableSelector},
        staticHeadings
          .Concat(properties.Select(p => new LabelCellContent(TypeUtils.NameWithSpaces(p.Name))))
          .ToList(),
        filters.Concat(properties.Select(PropertyFilterCell)).ToList()
      };

      var rowCount = 0;
      foreach (var entityId in reflectivePath.GetTable().Keys.OfType<int>())
      {
        var childPath = reflectivePath.EntityId(entityId);
        var filterFailed = (
          from property in properties
          let filter = _registry.PlayerPrefs.GetString(new PlayerPrefsService.Filter(_tableName, property))
          where !string.IsNullOrWhiteSpace(filter) &&
                !childPath.Property(property).RenderPreview().Contains(filter)
          select property).Any();

        if (filterFailed)
        {
          continue;
        }

        var staticColumns = new List<ICellContent>();

        if (showEditButton)
        {
          staticColumns.Add(new ViewChildButtonCellContent(childPath));
        }

        if (customColumn != null)
        {
          staticColumns.Add(customColumn.GetContent(_registry, entityId, childPath));
        }

        staticColumns.Add(RowDeleteButton(entityId));

        if (imageProperty != null)
        {
          staticColumns.Add(new ImageCellContent(childPath.Property(imageProperty)));
        }

        result.Add(staticColumns.Concat(properties
            .Select(property => new ReflectivePathCellContent(childPath.Property(property))))
          .ToList());

        rowCount++;
        if (rowCount > 50)
        {
          result.Add(new List<ICellContent> {new LabelCellContent("Content Truncated...")});
          break;
        }
      }

      result.Add(CollectionUtils
        .Single(new ButtonCellContent(
          $"Add {TypeUtils.NameWithSpaces(_underlyingType.Name).Replace("Data", "")}",
          () => { DatabaseInsert(TypeUtils.InstantiateWithDefaults(_underlyingType)); },
          (AddButtonKey, 0)))
        .ToList<ICellContent>());

      var columnWidths = new List<int>();

      if (showEditButton)
      {
        columnWidths.Add(115);
      }

      if (customColumn != null)
      {
        columnWidths.Add(customColumn.Width);
      }

      columnWidths.Add(50);

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

    ICellContent PropertyFilterCell(PropertyInfo arg)
    {
      var key = new PlayerPrefsService.Filter(_tableName, arg);
      return new FilterInputCellContent(_registry.PlayerPrefs.GetString(key), key);
    }

    ButtonCellContent RowDeleteButton(int entityId)
    {
      return new ButtonCellContent("x", () => DatabaseDelete(entityId));
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