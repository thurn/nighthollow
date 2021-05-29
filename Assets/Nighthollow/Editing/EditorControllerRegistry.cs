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
using Nighthollow.Items;
using Nighthollow.Rules;
using Nighthollow.Services;

#nullable enable

namespace Nighthollow.Editing
{
  public abstract class EditorController
  {
    public virtual TableEditorSheetDelegate? GetTableDelegate(
      ServiceRegistry registry,
      ReflectivePath reflectivePath,
      EditorSheetDelegate.DropdownCellContent tableSelector)
    {
      return null;
    }

    public virtual EditorSheetDelegate? GetCustomChildSheetDelegate(ReflectivePath reflectivePath)
    {
      return null;
    }

    public abstract string Preview(GameData gameData, object container, string propertyName, object? propertyValue);

    public virtual void WriteForeignKey(int id, ReflectivePath reflectivePath)
    {
      throw new InvalidOperationException($"No implementation provided for WriteForeignKey {id}");
    }

    public virtual int GetColumnWidth(string propertyName)
    {
      return EditorSheet.DefaultCellWidth;
    }

    public virtual bool ShowEditButton() => false;
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

    protected static string RenderForeignKeyList(GameData gameData, ImmutableList<int> list, ITableId tableId)
    {
      string Print(int i)
      {
        var table = tableId.GetInUnchecked(gameData);
        return table.Contains(i) ? table[i].ToString() : "None";
      }

      return list.Count == 0 ? "[]" : string.Join("\n", list.Select(Print));
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

    public override int GetColumnWidth(string propertyName) => propertyName switch
    {
      nameof(CreatureTypeData.PrefabAddress) => 400,
      nameof(CreatureTypeData.Owner) => 200,
      nameof(CreatureTypeData.Health) => 200,
      nameof(CreatureTypeData.ImageAddress) => 400,
      nameof(CreatureTypeData.BaseManaCost) => 100,
      nameof(CreatureTypeData.Speed) => 100,
      nameof(CreatureTypeData.ImplicitModifiers) => 500,
      _ => base.GetColumnWidth(propertyName)
    };
  }

  sealed class SkillTypeController : EditorController<SkillTypeData>
  {
    public SkillTypeController() : base(new Dictionary<string, Func<GameData, SkillTypeData, string>>
    {
      {nameof(SkillTypeData.ImplicitModifiers), (g, d) => RenderModifiers(g, d.ImplicitModifiers)},
      {
        nameof(SkillTypeData.StatusEffects), (g, d) =>
          RenderForeignKeyList(g, d.StatusEffects, TableId.StatusEffectTypes)
      }
    })
    {
    }

    public override int GetColumnWidth(string propertyName) => propertyName switch
    {
      nameof(SkillTypeData.ImplicitModifiers) => 500,
      _ => base.GetColumnWidth(propertyName)
    };
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
      else
      {
        base.WriteForeignKey(id, reflectivePath);
      }
    }

    public override int GetColumnWidth(string propertyName) => propertyName switch
    {
      nameof(CreatureItemData.ImplicitModifiers) => 500,
      _ => base.GetColumnWidth(propertyName)
    };
  }

  sealed class SkillItemController : EditorController<SkillItemData>
  {
    public SkillItemController() : base(new Dictionary<string, Func<GameData, SkillItemData, string>>
    {
      {nameof(SkillItemData.ImplicitModifiers), (g, d) => RenderModifiers(g, d.ImplicitModifiers)},
      {nameof(SkillItemData.StatusEffects), (g, d) => RenderStatusEffects(g, d.StatusEffects)}
    })
    {
    }

    static string RenderStatusEffects(GameData gameData, ImmutableList<StatusEffectItemData> statusEffects) =>
      string.Join(",", statusEffects.Select(effect => gameData.StatusEffects[effect.StatusEffectTypeId].Name));

    public override void WriteForeignKey(int id, ReflectivePath reflectivePath)
    {
      if (reflectivePath.Read() is SkillItemData _)
      {
        reflectivePath.Write(SkillTypeData.DefaultItem(id, reflectivePath.Database.Snapshot()));
      }
      else
      {
        base.WriteForeignKey(id, reflectivePath);
      }
    }

    public override int GetColumnWidth(string propertyName) => propertyName switch
    {
      nameof(SkillItemData.ImplicitModifiers) => 500,
      _ => base.GetColumnWidth(propertyName)
    };
  }

  sealed class StatusEffectTypeController : EditorController<StatusEffectTypeData>
  {
    public StatusEffectTypeController() : base(new Dictionary<string, Func<GameData, StatusEffectTypeData, string>>
    {
      {nameof(StatusEffectTypeData.ImplicitModifiers), (g, d) => RenderModifiers(g, d.ImplicitModifiers)}
    })
    {
    }

    public override int GetColumnWidth(string propertyName) => propertyName switch
    {
      nameof(StatusEffectTypeData.ImplicitModifiers) => 500,
      _ => base.GetColumnWidth(propertyName)
    };
  }

  sealed class StatusEffectItemController : EditorController<StatusEffectItemData>
  {
    public StatusEffectItemController() : base(new Dictionary<string, Func<GameData, StatusEffectItemData, string>>
    {
      {nameof(StatusEffectItemData.ImplicitModifiers), (g, d) => RenderModifiers(g, d.ImplicitModifiers)}
    })
    {
    }

    public override void WriteForeignKey(int id, ReflectivePath reflectivePath)
    {
      if (reflectivePath.Read() is StatusEffectItemData _)
      {
        reflectivePath.Write(StatusEffectTypeData.DefaultItem(id, reflectivePath.Database.Snapshot()));
      }
      else
      {
        base.WriteForeignKey(id, reflectivePath);
      }
    }

    public override int GetColumnWidth(string propertyName) => propertyName switch
    {
      nameof(StatusEffectItemData.ImplicitModifiers) => 500,
      _ => base.GetColumnWidth(propertyName)
    };
  }

  sealed class RuleDataController : EditorController<Rule>
  {
    public override EditorSheetDelegate GetCustomChildSheetDelegate(ReflectivePath reflectivePath)
    {
      return new RuleEditorSheetDelegate(reflectivePath);
    }

    public override bool ShowEditButton() => true;
  }

  sealed class GlobalDataController : EditorController<GlobalData>
  {
    public override int GetColumnWidth(string propertyName) => propertyName switch
    {
      nameof(GlobalData.Name) => 600,
      nameof(GlobalData.Value) => 100,
      nameof(GlobalData.Comment) => 1000,
      _ => base.GetColumnWidth(propertyName)
    };
  }

  public static class EditorControllerRegistry
  {
    static readonly IReadOnlyDictionary<Type, EditorController> Controllers =
      new Dictionary<Type, EditorController>
      {
        {typeof(CreatureTypeData), new CreatureTypeController()},
        {typeof(SkillTypeData), new SkillTypeController()},
        {typeof(CreatureItemData), new CreatureItemController()},
        {typeof(SkillItemData), new SkillItemController()},
        {typeof(StatusEffectTypeData), new StatusEffectTypeController()},
        {typeof(StatusEffectItemData), new StatusEffectItemController()},
        {typeof(Rule), new RuleDataController()},
        {typeof(GlobalData), new GlobalDataController()}
      };

    public static TableEditorSheetDelegate GetTableDelegate(
      ServiceRegistry registry,
      ReflectivePath reflectivePath,
      EditorSheetDelegate.DropdownCellContent tableSelector,
      string tableName)
    {
      var tableDelegate = Controllers.ContainsKey(reflectivePath.GetUnderlyingType())
        ? Controllers[reflectivePath.GetUnderlyingType()].GetTableDelegate(registry, reflectivePath, tableSelector)
        : null;
      return tableDelegate ?? new TableEditorSheetDelegate(reflectivePath, tableSelector, tableName);
    }

    public static string RenderPropertyPreview(GameData gameData, object? parentValue, PropertyInfo property)
    {
      if (parentValue == null)
      {
        return "ERROR: Null Parent";
      }

      var value = property.GetValue(parentValue);

      if (value == null)
      {
        return "<Null>";
      }

      return Controllers.ContainsKey(parentValue.GetType())
        ? Controllers[parentValue.GetType()].Preview(gameData, parentValue, property.Name, value)
        : RenderDefault(value);
    }

    public static void WriteForeignKey(int foreignId, ReflectivePath reflectivePath)
    {
      var parent = reflectivePath.Parent();
      var type = parent?.GetUnderlyingType();
      if (parent != null && type != null && Controllers.ContainsKey(type))
      {
        Controllers[type].WriteForeignKey(foreignId, parent);
      }
      else
      {
        reflectivePath.Write(foreignId);
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

    public static int GetColumnWidth(Type type, PropertyInfo propertyInfo) =>
      Controllers.ContainsKey(type)
        ? Controllers[type].GetColumnWidth(propertyInfo.Name)
        : EditorSheet.DefaultCellWidth;

    public static EditorSheetDelegate? GetCustomChildSheetDelegate(ReflectivePath reflectivePath)
    {
      return Controllers.ContainsKey(reflectivePath.GetUnderlyingType())
        ? Controllers[reflectivePath.GetUnderlyingType()].GetCustomChildSheetDelegate(reflectivePath)
        : null;
    }

    public static bool ShowEditButton(Type type) =>
      Controllers.ContainsKey(type) ? Controllers[type].ShowEditButton() : false;
  }
}