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
using System.Collections.Immutable;
using System.Linq;
using System.Reflection;
using Nighthollow.Data;
using Nighthollow.Delegates.Core;
using Nighthollow.Stats2;

#nullable enable

namespace Nighthollow.Editing
{
  public abstract class EditorController
  {
    public abstract string Preview(GameData gameData, object container, string propertyName, object? propertyValue);

    public virtual void WriteForeignKey(int id, ReflectivePath reflectivePath)
    {
    }
  }

  abstract class EditorController<T> : EditorController
  {
    readonly IReadOnlyDictionary<string, Func<GameData, T, string>> _propertyPreviewers;

    protected EditorController(IReadOnlyDictionary<string, Func<GameData, T, string>>? propertyPreviewers = null)
    {
      _propertyPreviewers = propertyPreviewers ?? new Dictionary<string, Func<GameData, T, string>>();
    }

    public override string Preview(GameData gameData, object container, string propertyName, object? propertyValue) =>
      _propertyPreviewers.ContainsKey(propertyName)
        ? _propertyPreviewers[propertyName](gameData, (T) container)
        : EditorControllerRegistry.RenderDefault(propertyValue);

    protected static string RenderModifiers(GameData gameData, ImmutableList<ModifierData> modifiers)
    {
      if (modifiers.Count == 0)
      {
        return "[]";
      }

      var entity = modifiers.Aggregate(new ModifierDescriptionProvider(gameData),
        (current, modifier) => current.Insert(modifier));
      return string.Join("\n", modifiers.Select(m => m.Describe(entity, gameData.StatData)));
    }
  }

  public sealed class ModifierDescriptionProvider : IStatDescriptionProvider
  {
    readonly GameData _gameData;
    readonly StatTable? _values;
    readonly StatTable? _low;
    readonly StatTable? _high;

    public ModifierDescriptionProvider(
      GameData gameData, StatTable? values = null, StatTable? low = null, StatTable? high = null)
    {
      _gameData = gameData;
      _values = values;
      _low = low;
      _high = high;
    }

    public ModifierDescriptionProvider Insert(ModifierData modifier)
    {
      var defaults = StatData.BuildDefaultStatTable(_gameData);
      var valueModifier = modifier.BuildStatModifier();
      if (valueModifier != null)
      {
        return new ModifierDescriptionProvider(
          _gameData,
          (_values ?? new StatTable(defaults)).InsertModifier(valueModifier),
          _low,
          _high
        );
      }
      else
      {
        return new ModifierDescriptionProvider(
          _gameData,
          _values,
          (_low ?? new StatTable(defaults)).InsertNullableModifier(modifier.ModifierForValue(modifier.ValueLow)),
          (_high ?? new StatTable(defaults)).InsertNullableModifier(modifier.ModifierForValue(modifier.ValueHigh))
        );
      }
    }

    public string Get<TModifier, TValue>(AbstractStat<TModifier, TValue> stat)
      where TModifier : IStatModifier where TValue : notnull
    {
      if (_values != null)
      {
        return _values.Get(stat).ToString();
      }
      else
      {
        var low = _low!.Get(stat);
        var high = _high!.Get(stat);
        return Equals(low, high) ? low.ToString() : $"({low} to {high})";
      }
    }
  }

  sealed class CreatureTypeController : EditorController<CreatureTypeData>
  {
    public CreatureTypeController() : base(new Dictionary<string, Func<GameData, CreatureTypeData, string>>
    {
      {nameof(CreatureTypeData.ImplicitModifiers), (g, d) => RenderModifiers(g, d.ImplicitModifiers)}
    })
    {
    }
  }

  sealed class SkillTypeController : EditorController<SkillTypeData>
  {
    public SkillTypeController() : base(new Dictionary<string, Func<GameData, SkillTypeData, string>>
    {
      {nameof(SkillTypeData.ImplicitModifiers), (g, d) => RenderModifiers(g, d.ImplicitModifiers)}
    })
    {
    }
  }

  sealed class CreatureItemController : EditorController<CreatureItemData>
  {
    public CreatureItemController() : base(new Dictionary<string, Func<GameData, CreatureItemData, string>>
    {
      {nameof(CreatureItemData.ImplicitModifiers), (g, d) => RenderModifiers(g, d.ImplicitModifiers)}
    })
    {
    }

    public override void WriteForeignKey(int id, ReflectivePath reflectivePath)
    {
      if (reflectivePath.Read() is CreatureItemData _)
      {
        reflectivePath.Write(CreatureTypeData.DefaultItem(id, reflectivePath.Database.Snapshot()));
      }
    }
  }

  public static class EditorControllerRegistry
  {
    static readonly IReadOnlyDictionary<Type, EditorController> Controllers =
      new Dictionary<Type, EditorController>
      {
        {typeof(CreatureTypeData), new CreatureTypeController()},
        {typeof(SkillTypeData), new SkillTypeController()},
        {typeof(CreatureItemData), new CreatureItemController()}
      };

    public static string RenderPropertyPreview(GameData gameData, object? parentValue, PropertyInfo property)
    {
      if (parentValue == null)
      {
        return "Null Parent";
      }

      var value = property.GetValue(parentValue);
      return Controllers.ContainsKey(parentValue.GetType())
        ? Controllers[parentValue.GetType()].Preview(gameData, parentValue, property.Name, value)
        : RenderDefault(value);
    }

    public static void WriteForeignKey(int id, ReflectivePath reflectivePath)
    {
      var parent = reflectivePath.Parent();
      var type = parent?.GetUnderlyingType();
      if (parent != null && type != null && Controllers.ContainsKey(type))
      {
        Controllers[type].WriteForeignKey(id, parent);
      }
      else
      {
        reflectivePath.Write(id);
      }
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
