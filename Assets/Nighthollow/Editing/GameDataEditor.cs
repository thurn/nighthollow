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

#nullable enable

namespace Nighthollow.Editing
{
  public sealed class GameDataEditor : HideableElement<GameDataEditor.Args>
  {
    readonly List<PropertyInfo> _properties;
    Database _database = null!;

    public GameDataEditor()
    {
      _properties = typeof(GameData).GetProperties().ToList();
    }

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
      Render(metadata.IsEmpty
        ? 1
        : metadata
          .OrderByDescending(pair => pair.Value.LastAccessedTime)
          .First()
          .Key);
    }

    void Render(int id)
    {
      Clear();
      var tableId = TableId.AllTableIds.First(t => t.Id == id);
      _database.Upsert(TableId.TableMetadata,
        id,
        new TableMetadata(),
        metadata => metadata.WithLastAccessedTime(DateTime.UtcNow.Ticks));

      var path = new ReflectivePath(_database, tableId);
      var tableSelector = new EditorSheetDelegate.DropdownCell(
        TableId.AllTableIds.Select(t => TypeUtils.NameWithSpaces(t.TableName)).ToList(),
        id,
        Render);
      var editor = new EditorSheet(Controller, new TableEditorSheetDelegate(path, tableSelector));
      Add(editor);
    }
  }
}
