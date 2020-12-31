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

#nullable enable

namespace Nighthollow.Editing
{
  public sealed class TableEditorSheetDelegate : EditorSheetDelegate
  {
    public TableEditorSheetDelegate(ReflectivePath path)
    {
      var properties = path.GetUnderlyingType().GetProperties();
      Headings = new List<string> {"ID"};
      Headings.AddRange(properties.Select(property => property.Name));

      Cells = new List<List<ReflectivePath>>();
      foreach (int entityId in path.GetTable().Keys)
      {
        var row = new List<ReflectivePath> {path.Constant(entityId)};
        row.AddRange(properties.Select(property => path.EntityId(entityId).Property(property)).ToList());
        Cells.Add(row);
      }
    }

    public override List<string> Headings { get; }

    public override List<List<ReflectivePath>> Cells { get; }

    public override int? ContentHeightOverride => 4000;
  }
}