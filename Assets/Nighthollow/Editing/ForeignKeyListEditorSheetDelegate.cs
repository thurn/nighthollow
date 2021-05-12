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

#nullable enable

namespace Nighthollow.Editing
{
  public sealed class ForeignKeyListEditorSheetDelegate : EditorSheetDelegate
  {
    const int AddButtonKey = 2;
    readonly ReflectivePath _reflectivePath;
    readonly Type _foreignType;

    public ForeignKeyListEditorSheetDelegate(ReflectivePath reflectivePath, Type foreignType)
    {
      _reflectivePath = reflectivePath;
      _foreignType = foreignType;
    }

    public override string SheetName() => $"ListOf{_foreignType.Name}";

    public override void Initialize(Action onModified)
    {
      _reflectivePath.OnEntityUpdated(onModified);
    }

    public override TableContent GetCells()
    {
      var list = (ImmutableList<int>) _reflectivePath.Read()!;
      var result = new List<List<ICellContent>>
      {
        new List<ICellContent>
        {
          new LabelCellContent("x"),
          new LabelCellContent(TypeUtils.NameWithSpaces(_foreignType.Name)),
          new LabelCellContent("ID")
        }
      };

      result.AddRange(list
        .Select((key, i) => new List<ICellContent>
        {
          new ButtonCellContent("x", () => Delete(i)),
          new ForeignKeyDropdownCellContent(_reflectivePath.ListIndex(typeof(int), i), _foreignType),
          new ReflectivePathCellContent(_reflectivePath.ListIndex(typeof(int), i))
        }));

      result.Add(new List<ICellContent>
      {
        new ButtonCellContent(
          $"Add {TypeUtils.NameWithSpaces(_foreignType.Name)}",
          OnAddClicked,
          (AddButtonKey, 0))
      });

      var widths = new List<int> {50, 800, 100};
      return new TableContent(result, widths);
    }

    void Delete(int i)
    {
      var list = (ImmutableList<int>) _reflectivePath.Read()!;
      _reflectivePath.Write(list.RemoveAt(i));
    }

    void OnAddClicked()
    {
      var list = (ImmutableList<int>) _reflectivePath.Read()!;
      _reflectivePath.Write(list.Add(0));
    }
  }
}
