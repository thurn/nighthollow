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
using Nighthollow.Utils;

#nullable enable

namespace Nighthollow.Editing
{
  public sealed class AffixTypeEditorSheetDelegate : EditorSheetDelegate
  {
    readonly ReflectivePath _reflectivePath;
    readonly NestedListEditorSheetDelegate _childDelegate;
    readonly string _name;

    public AffixTypeEditorSheetDelegate(ReflectivePath reflectivePath, ReflectivePath listPath, string name)
    {
      _reflectivePath = reflectivePath;
      _childDelegate = new NestedListEditorSheetDelegate(listPath);
      _name = name;
    }

    public override void Initialize(Action onModified)
    {
      _reflectivePath.OnEntityUpdated(_ => onModified());
    }

    public override List<List<ICellContent>> GetCells()
    {
      var properties = _reflectivePath.GetUnderlyingType().GetProperties();
      var result = new List<List<ICellContent>>
      {
        properties.Select(p => new LabelCell(TableEditorSheetDelegate.NameWithSpaces(p.Name))).ToList<ICellContent>(),
        properties
          .Select(_reflectivePath.Property).Select(p => new ReflectivePathCell(p)).ToList<ICellContent>(),
        CollectionUtils.Single(new LabelCell(_name)).ToList<ICellContent>()
      };
      result.AddRange(_childDelegate.GetCells());
      return result;
    }

    public override string? RenderPreview(object? value) => value?.ToString();
  }
}