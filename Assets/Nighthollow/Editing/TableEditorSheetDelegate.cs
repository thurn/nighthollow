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
using System.Reflection;
using System.Text.RegularExpressions;
using Nighthollow.Data;
using Nighthollow.Utils;

#nullable enable

namespace Nighthollow.Editing
{
  public sealed class TableEditorSheetDelegate : EditorSheetDelegate
  {
    const int AddButtonKey = 1;
    readonly ReflectivePath _reflectivePath;

    public TableEditorSheetDelegate(ReflectivePath path)
    {
      _reflectivePath = path;
    }

    public override void Initialize(Action onModified)
    {
      GetType().GetMethod(nameof(InitializeInternal), BindingFlags.Instance | BindingFlags.NonPublic)!
        .MakeGenericMethod(_reflectivePath.GetUnderlyingType()).Invoke(this, new object[] {onModified});
    }

    void InitializeInternal<T>(Action onModified) where T : class
    {
      _reflectivePath.Database.OnEntityAdded((TableId<T>) _reflectivePath.TableId, (eid, e) => onModified());
      _reflectivePath.Database.OnEntityRemoved((TableId<T>) _reflectivePath.TableId, eid => onModified());
    }

    public override List<string> GetHeadings()
    {
      var underlyingType = _reflectivePath.GetUnderlyingType();
      var properties = underlyingType.GetProperties();
      var result = new List<string> {"ID"};
      result.AddRange(properties.Select(p => NameWithSpaces(p.Name)));
      return result;
    }

    public override List<List<ICellContent>> GetCells()
    {
      var underlyingType = _reflectivePath.GetUnderlyingType();
      var properties = underlyingType.GetProperties();
      var result = new List<List<ICellContent>>();
      foreach (int entityId in _reflectivePath.GetTable().Keys)
      {
        var row = new List<ICellContent> {new LabelCell(entityId.ToString())};
        row.AddRange(properties
          .Select(property => new ReflectivePathCell(_reflectivePath.EntityId(entityId).Property(property)))
          .ToList());
        result.Add(row);
      }

      result.Add(CollectionUtils
        .Single(new ButtonCell(
          $"Add {NameWithSpaces(underlyingType.Name)}",
          (AddButtonKey, 0),
          () => { DatabaseInsert(TypeUtils.InstantiateWithDefaults(underlyingType)); }))
        .ToList<ICellContent>());

      return result;
    }

    public override int? ContentHeightOverride => 4000;

    public static string NameWithSpaces(string name) =>
      Regex.Replace(name, @"([A-Z])(?![A-Z])", " $1").Substring(1);

    void DatabaseInsert(object value)
    {
      typeof(Database)
          .GetMethod(nameof(Database.Insert))!
        .MakeGenericMethod(_reflectivePath.GetUnderlyingType())
        .Invoke(_reflectivePath.Database, new[] {_reflectivePath.TableId, value});
    }
  }
}