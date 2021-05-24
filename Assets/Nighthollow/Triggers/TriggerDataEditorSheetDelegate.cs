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
using MessagePack;
using Nighthollow.Editing;
using Nighthollow.Utils;

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
      _reflectivePath.OnEntityUpdated(onModified);
    }

    public override TableContent GetCells()
    {
      var triggerData = _reflectivePath.Read() as Rule;
      var content = GetContent(Errors.CheckNotNull(triggerData));
      return new TableContent(content!,
        CollectionUtils.Single(50)
          .Concat(Enumerable.Repeat(300, content!.Max(l => l.Count) - 1)).ToList());
    }

    List<List<ICellContent>> GetContent(Rule triggerData)
    {
      var result = new List<List<ICellContent>>
      {
        HeaderRow(triggerData),
        EventRow(triggerData),
      };

      result.AddRange(triggerData.Conditions.Select((c, i) => ConditionRow(triggerData, i, c)));
      result.Add(AddConditionRow(triggerData));
      result.AddRange(triggerData.Effects.Select((e, i) => EffectRow(triggerData, i, e)));
      result.Add(AddEffectRow(triggerData));

      return result;
    }

    List<ICellContent> HeaderRow(Rule triggerData)
    {
      return new List<ICellContent>
      {
        new LabelCellContent("x"),
        new LabelCellContent("Trigger Name"),
        new ReflectivePathCellContent(
          _reflectivePath.Property(triggerData.GetType().GetProperty(nameof(Rule.Name))!)
        )
      };
    }

    List<ICellContent> EventRow(Rule triggerData)
    {
      return new List<ICellContent>
      {
        new LabelCellContent(triggerData.TriggerEvent.GetSpec().Snippet())
      };
    }

    static List<Type> ConditionTypes() => typeof(TriggerCondition)
      .GetCustomAttributes<UnionAttribute>()
      .Select(attribute => attribute.SubType)
      .ToList();

    List<ICellContent> ConditionRow(Rule triggerData, int index, TriggerCondition condition)
    {
      var conditionPath =
        _reflectivePath.Property(triggerData.GetType().GetProperty(nameof(Rule.Conditions))!)
          .ListIndex(typeof(TriggerCondition), index);
      var result = new List<ICellContent>
      {
        new ButtonCellContent("x",
          () => { _reflectivePath.Write(triggerData.WithConditions(triggerData.Conditions.RemoveAt(index))); }),
      };

      Description.GetDescription(condition.GetType()).Iterate(condition,
        s => { result.Add(new LabelCellContent(s)); },
        p => { result.Add(PropertyCellContent(conditionPath, p)); },
        index == 0 ? "If" : "And");

      return result;
    }

    List<ICellContent> AddConditionRow(Rule triggerData)
    {
      var types = ConditionTypes().ToList();
      return new List<ICellContent>
      {
        new DropdownCellContent(
          types.Select(c => Description.Snippet(triggerData.Conditions.IsEmpty ? "If" : "And", c)).ToList(),
          currentlySelected: null,
          i =>
          {
            _reflectivePath.Write(
              triggerData.WithConditions(
                triggerData.Conditions.Add((TriggerCondition) TypeUtils.InstantiateWithDefaults(types[i]))));
          },
          "Add Condition...")
      };
    }

    static IEnumerable<Type> EffectTypes() => typeof(TriggerEffect)
      .GetCustomAttributes<UnionAttribute>()
      .Select(attribute => attribute.SubType);

    List<ICellContent> EffectRow(Rule triggerData, int index, TriggerEffect effect)
    {
      var effectPath =
        _reflectivePath.Property(triggerData.GetType().GetProperty(nameof(Rule.Effects))!)
          .ListIndex(typeof(TriggerEffect), index);
      var result = new List<ICellContent>
      {
        new ButtonCellContent("x",
          () => { _reflectivePath.Write(triggerData.WithEffects(triggerData.Effects.RemoveAt(index))); }),
      };

      Description.GetDescription(effect.GetType()).Iterate(effect,
        s => { result.Add(new LabelCellContent(s)); },
        p => { result.Add(PropertyCellContent(effectPath, p)); },
        index == 0 ? "Then" : "And");

      return result;
    }

    List<ICellContent> AddEffectRow(Rule triggerData)
    {
      var types = EffectTypes().Where(t => typeof(TriggerEffect).IsAssignableFrom(t)).ToList();
      return new List<ICellContent>
      {
        new DropdownCellContent(
          types.Select(c => Description.Snippet(triggerData.Effects.IsEmpty ? "Then" : "And", c)).ToList(),
          currentlySelected: null,
          i =>
          {
            _reflectivePath.Write(
              triggerData.WithEffects(
                triggerData.Effects.Add((TriggerEffect) TypeUtils.InstantiateWithDefaults(types[i]))));
          },
          "Add Effect...")
      };
    }

    ICellContent PropertyCellContent(ReflectivePath path, PropertyInfo property)
    {
      var attribute = property.GetCustomAttribute<TriggerId>();
      if (attribute != null)
      {
        var childPath = path.Property(property);
        var currentValue = (int?) childPath.Read();
        var invokedTriggers = new List<KeyValuePair<int, Rule>>();
        int? selectedIndex = null;
        var index = 0;

        foreach (var pair in path.Database.Snapshot().Triggers
          .Where(pair => pair.Value.TriggerEvent == TriggerEvent.TriggerInvoked))
        {
          invokedTriggers.Add(pair);
          if (currentValue == pair.Key)
          {
            selectedIndex = index;
          }

          index++;
        }

        return new DropdownCellContent(
          invokedTriggers.Select(pair => pair.Value.Name ?? $"Trigger {pair.Key}").ToList(),
          selectedIndex,
          selected => { childPath.Write(invokedTriggers[selected].Key); });
      }
      else
      {
        return new ReflectivePathCellContent(path.Property(property));
      }
    }
  }
}