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
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Nighthollow.Data;
using Nighthollow.Delegates.Core;
using Nighthollow.Stats2;

#nullable enable

namespace Nighthollow.Editing
{
  public interface IEditorCellPreviewer
  {
    string Preview(GameData gameData, object container, string propertyName, object? propertyValue);
  }

  public sealed class ModifierDescriptionProvider : IStatDescriptionProvider
  {
    readonly StatTable? _values;
    readonly StatTable? _low;
    readonly StatTable? _high;

    public ModifierDescriptionProvider(StatTable? values = null, StatTable? low = null, StatTable? high = null)
    {
      _values = values;
      _low = low;
      _high = high;
    }

    public ModifierDescriptionProvider Insert(ModifierData modifier)
    {
      var valueModifier = modifier.BuildStatModifier();
      if (valueModifier != null)
      {
        return new ModifierDescriptionProvider((_values ?? new StatTable()).InsertModifier(valueModifier), _low, _high);
      }
      else
      {
        return new ModifierDescriptionProvider(
          _values,
          (_low ?? new StatTable()).InsertNullableModifier(modifier.ModifierForValue(modifier.ValueLow)),
          (_high ?? new StatTable()).InsertNullableModifier(modifier.ModifierForValue(modifier.ValueHigh))
        );
      }
    }

    public string Get<TModifier, TValue>(AbstractStat<TModifier, TValue> stat)
      where TModifier : IStatModifier where TValue : notnull =>
      _values != null ? _values.Get(stat).ToString() : $"({_low!.Get(stat)} to {_high!.Get(stat)})";
  }

  abstract class EditorCellPreviewer<T> : IEditorCellPreviewer
  {
    readonly IReadOnlyDictionary<string, Func<GameData, T, string>> _propertyPreviewers;

    protected EditorCellPreviewer(IReadOnlyDictionary<string, Func<GameData, T, string>> propertyPreviewers)
    {
      _propertyPreviewers = propertyPreviewers;
    }

    public string Preview(GameData gameData, object container, string propertyName, object? propertyValue) =>
      _propertyPreviewers.ContainsKey(propertyName)
        ? _propertyPreviewers[propertyName](gameData, (T) container)
        : CellPreviewFactory.RenderDefault(propertyValue);
  }

  sealed class CreatureTypeCellPreviewer : EditorCellPreviewer<CreatureTypeData>
  {
    public CreatureTypeCellPreviewer() : base(new Dictionary<string, Func<GameData, CreatureTypeData, string>>
    {
      {nameof(CreatureTypeData.ImplicitModifiers), RenderImplicitModifiers}
    })
    {
    }

    static string RenderImplicitModifiers(GameData gameData, CreatureTypeData data)
    {
      if (data.ImplicitModifiers.Count == 0)
      {
        return "[]";
      }

      var entity = data.ImplicitModifiers.Aggregate(new ModifierDescriptionProvider(),
        (current, modifier) => current.Insert(modifier));
      return string.Join("\n", data.ImplicitModifiers.Select(m => m.Describe(entity, gameData.StatData)));
    }
  }

  public static class CellPreviewFactory
  {
    static readonly IReadOnlyDictionary<Type, IEditorCellPreviewer> Previewers =
      new Dictionary<Type, IEditorCellPreviewer>
      {
        {typeof(CreatureTypeData), new CreatureTypeCellPreviewer()}
      };

    public static string RenderPropertyPreview(GameData gameData, object parentValue, PropertyInfo property)
    {
      var value = property.GetValue(parentValue);
      return Previewers.ContainsKey(parentValue.GetType())
        ? Previewers[parentValue.GetType()].Preview(gameData, parentValue, property.Name, value)
        : RenderDefault(value);
    }

    public static string RenderDefault(object? value)
    {
      return value switch
      {
        IList list when list.Count > 0 => string.Join("\n", list.Cast<object>().Take(3).Select(o => o.ToString())) +
                                          (list.Count > 3 ? $"\n(+ {list.Count - 3} more)" : ""),
        IList _ => "[]",
        _ => value?.ToString() ?? "None"
      };
    }
  }
}