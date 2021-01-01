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
using Nighthollow.Utils;

#nullable enable

namespace Nighthollow.Editing
{
  public sealed class AffixTypeEditorSheetDelegate : EditorSheetDelegate
  {
    public AffixTypeEditorSheetDelegate(ReflectivePath reflectivePath, ReflectivePath listPath, string name)
    {
      var properties = reflectivePath.GetUnderlyingType().GetProperties();
      var childList = new NestedListEditorSheetDelegate(listPath);

      Headings = properties.Select(TableEditorSheetDelegate.PropertyName).ToList();

      Cells = new List<List<ICellContent>>
      {
        properties.Select(reflectivePath.Property).Select(p => new ReflectivePathCell(p)).ToList<ICellContent>(),
        CollectionUtils.Single(new LabelCell(name)).ToList<ICellContent>(),
        childList.Headings.Select(h => new LabelCell(h)).ToList<ICellContent>()
      };

      Cells.AddRange(childList.Cells);
    }

    public override List<string> Headings { get; }

    public override List<List<ICellContent>> Cells { get; }

    public override string? RenderPreview(object? value) => value?.ToString();
  }
}