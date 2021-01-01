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
using System.Text.RegularExpressions;

#nullable enable

namespace Nighthollow.Editing
{
  public sealed class TableEditorSheetDelegate : EditorSheetDelegate
  {
    public TableEditorSheetDelegate(ReflectivePath path)
    {
      var properties = path.GetUnderlyingType().GetProperties();
      Headings = new List<string> {"ID"};
      Headings.AddRange(properties.Select(PropertyName));

      Cells = new List<List<ICellContent>>();
      foreach (int entityId in path.GetTable().Keys)
      {
        var row = new List<ICellContent> {new LabelCell(entityId.ToString())};
        row.AddRange(properties
          .Select(property => new ReflectivePathCell(path.EntityId(entityId).Property(property)))
          .ToList());
        Cells.Add(row);
      }
    }

    public override List<string> Headings { get; }

    public override List<List<ICellContent>> Cells { get; }

    public override int? ContentHeightOverride => 4000;

    public static string PropertyName(PropertyInfo info) =>
      Regex.Replace(info.Name, @"([A-Z])(?![A-Z])", " $1").Substring(1);
  }
}