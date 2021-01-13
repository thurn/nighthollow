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
using System.Collections.Immutable;
using System.Linq;
using Nighthollow.Data;
using Nighthollow.Interface;
using UnityEngine;

#nullable enable

namespace Nighthollow.Editing
{
  public sealed class GameDataEditor : HideableElement<GameDataEditor.Args>
  {
    List<ITableId> _tables = new List<ITableId>();
    Database _database = null!;
    EditorSheet? _sheet;
    List<Vector2Int?> _selectionStack = new List<Vector2Int?>();

    public sealed class Args
    {
      public Args(Database database)
      {
        Database = database;
      }

      public Database Database { get; }
    }

    protected override void Initialize()
    {
      AddToClassList("editor-window");
    }

    protected override void OnShow(Args argument)
    {
      _database = argument.Database;
      var metadata = _database.Snapshot().TableMetadata;
      _tables = TableId.AllTableIds
        .OrderByDescending(tid => metadata.GetValueOrDefault(tid.Id, new TableMetadata()).LastAccessedTime)
        .ToList();

      RenderTableIndex(0);
    }

    public void RenderChildSheet(
      ReflectivePath reflectivePath,
      Vector2Int? initiallySelected = null,
      bool isBackOperation = false)
    {
      if (_sheet != null && !isBackOperation)
      {
        _selectionStack.Add(_sheet.CurrentlySelected);
      }

      ClearCurrentSheet();
      _sheet = new EditorSheet(
        Controller,
        new NestedListEditorSheetDelegate(reflectivePath),
        initiallySelected,
        () => { PreviousSheet(reflectivePath); });
      Add(_sheet);
    }

    void PreviousSheet(ReflectivePath reflectivePath)
    {
      Vector2Int? selection = null;
      if (_selectionStack.Count > 0)
      {
        selection = _selectionStack.Last();
        _selectionStack.RemoveAt(_selectionStack.Count - 1);
      }

      var (tableId, parentPath) = reflectivePath.FindParentTable();
      if (parentPath != null)
      {
        RenderChildSheet(parentPath, initiallySelected: selection, isBackOperation: true);
      }
      else if (tableId != null)
      {
        RenderTableId(tableId, selection);
      }
    }

    void RenderTableIndex(int index)
    {
      RenderTableId(_tables[index]);
    }

    void RenderTableId(ITableId tableId, Vector2Int? initiallySelected = null)
    {
      _selectionStack.Clear();

      ClearCurrentSheet();
      _database.Upsert(TableId.TableMetadata,
        tableId.Id,
        new TableMetadata(),
        metadata => metadata.WithLastAccessedTime(DateTime.UtcNow.Ticks));

      var path = new ReflectivePath(_database, tableId);
      var tableSelector = new EditorSheetDelegate.DropdownCell(
        _tables.Select(tid => TypeUtils.NameWithSpaces(tid.TableName)).ToList(),
        _tables.FindIndex(t => t.Id == tableId.Id),
        RenderTableIndex,
        "Select Table");
      _sheet = new EditorSheet(Controller, new TableEditorSheetDelegate(path, tableSelector), initiallySelected);
      Add(_sheet);
    }

    void ClearCurrentSheet()
    {
      _sheet?.DeactivateAllCells();
      _database.ClearListeners();
      Clear();
      _sheet = null;
    }
  }
}