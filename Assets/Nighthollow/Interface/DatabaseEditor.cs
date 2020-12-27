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
using System.Linq;
using Nighthollow.Data;
using UnityEngine;
using UnityEngine.UIElements;

#nullable enable

namespace Nighthollow.Interface
{
  public sealed class DatabaseEditor : HideableElement<DatabaseEditor.Args>
  {
    public sealed class Args
    {
      public Args(GameData gameData)
      {
        GameData = gameData;
      }

      public GameData GameData { get; }
    }

    public new sealed class UxmlFactory : UxmlFactory<DatabaseEditor, UxmlTraits>
    {
    }

    VisualElement _tabs = null!;
    VisualElement _content = null!;

    protected override void Initialize()
    {
      _tabs = FindElement("DatabaseTabs");
      _content = FindElement("DatabaseContent");

      foreach (var property in typeof(GameData).GetProperties())
      {
        _tabs.Add(ButtonUtil.Create(new ButtonUtil.Button(property.Name, () => { })));
      }
    }

    protected override void OnShow(Args argument)
    {
      var property = typeof(GameData).GetProperties().First();
      Debug.Log(property.Name);
      RenderTable(
        _content,
        property.PropertyType.GetGenericArguments()[1],
        (property.GetValue(argument.GameData) as IDictionary)!);
    }

    void RenderTable(VisualElement container, Type valueType, IDictionary table)
    {
      foreach (var property in valueType.GetProperties())
      {
        container.Add(LabelCell(property.Name));
      }

      foreach (var rowId in table.Keys)
      {
        var row = table[rowId];
        foreach (var property in row.GetType().GetProperties())
        {
          var value = property.GetValue(row);
          var type = property.PropertyType;
          container.Add(new EditorCell(Controller, property.Name, type, value));
        }
      }
    }

    static VisualElement LabelCell(string? text)
    {
      var label = new Label();
      if (text != null)
      {
        label.text = text.Length > 50 ? text.Substring(0, 49) + "..." : text;
      }

      label.AddToClassList("editor-text");

      var cell = new VisualElement();
      cell.AddToClassList("editor-cell");
      cell.Add(label);
      return cell;
    }
  }
}
