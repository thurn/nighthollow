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
using Nighthollow.Data;
using Nighthollow.Editing;
using Nighthollow.Services;
using Nighthollow.Utils;

#nullable enable

namespace Nighthollow.Triggers
{
  public sealed class TriggerTableEditorSheetDelegate : TableEditorSheetDelegate
  {
    // who needs proper persistence mechanisms when you can randomly make stuff static?
    static TriggerCategory? _category;
    readonly ServiceRegistry _registry;

    public TriggerTableEditorSheetDelegate(
      ServiceRegistry registry,
      ReflectivePath path,
      DropdownCellContent tableSelector) : base(path, tableSelector, "Triggers")
    {
      _registry = registry;
    }

    protected override TableContent RenderTable(ReflectivePath databasePath, DropdownCellContent tableSelector)
    {
      var result = new List<List<ICellContent>>
      {
        new List<ICellContent> {tableSelector},
        new List<ICellContent?>
        {
          new LabelCellContent("v"),
          new LabelCellContent("x"),
          _category == TriggerCategory.Debug ? new LabelCellContent(">") : null,
          new LabelCellContent("Filter Category"),
          EnumDropdownEditorCell.CreateEnumContent(_category?.ToString(), typeof(TriggerCategory),
            selected =>
            {
              _category = (TriggerCategory) selected;
              OnModified?.Invoke();
            })
        }.WhereNotNull().ToList(),
        new List<ICellContent?>
        {
          new LabelCellContent("v"),
          new LabelCellContent("x"),
          _category == TriggerCategory.Debug ? new LabelCellContent(">") : null,
          new LabelCellContent("Name"),
          new LabelCellContent("Category"),
          new LabelCellContent("Description"),
          new LabelCellContent("Looping?"),
          new LabelCellContent("Disabled?"),
        }.WhereNotNull().ToList()
      };

      foreach (int entityId in databasePath.GetTable().Keys)
      {
        var trigger = (ITrigger) databasePath.GetTable()[entityId];
        Errors.CheckNotNull(trigger);
        if (_category != null && _category != TriggerCategory.NoCategory && trigger.Category != _category)
        {
          continue;
        }

        var path = databasePath.EntityId(entityId);
        result.Add(new List<ICellContent?>
        {
          new ViewChildButtonCellContent(path),
          RowDeleteButton(entityId),
          _category == TriggerCategory.Debug
            ? new ButtonCellContent(">", () => { _registry.TriggerService.InvokeTriggerId(_registry, entityId); })
            : null,
          new ReflectivePathCellContent(path.Property(trigger.GetType().GetProperty("Name")!)),
          new ReflectivePathCellContent(path.Property(trigger.GetType().GetProperty("Category")!)),
          new ReflectivePathCellContent(path),
          new ReflectivePathCellContent(path.Property(trigger.GetType().GetProperty("Looping")!)),
          new ReflectivePathCellContent(path.Property(trigger.GetType().GetProperty("Disabled")!))
        }.WhereNotNull().ToList());
      }

      result.Add(GetAddButtonRow(databasePath));

      return new TableContent(result, new List<int?>
      {
        50,
        50,
        _category == TriggerCategory.Debug ? 50 : (int?) null,
        400,
        250,
        800,
        150,
        150
      }.WhereNotNull().ToList());
    }

    static List<Type> EventTypes() =>
      typeof(ITrigger).GetCustomAttributes<UnionAttribute>()
        .Select(attribute => attribute.SubType.GetGenericArguments()[0])
        .ToList();

    void AddNewTrigger<TEvent>(ReflectivePath path) where TEvent : TriggerEvent
    {
      path.Database.Insert(TableId.Triggers, new TriggerData<TEvent>(
        "New Trigger",
        _category ?? TriggerCategory.NoCategory));
    }

    List<ICellContent> GetAddButtonRow(ReflectivePath reflectivePath)
    {
      var eventTypes = EventTypes();
      return new List<ICellContent>
      {
        new DropdownCellContent(
          eventTypes.Select(t => Description.Snippet("When", t)).ToList(),
          currentlySelected: null,
          i =>
          {
            GetType()
              .GetMethod(nameof(AddNewTrigger), BindingFlags.Instance | BindingFlags.NonPublic)!
              .MakeGenericMethod(eventTypes[i])
              .Invoke(this, new object[] {reflectivePath});
          },
          "Add Trigger...")
      };
    }
  }
}