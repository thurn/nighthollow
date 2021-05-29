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

namespace Nighthollow.Rules
{
  public sealed class RuleEditorSheetDelegate : EditorSheetDelegate
  {
    readonly ReflectivePath _reflectivePath;

    public RuleEditorSheetDelegate(ReflectivePath reflectivePath)
    {
      _reflectivePath = reflectivePath;
    }

    public override string SheetName() => "Rule Data";

    public override void Initialize(Action onModified)
    {
      _reflectivePath.OnEntityUpdated(onModified);
    }

    public override TableContent GetCells()
    {
      var rule = _reflectivePath.Read() as Rule;
      var content = GetContent(Errors.CheckNotNull(rule));
      return new TableContent(content!,
        CollectionUtils.Single(50)
          .Concat(Enumerable.Repeat(300, content!.Max(l => l.Count) - 1)).ToList());
    }

    List<List<ICellContent>> GetContent(Rule rule)
    {
      var result = new List<List<ICellContent>>
      {
        HeaderRow(rule),
        EventRow(rule),
      };

      result.AddRange(rule.Conditions.Select((c, i) => ConditionRow(rule, i, c)));
      result.Add(AddConditionRow(rule));
      result.AddRange(rule.Effects.Select((e, i) => EffectRow(rule, i, e)));
      result.Add(AddEffectRow(rule));

      return result;
    }

    List<ICellContent> HeaderRow(Rule rule)
    {
      return new List<ICellContent>
      {
        new LabelCellContent("x"),
        new LabelCellContent("Rule Name"),
        new ReflectivePathCellContent(
          _reflectivePath.Property(rule.GetType().GetProperty(nameof(Rule.Name))!)
        )
      };
    }

    List<ICellContent> EventRow(Rule rule)
    {
      return new List<ICellContent>
      {
        new LabelCellContent(rule.EventName.GetSpec().Snippet())
      };
    }

    static List<Type> ConditionTypes() => typeof(RuleCondition)
      .GetCustomAttributes<UnionAttribute>()
      .Select(attribute => attribute.SubType)
      .ToList();

    List<ICellContent> ConditionRow(Rule rule, int index, RuleCondition condition)
    {
      var conditionPath =
        _reflectivePath.Property(rule.GetType().GetProperty(nameof(Rule.Conditions))!)
          .ListIndex(typeof(RuleCondition), index);
      var result = new List<ICellContent>
      {
        new ButtonCellContent("x",
          () => { _reflectivePath.Write(rule.WithConditions(rule.Conditions.RemoveAt(index))); }),
      };

      Description.GetDescription(condition.GetType()).Iterate(condition,
        s => { result.Add(new LabelCellContent(s)); },
        p => { result.Add(PropertyCellContent(conditionPath, p)); },
        index == 0 ? "If" : "And");

      return result;
    }

    List<ICellContent> AddConditionRow(Rule rule)
    {
      var types = ConditionTypes().ToList();
      return new List<ICellContent>
      {
        new DropdownCellContent(
          types.Select(c => Description.Snippet(rule.Conditions.IsEmpty ? "If" : "And", c)).ToList(),
          currentlySelected: null,
          i =>
          {
            _reflectivePath.Write(
              rule.WithConditions(
                rule.Conditions.Add((RuleCondition) TypeUtils.InstantiateWithDefaults(types[i]))));
          },
          "Add Condition...")
      };
    }

    static IEnumerable<Type> EffectTypes() => typeof(RuleEffect)
      .GetCustomAttributes<UnionAttribute>()
      .Select(attribute => attribute.SubType);

    List<ICellContent> EffectRow(Rule rule, int index, RuleEffect effect)
    {
      var effectPath =
        _reflectivePath.Property(rule.GetType().GetProperty(nameof(Rule.Effects))!)
          .ListIndex(typeof(RuleEffect), index);
      var result = new List<ICellContent>
      {
        new ButtonCellContent("x",
          () => { _reflectivePath.Write(rule.WithEffects(rule.Effects.RemoveAt(index))); }),
      };

      Description.GetDescription(effect.GetType()).Iterate(effect,
        s => { result.Add(new LabelCellContent(s)); },
        p => { result.Add(PropertyCellContent(effectPath, p)); },
        index == 0 ? "Then" : "And");

      return result;
    }

    List<ICellContent> AddEffectRow(Rule rule)
    {
      var types = EffectTypes().Where(t => typeof(RuleEffect).IsAssignableFrom(t)).ToList();
      return new List<ICellContent>
      {
        new DropdownCellContent(
          types.Select(c => Description.Snippet(rule.Effects.IsEmpty ? "Then" : "And", c)).ToList(),
          currentlySelected: null,
          i =>
          {
            _reflectivePath.Write(
              rule.WithEffects(
                rule.Effects.Add((RuleEffect) TypeUtils.InstantiateWithDefaults(types[i]))));
          },
          "Add Effect...")
      };
    }

    ICellContent PropertyCellContent(ReflectivePath path, PropertyInfo property)
    {
      var attribute = property.GetCustomAttribute<RuleId>();
      if (attribute != null)
      {
        var childPath = path.Property(property);
        var currentValue = (int?) childPath.Read();
        var invokedRules = new List<KeyValuePair<int, Rule>>();
        int? selectedIndex = null;
        var index = 0;

        foreach (var pair in path.Database.Snapshot().Rules
          .Where(pair => pair.Value.EventName == EventName.RuleInvoked))
        {
          invokedRules.Add(pair);
          if (currentValue == pair.Key)
          {
            selectedIndex = index;
          }

          index++;
        }

        return new DropdownCellContent(
          invokedRules.Select(pair => pair.Value.Name ?? $"Rule {pair.Key}").ToList(),
          selectedIndex,
          selected => { childPath.Write(invokedRules[selected].Key); });
      }
      else
      {
        return new ReflectivePathCellContent(path.Property(property));
      }
    }
  }
}
