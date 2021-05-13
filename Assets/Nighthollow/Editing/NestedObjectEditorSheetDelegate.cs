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

#nullable enable

namespace Nighthollow.Editing
{
  public sealed class NestedObjectEditorSheetDelegate : EditorSheetDelegate
  {
    readonly ReflectivePath _reflectivePath;

    public NestedObjectEditorSheetDelegate(ReflectivePath path)
    {
      _reflectivePath = path;
    }

    public override string SheetName() => TypeUtils.NameWithSpaces(_reflectivePath.GetUnderlyingType().Name);

    public override void Initialize(Action onModified)
    {
      _reflectivePath.OnEntityUpdated(onModified);
      if (_reflectivePath.Read() == null)
      {
        // We navigated into a null value, create a default instance of it.
        _reflectivePath.Write(TypeUtils.InstantiateWithDefaults(_reflectivePath.GetUnderlyingType()));
      }
    }

    public override TableContent GetCells()
    {
      var result = _reflectivePath.GetUnderlyingType().GetProperties()
        .Select(property => new List<ICellContent>
        {
          new LabelCellContent(property.Name),
          new ReflectivePathCellContent(_reflectivePath.Property(property))
        })
        .ToList();

      return new TableContent(result, new List<int> {400, 600});
    }
  }
}