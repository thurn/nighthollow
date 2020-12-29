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

using System.Linq;
using System.Reflection;
using Nighthollow.Data;
using UnityEngine;
using UnityEngine.UIElements;

#nullable enable

namespace Nighthollow.Interface
{
  public sealed class EditorDataHelper
  {
    readonly Database _database;
    readonly TableId _tableId;

    public EditorDataHelper(Database database, TableId tableId)
    {
      _database = database;
      _tableId = tableId;
    }

    public void WriteProperty(int entityId, PropertyInfo property, object newValue)
    {
      var method =
        typeof(EditorDataHelper).GetMethod(nameof(WritePropertyInternal),
          BindingFlags.Instance | BindingFlags.NonPublic)!.MakeGenericMethod(_tableId.GetUnderlyingType());
      method.Invoke(this, new[] {entityId, property, newValue});
    }

    void WritePropertyInternal<T>(int entityId, PropertyInfo property, object newValue) where T : class
    {
      var tableId = (TableId<T>) _tableId;
      Debug.Log($"Table ID {tableId}");
    }
  }

  public sealed class GameDataEditor : HideableElement<GameDataEditor.Args>
  {
    public sealed class Args
    {
      public Args(Database database)
      {
        Database = database;
      }

      public Database Database { get; }
    }

    public new sealed class UxmlFactory : UxmlFactory<GameDataEditor, UxmlTraits>
    {
    }

    protected override void Initialize()
    {
    }

    protected override void OnShow(Args argument)
    {
      var gameData = argument.Database.Snapshot();
      var tableId = gameData.Keys.First();
      var dataHelper = new EditorDataHelper(argument.Database, tableId);
      dataHelper.WriteProperty(12, null!, null!);
      var editor = new ObjectEditor(Controller, ObjectEditor.ForTable(
          tableId.GetUnderlyingType(),
          tableId.LookUpIn(gameData)),
        height: 4000);
      Add(editor);
    }
  }
}
