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
using UnityEngine;
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
    readonly VisualElement _content;
    readonly Dictionary<Vector2Int, EditorCell> _cells;
    readonly ScrollView _scrollView;
    Vector2Int? _currentlySelected;
    Typeahead? _typeahead;
    VisualElement? _inlineEditor;
    bool _focusedOnInlineEditor;

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

      _cells = RenderTable();

      RegisterCallback<KeyUpEvent>(OnKeyUp);
    }

    Dictionary<Vector2Int, EditorCell> RenderTable()
    {
      var result = new Dictionary<Vector2Int, EditorCell>();

      foreach (var heading in _config.Headings)
      {
        _content.Add(HeadingCell(heading));
      }

      for (var rowIndex = 0; rowIndex < _config.Rows.Count; ++rowIndex)
      {
        var row = _config.Rows[rowIndex].Values;
        for (var columnIndex = 0; columnIndex < row.Count; ++columnIndex)
        {
          var value = row[columnIndex];
          var position = new Vector2Int(columnIndex, rowIndex);
          var cell = new EditorCell(
            _controller,
            this,
            position,
            value.Type,
            DefaultCellWidth,
            value.Object,
            value.Name);
          result[position] = cell;
          _content.Add(cell);
        }
      }

      return result;
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

    void SelectPosition(Vector2Int position)
    {
      if (_cells.ContainsKey(position))
      {
        HidePopups();

        if (_currentlySelected.HasValue)
        {
          _cells[_currentlySelected.Value].Unselect();
        }

        _cells[position].Select();
        _currentlySelected = position;
        _scrollView.ScrollTo(_cells[position]);
      }
    }

    public void OnClick(Vector2Int position) => SelectPosition(position);

    void OnKeyUp(KeyUpEvent evt)
    {
      if (!_currentlySelected.HasValue)
      {
        return;
      }

      switch (evt.keyCode)
      {
        case KeyCode.Tab when !evt.shiftKey:
        case KeyCode.RightArrow:
          SelectPosition(_currentlySelected.Value + new Vector2Int(1, 0));
          break;
        case KeyCode.Tab when evt.shiftKey:
        case KeyCode.LeftArrow:
          SelectPosition(_currentlySelected.Value + new Vector2Int(-1, 0));
          break;
        case KeyCode.UpArrow:
          if (_typeahead != null)
          {
            _typeahead.Previous();
          }
          else
          {
            SelectPosition(_currentlySelected.Value + new Vector2Int(0, -1));
          }

          break;
        case KeyCode.DownArrow:
          if (_typeahead != null)
          {
            _typeahead.Next();
          }
          else
          {
            SelectPosition(_currentlySelected.Value + new Vector2Int(0, 1));
          }

          break;
        case KeyCode.Return:
        case KeyCode.KeypadEnter:
          _typeahead?.Confirm();
          FocusOnInlineEditor(_currentlySelected.Value, true);
          break;
        case KeyCode.Escape:
          HidePopups();
          FocusOnInlineEditor(_currentlySelected.Value, false);
          break;
      }
    }

    void FocusOnInlineEditor(Vector2Int currentlySelected, bool focused)
    {
      if (_inlineEditor == null)
      {
        return;
      }

      _focusedOnInlineEditor = focused;
      if (focused)
      {
        _cells[currentlySelected].AddToClassList("focused");
        _inlineEditor.AddToClassList("focused");
      }
      else
      {
        _cells[currentlySelected].RemoveFromClassList("focused");
        _inlineEditor.RemoveFromClassList("focused");
      }
    }

    public void HidePopups()
    {
      _inlineEditor?.RemoveFromHierarchy();
      _inlineEditor = null;

      _typeahead?.RemoveFromHierarchy();
      _typeahead = null;
    }

    public void ShowTypeahead(TextField field, List<string> suggestions)
    {
      HidePopups();
      _typeahead = new Typeahead(field, suggestions);
      _typeahead.AddToClassList("typeahead");
      _controller.Screen.Add(_typeahead);
    }

    public void ShowInlineEditor(VisualElement anchor, Config config)
    {
      _inlineEditor = new VisualElement();
      _inlineEditor.AddToClassList("inline-editor");
      _inlineEditor.Add(new ObjectEditor(_controller, config));
      _inlineEditor.style.top = anchor.worldBound.y + anchor.worldBound.height;
      _inlineEditor.style.left = anchor.worldBound.x - config.Width + anchor.worldBound.width;
      _controller.Screen.Add(_inlineEditor);
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
        Values = values.ToList();
      }

      public List<Value> Values { get; }
    }

    public sealed class Config
    {
      public Config(IEnumerable<string> headings, IEnumerable<Row> rows, int width)
      {
        Headings = headings;
        Rows = rows.ToList();
        Width = width;
      }

      public IEnumerable<string> Headings { get; }
      public List<Row> Rows { get; }
      public int Width { get; }
    }
  }
}
