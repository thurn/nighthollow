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
using UnityEngine.UIElements;

#nullable enable

namespace Nighthollow.Interface
{
  public sealed class ObjectEditor : VisualElement
  {
    const int DefaultCellWidth = 250;
    const int ContentPadding = 16;

    readonly ScreenController _controller;
    readonly Config _config;
    readonly ScrollView _scrollView;
    readonly VisualElement _content;

    public static Config ForTable(Type type, IDictionary table)
    {
      var rows = from object? rowId in table.Keys
        select type.GetProperties()
          .Select(property => new Value(property.PropertyType, property.GetValue(table[rowId]), property.Name))
          .ToList()
        into valueList
        select new Row(valueList);

      return new Config(
        type.GetProperties().Select(p => p.Name),
        rows,
        width: (2 * ContentPadding) + (DefaultCellWidth * type.GetProperties().Length)
      );
    }

    public static Config ForPrimitiveDictionary(Type type, IDictionary? dictionary)
    {
      var rows = new List<Row>();
      if (dictionary != null)
      {
        rows.AddRange(from object? key in dictionary.Keys
          select new Row(new List<Value>
          {
            new Value(key.GetType(), key),
            new Value(dictionary[key].GetType(), dictionary[key])
          }));
      }

      return new Config(
        new List<string> {"Key", "Value"},
        rows,
        width: (2 * ContentPadding) + (DefaultCellWidth * 2));
    }

    public ObjectEditor(ScreenController controller, Config config, int? height = null)
    {
      _controller = controller;
      _config = config;

      _content = new VisualElement();
      _content.AddToClassList("editor-content");

      _content.style.width = config.Width;

      if (height != null)
      {
        _content.style.height = height.Value;
      }

      _scrollView = new ScrollView();
      _scrollView.AddToClassList("editor-scroll-view");
      _scrollView.Add(_content);

      Add(_scrollView);

      RenderTable();
    }

    void RenderTable()
    {
      foreach (var heading in _config.Headings)
      {
        _content.Add(HeadingCell(heading));
      }

      foreach (var value in _config.Rows.SelectMany(row => row.Values))
      {
        _content.Add(new EditorCell(_controller, _scrollView, value.Type, DefaultCellWidth, value.Object, value.Name));
      }
    }

    static VisualElement HeadingCell(string? text)
    {
      var label = new Label();
      if (text != null)
      {
        label.text = text.Length > 50 ? text.Substring(0, 49) + "..." : text;
      }

      label.AddToClassList("editor-heading");

      var cell = new VisualElement();
      cell.AddToClassList("editor-cell");
      cell.Add(label);
      cell.style.width = DefaultCellWidth;
      return cell;
    }

    public sealed class Value
    {
      public Value(Type type, object? o = null, string? name = null)
      {
        Type = type;
        Object = o;
        Name = name;
      }

      public Type Type { get; }
      public object? Object { get; }
      public string? Name { get; }
    }

    public sealed class Row
    {
      public Row(IEnumerable<Value> values)
      {
        Values = values;
      }

      public IEnumerable<Value> Values { get; }
    }

    public sealed class Config
    {
      public Config(IEnumerable<string> headings, IEnumerable<Row> rows, int width)
      {
        Headings = headings;
        Rows = rows;
        Width = width;
      }

      public IEnumerable<string> Headings { get; }
      public IEnumerable<Row> Rows { get; }
      public int Width { get; }
      public int Height { get; }
    }
  }
}
