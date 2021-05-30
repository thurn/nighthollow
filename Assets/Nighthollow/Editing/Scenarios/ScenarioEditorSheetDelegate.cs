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
using Nighthollow.Rules;
using Nighthollow.Utils;

#nullable enable

namespace Nighthollow.Editing.Scenarios
{
  public sealed class ScenarioEditorSheetDelegate : EditorSheetDelegate
  {
    readonly ReflectivePath _reflectivePath;

    public ScenarioEditorSheetDelegate(ReflectivePath reflectivePath)
    {
      _reflectivePath = reflectivePath;
    }

    public override string SheetName() => "Scenario";

    public override void Initialize(Action onModified)
    {
      _reflectivePath.OnEntityUpdated(onModified);
    }

    public override TableContent GetCells()
    {
      var rule = _reflectivePath.Read() as ScenarioData;
      var content = GetContent(Errors.CheckNotNull(rule));
      return new TableContent(content!,
        CollectionUtils.Single(50)
          .Concat(Enumerable.Repeat(300, content!.Max(l => l.Count) - 1)).ToList());
    }

    List<List<ICellContent>> GetContent(ScenarioData scenario)
    {
      var result = new List<List<ICellContent>>
      {
        NameRow(scenario)
      };

      result.AddRange(scenario.Effects.Select((e, i) =>
        RuleEditorSheetDelegate.EffectRow(_reflectivePath, scenario, i, e)));
      result.Add(RuleEditorSheetDelegate.AddEffectRow(_reflectivePath, scenario));

      return result;
    }

    List<ICellContent> NameRow(ScenarioData scenario)
    {
      return new List<ICellContent>
      {
        new LabelCellContent("x"),
        new LabelCellContent("Scenario Name"),
        new ReflectivePathCellContent(
          _reflectivePath.Property(scenario.GetType().GetProperty(nameof(ScenarioData.Name))!)
        )
      };
    }
  }
}