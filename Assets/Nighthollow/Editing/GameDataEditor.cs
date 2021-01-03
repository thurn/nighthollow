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
      Render(1);
    }

    void Render(int propertyIndex)
    {
      Clear();
      var fields = typeof(TableId).GetFields(BindingFlags.Public | BindingFlags.Static);
      var path = new ReflectivePath(_database, (ITableId) fields[propertyIndex].GetValue(typeof(TableId)));
      var tableSelector = new EditorSheetDelegate.DropdownCell(
        fields.Select(f => TypeUtils.NameWithSpaces(f.Name)).ToList(),
        propertyIndex,
        Render);
      var editor = new EditorSheet(Controller, new TableEditorSheetDelegate(path, tableSelector));
      Add(editor);
    }
  }
}
