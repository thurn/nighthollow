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

using System.Collections;
using System.Linq;
using Nighthollow.Data;
using UnityEngine.UIElements;

#nullable enable

namespace Nighthollow.Interface
{
  public sealed class GameDataEditor : HideableElement<GameDataEditor.Args>
  {
    public sealed class Args
    {
      public Args(GameData gameData)
      {
        GameData = gameData;
      }

      public GameData GameData { get; }
    }

    public new sealed class UxmlFactory : UxmlFactory<GameDataEditor, UxmlTraits>
    {
    }

    protected override void Initialize()
    {
    }

    protected override void OnShow(Args argument)
    {
      var property = typeof(GameData).GetProperties().First();
      var editor = new ObjectEditor(Controller, ObjectEditor.ForTable(
        property.PropertyType.GetGenericArguments()[1],
        (property.GetValue(argument.GameData) as IDictionary)!),
        height: 4000);
      Add(editor);
    }
  }
}
