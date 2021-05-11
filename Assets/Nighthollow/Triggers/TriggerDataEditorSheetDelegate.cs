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
using Nighthollow.Editing;

#nullable enable

namespace Nighthollow.Triggers
{
  public sealed class TriggerDataEditorSheetDelegate : EditorSheetDelegate
  {
    readonly ReflectivePath _reflectivePath;

    public TriggerDataEditorSheetDelegate(ReflectivePath reflectivePath)
    {
      _reflectivePath = reflectivePath;
    }

    public override string SheetName() => "Trigger Data";

    public override void Initialize(Action onModified)
    {
      _reflectivePath.OnEntityUpdated(_ => onModified());
    }

    public override TableContent GetCells()
    {
      var result = new List<List<ICellContent>>();
      var header = new List<ICellContent> {new LabelCellContent("Hello"), new LabelCellContent("World")};
      result.Add(header);
      return new TableContent(result, new List<int> {200, 200});
    }
  }
}