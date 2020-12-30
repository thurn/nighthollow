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

using Nighthollow.Data;
using UnityEngine.UIElements;

#nullable enable

namespace Nighthollow.Interface
{
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
      var tableId = (ITableId) TableId.CreatureTypes;
      var editor = new ObjectEditor(Controller, ObjectEditor.ForTable(
          tableId.GetUnderlyingType(),
          tableId.GetInUnchecked(gameData)),
        height: 4000);
      Add(editor);
    }
  }
}
