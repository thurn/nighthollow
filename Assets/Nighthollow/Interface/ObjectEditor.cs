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
using Nighthollow.Editing;
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
    readonly Dictionary<Vector2Int, OldCell> _cells;
    readonly ScrollView _scrollView;
    readonly ObjectEditor? _parentEditor;
    readonly int _width;
    Vector2Int? _currentlySelected;
    Typeahead? _typeahead;
    VisualElement? _inlineEditorContainer;
    bool _exclusiveFocused;
    ObjectEditor? _inlineEditor;

    public static Config ForTable(ReflectivePath path, Type type, IDictionary table)
    {
      var rows = new List<Row>();
      foreach (var rowId in table.Keys)
      {
        rows.Add(new Row(
          type.GetProperties()
            .Select(property => new Value(
              path.EntityId((int) rowId).Property(property),
              property.PropertyType,
              property.GetValue(table[rowId]),
              property.Name))
            .Prepend(new Value(reflectivePath: null, rowId.GetType(), rowId, "Id"))));
      }

      return new Config(
        type.GetProperties().Select(p => p.Name).Prepend("ID"),
        rows
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
            new Value(null, key.GetType(), key),
            new Value(null, dictionary[key].GetType(), dictionary[key])
          }));
      }

      return new Config(new List<string> {"Key", "Value"}, rows);
    }

    public ObjectEditor(
      ScreenController controller,
      Config config,
      int? height = null,
      ObjectEditor? parentEditor = null)
    {
      _controller = controller;
      _config = config;
      _parentEditor = parentEditor;

      _content = new VisualElement();
      _content.AddToClassList("editor-content");

      _width = (2 * ContentPadding) + (DefaultCellWidth * _config.Headings.Count);
      _content.style.width = _width;

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

    Dictionary<Vector2Int, OldCell> RenderTable()
    {
      var result = new Dictionary<Vector2Int, OldCell>();

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
          var cell = new OldCell(
            _controller,
            value.ReflectivePath,
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

    void SelectPosition(Vector2Int? position)
    {
      if (position.HasValue && _cells.ContainsKey(position.Value) && position != _currentlySelected)
      {
        HidePopups();

        if (_currentlySelected.HasValue)
        {
          _cells[_currentlySelected.Value].Unselect();
        }

        _cells[position.Value].Select();
        _currentlySelected = position;
        _scrollView.ScrollTo(_cells[position.Value]);
      }
    }

    public void OnClick(Vector2Int position) => SelectPosition(position);

    void OnKeyUp(KeyUpEvent evt)
    {
      if (!_currentlySelected.HasValue)
      {
        return;
      }

      if (evt.keyCode == KeyCode.Escape)
      {
        ExclusiveFocus(false);
        HidePopups();

        if (_parentEditor != null)
        {
          _parentEditor.ExclusiveFocus(false);
          _parentEditor.HidePopups();
        }
      }

      if (_exclusiveFocused)
      {
        _inlineEditor?.OnKeyUp(evt);
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
          ExclusiveFocus(true);
          break;
      }
    }

    void ExclusiveFocus(bool focused)
    {
      if (_inlineEditorContainer == null || _inlineEditor == null || _currentlySelected == null)
      {
        return;
      }

      _exclusiveFocused = focused;

      if (focused)
      {
        _cells[_currentlySelected.Value].Focusable = false;
        _cells[_currentlySelected.Value].AddToClassList("focused");
        _inlineEditorContainer.AddToClassList("focused");
        _inlineEditor.SelectPosition(new Vector2Int(0, 0));
      }
      else
      {
        _cells[_currentlySelected.Value].Focusable = true;
        _cells[_currentlySelected.Value].RemoveFromClassList("focused");
      }
    }

    public void HidePopups()
    {
      _inlineEditorContainer?.RemoveFromHierarchy();
      _inlineEditorContainer = null;
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
      _inlineEditor = new ObjectEditor(_controller, config, parentEditor: this);
      _inlineEditorContainer = new VisualElement();
      _inlineEditorContainer.AddToClassList("inline-editor");
      _inlineEditorContainer.Add(_inlineEditor);
      _inlineEditorContainer.style.top = anchor.worldBound.y + anchor.worldBound.height;
      _inlineEditorContainer.style.left = anchor.worldBound.x - _inlineEditor._width + anchor.worldBound.width;
      _controller.Screen.Add(_inlineEditorContainer);
    }

    public sealed class Value
    {
      public Value(ReflectivePath? reflectivePath, Type type, object? o = null, string? name = null)
      {
        ReflectivePath = reflectivePath;
        Type = type;
        Object = o;
        Name = name;
      }

      public ReflectivePath? ReflectivePath { get; }
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
      public Config(IEnumerable<string> headings, IEnumerable<Row> rows)
      {
        Headings = headings.ToList();
        Rows = rows.ToList();
      }

      public List<string> Headings { get; }
      public List<Row> Rows { get; }
    }
  }
}
