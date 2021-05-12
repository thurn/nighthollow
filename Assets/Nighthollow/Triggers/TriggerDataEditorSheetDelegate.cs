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
      _reflectivePath = Errors.CheckNotNull(reflectivePath.Parent());
    }

    public override string SheetName() => "Trigger Data";

    public override void Initialize(Action onModified)
    {
      _reflectivePath.OnEntityUpdated(onModified);
    }

    public override TableContent GetCells()
    {
      var triggerData = _reflectivePath.Read() as ITrigger;
      var content = GetType()
        .GetMethod(nameof(GetContent), BindingFlags.NonPublic | BindingFlags.Instance)
        !.MakeGenericMethod(triggerData?.GetType().GetGenericArguments()[0])
        .Invoke(this, new object[] {triggerData!}) as List<List<ICellContent>>;

      return new TableContent(content!,
        CollectionUtils.Single(50)
          .Concat(Enumerable.Repeat(300, content!.Max(l => l.Count) - 1)).ToList());
    }

    List<List<ICellContent>> GetContent<TEvent>(TriggerData<TEvent> triggerData) where TEvent : TriggerEvent
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

    List<ICellContent> HeaderRow<TEvent>(TriggerData<TEvent> triggerData) where TEvent : TriggerEvent
    {
      return new List<ICellContent>
      {
        new LabelCellContent("x"),
        new LabelCellContent("Trigger Name"),
        new ReflectivePathCellContent(
          _reflectivePath.Property(triggerData.GetType().GetProperty(nameof(TriggerData<TEvent>.Name))!)
        )
      };
    }

    List<ICellContent> EventRow<TEvent>(TriggerData<TEvent> triggerData) where TEvent : TriggerEvent
    {
      return new List<ICellContent>
      {
        new LabelCellContent(Description.Snippet("When", typeof(TEvent)))
      };
    }

    static List<Type> ConditionTypes() => typeof(ICondition)
      .GetCustomAttributes<UnionAttribute>()
      .Select(attribute => attribute.SubType)
      .ToList();

    List<ICellContent> ConditionRow<TEvent>(TriggerData<TEvent> triggerData, int index, ICondition<TEvent> condition)
      where TEvent : TriggerEvent
    {
      var conditionPath =
        _reflectivePath.Property(triggerData.GetType().GetProperty(nameof(TriggerData<TEvent>.Conditions))!)
          .ListIndex(typeof(ICondition<TEvent>), index);
      var result = new List<ICellContent>
      {
        new ButtonCellContent("x",
          () => { _reflectivePath.Write(triggerData.WithConditions(triggerData.Conditions.RemoveAt(index))); }),
      };

      Description.GetDescription(condition.GetType()).Iterate(condition,
        s => { result.Add(new LabelCellContent(s)); },
        p => { result.Add(new ReflectivePathCellContent(conditionPath.Property(p))); },
        index == 0 ? "If" : "And");

      return result;
    }

    List<ICellContent> AddConditionRow<TEvent>(TriggerData<TEvent> triggerData) where TEvent : TriggerEvent
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
                triggerData.Conditions.Add((ICondition<TEvent>) TypeUtils.InstantiateWithDefaults(types[i]))));
          },
          "Add Condition...")
      };
    }

    static List<Type> EffectTypes() => typeof(IEffect)
      .GetCustomAttributes<UnionAttribute>()
      .Select(attribute => attribute.SubType)
      .ToList();

    List<ICellContent> EffectRow<TEvent>(TriggerData<TEvent> triggerData, int index, IEffect<TEvent> effect)
      where TEvent : TriggerEvent
    {
      var effectPath =
        _reflectivePath.Property(triggerData.GetType().GetProperty(nameof(TriggerData<TEvent>.Effects))!)
          .ListIndex(typeof(IEffect<TEvent>), index);
      var result = new List<ICellContent>
      {
        new ButtonCellContent("x",
          () => { _reflectivePath.Write(triggerData.WithEffects(triggerData.Effects.RemoveAt(index))); }),
      };

      Description.GetDescription(effect.GetType()).Iterate(effect,
        s => { result.Add(new LabelCellContent(s)); },
        p => { result.Add(new ReflectivePathCellContent(effectPath.Property(p))); },
        index == 0 ? "Then" : "And");

      return result;
    }

    List<ICellContent> AddEffectRow<TEvent>(TriggerData<TEvent> triggerData) where TEvent : TriggerEvent
    {
      var types = EffectTypes();
      return new List<ICellContent>
      {
        new DropdownCellContent(
          types.Select(c => Description.Snippet(triggerData.Effects.IsEmpty ? "Then" : "And", c)).ToList(),
          currentlySelected: null,
          i =>
          {
            _reflectivePath.Write(
              triggerData.WithEffects(
                triggerData.Effects.Add((IEffect<TEvent>) TypeUtils.InstantiateWithDefaults(types[i]))));
          },
          "Add Effect...")
      };
    }
  }
}