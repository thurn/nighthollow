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
using Nighthollow.Interface;
using Nighthollow.Services;
using UnityEngine;

#nullable enable

namespace Nighthollow.Editing
{
  public sealed class GameDataEditor : HideableElement<GameDataEditor.Args>
  {
    List<ITableId> _tables = new List<ITableId>();
    Database _database = null!;
    EditorSheet? _sheet;
    readonly List<Vector2Int?> _selectionStack = new List<Vector2Int?>();
    public override bool ExclusiveFocus => true;

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
      _tables = TableId.AllTableIds
        .OrderByDescending(tid =>
          Registry.PlayerPrefs.GetInt(new PlayerPrefsService.TablePreference(tid,
            PlayerPrefsService.TablePreference.Field.LastAccessTime)))
        .ToList();

      RenderTableIndex(0);

      Time.timeScale = 0f;
    }

    protected override void OnHide()
    {
      Time.timeScale = 1f;
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

      EditorSheetDelegate sheetDelegate;
      var foreignKeyList = reflectivePath.AsPropertyInfo()?.GetCustomAttribute<ForeignKeyList>();

      var customDelegate = EditorControllerRegistry.GetCustomChildSheetDelegate(reflectivePath);
      if (customDelegate != null)
      {
        sheetDelegate = customDelegate;
      }
      else if (foreignKeyList != null)
      {
        sheetDelegate = new ForeignKeyListEditorSheetDelegate(reflectivePath, foreignKeyList.TableType);
      }
      else if (reflectivePath.GetUnderlyingType().GetInterface("IList") != null)
      {
        sheetDelegate = new NestedListEditorSheetDelegate(Registry, reflectivePath);
      }
      else
      {
        sheetDelegate = new NestedObjectEditorSheetDelegate(reflectivePath);
      }

      ClearCurrentSheet();
      _sheet = new EditorSheet(
        Registry,
        sheetDelegate,
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

    void RenderTableId(ITableId currentTableId, Vector2Int? initiallySelected = null)
    {
      _selectionStack.Clear();

      ClearCurrentSheet();
      Registry.PlayerPrefs.SetInt(
        new PlayerPrefsService.TablePreference(
          currentTableId,
          PlayerPrefsService.TablePreference.Field.LastAccessTime),
        // change this in 2038 I guess :)
        (int) DateTimeOffset.Now.ToUnixTimeSeconds());

      var path = new ReflectivePath(_database, currentTableId);
      var tableNames = _tables.Select(tid => TypeUtils.NameWithSpaces(tid.TableName)).ToList();
      var currentTableIndex = _tables.FindIndex(t => t.Id == currentTableId.Id);

      var tableSelector = new EditorSheetDelegate.DropdownCellContent(
        tableNames,
        currentTableIndex,
        RenderTableIndex,
        "Select Table");

      _sheet = new EditorSheet(
        Registry,
        new TableEditorSheetDelegate(Registry, path, tableSelector, currentTableId.TableName),
        initiallySelected);
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