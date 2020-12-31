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

using System.Collections.Generic;
using Nighthollow.Interface;
using UnityEngine;
using UnityEngine.UIElements;

#nullable enable

namespace Nighthollow.Editing
{
  public interface IEditor
  {
    void OnChildEditingComplete(string? preview);
  }

  public abstract class EditorSheetDelegate
  {
    public abstract List<string> Headings { get; }

    public abstract List<List<ReflectivePath>> Cells { get; }

    public virtual int? ContentHeightOverride => null;
  }

  public sealed class EditorSheet : VisualElement, IEditor
  {
    const int DefaultCellWidth = 250;
    const int ContentPadding = 16;

    readonly ScreenController _screenController;
    readonly EditorSheetDelegate _sheetDelegate;
    readonly ScrollView _scrollView;
    readonly VisualElement _content;
    readonly Dictionary<Vector2Int, EditorCell> _cells;

    Vector2Int? _currentlySelected;
    EditorCell? _currentlyActive;

    public EditorSheet(ScreenController controller, EditorSheetDelegate sheetDelegate)
    {
      _screenController = controller;
      _sheetDelegate = sheetDelegate;

      _content = new VisualElement();
      _content.AddToClassList("editor-content");

      var width = (2 * ContentPadding) + (DefaultCellWidth * _sheetDelegate.Headings.Count);
      _content.style.width = width;

      if (_sheetDelegate.ContentHeightOverride != null)
      {
        _content.style.height = _sheetDelegate.ContentHeightOverride.Value;
      }

      _scrollView = new ScrollView();
      _scrollView.AddToClassList("editor-scroll-view");
      _scrollView.Add(_content);

      Add(_scrollView);
      focusable = true;

      _cells = RenderCells();
      RegisterCallback<KeyDownEvent>(OnKeyDown);

      InterfaceUtils.After(0.01f, () => { SelectPosition(Vector2Int.zero); });
    }

    Dictionary<Vector2Int, EditorCell> RenderCells()
    {
      var result = new Dictionary<Vector2Int, EditorCell>();

      foreach (var heading in _sheetDelegate.Headings)
      {
        _content.Add(HeadingCell(heading));
      }

      for (var rowIndex = 0; rowIndex < _sheetDelegate.Cells.Count; rowIndex++)
      {
        var list = _sheetDelegate.Cells[rowIndex];
        for (var columnIndex = 0; columnIndex < list.Count; columnIndex++)
        {
          var position = new Vector2Int(columnIndex, rowIndex);
          var cell = EditorCellFactory.Create(_screenController, list[columnIndex], this);
          cell.RegisterCallback<ClickEvent>(e =>
          {
            if (position == _currentlySelected)
            {
              ActivateCell(position);
            }
            else
            {
              SelectPosition(position);
            }
          });
          cell.style.width = DefaultCellWidth;
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

    void OnKeyDown(KeyDownEvent evt)
    {
      if (!_currentlySelected.HasValue)
      {
        return;
      }

      if (_currentlyActive != null)
      {
        _currentlyActive.OnParentKeyDown(evt);
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
          SelectPosition(_currentlySelected.Value + new Vector2Int(0, -1));
          break;
        case KeyCode.DownArrow:
          SelectPosition(_currentlySelected.Value + new Vector2Int(0, 1));
          break;
        case KeyCode.KeypadEnter:
        case KeyCode.Return:
          ActivateCell(_currentlySelected.Value);
          break;
      }
    }

    void ActivateCell(Vector2Int position)
    {
      _currentlyActive?.Deactivate();
      _currentlyActive = _cells[position];
      _scrollView.ScrollTo(_currentlyActive);
      _currentlyActive.Activate();
    }

    void SelectPosition(Vector2Int? position)
    {
      if (position.HasValue && _cells.ContainsKey(position.Value) && position != _currentlySelected)
      {
        _currentlyActive?.Deactivate();
        _currentlyActive = null;

        Focus();

        if (_currentlySelected.HasValue)
        {
          _cells[_currentlySelected.Value].Unselect();
        }

        _cells[position.Value].Select();
        _currentlySelected = position;
        _scrollView.ScrollTo(_cells[position.Value]);
      }
    }

    public void OnChildEditingComplete(string? preview)
    {
      _currentlyActive = null;
      Focus();
    }
  }
}